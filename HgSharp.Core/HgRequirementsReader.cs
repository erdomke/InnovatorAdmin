// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.
using System;
using System.IO;

namespace HgSharp.Core
{
    public class HgRequirementsReader
    {
        private readonly HgFileSystem fileSystem;

        public HgRequirementsReader(HgFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public HgRequirements ReadRequirements(string requirementsFilePath)
        {
            var requirementsString = "";
            
            using(var streamReader = new StreamReader(fileSystem.OpenRead(requirementsFilePath)))
                requirementsString = streamReader.ReadToEnd();

            var requirements = requirementsString.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return new HgRequirements(requirements);
        }
    }
}