using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client
{
  public static class Utils
  {
    internal static T[] ToArray<T>(this ArraySegment<T> data)
    {
      return data.Array.ToArray(data.Offset, data.Count);
    }
    internal static T[] ToArray<T>(this T[] data, int offset, int length)
    {
      var result = new T[length];
      Array.Copy(data, offset, result, 0, result.Length);
      return result;
    }

    internal static bool IsNullOrWhitespace(this string value)
    {
      return value == null || value.Trim().Length == 0;
    }

    internal static bool EndsWith<T>(this IEnumerable<T> values, params T[] compare)
    {
      return EndsWith(values, (IEnumerable<T>)compare);
    }
    internal static bool EndsWith<T>(this IEnumerable<T> values, IEnumerable<T> compare)
    {
      using (var compareEnum = compare.Reverse().GetEnumerator())
      {
        using (var valueEnum = values.Reverse().GetEnumerator())
        {
          while (compareEnum.MoveNext() && valueEnum.MoveNext())
          {
            if (!valueEnum.Current.Equals(compareEnum.Current)) return false;
          }
          if (compareEnum.MoveNext()) return false;
          return true;
        }
      }
    }
    public static string GroupConcat<T>(this IEnumerable<T> values, string separator, Func<T, string> renderer = null)
    {
      if (values.Any())
      {
        if (renderer == null)
        {
          return values.Select(v => v.ToString()).Aggregate((p, c) => p + separator + c);
        }
        return values.Select(renderer).Aggregate((p, c) => p + separator + c);
      }
      else
      {
        return string.Empty;
      }
    }
    internal static byte[] Compress(byte[] data, CompressionType type)
    {
      byte[] result;
      using (var compressedStream = new MemoryStream())
      {
        switch (type)
        {
          case CompressionType.gzip:
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
              zipStream.Write(data, 0, data.Length);
            }
            break;
          case CompressionType.deflate:
            using (var deflateStream = new DeflateStream(compressedStream, CompressionMode.Compress))
            {
              deflateStream.Write(data, 0, data.Length);
            }
            break;
          default:
            result = data;
            return result;
        }
        result = compressedStream.ToArray();
      }
      return result;
    }

    public static bool EnumTryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct
    {
#if NET4
      return Enum.TryParse<TEnum>(value, ignoreCase, out result);
#else
      try
      {
        result = (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        return true;
      }
      catch (ArgumentNullException)
      {
        result = default(TEnum);
        return false;
      }
      catch (ArgumentException)
      {
        result = default(TEnum);
        return false;
      }
#endif
    }

#if !NET4
    public static void CopyTo(this Stream input, Stream output)
    {
      byte[] buffer = new byte[4096];
      int read;
      while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
      {
        output.Write(buffer, 0, read);
      }
    }
#endif

#if !NET4
    /// <summary>
    /// Used for fixing a bug with cookie domain handling
    /// </summary>
    /// <remarks>See http://stackoverflow.com/questions/1047669/cookiecontainer-bug</remarks>
    public static void BugFix_CookieDomain(this CookieContainer cookieContainer)
    {
      var _ContainerType = typeof(CookieContainer);
      var table = (Hashtable)_ContainerType.InvokeMember("m_domainTable",
                                 System.Reflection.BindingFlags.NonPublic |
                                 System.Reflection.BindingFlags.GetField |
                                 System.Reflection.BindingFlags.Instance,
                                 null,
                                 cookieContainer,
                                 new object[] { });
      var keys = new ArrayList(table.Keys);
      foreach (var keyObj in keys)
      {
        var key = (keyObj as string);
        if (key[0] == '.')
        {
          var newKey = key.Remove(0, 1);
          table[newKey] = table[keyObj];
        }
      }
    }
#endif

    public static bool ShouldClose(this Stream input)
    {
      return !(input is MemoryStream);
    }

    public static void Save(this Stream input, string path)
    {
      using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
      {
        input.CopyTo(fs);
      }
    }

    public static IPromise<T> AsyncInvoke<T>(Func<T> method)
    {
      var result = new Promise<T>();
      method.BeginInvoke(iar =>
      {
        try
        {
          result.Resolve(((Func<T>)iar.AsyncState).EndInvoke(iar));
        }
        catch (Exception ex)
        {
          result.Reject(ex);
        }
      }, method);
      return result;
    }
    public static IPromise<T> AsyncInvoke<T1, T>(Func<T1, T> method, T1 arg1)
    {
      var result = new Promise<T>();
      method.BeginInvoke(arg1, iar =>
      {
        try
        {
          result.Resolve(((Func<T1, T>)iar.AsyncState).EndInvoke(iar));
        }
        catch (Exception ex)
        {
          result.Reject(ex);
        }
      }, method);
      return result;
    }
    public static IPromise<T> AsyncInvoke<T1, T2, T>(Func<T1, T2, T> method, T1 arg1, T2 arg2)
    {
      var result = new Promise<T>();
      method.BeginInvoke(arg1, arg2, iar =>
      {
        try
        {
          result.Resolve(((Func<T1, T2, T>)iar.AsyncState).EndInvoke(iar));
        }
        catch (Exception ex)
        {
          result.Reject(ex);
        }
      }, method);
      return result;
    }
    public static IPromise<T> AsyncInvoke<T1, T2, T3, T>(Func<T1, T2, T3, T> method, T1 arg1, T2 arg2, T3 arg3)
    {
      var result = new Promise<T>();
      method.BeginInvoke(arg1, arg2, arg3, iar =>
      {
        try
        {
          result.Resolve(((Func<T1, T2, T3, T>)iar.AsyncState).EndInvoke(iar));
        }
        catch (Exception ex)
        {
          result.Reject(ex);
        }
      }, method);
      return result;
    }

    public static void AsyncInvoke(Action method)
    {
      method.BeginInvoke(iar =>
      {
        ((Action)iar.AsyncState).EndInvoke(iar);
      }, method);
    }
    public static void AsyncInvoke<T>(Action<T> method, T arg1)
    {
      method.BeginInvoke(arg1, iar =>
      {
        ((Action<T>)iar.AsyncState).EndInvoke(iar);
      }, method);
    }
    public static void AsyncInvoke<T1, T2>(Action<T1, T2> method, T1 arg1, T2 arg2)
    {
      method.BeginInvoke(arg1, arg2, iar =>
      {
        ((Action<T1, T2>)iar.AsyncState).EndInvoke(iar);
      }, method);
    }
  }
}