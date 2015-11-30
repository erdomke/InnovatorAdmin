using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin
{
  public class DatabasePackage
  {
    private IAsyncConnection _conn;

    public DatabasePackage(IAsyncConnection conn)
    {
      _conn = conn;
    }

    public bool Write(InstallScript script,
      Func<string, DatabasePackageAction> errorHandler = null,
      Action<int, string> reportProgress = null)
    {
      var cont = true;
      var typeGroups = from l in script.Lines
                       where l.Type == InstallType.Create
                       group l by l.Reference.Type into typeGroup
                       select typeGroup;
      var cnt = typeGroups.Count();
      var idx = 0;
      var packageGroups = new HashSet<string>();
      string currPackageId = null;

      while (cont)
      {
        IEnumerable<IReadOnlyItem> elements;
        foreach (var typeGroup in typeGroups)
        {
          if (reportProgress != null) reportProgress((int)(idx * 50.0 / cnt), string.Format("Checking for existing package elements ({0} of {1}) ", idx + 1, cnt));

          if (typeGroup.First().Reference.Unique.IsGuid())
          {
            elements = _conn.Apply("<Item type=\"PackageElement\" action=\"get\" select=\"element_id,name,source_id\"><element_type>"
              + typeGroup.Key
              + "</element_type><element_id condition=\"in\">'"
              + typeGroup.Select(i => i.Reference.Unique).Aggregate((p, c) => p + "','" + c)
              + "'</element_id></Item>").Items();
          }
          else
          {
            elements = _conn.Apply("<Item type=\"PackageElement\" action=\"get\" select=\"element_id,name,source_id\"><element_type>"
              + typeGroup.Key
              + "</element_type><element_id condition=\"in\">(select id from innovator.["
              + typeGroup.Key.Replace(' ', '_')
              + "] where "
              + typeGroup.Select(i => i.Reference.Unique).Aggregate((p, c) => p + " or " + c)
              + ")</element_id></Item>").Items();
          }

          packageGroups.UnionWith(elements.Select(e => e.SourceId().Value));
          idx++;
        }

        var packages = _conn.Apply("<Item type=\"PackageDefinition\" action=\"get\" select=\"name\"><id condition=\"in\">(select SOURCE_ID FROM innovator.PACKAGEGROUP where id in ('"
          + packageGroups.Aggregate((p, c) => p + "','" + c)
          + "'))</id></Item>").Items();
        currPackageId = packages.Where(p => p.Property("name").Value == script.Title).SingleOrDefault().Id();

        cont = false;
        if (packages.Any(p => p.Property("name").Value != script.Title))
        {
          if (errorHandler != null)
          {
            var packageList = (from p in packages
                               where p.Property("name").Value != script.Title
                               select p.Property("name").Value)
                              .Aggregate((p, c) => p + ", " + c);
            switch (errorHandler("The package cannot be created because one or more elements exist in the packages: " + packageList))
            {
              case DatabasePackageAction.TryAgain:
                cont = true;
                break;
              case DatabasePackageAction.RemoveElementsFromPackages:
                foreach (var typeGroup in typeGroups)
                {
                  if (reportProgress != null) reportProgress((int)(idx * 50.0 / cnt), string.Format("Removing package elements ({0} of {1}) ", idx + 1, cnt));

                  if (typeGroup.First().Reference.Unique.IsGuid())
                  {
                    elements = _conn.Apply("<Item type=\"PackageElement\" action=\"purge\" where=\"[PackageElement].[element_type] = '"
                      + typeGroup.Key
                      + "' and [PackageElement].[element_id] in ('"
                      + typeGroup.Select(i => i.Reference.Unique).Aggregate((p, c) => p + "','" + c)
                      + "')\" />").Items();
                  }
                  else
                  {
                    elements = _conn.Apply("<Item type=\"PackageElement\" action=\"purge\" where=\"[PackageElement].[element_type] = '"
                      + typeGroup.Key
                      + "' and [PackageElement].[element_id] in (select id from innovator.["
                      + typeGroup.Key.Replace(' ', '_')
                      + "] where "
                      + typeGroup.Select(i => i.Reference.Unique).Aggregate((p, c) => p + " or " + c)
                      + ")\" />").Items();
                  }

                  idx++;
                }


                break;
              default:
                return false;
            }
          }
          else
          {
            return false;
          }
        }
      }

      // Try one more time to get the package
      if (string.IsNullOrEmpty(currPackageId))
      {
        var packages = _conn.Apply("<Item type=\"PackageDefinition\" action=\"get\" select=\"name\"><name>" + script.Title + "</name></Item>");
        currPackageId = packages.AssertItem().Id();
      }

      // Add the package
      if (string.IsNullOrEmpty(currPackageId))
      {
        var packages = _conn.Apply("<Item type=\"PackageDefinition\" action=\"add\" ><name>" + script.Title + "</name></Item>", true);
        currPackageId = packages.AssertItem().Id();
      }

      string groupId;
      foreach (var typeGroup in typeGroups)
      {
        if (reportProgress != null) reportProgress((int)(50 + idx * 50.0 / cnt), string.Format("Adding package elements of type ({0} of {1}) ", idx + 1, cnt));

        groupId = _conn.Apply("<Item type=\"PackageGroup\" action=\"merge\" where=\"[PackageGroup].[source_id] = '"
          +  currPackageId
          + "' and [PackageGroup].[name] = '"
          + typeGroup.Key
          + "'\"><name>"
          + typeGroup.Key
          + "</name></Item>", true).AssertItem().Id();

        foreach (var elem in typeGroup)
        {
          _conn.Apply("<Item type=\"PackageElement\" action=\"merge\" where=\"[PackageElement].[source_id] = '"
            + groupId
            + "' and [PackageElement].[element_id] = '"
            + (elem.InstalledId ?? elem.Reference.Unique)
            + "'\">"
            + "<element_type>" + typeGroup.Key + "</element_type>"
            + "<element_id>" + (elem.InstalledId ?? elem.Reference.Unique) + "</element_id>"
            + "<source_id>" + groupId + "</source_id>"
            + "<name>" + elem.Reference.KeyedName + "</name></Item>").AssertNoError();
        }

        idx++;
      }

      return true;
    }
  }
}
