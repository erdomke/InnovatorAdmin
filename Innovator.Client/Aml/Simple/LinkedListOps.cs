using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  static class LinkedListOps
  {
    public static T Add<T>(T lastLink, T newLink) where T : class, ILink<T>
    {
      if (lastLink == null)
      {
        newLink.Next = newLink;
        return newLink;
      }
      else if (string.Compare(newLink.Name, lastLink.Name) >= 0)
      {
        newLink.Next = lastLink.Next;
        lastLink.Next = newLink;
        return newLink;
      }
      else
      {
        T prev;
        var curr = lastLink;

        do
        {
          prev = curr;
          curr = curr.Next;
          if (string.Compare(newLink.Name, curr.Name) < 0)
          {
            newLink.Next = curr;
            prev.Next = newLink;
            return lastLink;
          }
        }
        while (curr != lastLink);

        throw new InvalidOperationException();
      }
    }
    public static IEnumerable<T> Enumerate<T>(T lastLink) where T : class, ILink<T>
    {
      var curr = lastLink;
      if (curr == null)
        yield break;

      do
      {
        curr = curr.Next;
        yield return curr;
      }
      while (curr != lastLink);
    }
    public static T Find<T>(T lastLink, string name) where T : class, ILink<T>
    {
      if (lastLink == null
        || string.Compare(name, lastLink.Name) > 0
        || string.Compare(name, lastLink.Next.Name) < 0)
        return null;

      var curr = lastLink;
      if (string.Equals(curr.Name, name))
        return curr;

      curr = curr.Next;
      while (curr != lastLink && string.Compare(name, curr.Name) >= 0)
      {
        if (string.Equals(curr.Name, name))
          return curr;
        curr = curr.Next;
      }
      return null;
    }
    public static IEnumerable<T> FindAll<T>(T lastLink, string name) where T : class, ILink<T>
    {
      if (lastLink == null
        || string.Compare(name, lastLink.Name) > 0
        || string.Compare(name, lastLink.Next.Name) < 0)
        yield break;

      var curr = lastLink;
      do
      {
        curr = curr.Next;
        if (string.Equals(curr.Name, name))
          yield return curr;
      }
      while (curr != lastLink && string.Compare(name, curr.Name) >= 0);
    }
    public static T Remove<T>(T lastLink, T removeLink) where T : class, ILink<T>
    {
      if (lastLink == null || lastLink == removeLink)
        return null;

      T prev;
      var curr = lastLink;
      do
      {
        prev = curr;
        curr = curr.Next;
        if (curr == removeLink)
        {
          prev.Next = curr.Next;
          return lastLink;
        }
      }
      while (curr != lastLink);

      return lastLink;
    }
  }
}
