// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using HgSharp.Core.Util;

namespace HgSharp.Core
{
    public class HgPathEncoder
    {
        private readonly HgPathEncodings encodings;
        private readonly HgEncoder hgEncoder;
        private const int MaxStorePathLength = 120;
        private const int DirectoryPrefixLength = 8;
        private const int MaxShortDirectoriesLength = 8 * (DirectoryPrefixLength + 1) - 4;

        private readonly string[] windowsReservedNames = "con prn aux nul com1 com2 com3 com4 com5 com6 com7 com8 com9 lpt1 lpt2 lpt3 lpt4 lpt5 lpt6 lpt7 lpt8 lpt9".Split(' ');
        private readonly IDictionary<string, string> auxdecodemap = new Dictionary<string, string>(); 

        private Func<string, string> encodefilename, decodefilename;

        public HgPathEncoder(HgPathEncodings encodings, HgEncoder hgEncoder)
        {
            this.encodings = encodings;
            this.hgEncoder = hgEncoder;
            var t = _buildencodefun();
            encodefilename = t.Item1;
            decodefilename = t.Item2;

            foreach(var w in windowsReservedNames)
                auxdecodemap[w.Substring(0, 2) + "~" + BitConverter.ToString(new[] { (byte)w[2] }).ToLowerInvariant()  + w.Substring(3)] = w;

            auxdecodemap["~20"] = " ";
            auxdecodemap["~2e"] = ".";
        }

        public string GetEncodedData(HgPath path)
        {
            return EncodePath("data/" + path.FullPath.TrimStart('/'));
        }

        public string DecodePath(string path)
        {
            return decodefilename(auxdecode(path));
        }

        public string EncodePath(string path)
        {
            if(!path.StartsWith("data/")) return path;

            path = EncodeDirectory(path);
            var ndpath = path.Substring("data/".Length);
            var res = "data/" + auxencode(encodefilename(ndpath));
            if(res.Length > MaxStorePathLength)
            {
                var digest = sha1digest(path);
                string aep;
                aep = auxencode(lowerencode(ndpath));
                var ext = Path.GetExtension(aep);

                var parts = aep.Split('/');
                var basename = parts.Last();

                List<string> sdirs = new List<string>();

                foreach(var p in parts.Take(parts.Length - 1))
                {
                    var d = p.Length < DirectoryPrefixLength ? p : p.Substring(0, DirectoryPrefixLength);
                    if(d.Last() == '.' || d.Last() == ' ')
                        d = d.Substring(0, d.Length - 1) + "_";

                    var t = string.Join("/", sdirs) + "/" + d;
                    if(t.Length > MaxShortDirectoriesLength) break;

                    sdirs.Add(d);
                } // if

                var dirs = string.Join("/", sdirs);
                if(dirs.Length > 0)
                    dirs += "/";

                res = "dh/" + dirs + digest + ext;
                var spaceleft = MaxStorePathLength - res.Length;
                if(spaceleft > 0)
                {
                    var filler = basename.Substring(0, Math.Min(basename.Length, spaceleft));
                    res = "dh/" + dirs + filler + digest + ext;
                }
            }

            return res;
        }

        private Tuple<Func<string, string>, Func<string, string>> _buildencodefun()
        {
            var winreserved = @"\\:*?""<>|".Select(c => (char)c).ToArray();
            var cmap = Enumerable.Range(0, 127).ToDictionary(i => (byte)i, i => ((char)i).ToString(CultureInfo.InvariantCulture));

            foreach(var i in Enumerable.Range(0, 32))
                cmap[(byte)i] = "~" + BitConverter.ToString(new[] { (byte)i }).ToLowerInvariant();

            foreach(var i in Enumerable.Range(126, 130))
                cmap[(byte)i] = "~" + BitConverter.ToString(new[] { (byte)i }).ToLowerInvariant();

            foreach(var i in winreserved)
                cmap[(byte)i] = "~" + BitConverter.ToString(new[] { (byte)i }).ToLowerInvariant();

            foreach(var i in Enumerable.Range('A', 'Z' - 'A' + 1))
                cmap[(byte)i] = "_" + ((char)i).ToString().ToLowerInvariant();

            cmap[(byte)'_'] = "__";

            var vals = cmap.Values.GroupBy(v => v).OrderByDescending(g => g.Count()).ToList();
            var dmap = cmap.ToDictionary(k => k.Value, k => k.Key);

            Func<string, string> decode = (s) => {
                var i = 0;
                var _s = "";
                while(i < s.Length)
                {
                    foreach(var l in Enumerable.Range(1, 4))
                    {
                        var c = s.Substring(i, l);
                        if(dmap.ContainsKey(c))
                        {
                            _s += hgEncoder.DecodeAsLocal(new[] { dmap[c] });
                            i += l;
                            break;
                        } // if

                        if(l == 4) throw new KeyNotFoundException(s.Substring(i, 4));
                    } // foreach
                } // while

                return _s;
            };

            Func<string, string> d = s => DecodeDirectory(decode(s));


            Func<string, string> encode = (s) => {    
                var bytes = hgEncoder.EncodeAsLocal(s);
                //var chars = Encoding.Default.GetChars(bytes);
                return string.Join("", bytes.Select(c => cmap[c]));
            };

            return Tuple.Create(encode, d);
        }

