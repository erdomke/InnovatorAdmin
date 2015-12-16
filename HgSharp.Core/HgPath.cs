// HgSharp
//
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
//
// The following code is a derivative work of the code from the Mercurial project,
// which is licensed GPLv2. This code therefore is also licensed under the terms
// of the GNU Public License, verison 2.
using System;
using System.Diagnostics;

using System.Linq;

namespace HgSharp.Core
{
  [DebuggerDisplay("{FullPath,nq}")]
  public class HgPath : IEquatable<HgPath>
  {
    private string[] segments;

    public string FullPath { get; private set; }

    public string FileName
    {
      get
      {
        return Segments.Length == 0 ?
            "" :
            Segments[Segments.Length - 1];
      }
    }

    //public string DirectoryPath { get; private set; }

    public string[] Segments
    {
      get
      {
        if (segments == null)
          segments = FullPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

        return segments;
      }
    }

    [DebuggerStepThrough]
    public HgPath(string fullPath)
    {
      if (fullPath == null) throw new ArgumentNullException("fullPath");

      FullPath = "/" + fullPath.TrimStart('/');

      /*if(Segments.Length == 0)
      {
          DirectoryPath = "/";
          FileName = "";
      }
      else if(Segments.Length == 1)
      {
          DirectoryPath = "/";
          FileName = Segments[0];
      } // if
      else
      {
          DirectoryPath = "/" + string.Join("/", Segments.Take(Segments.Length - 1));
          FileName = Segments[Segments.Length - 1];
      } // else*/
    }

    public HgPath Combine(string path)
    {
      return new HgPath(FullPath.TrimEnd('/') + "/" + path.TrimStart('/'));
    }

    public bool Equals(HgPath other)
    {
      if (ReferenceEquals(null, other)) return false;
      if (ReferenceEquals(this, other)) return true;
      return string.Compare(other.FullPath, FullPath, StringComparison.Ordinal) == 0;
    }

    public override bool Equals(object obj)
    {
      if (ReferenceEquals(null, obj)) return false;
      if (ReferenceEquals(this, obj)) return true;
      if (obj.GetType() != typeof(HgPath)) return false;
      return Equals((HgPath)obj);
    }

    public override int GetHashCode()
    {
      return (FullPath != null ? FullPath.GetHashCode() : 0);
    }

    public static bool operator ==(HgPath left, HgPath right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(HgPath left, HgPath right)
    {
      return !Equals(left, right);
    }

    public static implicit operator HgPath(string value)
    {
      return new HgPath(value);
    }

    public static HgPath Combine(string path, HgPath hgPath)
    {
      return new HgPath(path.TrimEnd('/') + '/' + hgPath.FullPath.TrimStart('/'));
    }

    public static HgPath Combine(HgPath hgPath, string path)
    {
      return new HgPath(hgPath.FullPath.TrimEnd('/') + '/' + path.TrimStart('/'));
    }
  }
}
