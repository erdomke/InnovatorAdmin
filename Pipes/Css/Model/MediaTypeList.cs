using System.Collections;
using System.Collections.Generic;
using Pipes.Css.Model.Extensions;
using Pipes.Css.Model;

// ReSharper disable once CheckNamespace
namespace Pipes.Css
{
  public class MediaTypeList : IEnumerable<MediaDefinition>
  {
    private readonly List<MediaDefinition> _media;

    internal MediaTypeList()
    {
      _media = new List<MediaDefinition>();
    }

    public MediaDefinition this[int index]
    {
      get
      {
        if (index < 0 || index >= _media.Count)
        {
          return null;
        }

        return _media[index];
      }
      set
      {
        _media[index] = value;
      }
    }

    public int Count
    {
      get { return _media.Count; }
    }

    internal MediaTypeList AppendMedium(MediaDefinition newMedium)
    {
      _media.Add(newMedium);
      return this;
    }

    public override string ToString()
    {
      return ToString(false);
    }

    public string ToString(bool friendlyFormat, int indentation = 0)
    {
      return friendlyFormat
          ? _media.GroupConcat(", ", m => m.ToString(friendlyFormat, indentation))
          : _media.GroupConcat(",", m => m.ToString(friendlyFormat, indentation));
    }

    public IEnumerator<MediaDefinition> GetEnumerator()
    {
      return _media.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}