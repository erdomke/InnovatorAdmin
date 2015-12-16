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
using System.Linq;

namespace HgSharp.Core
{
    public class MergeStream : Stream
    {
        private readonly Queue<Stream> streams;
        private long position;
 
        public MergeStream(params Stream[] streams)
        {
            this.streams = new Queue<Stream>(streams);
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if(streams.Count == 0) return 0;
            
            var read = streams.Peek().Read(buffer, offset, count);
            while(read == 0)
            {
                var stream = streams.Dequeue();
                stream.Dispose();

                if(streams.Count == 0) return 0;

                read = streams.Peek().Read(buffer, offset, count);
            } // if
            
            position += read;

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { return streams.Sum(s => s.Length); }
        }

        public override long Position
        {
            get { return position; } 
            set { throw new NotImplementedException(); }
        }
    }
}