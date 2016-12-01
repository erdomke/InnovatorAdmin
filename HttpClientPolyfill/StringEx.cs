namespace System.Net.Http
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  /// <summary>
  /// Provides extension and utility methods for the <see cref="string"/> class.
  /// </summary>
  internal static class StringEx
  {
    /// <summary>
    /// Indicates whether a specified string is <see langword="null"/>, empty, or consists only of white-space characters.
    /// </summary>
    /// <param name="value">The string to test.</param>
    /// <returns><see langword="true"/> if the <paramref name="value"/> is null or <see cref="string.Empty"/>, or if <paramref name="value"/> consists exclusively of white-space characters.</returns>
    public static bool IsNullOrWhiteSpace(string value)
    {
      return string.IsNullOrEmpty(value) || value.Trim().Length == 0;
    }

    /// <summary>
    /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type <see cref="String"/>, using the specified separator between each member.
    /// </summary>
    /// <remarks>
    /// If <paramref name="separator"/> is <see langword="null"/>, an empty string (<see cref="String.Empty"/>) is used instead. If any member of <paramref name="values"/> is <see langword="null"/>, an empty string is used instead.
    /// </remarks>
    /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="values"/> contains more than one element.</param>
    /// <param name="values">A collection that contains the strings to concatenate.</param>
    /// <returns>A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/> string. If <paramref name="values"/> has no members, the method returns <see cref="String.Empty"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="values"/> is <see langword="null"/>.</exception>
    public static string Join(string separator, IEnumerable<string> values)
    {
      if (values == null)
        throw new ArgumentNullException("values");

      StringBuilder builder = new StringBuilder();
      bool first = true;
      foreach (string value in values)
      {
        if (first)
          first = false;
        else
          builder.Append(separator);

        builder.Append(value);
      }

      return builder.ToString();
    }

    /// <summary>
    /// Concatenates the elements of an object array, using the specified separator between each element.
    /// </summary>
    /// <remarks>
    /// If <paramref name="separator"/> is <see langword="null"/> or if any element of values other than the first element is <see langword="null"/>, an empty string (<see cref="String.Empty"/>) is used instead. See the Notes for Callers section if the first element of <paramref name="values"/> is <see langword="null"/>.
    /// <para>
    /// <see cref="Join(String, Object[])"/> is a convenience method that lets you concatenate each element in an object array without explicitly converting its elements to strings. The string representation of each object in the array is derived by calling that object's <see cref="ToString"/> method.
    /// </para>
    /// <note type="caller">
    /// If the first element of <paramref name="values"/> is <see langword="null"/>, the <see cref="Join(String, Object[])"/> method does not concatenate the elements in <paramref name="values"/> but instead returns <see cref="String.Empty"/>.
    /// </note>
    /// </remarks>
    /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="values"/> contains more than one element.</param>
    /// <param name="values">An array that contains the elements to concatenate.</param>
    /// <returns>A string that consists of the elements of <paramref name="values"/> delimited by the <paramref name="separator"/> string. If <paramref name="values"/> is an empty array, the method returns <see cref="String.Empty"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="values"/> is <see langword="null"/>.</exception>
    public static string Join(string separator, params object[] values)
    {
      if (values == null)
        throw new ArgumentNullException("values");

      if (values.Length > 0 && values[0] == null)
        return string.Empty;

      return Join(separator, values.AsEnumerable());
    }

    /// <summary>
    /// Concatenates the members of a collection, using the specified separator between each member.
    /// </summary>
    /// <remarks>
    /// If <paramref name="separator"/> is <see langword="null"/>, an empty string (<see cref="String.Empty"/>) is used instead. If any member of <paramref name="values"/> is <see langword="null"/>, an empty string is used instead.
    /// <para>
    /// <see cref="Join{T}"/> is a convenience method that lets you concatenate each member of an <see cref="IEnumerable{T}"/> collection without first converting them to strings. The string representation of each object
    /// in the <see cref="IEnumerable{T}"/> collection is derived by calling that object's <see cref="ToString"/> method.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the members of <paramref name="values"/>.</typeparam>
    /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="values"/> contains more than one element.</param>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <returns>A string that consists of the members of <paramref name="values"/> delimited by the <paramref name="separator"/> string. If <paramref name="values"/> has no members, the method returns <see cref="String.Empty"/>.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="values"/> is <see langword="null"/>.</exception>
    public static string Join<T>(string separator, IEnumerable<T> values)
    {
      if (values == null)
        throw new ArgumentNullException("values");

      IEnumerable<string> stringValues = values.Select(i => i != null ? i.ToString() : null);
      return Join(separator, stringValues);
    }
  }
}
