using ICSharpCode.AvalonEdit.CodeCompletion;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class PropertyCompletionFactory
  {
    protected ArasMetadataProvider _metadata;
    protected ItemType _itemType;

    public PropertyCompletionFactory() { }
    public PropertyCompletionFactory(ArasMetadataProvider metadata, ItemType itemType)
    {
      _metadata = metadata;
      _itemType = itemType;
    }

    public IEnumerable<ICompletionData> GetCompletions(IEnumerable<IListValue> p)
    {
      var hash = new HashSet<string>(p.Select(pr => pr.Value), StringComparer.CurrentCultureIgnoreCase);
      return GetCompletions(p, p.Where(pr => !string.IsNullOrWhiteSpace(pr.Label) && !hash.Contains(pr.Label.Replace(' ', '_'))));
    }

    public IPromise<IEnumerable<ICompletionData>> GetPromise(IEnumerable<ICompletionData> buffer = null)
    {
      buffer = buffer ?? Enumerable.Empty<ICompletionData>();
      return _metadata.GetProperties(_itemType)
        .Convert(p => GetCompletions(p).Concat(buffer));
    }

    protected virtual IEnumerable<ICompletionData> GetCompletions(IEnumerable<IListValue> normal, IEnumerable<IListValue> byLabel)
    {
      return normal.Select(i => ConfigureNormalProperty(CreateCompletion(), i))
        .Concat(byLabel.Select(pr => ConfigureLabeledProperty(CreateCompletion(), pr)));
    }

    protected virtual BasicCompletionData CreateCompletion()
    {
      return new BasicCompletionData();
    }
    protected virtual BasicCompletionData ConfigureNormalProperty(BasicCompletionData data, IListValue prop)
    {
      data.Text = prop.Value;
      data.Description = prop.Label;
      return data;
    }
    protected virtual BasicCompletionData ConfigureLabeledProperty(BasicCompletionData data, IListValue prop)
    {
      var label = prop.Label + " (" + prop.Value + ")";
      data.Text = label;
      data.Description = prop.Value;
      data.Content = FormatText.MutedText(label);
      data.Action = () => prop.Value;
      return data;
    }
  }
}
