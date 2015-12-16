// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System.Text;

namespace HgSharp.Core
{
    public class HgEncoder
    {
        public Encoding Local { get; private set; }
        public Encoding Utf8 { get; private set; }

        public HgEncoder() :
            this(Encoding.Default, Encoding.UTF8)
        {
        }

        public HgEncoder(Encoding local, Encoding utf8)
        {
            Local = local;
            Utf8 = utf8;
        }

        public byte[] EncodeAsLocal(string value)
        {
            return Local.GetBytes(value);
        }

        public byte[] EncodeAsUtf8(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public string DecodeAsLocal(byte[] bytes)
        {
            return Local.GetString(bytes);
        }

        public string DecodeAsUtf8(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public string DecodeAsLocal(byte[] bytes, int index, int count)
        {
            return Local.GetString(bytes, index, count);
        }

        public string DecodeAsUtf8(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }
    }
}
