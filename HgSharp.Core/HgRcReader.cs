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
using System.IO;
using System.Text;

using HgSharp.Core.Util;

using NLog;

namespace HgSharp.Core
{
    public class HgRcReader
    {
        private readonly Logger log = LogManager.GetCurrentClassLogger();

        public HgRc ReadHgRc(string hgRcPath)
        {
            log.Trace("attempting to read hgrc from '{0}'", hgRcPath);
            if(!Alphaleonis.Win32.Filesystem.File.Exists(hgRcPath))
            {
                log.Trace("'{0}' does not exist", hgRcPath);
                return null;
            } // if

            using(var fileStream = Alphaleonis.Win32.Filesystem.File.OpenRead(hgRcPath))
            using(var textReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var line = "";
                
                HgRcSection hgRcSection = null;
                IList<HgRcSection> hgRcSections = new List<HgRcSection>();

                while((line = textReader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if(string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    {
                        if(hgRcSection == null)
                        {
                            hgRcSection = new HgRcSection(null, null);
                            hgRcSections.Add(hgRcSection);
                        } // if

                        hgRcSection.Add(new HgRcSectionEntry { Verbatim = line });
                    } // if 
                    else if(line.StartsWith("[") && line.EndsWith("]"))
                    {
                        var sectionName = line.SubstringBetween("[", "]");
                        
                        hgRcSection = new HgRcSection(sectionName, null);
                        hgRcSections.Add(hgRcSection);
                    } // if
                    else
                    {
                        var propertyName = line.SubstringBefore("=").Trim();
                        var propertyValue = line.SubstringAfter("=").Trim();

                        if(hgRcSection == null) 
                            throw new InvalidOperationException();

                        hgRcSection[propertyName] = propertyValue;
                    } // else
                } // while
                
                return new HgRc(hgRcSections);
            } // using
        }
    }
}