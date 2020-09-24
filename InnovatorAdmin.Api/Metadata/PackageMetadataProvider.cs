using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  public class PackageMetadataProvider : IArasMetadataProvider
  {
    private readonly List<Method> _methods = new List<Method>();
    private readonly Dictionary<string, ItemType> _itemTypesByName
      = new Dictionary<string, ItemType>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, Sql> _sql
      = new Dictionary<string, Sql>(StringComparer.OrdinalIgnoreCase);
    private readonly ItemReference[] _systemIdentities;
    private readonly Dictionary<string, string> _propertyNames
      = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DatabaseList> _listsById
      = new Dictionary<string, DatabaseList>();

    /// <summary>
    /// Enumerable of all item types
    /// </summary>
    public IEnumerable<ItemType> ItemTypes => _itemTypesByName.Values;

    public IEnumerable<ItemReference> SystemIdentities => _systemIdentities;

    public IEnumerable<Method> Methods => _methods;

    public string Title { get; private set; }

    public bool CustomPropertyByPath(ItemProperty path, out ItemReference propRef)
    {
      propRef = null;
      return false;
    }

    /// <summary>
    /// Try to get an Item Type by name
    /// </summary>
    public bool ItemTypeByName(string name, out ItemType type)
    {
      return _itemTypesByName.TryGetValue(name, out type);
    }

    public bool PropById(string id, out string name)
    {
      return _propertyNames.TryGetValue(id, out name);
    }

    /// <summary>
    /// Try to get SQL information from a name
    /// </summary>
    public bool SqlRefByName(string name, out ItemReference sql)
    {
      Sql buffer;
      sql = null;
      if (_sql.TryGetValue(name, out buffer))
      {
        sql = buffer;
        return true;
      }
      return false;
    }

    public PackageMetadataProvider()
    {
      _systemIdentities = ElementFactory.Local.FromXml(@"<Result>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='8FE5430B42014D94AE83246F299D9CC4'>
    <id keyed_name='Creator' type='Identity'>8FE5430B42014D94AE83246F299D9CC4</id>
    <name>Creator</name>
  </Item>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='DBA5D86402BF43D5976854B8B48FCDD1'>
    <id keyed_name='Innovator Admin' type='Identity'>DBA5D86402BF43D5976854B8B48FCDD1</id>
    <name>Innovator Admin</name>
  </Item>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='9200A800443E4A5AAA80D0BCE5760307'>
    <id keyed_name='Manager' type='Identity'>9200A800443E4A5AAA80D0BCE5760307</id>
    <name>Manager</name>
  </Item>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='538B300BB2A347F396C436E9EEE1976C'>
    <id keyed_name='Owner' type='Identity'>538B300BB2A347F396C436E9EEE1976C</id>
    <name>Owner</name>
  </Item>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='6B14D33C4A7D41C188CCF2BC15BD01A3'>
    <id keyed_name='Super User' type='Identity'>6B14D33C4A7D41C188CCF2BC15BD01A3</id>
    <name>Super User</name>
  </Item>
  <Item type='Identity' typeId='E582AB17663F4EF28460015B2BE9E094' id='A73B655731924CD0B027E4F4D5FCC0A9'>
    <id keyed_name='World' type='Identity'>A73B655731924CD0B027E4F4D5FCC0A9</id>
    <name>World</name>
  </Item>
</Result>")
      .Items()
      .Select(i =>
      {
        var itemRef = ItemReference.FromFullItem(i, false);
        itemRef.KeyedName = i.Property("name").AsString("");
        return itemRef;
      })
      .ToArray();
    }

    public void Add(XElement xml)
    {
      foreach (var item in ElementFactory.Local.FromXml(xml).Items())
      {
        Add(item);
      }
    }

    public void AddRange(IEnumerable<IReadOnlyItem> items)
    {
      foreach (var item in items
        .OrderBy(i => string.Equals(i.TypeName(), "relationshiptype", StringComparison.OrdinalIgnoreCase)
          || string.Equals(i.TypeName(), "morphae", StringComparison.OrdinalIgnoreCase) ? 99 : 0))
        Add(item);
    }

    public void Add(IReadOnlyItem item)
    {
      if (item.Attribute(XmlFlags.Attr_ScriptType).HasValue())
        return;

      switch (item.TypeName().ToLowerInvariant())
      {
        case "method":
          _methods.Add(new Method(item, _coreIds.Contains(item.ConfigId().Value ?? item.Id())));
          break;
        case "relationshiptype":
          if (item.Property("relationship_id").HasValue())
          {
            var relType = new ItemType(item, null, true);
            if (string.IsNullOrEmpty(relType.Name))
              return;
            _itemTypesByName[relType.Name] = relType;
            var source = _itemTypesByName.Values
              .FirstOrDefault(i => i.Id == item.SourceId().Value);
            if (source != null)
              source.Relationships.Add(relType);
          }
          break;
        case "itemtype":
          var itemType = new ItemType(item, null, item.Property("name").HasValue());
          if (!string.IsNullOrEmpty(itemType.Name) && !_itemTypesByName.ContainsKey(itemType.Name))
          {
            _itemTypesByName[itemType.Name] = itemType;
            AddRange(item.Relationships("Property"));
          }
          else if (!string.IsNullOrEmpty(itemType.Id))
          {
            _itemTypesByName.Values
              .FirstOrDefault(i => i.Id == itemType.Id)
              ?.WithScripts(item);
          }
          break;
        case "sql":
          var sql = Sql.FromFullItem(item, false);
          sql.KeyedName = item.Property("name").Value
            ?? item.KeyedName().Value
            ?? item.IdProp().KeyedName().Value
            ?? "";
          sql.Type = item.Property("type").AsString("");
          if (string.IsNullOrEmpty(sql.KeyedName))
            return;
          _sql[sql.KeyedName.ToLowerInvariant()] = sql;
          break;
        case "property":
          _propertyNames[item.Id()] = item.Property("name").Value
            ?? item.KeyedName().Value
            ?? item.IdProp().KeyedName().Value;
          break;
        case "list":
          _listsById[item.Id()] = new DatabaseList(item);
          break;
      }
    }

    public static PackageMetadataProvider FromFile(string path)
    {
      if (string.Equals(System.IO.Path.GetExtension(path), ".mf", StringComparison.OrdinalIgnoreCase))
        return FromPackage(new ManifestFolder(path));
      else
        return FromPackage(InnovatorPackage.Load(path));
    }

    public static PackageMetadataProvider FromPackage(ManifestFolder package)
    {
      var doc = package.Read(out var title);
      var metadata = new PackageMetadataProvider() { Title = title };
      metadata.AddRange(doc.SelectNodes("/AML/AML/Item")
        .OfType<System.Xml.XmlElement>()
        .Select(e => ElementFactory.Local.FromXml(e).AssertItem()));
      return metadata;
    }

    public static PackageMetadataProvider FromPackage(InnovatorPackage package)
    {
      var script = package.Read();
      var metadata = new PackageMetadataProvider() { Title = script.Title };
      metadata.AddRange(script.Lines.Select(i => ElementFactory.Local.FromXml(i.Script).AssertItem()));
      return metadata;
    }

    // Loaded from 11sp9
    private static readonly HashSet<string> _coreIds = new HashSet<string>(new string[]
    {
      "61346634B3DC4D51964CD7AD988051D7",
      "4E5CD26D9D8E4B92A27307B87556D32D",
      "3D684C9D28A14A7EB408195FCF464389",
      "8983DFE74A1D46F6996413C072DC1802",
      "2BA15636181345E3AC88FB0108765F09",
      "C01483311E4749FE877A64801BD52370",
      "CC00D78FF62649A88435AA24DDFE8270",
      "797973047F3045888340E8B477448A7D",
      "5453FAA8A1A14316AD2D38D4AC7383BF",
      "70512B7E579948C39BF826B6E71A8710",
      "C937237753194F249623A37A56465A3B",
      "D81066A390E54E1A88A8EFF03ECA3CC0",
      "A48720B96EB64147A392060806967796",
      "DC196FC44D124584827EC8538CBD5381",
      "FA53646205B64F9599462CD65E799432",
      "24C737574FA44FC4B6F4A3282C1B8B5D",
      "277862E0981E4ADD91FF4BBDA6B776D2",
      "6409F2A814914F5EA49FAF5EA6FFE7B6",
      "5723382025DA43D88FFC1A0835105720",
      "631C4070A10C4EB881D8289EC1B6E6C3",
      "81609BA70FBC4B9EAA30129CAE69C11D",
      "A18EAEA1BBDF4878A10EA94F924317DC",
      "EC0D87C174814CB7BA574593456113F7",
      "4134C7C54DBF43B3BF749B289A769851",
      "DA854164964D4DD796C2C00DB1AA779F",
      "CC30E72FB0DF47E0AA2941997E8B0449",
      "3D1911B178AC466C99393A39D0E80592",
      "787E03F8264446688AEC57AC1233DA59",
      "E48CA3EAB7F044A99D4B9AEB6084746C",
      "176A7CA23A034D86A2E995859F9BAB5B",
      "F8C2EF4B573D40B58DAA5C5E1A129E6F",
      "11E9A8E547354EBBB7ABCE730C825F19",
      "F7E616B8C8BA427E8B05A7660920E666",
      "6D497A0F23654C2190DE5A42F9118FAB",
      "E9F352498F624672ABC1C44C96B81B0B",
      "B118DC20697745BDB2154126E3B0748D",
      "1A3633033ED14CDAB5014B47A932E168",
      "95C541B219E7421DB420BAE02D5F6BA9",
      "33D6A6BA26AD4B85B35127A53018B565",
      "BDA02A8FC5704D7884E10AD6A2556944",
      "AEFCD3D2DC1D4E3EA126D49D68041EB6",
      "483228BE6B9A4C0E99ACD55FDF328DEC",
      "937CE47DE2854308BE6FF5AB1CFB19D4",
      "85924010F3184E77B24E9142FDBB481B",
      "90D8880C29CD45C3AA8DAFF1DAEBC60E",
      "BF3EAD31AAA2403592CAAC2446FF7797",
      "46BDE53304404C28B5C45610E41C1DD5",
      "BD4A250787A742A484C7B174A4AED1E2",
      "7A3EFE7242DB4403965890C053A57A0B",
      "96C238700CD840DEBE512EE85D440AF3",
      "67735DF455F54736A9D51CB53AB129E3",
      "05A68B0BC74C47A6A2FD4404A73C815F",
      "E32DB4E6E3B64F98A12B550C578E6A01",
      "FA1755A31ACF4EDFBBFFCD6A8C6F7AF8",
      "8B58025F8E504DBDADF0E1176D3CE178",
      "CB7696CC33EC4D1BB98716465F1AD580",
      "4E355E04444B4676AE723B43DECA37DC",
      "DB54505FA3E9419DA3C1E1AFB7A48C1C",
      "10B8BB84EEE9413AAD071C8341BBAB04",
      "6B9057491021453ABA0A425570CC10D2",
      "BF60433C7E924BE6B78D901809F8FEF6",
      "3A65F41FF1FC42518A702FDA164AF420",
      "432E29895A994D0DBC9DF9B0918E189F",
      "0EA142227FAE40148A4BDD8CF1D450EF",
      "789104C736664FAA9748352CBBF86BCA",
      "F91DE6AE038A4CC8B53D47D5A7FA49FC",
      "1549FB139D6B4AD99FDC1ED4B8011DB9",
      "0BB5B81FEB37475BB9C779408080DB61",
      "3572C4D1479445D9959C413624A9FFF6",
      "8052A558B9084D41B9F11805E464F443",
      "13EC84A626F1457BB5F60A13DA03580B",
      "BCC8053D365143A18B033850EFE56F3C",
      "47573682FB7549F59ADECD4BFE04F1DE",
      "96730C617F4C40729D730D97395FD620",
      "AC218DF3AE43488C9E64E1AA551D2522",
      "CA289ED4F1A84A9EB6CDD822846FD745",
      "8EABD91B465443F0A4995418F483DC51",
      "C24A87B33E3740B3B01254DC776F1EFE",
      "E2DECCA6300E4815B466C62C66E9D3AF",
      "1718DC9FB25043B9A3F0B76DB5DC6637",
      "ACD5116613FF40C9BC5EF7447CBEBBCB",
      "213B74E721ED457C9BE735C038C7CB95",
      "B70DE4074ABC49C69B0D8729D9212982",
      "6323EE2C82E94CC4BB83779DDFDDD6F5",
      "06E0660816FE40A2BF1411B2280062B3",
      "E582AB17663F4EF28460015B2BE9E094",
      "6C280692633D4498BD9CFEA7989138AB",
      "5BD6BED0CD794A078AA42476F47ECF46",
      "450906E86E304F55A34B3C0D65C097EA",
      "AC32527D85604A4D9FC9107C516AEF47",
      "5EFB53D35BAE468B851CD388BEA46B30",
      "1E764495A5134823B30060D83FD6A2F9",
      "5736C479A8CB49BCA20138514C637266",
      "5698BACD2A7A45D6AC3FA60EAB3E6566",
      "8EF041900FE2428AABD404063AB979B6",
      "7348E620D27E40D1868C54247B5DE8D1",
      "87879A09B8044DE380D59DF22DE1867F",
      "5ED4936039F64E9E99F703F8F46A1DB1",
      "C6A89FDE1294451497801DF78341B473",
      "26D7CD4E033242148E2724D3D054B4D3",
      "FF5EFC69BB8F4E56839A43A8477AE58F",
      "C57EC6AAFE22490082F06FD8DCBE2E1C",
      "471932C33B604C3099070F4106EE5024",
      "F0834BBA6FB64394B78DF5BB725532DD",
      "F81CDEF9FE324D01947CC9023BC38317",
      "E5BC8090E82D4F8D8E3F389C95316433",
      "DA581F2EAD1641CF976A1F1211E1ADBA",
      "D2ECF1B2A7FC45DB9D916996CE9BE9C9",
      "2B46201802CE46708C269667DB4798AC",
      "8214ECDE53F04AFC95243E10B2C7BBD4",
      "D2794EA7FB7B4B52BA2CE4681E2D9DFB",
      "F0B2EAE5414249748F2986CC1EE78340",
      "F0CC0E7AEC0A401A94E8A63C9AC4F4D3",
      "38C9CE2A4E06401DABF942E1D0224E87",
      "F356C4CED1584EBF812912F2D926066B",
      "1DA22B3D6F12458290F8549165B490EC",
      "8E5FA57F5168436BA998A70CB2C7F259",
      "45E899CD2859442982EB22BB2DF683E5",
      "122BF06C8B8E423A9931604DD939172F",
      "8651DCAB4D714EF6AA747BB8F50719BA",
      "6DAB4ACC09E6471DB4BDD15F36C3482B",
      "8FC29FEF933641A09CEE13A604A9DC74",
      "8EFCE40BCB74478B8254CEB594CE8774",
      "2D700440AC084B99AD123528BAE67D29",
      "602D9828174C48EBA648B1D261C54E43",
      "9E212D4ED3C64493B631EE15D0A62AF7",
      "B19D349CC6FC44BC97D50A6D70AE79CB",
      "42A80AD3F88443F785C005BAF2121E01",
      "97F00180EC8442B3A1CB67E6349D7BDE",
      "AE7AC22E64D746B69F970EA1EC65DB05",
      "7937E5F0640240FDBBF9B158F45F4F6C",
      "FE597F427BF84EC783435F4471520403",
      "261EAC08AE9144FC95C49182ACE0D3FE",
      "F3F07BFCCDDF48E79ED239F0111E4710",
      "34682D3EB66141ECACC8796C9D3A42B8",
      "4245947FE6244E37982F46D2FA46D74E",
      "30F30E1181BA43FE99706038200EFEBF",
      "40F45DEED2D84BA58B223F556FA25617",
      "385D22BE07F64985A7895EDBB611E10F",
      "C92F589A534241B09EBD4FE0ECD903E2",
      "5B9EC188C193425D86A05FA772EBC5E3",
      "A9A7E355BD8141478B32BEB2E6140098",
      "3A4758DC9DE14F98BB3D1837D9921639",
      "579877E00D244DA8A1CE95215BBFFEB2",
      "A0D137DDD4484250BF5323E37BD3C225",
      "77DF00235A4049A48D5C6A50A4682E5D",
      "C3BC3A50ED2C4BC7AC4DCA04AC48057F",
      "67F33A3C251747AA9A10214787BECFBE",
      "921950E061FD4604BE8C89E9976CBB7D",
      "6A688F84FB7B48EDBFDA7C9C053EAFFE",
      "41F4BEB598464C9F915E12ECAFCEE145",
      "C9D16426CE034B28800C661FE90FB345",
      "5EE97A61442B4391A5EDB676274A16C6",
      "CA30A1ADE4CB40A3ACE53D46CC53E7BE",
      "63E89E7FA3AE4CB0A3B0176A70652864",
      "BAE8089456F74B3BBC793A473E581FC6",
      "5D9124E259EE402BBBBECA7634CE5A0E",
      "18671D11A7D743A389DD656A1446DE5A",
      "329F8A07E7024481AEE224CC553BEA07",
      "B544726E528B4F28AE9CB3B7B3DB20B6",
      "A956968848A94CB5B08B4F5F2131947D",
      "D55863B80B9048C18523E1C70C3320E1",
      "778F4966401C4A3AB5DCB009A46008BD",
      "2248BB06C1F84CEDAC7912B9D55256C6",
      "544CE6CD3E154C32B5DF9E5AFB21EEEF",
      "4F237598C8B7423D98F4E84427559C80",
      "632AFE92E2D243B0BFFADFE1E0119CB7",
      "B5E8B6D8983E4EC2BCC8B74135F6027E",
      "1EB10F2599644CBC85029BCF9E6B7248",
      "0A5C3D26B1C14BC4B28D643DDEAFAA98",
      "26AF432F7B394F0287FBD549B7A38825",
      "0757FE2F153F42E0854AADBB813339EC",
      "96C4009D4CB54A56A236139E90E3FE48",
      "C0351BDEAEC04B4F9542F787D8FE8BE5",
      "63D1EF1C149B4A05A75B4FDE0E027ACD",
      "49C8B38BCE7040E69B0F3E8EFE055002",
      "C0753BF9614B4184989A3E12791250C9",
      "497BB04B15C0445F9710016C24222F98",
      "E9B2C6D32CFC4987B225F28A19E34988",
      "49DF321A41344E69A990901ECBB91E50",
      "D98433ED30FF48CDA8D2A84E846EC2DF",
      "D4051D4EE4A64EEFA4B661E5F6E325D1",
      "E066B5A2985144F98922A0F1239444CA",
      "0387FDF536AC4D18AF37CBF03DC53DBA",
      "5B86E53360B2405E80BFAC04751A0D47",
      "E07AD7930CA54CC989DF0437608AF35F",
      "72AA38D95002437BAB6548F2DB62BD5E",
      "A1C04F0A132240808082FF73CD35DBEF",
      "B660D634F8AE4ED1AB58FA24FD05A250",
      "ADBF7775FD244855B3F605DB83E475BE",
      "75015B6FDAA847E29DCC9EA1CD9EA5F9",
      "3C51DE7A7B74465EAFC9EB6382F20369",
      "19EB203FFD8B439D8C23023F9DD1F953",
      "A7CEB8E754C9419A8EAFAA9BE428E78D",
      "8A40526E17E8449EA67698FDD3BEB374",
      "672041E2909F4F2ABA1EC7BBC3F32080",
      "888CBB5016284E8692D22380F4ED74EF",
      "02AD70C867FE416599D884032226ECC6",
      "8077764B09654665A5F862266D4F6FAE",
      "E399ED2801A94149B40C5788FE260B86",
      "0DA90A3171434AD38569A4B33DDE8A0E",
      "2191DD1541404EBFB23F64E116F9B69C",
      "27206F72FED74C429C8D4B9726E4DE8C",
      "2A18C4ED7C05419591766276B388B485",
      "A4C176FC037B4C0C8C5F9B04144EEAA2",
      "A780BB99FA284EAF9F5B9EFF6458A220",
      "8D514EBD6C7E4073BA9C28369746DC67",
    });
  }
}
