using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public static class EnumerableExtensions
  {
    /// <summary>Determines whether a sequence contains any elements.</summary>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable" /> to check for emptiness.</param>
    public static bool Any(this IEnumerable collection)
    {
      return !collection.IsEmpty();
    }

    /// <summary>
    /// Does the collection have at least <param name="count"></param> elements?
    /// </summary>
    public static bool CountAtLeast(this IEnumerable collection, int count)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        if (collection == null) throw new ArgumentNullException("collection");
        if (count <= 0) return true;

        int cnt = 0;
        while (enumerator.MoveNext())
        {
          cnt += 1;
          if (cnt >= count) return true;
        }
        return cnt >= count;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    /// <summary>
    /// Does the collection have at most <param name="count"></param> elements?
    /// </summary>
    public static bool CountAtMost(this IEnumerable collection, int count)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        if (collection == null) throw new ArgumentNullException("collection");
        if (count < 0) return false;
        int cnt = 0;
        while (enumerator.MoveNext())
        {
          cnt += 1;
          if (cnt > count) return false;
        }
        return cnt <= count;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    /// <summary>
    /// Does the collection have at least <param name="count"></param> elements?
    /// </summary>
    public static bool CountExactly(this IEnumerable collection, int count)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        if (collection == null) throw new ArgumentNullException("collection");
        if (count < 0) return false;
        int cnt = 0;
        while (enumerator.MoveNext())
        {
          cnt += 1;
          if (cnt > count) return false;
        }
        return cnt == count;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    ///// <summary>Returns the element at a specified index in a sequence.</summary>
    ///// <returns>The element at the specified position in the source sequence.</returns>
    ///// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable" /> to return an element from.</param>
    ///// <param name="index">The zero-based index of the element to retrieve.</param>
    //public static object ElementAt(this IEnumerable collection, int index)
    //{
    //  var enumerator = collection.GetEnumerator();
    //  try
    //  {
    //    if (collection == null) throw new ArgumentNullException("collection");
    //    while (enumerator.MoveNext())
    //    {
    //      if (index == 0) return enumerator.Current;
    //      index -= 1;
    //    }

    //    throw new ArgumentOutOfRangeException("index");
    //  }
    //  finally
    //  {
    //    var disposable = enumerator as IDisposable;
    //    if (disposable != null) disposable.Dispose();
    //  }
    //}

    public static int IndexOf(this IEnumerable collection, object value)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        int index = 0;
        while (enumerator.MoveNext())
        {
          if (Extension.IsEqual(enumerator.Current, value)) return index;
          index += 1;
        }
        return -1;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    public static int IndexOf(this IEnumerable collection, object value, IComparer comparer)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        int index = 0;
        while (enumerator.MoveNext())
        {
          if (comparer.Compare(enumerator.Current, value) == 0)
            return index;
          index += 1;
        }
        return -1;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    public static int IndexOf(this IEnumerable collection, Predicate<object> predicate)
    {
      var enumerator = collection.GetEnumerator();
      try
      {
        int index = 0;
        while (enumerator.MoveNext())
        {
          if (predicate.Invoke(enumerator.Current))
            return index;
          index += 1;
        }
        return -1;
      }
      finally
      {
        var disposable = enumerator as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    /// <summary>Determines whether a sequence contains any elements.</summary>
    /// <returns>true if the source sequence contains no elements; otherwise, false.</returns>
    /// <param name="source">The <see cref="T:System.Collections.Generic.IEnumerable" /> to check for emptiness.</param>
    public static bool IsEmpty(this IEnumerable collection)
    {
      if (collection == null)
      {
        return true;
      }
      else
      {
        var enumerator = collection.GetEnumerator();
        try
        {
          return !enumerator.MoveNext();
        }
        finally
        {
          var disposable = enumerator as IDisposable;
          if (disposable != null) disposable.Dispose();
        }
      }
    }

    // Convenience method on IEnumerable<T> to allow passing of a
    // Comparison<T> delegate to the OrderBy method.
    public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparison<T> comparison)
    {
      return list.OrderBy(x => x, new ComparisonComparer<T>(comparison));
    }

    // Wraps a generic Comparison<T> delegate in an IComparer to make it easy
    // to use a lambda expression for methods that take an IComparer or IComparer<T>
    private class ComparisonComparer<T> : IComparer<T>, IComparer
    {

      private readonly Comparison<T> _comparison;
      public ComparisonComparer(Comparison<T> comparison)
      {
        _comparison = comparison;
      }

      public int Compare(T x, T y)
      {
        return _comparison(x, y);
      }

      public int Compare(object o1, object o2)
      {
        return _comparison((T)o1, (T)o2);
      }
    }

    public static string GroupConcat(this IEnumerable<string> values, string separator)
    {
      if (values == null) throw new ArgumentNullException("values");
      if (separator == null) separator = string.Empty;

      string result;
      using (var enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
        {
          result = string.Empty;
        }
        else
        {
          var stringBuilder = new StringBuilder(16);
          if (enumerator.Current != null) stringBuilder.Append(enumerator.Current);
          while (enumerator.MoveNext())
          {
            stringBuilder.Append(separator);
            if (enumerator.Current != null) stringBuilder.Append(enumerator.Current);
          }
          result = stringBuilder.ToString();
        }
      }
      return result;
    }

    public static string GroupConcat<T>(this IEnumerable<T> values, string separator, Func<T, string> conv = null)
    {
      var builder = new StringBuilder(16);
      GroupConcat(values, builder, separator, conv);
      return builder.ToString();
          
    }
    public static void GroupConcat<T>(this IEnumerable<T> values, StringBuilder stringBuilder, string separator, Func<T, string> conv = null)
    {
      if (values == null) throw new ArgumentNullException("values");
      if (separator == null) separator = string.Empty;

      using (var enumerator = values.GetEnumerator())
      {
        if (!enumerator.MoveNext())
        {
          // Do nothing
        }
        else
        {
          Append(stringBuilder, enumerator.Current, conv);
          while (enumerator.MoveNext())
          {
            stringBuilder.Append(separator);
            Append(stringBuilder, enumerator.Current, conv);
          }
        }
      }
    }

    private static void Append<T>(StringBuilder builder, T value, Func<T, string> conv = null)
    {
      if (value != null)
      {
        var text = (conv == null ? value.ToString() : conv(value));
        if (text != null) builder.Append(text);
      }
    }
  }
}
