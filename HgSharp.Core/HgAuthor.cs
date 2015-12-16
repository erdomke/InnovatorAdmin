// HgSharp
// 
// Copyright 2005-2015 Matt Mackall <mpm@selenic.com> and Mercurial contributors
// Copyright 2011-2015 Anton Gogolev <anton.gogolev@hglabhq.com>
// 
// The following code is a derivative work of the code from the Mercurial project, 
// which is licensed GPLv2. This code therefore is also licensed under the terms 
// of the GNU Public License, verison 2.

using System;
using System.Text.RegularExpressions;

namespace HgSharp.Core
{
    public class HgAuthor : IEquatable<HgAuthor>
    {
        private static readonly Regex EmailAddressPartRegex = new Regex(@"(\<(.+)\>)", RegexOptions.Compiled | RegexOptions.Singleline);

        public string FullName { get; private set; }

        public string EmailAddress { get; private set; }

        public HgAuthor(string fullName, string emailAddress)
        {
            FullName = fullName;
            EmailAddress = emailAddress;
        }

        public static HgAuthor Parse(string authorName)
        {
            var match = EmailAddressPartRegex.Match(authorName);
            var emailAddress = 
                match.Success ?
                    match.Groups[2].Value.Trim() :
                    "";
            var fullName =
                match.Success ?
                    EmailAddressPartRegex.Replace(authorName, "").Trim() :
                    authorName;
            return new HgAuthor(fullName, emailAddress);
        }

        public bool Equals(HgAuthor other)
        {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(FullName, other.FullName) && string.Equals(EmailAddress, other.EmailAddress);
        }

        public override bool Equals(object obj)
        {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != this.GetType()) return false;
            return Equals((HgAuthor)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FullName != null ? FullName.GetHashCode() : 0) * 397) ^ (EmailAddress != null ? EmailAddress.GetHashCode() : 0);
            }
        }

        public static bool operator ==(HgAuthor left, HgAuthor right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(HgAuthor left, HgAuthor right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return string.Format("{0} <{1}>", FullName, EmailAddress);
        }
    }
}