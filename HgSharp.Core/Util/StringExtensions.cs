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
using System.Text.RegularExpressions;

namespace HgSharp.Core.Util
{
    internal static class StringExtensions
    {
        public static IEnumerable<string> ReadLines(this string s)
        {
            using(var textReader = new StringReader(s))
            {
                var line = "";

                while((line = textReader.ReadLine()) != null)
                    yield return line;
            }
        } 

        public static IEnumerable<byte> DigitsOnly(this string s)
        {
            return s.Where(Char.IsDigit).Select(c => byte.Parse(c.ToString(CultureInfo.InvariantCulture)));
        }

        public static string RemoveSpaces(this string s)
        {
            if (String.IsNullOrEmpty(s)) return s;
            s = s.Replace(" ", "");
            return s;
        }

        public static string NullSafeTrim(this string s)
        {
            return s.Or("").Trim();
        }

        public static string MakeLess(this string s, Int32 chrNum)
        {
            if (s.Length > chrNum)
                return s.Substring(0, chrNum - 3) + "...";
            return s;
        }

        public static string Or(this string s, string fallback)
        {
            return string.IsNullOrEmpty(s) ? fallback : s;
        }

        public static string Or(this string s, Func<string> fallback)
        {
            return string.IsNullOrEmpty(s) ? fallback() : s;
        }

        public static string WithoutPrefix(this string s, string prefix, StringComparison stringComparison)
        {
            if (s == null) throw new ArgumentNullException("s");
            if (prefix == null) throw new ArgumentNullException("prefix");

            var l = prefix.Length;

            return s.StartsWith(prefix, stringComparison) ?
                s.Substring(l) :
                s;
        }

        public static string WithoutPrefix(this string s, string prefix)
        {
            return s.WithoutPrefix(prefix, StringComparison.CurrentCulture);
        }

