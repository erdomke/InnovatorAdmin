using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client
{
  public static class Extensions
  {
    public static bool IsNullOrEmpty(this SecureToken token)
    {
      return token == null || token.Length < 1;
    }

    public static string Checksum(this FileInfo fileInfo)
    {
      if (!File.Exists(fileInfo.FullName))
        throw new ArgumentException("The spcecified file doesn't exist.", "fileName");

      if (Directory.Exists(fileInfo.FullName))
        throw new ArgumentException("The specified path is a directory and not a file.", "fileName");

      using (var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
      {
        using (var bufferedStream = new BufferedStream(fileStream, 32768))
        {
          using (var mD = MD5.Create())
          {
            return mD.ComputeHash(bufferedStream).HexString();
          }
        }
      }
    }

    public static string HexString(this byte[] value, int offset = 0, int length = -1)
    {
      if (length < 0) length = value.Length;
      var builder = new StringBuilder(value.Length * 2);
      int b;
      for (int i = 0; i < value.Length; i++)
      {
        b = value[i] >> 4;
        builder.Append((char)(55 + b + (((b - 10) >> 31) & -7)));
        b = value[i] & 0xF;
        builder.Append((char)(55 + b + (((b - 10) >> 31) & -7)));
      }
      return builder.ToString();
    }

    public static string AsString(this Stream data)
    {
      using (var reader = new StreamReader(data))
      {
        return reader.ReadToEnd();
      }
    }
    public static byte[] AsBytes(this Stream data)
    {
      using (var memStream = new MemoryStream())
      {
        data.CopyTo(memStream);
        return memStream.ToArray();
      }
    }

    public static string Left(this string str, int count)
    {
      if (string.IsNullOrEmpty(str) || count < 1)
        return string.Empty;
      else
        return str.Substring(0, Math.Min(count, str.Length));
    }

    internal static void Rethrow(this Exception ex)
    {
      if (!string.IsNullOrEmpty(ex.StackTrace))
      {
        typeof(Exception).GetMethod("PrepForRemoting",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .Invoke(ex, new object[0]);
      }
      throw ex;
    }

    internal static T Single<T>(this IEnumerable<T> source, Func<T, bool> predicate, Func<int, Exception> errorHandler)
    {
      if (source == null) throw new ArgumentNullException("source");
      if (predicate == null) throw new ArgumentNullException("predicate");
      if (errorHandler == null) throw new ArgumentNullException("errorHandler");

      var found = false;
      var result = default(T);
      foreach (var current in source)
      {
        if (predicate(current))
        {
          if (found)
          {
            throw errorHandler(2);
          }
          else
          {
            result = current;
            found = true;
          }
        }
      }

      if (found) return result;
      throw errorHandler(0);
    }

    public static bool IsGuid(this string value)
    {
      if (string.IsNullOrEmpty(value)) return false;
      if (value.Length != 32) return false;
      for (var i = 0; i < value.Length; i++)
      {
        if (!char.IsDigit(value[i])
          && value[i] != 'A' && value[i] != 'B'
          && value[i] != 'C' && value[i] != 'D'
          && value[i] != 'E' && value[i] != 'F'
          && value[i] != 'a' && value[i] != 'b'
          && value[i] != 'c' && value[i] != 'd'
          && value[i] != 'e' && value[i] != 'f')
          return false;
      }
      return true;
    }

    public static string AsString(this IHttpResponse resp)
    {
      if (resp.AsStream.CanSeek) resp.AsStream.Position = 0;
      using (var reader = new System.IO.StreamReader(resp.AsStream))
      {
        return reader.ReadToEnd();
      }
    }
    public static XElement AsXml(this IHttpResponse resp)
    {
      if (resp.AsStream.CanSeek) resp.AsStream.Position = 0;
      using (var reader = new System.IO.StreamReader(resp.AsStream))
      {
        return XElement.Load(reader);
      }
    }



  }
}
