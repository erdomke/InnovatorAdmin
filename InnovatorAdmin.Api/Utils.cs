using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

[assembly: InternalsVisibleTo("InnovatorAdmin.ApiTests")]
namespace InnovatorAdmin
{
  internal static class Utils
  {
    public static XElement LoadXml(Stream stream)
    {
      if (stream == null || (stream.CanSeek && stream.Length < 1))
        return null;
      return XElement.Load(stream);
    }

    public static void RemoveWhere<T>(this IList<T> list, Func<T, bool> predicate)
    {
      var i = 0;
      while (i < list.Count)
      {
        if (predicate(list[i]))
          list.RemoveAt(i);
        else
          i++;
      }
    }

    public static string Left(this string value, int count)
    {
      if (value == null)
        return value;
      return value.Length > count
        ? value.Substring(0, count)
        : value;
    }

    public static StringBuilder AppendSeparator(this StringBuilder builder, string separator, object value)
    {
      if (builder.Length > 0) builder.Append(separator);
      return builder.Append(value);
    }

    public static IEnumerable<T> DependencySort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = false)
    {
      IList<T> cycle = new List<T>();
      return DependencySort(source, dependencies, ref cycle, throwOnCycle);
    }
    public static IEnumerable<T> DependencySort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, ref IList<T> cycle, bool throwOnCycle = false)
    {
      var sorted = new List<T>();
      var visited = new HashSet<T>();

      foreach (var item in source)
        Visit(item, visited, sorted, dependencies, cycle, throwOnCycle);

      return sorted;
    }

    private static bool Visit<T>(T item, HashSet<T> visited, List<T> sorted, Func<T, IEnumerable<T>> dependencies, IList<T> cycle, bool throwOnCycle)
    {
      var hasCycle = false;
      if (!hasCycle) cycle.Add(item);

      if (!visited.Contains(item))
      {
        visited.Add(item);

        var cycleList = cycle;
        foreach (var dep in (dependencies(item) ?? Enumerable.Empty<T>()).Where(d => !Object.ReferenceEquals(d, item)))
        {
          if (Visit(dep, visited, sorted, dependencies, cycleList, throwOnCycle))
          {
            hasCycle = true;
            cycleList = new List<T>();
          }
        }

        sorted.Add(item);
      }
      else
      {
        if (!sorted.Contains(item))
        {
          System.Diagnostics.Debug.Print("Cyclic dependency found");
          if (throwOnCycle)
          {
            var ex = new Exception("Cyclic dependency found");
            ex.Data["Cycle"] = cycle;
            throw ex;
          }
          else
          {
            return true;
          }
        }
      }

      if (!hasCycle) cycle.RemoveAt(cycle.Count - 1);
      return hasCycle;
    }

    public static string GroupConcat<T>(this IEnumerable<T> values, string separator, Func<T, string> renderer)
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

    public static string GetFileChecksum(string fileName)
    {
      var fileInfo = new FileInfo(fileName);
      if (!File.Exists(fileInfo.FullName)) throw new ArgumentException("The specified file doesn't exist.", "fileName");
      if (Directory.Exists(fileInfo.FullName)) throw new ArgumentException("The specified path is a directory and not a file.", "fileName");

      byte[] array;
      using (var mD = MD5.Create())
      {
        FileStream fileStream = null;
        try
        {
          fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
          using (BufferedStream bufferedStream = new BufferedStream(fileStream, 32768))
          {
            fileStream = null;
            array = mD.ComputeHash(bufferedStream);
          }
        }
        finally
        {
          if (fileStream != null)
          {
            fileStream.Close();
          }
        }
      }

      return FormatChecksum(array);
    }
    public static string GetChecksum(byte[] data)
    {
      using (var mD = MD5.Create())
      {
        return FormatChecksum(mD.ComputeHash(data));
      }
    }
    public static async Task<MemoryStream> ToMemoryStream(this Stream stream)
    {
      var memStream = stream as MemoryStream;
      if (memStream == null)
      {
        memStream = new MemoryStream();
        await stream.CopyToAsync(memStream);
      }
      return memStream;
    }
    public static async Task<string> ReadStart(this Stream stream, int length)
    {
      var reader = new StreamReader(stream);
      var buffer = new char[length];
      var count = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
      return new string(buffer, 0, count);
    }
    private static string FormatChecksum(byte[] array)
    {
      var stringBuilder = new StringBuilder(array.Length);
      for (var i = 0; i < array.Length; i++)
      {
        var b = (byte)((array[i] & 240) >> 4);
        var b2 = (byte)(array[i] & 15);
        if (b < 10)
        {
          stringBuilder.Append((char)(48 + b));
        }
        else
        {
          stringBuilder.Append((char)(65 + (b - 10)));
        }
        if (b2 < 10)
        {
          stringBuilder.Append((char)(48 + b2));
        }
        else
        {
          stringBuilder.Append((char)(65 + (b2 - 10)));
        }
      }
      return stringBuilder.ToString();
    }

    /// <summary>
    /// Removes invalid characters from the path
    /// </summary>
    public static string CleanFileName(string path)
    {
      var invalidChars = System.IO.Path.GetInvalidFileNameChars();
      Array.Sort(invalidChars);
      var builder = new System.Text.StringBuilder(path.Length);
      for (int i = 0; i < path.Length; i++)
      {
        if (Array.BinarySearch(invalidChars, path[i]) < 0 && path[i] != '/')
        {
          builder.Append(path[i]);
        }
      }
      return builder.ToString();
    }

    public static void Shuffle<T>(this IList<T> list)
    {
      var rng = new Random();
      var n = list.Count;
      while (n > 1)
      {
        n--;
        var k = rng.Next(n + 1);
        T value = list[k];
        list[k] = list[n];
        list[n] = value;
      }
    }
  }
}