        public static string[] Split(this string value, string separator)
        {
            return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static int IndexOfAny(this string value, int startIndex, params char[] anyOf)
        {
            return value.IndexOfAny(anyOf, startIndex);
        }

        public static int IndexOfAny(this string value, string anyOf, int startIndex)
        {
            return value.IndexOfAny(startIndex, anyOf.ToCharArray());
        }

        public static string ToLowerCamelCase(this string s)
        {
            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        public static string SubstringBefore(this string s, string substring)
        {
            var substringOffset = s.IndexOf(substring);

            return substringOffset == -1 ?
                s :
                s.Substring(0, substringOffset);
        }

        /// <summary>
        /// Returns a substring of <paramref name="s"/> until <paramref name="substring"/>
        /// or <paramref name="default"/>, if there's no substring in <paramref name="s"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="substring"></param>
        /// <param name="default"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// "ab" == "abc".SubstringBefore("c", null);
        /// null == "abc".SubstringBefore("d", null);
        /// "xx" == "abc".SubstringBefore("e", "xx");
        /// </code>
        /// </example>
        public static string SubstringBefore(this string s, string substring, string @default)
        {
            var substringOffset = s.IndexOf(substring);

            return substringOffset == -1 ?
                @default :
                s.Substring(0, substringOffset);
        }

        public static string SubstringBeforeSuffix(this string s, string substring)
        {
            if (!s.EndsWithSuffix(substring)) return s;

            var substringOffset = s.LastIndexOf(substring);

            return substringOffset == -1 ?
                s :
                s.Substring(0, substringOffset);
        }

        public static string SubstringAfter(this string s, string substring)
        {
            var start = s.IndexOf(substring);

            return start == -1 ?
                s :
                s.Substring(start + substring.Length);
        }

        /// <summary>
        /// Returns a substring from <paramref name="s"/> before <paramref name="nth"/> (<paramref name="nth"/> is 0-based) occurence of <paramref name="substring"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="substring"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static string SubstringBeforeNth(this string s, string substring, int nth)
        {
            var indexOfNth = s.IndexOfNth(substring, nth);
            
            return indexOfNth == -1 ?
                s :
                s.Substring(0, indexOfNth);
        }

        /// <summary>
        /// Returns a substring from <paramref name="s"/> after <paramref name="nth"/> (<paramref name="nth"/> is 0-based) occurence of <paramref name="substring"/>.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="substring"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static string SubstringAfterNth(this string s, string substring, int nth)
        {
            var indexOfNth = s.IndexOfNth(substring, nth);

            return indexOfNth == -1 ?
                "" :
                s.Substring(indexOfNth + substring.Length);
        }


        public static string SubstringAfterLast(this string s, string substring)
        {
            var start = s.LastIndexOf(substring);

            return start == -1 ?
                s :
                s.Substring(start + substring.Length);
        }


        public static string SubstringBetween(this string s, string start, string end)
        {
            return s.SubstringAfter(start).SubstringBefore(end);
        }

        public static bool EndsWithSuffix(this string s, string suffix)
        {
            return s.Length > suffix.Length && s.EndsWith(suffix);
        }

        /// <summary>
        /// Returns a zero-based index of <paramref name="nth"/> (<paramref name="nth"/> is 0-based) occurence of <paramref name="substring"/> in <paramref name="s"/> or -1.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="substring"></param>
        /// <param name="nth"></param>
        /// <returns></returns>
        public static int IndexOfNth(this string s, string substring, int nth)
        {
            var indexOfNth = s.IndexOf(substring, StringComparison.Ordinal);
            while(nth > 0)
            {
                indexOfNth = s.IndexOf(substring, indexOfNth + 1, StringComparison.Ordinal);
                nth--;

                if(indexOfNth == -1) return -1;
            } // while

            return indexOfNth;
        }

        /// <summary>
        /// Joins non-null and non-empty values from <paramref name="strings"/>.
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string JoinNonEmpty(this string[] strings, string separator)
        {
            var nonEmpty = strings.Where(s => s != null && !string.IsNullOrEmpty(s)).ToArray();
            if (nonEmpty.Length == 0) return "";

            return string.Join(separator, nonEmpty);
        }

        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        public static string ToSlug(this string value)
        {
            return BuildSlugCore(RemoveDiacritics(value));
        }

        private static string BuildSlugCore(string value, int maxLength = 200)
        {
            var match = Regex.Match(value.ToLower(), "[\\w]+");
            var result = new StringBuilder("");
            var maxLengthHit = false;
            
            while(match.Success && !maxLengthHit)
            {
                if(result.Length + match.Value.Length <= maxLength)
                    result.Append(match.Value + "-");
                else
                {
                    maxLengthHit = true;
                    
                    //
                    // Handle a situation where there is only one word and it is greater than the max length.
                    if (result.Length == 0) result.Append(match.Value.Substring(0, maxLength));
                } // else

                match = match.NextMatch();
            } // while

            //
            // Remove trailing '-'
            while(result.Length > 0 && result[result.Length - 1] == '-') 
                result.Remove(result.Length - 1, 1);
            
            var r = result.ToString();

            while(r.Contains("--"))
                r = r.Replace("--", "-");

            return r;
        }

        public static string RemoveDiacritics(string s)
        {
            var stFormD = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach(var c in stFormD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if(uc != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            } // foreach
             
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Trim() == "";
        }

        public static string ComputeHash(this string s)
        {
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "").ToLowerInvariant();
        }

        public static bool CaseInsensitivelyEquals(this string s, string other)
        {
            return string.Compare(s, other, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public static string ConvertToUriSafeBase64(this string s)
        {
            return s.Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public static string ConvertFromUriSafeBase64(this string s)
        {
            var res = s.Replace("-", "+").Replace("_", "/");
            while (res.Length % 4 != 0)
                res = res + "=";

            return res;
        }

        public static string DoDotsInTime(this string s)
        {
            if (s.Length == 4)
            {
                return String.Format("{0}:{1}", s.Substring(0, 2), s.Substring(2));
            }
            return s;
        }

        public static string ToTitleCase(this string s)
        {
            if (s.IsNullOrEmpty()) return s;

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            return ti.ToTitleCase(s.ToLower());

        }
    }
}