        private string lowerencode(string ndpath)
        {
            var winreserved = @"\\:*?""<>|".Select(c => (char)c).ToArray();
            var cmap = Enumerable.Range(0, 127).ToDictionary(i => (byte)i, i => ((char)i).ToString(CultureInfo.InvariantCulture));
            
            foreach(byte i in Enumerable.Range(128, 127))
                cmap[i] = "~" + BitConverter.ToString(new[] { i }).ToLowerInvariant();;

            foreach(byte i in Enumerable.Range(0, 32))
                cmap[i] = "~" + BitConverter.ToString(new[] { i }).ToLowerInvariant();

            foreach(byte i in Enumerable.Range(126, 130))
                cmap[i] = "~" + BitConverter.ToString(new[] { i }).ToLowerInvariant();

            foreach(byte i in winreserved)
                cmap[i] = "~" + BitConverter.ToString(new[] { i }).ToLowerInvariant();

            foreach(byte i in Enumerable.Range('A', 'Z' - 'A' + 1))
                cmap[i] = ((char)i).ToString().ToLowerInvariant();

            var bytes = hgEncoder.EncodeAsLocal(ndpath);

            var res = bytes.Select(b => {
                if(!cmap.ContainsKey(b)) throw new ArgumentException(b.ToString());
                return cmap[b];
            }).Aggregate("", (s, s1) => s += s1);
            return res;
        }

        private string sha1digest(string path)
        {
            return BitConverter.ToString(new SHA1Managed().ComputeHash(hgEncoder.EncodeAsLocal(path))).ToLowerInvariant().Replace("-", "");
        }

        private string auxdecode(string path)
        {
            foreach(var aux in auxdecodemap)
            {
                while(path.Contains(aux.Key))
                    path = path.Replace(aux.Key, aux.Value);
            }

            return path;
        }
        
        private string auxencode(string path)
        {
            var res = new List<string>();

            foreach(var n in path.Split('/'))
            {
                var _n = n;
                var @base = _n.Split('.')[0];
                if(!string.IsNullOrWhiteSpace(@base) && windowsReservedNames.Contains(@base))
                {
                    var ec = "~" + BitConverter.ToString(new[] { (byte)@base[2] }).ToLowerInvariant();
                    _n = _n.Substring(0, 2) + ec + _n.Substring(3);
                }
                if((_n.Last() == '.' || _n.Last() == ' ') && _n.Length > 1)
                {
                    _n = _n.Substring(0, _n.Length - 1) + "~" + BitConverter.ToString(new[] { (byte)_n.Last() }).ToLowerInvariant();
                }

                if((_n[0] == '.' || _n[0] == ' ') && ((encodings & HgPathEncodings.DotEncode) == HgPathEncodings.DotEncode))
                {
                    _n = "~" + BitConverter.ToString(new[] { (byte)_n[0] }).ToLowerInvariant() + _n.Substring(1);
                }

                res.Add(_n);
            }

            return string.Join("/", res);
        }

        public string EncodeDirectory(string path)
        {
            if(!path.StartsWith("data/")) return path;
            return path.Replace(".hg/", ".hg.hg/").Replace(".i/", ".i.hg/").Replace(".d/", ".d.hg/");
        }

        public static string DecodeDirectory(string path)
        {
            if(!path.StartsWith("data/")) return path;
            return path.Replace(".d.hg/", ".d/").Replace(".i.hg/", ".i/").Replace(".hg.hg/", ".hg/");
        }
    }
}
