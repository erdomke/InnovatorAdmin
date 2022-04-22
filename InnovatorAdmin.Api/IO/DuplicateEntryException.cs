using System;
using System.Collections.Generic;
using System.Linq;

namespace InnovatorAdmin
{
  internal class DuplicateEntryException : Exception
  {
    public IEnumerable<InstallItem> Duplicates { get; }

    public DuplicateEntryException(IEnumerable<InstallItem> duplicates)
      : base($"The package has duplicate entries for creating {duplicates.First().Reference.Type} {duplicates.First().Reference.KeyedName} with ID {duplicates.First().Reference.Unique} at {string.Join(", ", duplicates.Select(i => i.Path))}")
    {
      Duplicates = duplicates;
    }
  }
}
