using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin
{
  public class DependencySorter
  {
    /// <summary>
    /// Indicates that the item should be sorted as the first item of it's group / type
    /// </summary>
    public HashSet<string> FirstOfGroup { get; } = new HashSet<string>();

    public async Task<IEnumerable<InstallItem>> SortByDependencies(IEnumerable<InstallItem> items, IAsyncConnection conn)
    {
      var metadata = ArasMetadataProvider.Cached(conn);
      await metadata.ReloadTask().ConfigureAwait(false);
      return SortByDependencies(items, metadata);
    }

    public IEnumerable<InstallItem> SortByDependencies(IEnumerable<InstallItem> items, IArasMetadataProvider metadata, int maxLoops = 10)
    {
      int loops = 0;
      var state = CycleState.ResolvedCycle;
      var results = items ?? Enumerable.Empty<InstallItem>();
      var analyzer = new DependencyAnalyzer(metadata);
      while (loops < maxLoops && state == CycleState.ResolvedCycle)
      {
        if (loops > 0)
          analyzer.Reset();

        foreach (var newInstallItem in items)
        {
          analyzer.AddReferenceAndDependencies(newInstallItem);
        }
        analyzer.FinishAdding();

        results = GetDependencyList(analyzer, items, out state).ToList();
        loops++;
      }

      results = results
        .Where(i => !i.IsDelete())
        .Concat(results
          .Where(i => i.IsDelete())
          .OrderByDescending(DefaultInstallOrder)
        ).ToArray();

      return results;
    }

    internal IEnumerable<InstallItem> GetDependencyList(DependencyAnalyzer dependAnalyzer
      , IEnumerable<InstallItem> values, out CycleState cycleState)
    {
      cycleState = CycleState.NoCycle;
      var lookup = (from i in values
                    group i by i.Reference into refGroup
                    select refGroup)
                  .ToDictionary(i => i.Key, i => (IEnumerable<InstallItem>)i);

      IEnumerable<ItemReference> sorted = null;
      IList<ItemReference> cycle = new List<ItemReference>() { null };
      List<XmlNode> refs;

      // Bias the install order initially to try and help the dependency sort properly handle anything
      // where explicit dependencies don't exist.  Then, perform the dependency sort.
      var initialSort = lookup.Keys
        .OrderBy(DefaultInstallOrder)
        .ThenBy(i => i.Type.ToLowerInvariant())
        .ThenBy(i => i.Unique)
        .ToList();
      sorted = initialSort.DependencySort<ItemReference>(d =>
      {
        IEnumerable<InstallItem> res = null;
        if (lookup.TryGetValue(d, out res))
        {
          return res.SelectMany(r =>
          {
            var ii = r as InstallItem;
            if (ii == null) return Enumerable.Empty<ItemReference>();
            return dependAnalyzer.GetDependencies(ii.Reference);
          });
        }

        return Enumerable.Empty<ItemReference>();
      }, ref cycle, false);

      // Attempt to remove cycles by identifying a Relationships tag in one of the cycles
      // and moving the Relationships to the top level
      if (cycle.Count > 0 && (cycle[0] != null || cycle.Count > 1))
      {
        cycleState = CycleState.UnresolvedCycle;
        for (int i = (cycle[0] == null ? 2 : 1); i < cycle.Count; i++)
        {
          refs = dependAnalyzer.GetReferences(cycle[i - 1], cycle[i]).ToList();
          if (refs.Count == 1)
          {
            var relTag = refs[0].Parents().FirstOrDefault(e => e.LocalName == "Relationships");
            if (relTag != null)
            {
              var parentTag = refs[0].Parents().Last(e => e.LocalName == "Item").Parent();
              foreach (var child in relTag.Elements().ToList())
              {
                child.Detach();
                parentTag.AppendChild(child);
                var sourceId = (XmlElement)child.AppendChild(child.OwnerDocument.CreateElement("source_id"));
                sourceId.SetAttribute("type", relTag.Parent().Attribute("type"));
                sourceId.SetAttribute("keyed_name", relTag.Parent().Attribute("_keyed_name"));
                sourceId.InnerText = relTag.Parent().Attribute("id");
              }
              relTag.Detach();
              cycleState = CycleState.ResolvedCycle;
              //return Enumerable.Empty<InstallItem>();
            }
          }
        }
      }

      var result = new List<InstallItem>();
      IEnumerable<InstallItem> items = null;
      foreach (var sort in sorted)
      {
        if (lookup.TryGetValue(sort, out items))
        {
          foreach (var item in items)
            result.Add(item);
        }
        else
        {
          result.Add(InstallItem.FromDependency(sort));
        }
      }

      return result;
    }

    private int DefaultInstallOrder(InstallItem line)
    {
      var itemRef = line.Reference;
      if (line.Reference.Type == InstallItem.ScriptType)
        itemRef = ItemReference.FromElement(line.Script);
      return DefaultInstallOrder(itemRef);
    }

    private static HashSet<string> _coreItemTypeIds = new HashSet<string>()
    {
      "483228BE6B9A4C0E99ACD55FDF328DEC",
      "937CE47DE2854308BE6FF5AB1CFB19D4",
      "7A3EFE7242DB4403965890C053A57A0B",
      "6B9057491021453ABA0A425570CC10D2",
      "E8335FB6E4834AFF9BA8F9D4C6E3DE2C",
      "1DD844998D3941D1BB8680612D9B4B0C",
      "5D9A5583B5A54CA78586FDBF9716E53A",
      "FC1A83F673B542C7AC8EEC29F25C602E",
      "EFC5E36233D541FDA5B23B7BD374653C",
      "D003393AB1E447B3A8D3B274CAA70F69",
      "3538F62649F3477EA1F8990EB20F88B9",
      "1675E79E847F4D3E9408B57EE7DA69EF",
      "B228811F241240FE8962E14AD584F2AD",
      "94E345F15EB94D86ADE8FF1B6AE2B439",
      "550DEE262619410A82F8F7378D6184D0",
      "D3F830490AB143E4ACC00D16538C6DB0",
      "2C95B39863F346EFB224DCB85390F465",
      "7BFF05B795FA4C1293C0B68C282F78F1",
      "BF62E8B6450447D89EF86B539F49F992",
      "31392C4909A249BF9EA143915F3F9553",
      "B5175846B27145EEA5653DA35ED78BE4",
      "51F1815885E3477D8B6385A69236C5AA",
      "A16A77E9E9B84559AA435361BAB057F7",
      "6DF95CF17D824F5DAC6A996FFC34D5F8",
      "837D345236384B5DAE4B52042419F966",
      "05DF56FF833542F98251528F3FFE2FA0",
      "02FA838247DF47C2BB85AAB299E646B2",
      "0300B828CBEE4610B77C41377209C900",
      "0DE14E76AA794A039DA8D2CDC34E6B1D",
      "645774D6072F41FD8F998C861E211741",
      "4DAD707F62B54823AE2E4730BB00C649",
      "09586A35D3534D5EAD92B94CB1036AEF",
      "F91DE6AE038A4CC8B53D47D5A7FA49FC",
      "CE0C4143D35E46CDA3874C4339F159BE",
      "8052A558B9084D41B9F11805E464F443",
      "41EF49EFD2ED4F6EAB04C047681F33AC",
      "45F28B3088E74905913152E9BE3B9B12",
      "68D1EEEAA8B84B75962D3008C00E2280",
      "2A69EFFD543B4F74AB5AE964FE83203D",
      "B8940FB948604232B27FC1263FC7E203",
      "13EC84A626F1457BB5F60A13DA03580B",
      "47573682FB7549F59ADECD4BFE04F1DE",
      "2E25B49E218A45D28D0C7D3C0633710C",
      "8AB5CEB9824B4CF7920AFED29F662C66",
      "8EABD91B465443F0A4995418F483DC51",
      "1718DC9FB25043B9A3F0B76DB5DC6637",
      "B2D9F59116B944CE9151C7C05F4D946C",
      "06E0660816FE40A2BF1411B2280062B3",
      "BA053C68D62E4E5293039323F10E116E",
      "38CAFF4302AC45EBAF91EE4DCE4948C7",
      "E582AB17663F4EF28460015B2BE9E094",
      "BC7977377FFF40D59FF14205914E9C71",
      "450906E86E304F55A34B3C0D65C097EA",
      "030019FA30FE40FEB5E32AD2FC9B1F20",
      "AC32527D85604A4D9FC9107C516AEF47",
      "5736C479A8CB49BCA20138514C637266",
      "137D24DFD9AC4D0CA2ABF8D90346AABB",
      "F8F19489113C487B860733E7F7D5B12D",
      "87879A09B8044DE380D59DF22DE1867F",
      "1C5BA0A1491843378E48FF481F6F1DF1",
      "0B9D641B40D24036A117D911558CBDCE",
      "F312562D6AD948DCBCCCCF6A615EE0EA",
      "C6A89FDE1294451497801DF78341B473",
      "7C63771EBC8D46FE8E902C5188033515",
      "AD09B1279AC246BB9EE39BD153D28586",
      "80881F5852BC439E9F3CF0AEC03ABE2A",
      "2DAC2B407B0043A692905CF6A94296A8",
      "98FF7C1BFDFA43448B1EC5A95EA13AEA",
      "F0834BBA6FB64394B78DF5BB725532DD",
      "F81CDEF9FE324D01947CC9023BC38317",
      "18C15AB147F84834874F2E0CB6B8B4C0",
      "E5BC8090E82D4F8D8E3F389C95316433",
      "DA581F2EAD1641CF976A1F1211E1ADBA",
      "B5048D604A6A4D53B3FC6C3BF3D81157",
      "BB3394EA2B014A6493267E7867B4ABD7",
      "920AAA3EAE684F6E99A0EAB95516D064",
      "A46890D3535C41D4A5D79240B8C373B0",
      "DF17056D3AC1479DBC7196255105D04B",
      "2B46201802CE46708C269667DB4798AC",
      "42EA5C9FEC6B49CD8E2784E9E846EAFE",
      "E5326B5A2B93464A9795B1F8A6E6B666",
      "FC3E32F18F804FD9BE4B175973D29112",
      "B7DF834246F24F10BC9B91056D828538",
      "E7A68175B2024FFB876E87D0081071A9",
      "B30032741C894BB086148DDB551D3BEE",
      "2F4E2B53BFBA4351BFFCEE0E438ECF97",
      "FF53A19A424D4B2F80938A5A5C1A29EA",
      "83EBCDAE9D834E169ABC95CC0C7CCB28",
      "CC23F9130F574E7D99DF9659F27590A6",
      "63EC2E6B69FC4FB09859077EE073D9A5",
      "ED9F61ADA4334D7D94361F426C081DB5",
      "E4A23B0AC84D4155BF4C1E44B84CBD45",
      "DE828FBA99FF4ABB9E251E8A4118B397",
      "DD54C11BF6004B09A9E152AFD61ABEA9",
      "45E899CD2859442982EB22BB2DF683E5",
      "122BF06C8B8E423A9931604DD939172F",
      "6DAB4ACC09E6471DB4BDD15F36C3482B",
      "8FC29FEF933641A09CEE13A604A9DC74",
      "602D9828174C48EBA648B1D261C54E43",
      "B19D349CC6FC44BC97D50A6D70AE79CB",
      "261EAC08AE9144FC95C49182ACE0D3FE",
      "321BD822949149C597FD596B1212B85C",
      "EDC5F1D5759D4C7CBDC7F8C20D76087D",
    };

    private static HashSet<string> _coreRelationshipTypeIds = new HashSet<string>()
    {
      "AEFCD3D2DC1D4E3EA126D49D68041EB6",
      "85924010F3184E77B24E9142FDBB481B",
      "90D8880C29CD45C3AA8DAFF1DAEBC60E",
      "BF3EAD31AAA2403592CAAC2446FF7797",
      "46BDE53304404C28B5C45610E41C1DD5",
      "BD4A250787A742A484C7B174A4AED1E2",
      "96C238700CD840DEBE512EE85D440AF3",
      "67735DF455F54736A9D51CB53AB129E3",
      "05A68B0BC74C47A6A2FD4404A73C815F",
      "E32DB4E6E3B64F98A12B550C578E6A01",
      "F88A91D29C4F446BB309BEEE925AADD1",
      "FA1755A31ACF4EDFBBFFCD6A8C6F7AF8",
      "EB4ADB2BC83C410FB265CB42ED5C633B",
      "8B58025F8E504DBDADF0E1176D3CE178",
      "CB7696CC33EC4D1BB98716465F1AD580",
      "4E355E04444B4676AE723B43DECA37DC",
      "DB54505FA3E9419DA3C1E1AFB7A48C1C",
      "10B8BB84EEE9413AAD071C8341BBAB04",
      "BF60433C7E924BE6B78D901809F8FEF6",
      "3A65F41FF1FC42518A702FDA164AF420",
      "6EA4299D271743BFB50DBB14C08AC55B",
      "432E29895A994D0DBC9DF9B0918E189F",
      "421A0EC1E68C4661AA9274C297BD410C",
      "D4C8D8008DAE4799A426518C2B6D0889",
      "4E24BC6E89394C8D8D05D3F9871EA5D8",
      "103F7DC6DDEA415389587AC6D37C135C",
      "21E1EF3C68744A53BF5FAC205A1B51CF",
      "8EC6FCD4C5344652A2C751023B66B889",
      "7FAFAF8FFCE143A0985010F83DACDA88",
      "0CFC2324C1A141C59B2B4612442D0433",
      "64DA1324EE1D4A6FA84783D93CEE0EA2",
      "91D179E2AC9C4BDA92E0CB877D45E051",
      "A008CBA8749B405C9A3FE02904EBA067",
      "DC80041DD3544835BDB50A8CAA535903",
      "692D9D11B6524E54BC96AE8244EE6AE3",
      "9CB248B6DD5F4EBD9D56A75B30DBEBCB",
      "B5C980B0A6494F1FBEDE80F96F96BEBE",
      "1F5F9158F3ED435CAD9757BCFA3A4453",
      "8F3CDD9B7F8B4B8FB6A35BB39DED6EC1",
      "C779CBB024A04E519D2806435882F7F9",
      "0EA142227FAE40148A4BDD8CF1D450EF",
      "621D9BA267174927B0E6A08601BDCF08",
      "8E7749E9F3C84E11A5F7F5E4E7D51B22",
      "17F8471D9F0346379B33D94E3A90689B",
      "52E7E19747864AB5900BBABF82E66382",
      "617069E6F5C04687B0526750978CFA51",
      "39C52F7B48ED4CEA824196BE9DBE6784",
      "E45E85D702F14A37B0E65F3C770D1195",
      "C5283EF5760B424C974E2E2F81CE02CE",
      "6733DCABB12E49E7A1FA6A76863DDA95",
      "70E817CC22644F538032177DEC7B6ECD",
      "95D6B96F155543CF86ED9CC9989A1059",
      "0BDF3A9BA8D94A18A016541653EA9097",
      "241944ED565940B5BC4987C3D9EAB6C7",
      "91ABEDE7189D4F509795396D3C646ADA",
      "D3F7714E036040BA8B24F0B8F5601452",
      "D3E6C97153E1408FB218FA34D8B65A3D",
      "CB10A07383CC4C35A22B1577B7712D38",
      "789104C736664FAA9748352CBBF86BCA",
      "3E85D9DD379643F8A207B99A9DFB72C2",
      "9C0541DDA9644E4EBA9CBC68BBD37C52",
      "C64BA744050149A1A32F402ED965CB1A",
      "ED06F0AEA37242349F2C499B7FEA6A26",
      "1549FB139D6B4AD99FDC1ED4B8011DB9",
      "721F8FFCBA8448D8BA61E9AD980A52C1",
      "4697179F1FC94E25A8274E586EEF2F39",
      "0BB5B81FEB37475BB9C779408080DB61",
      "3572C4D1479445D9959C413624A9FFF6",
      "EA473F6BF22F4F34807F2FA03497147F",
      "F03364FA841641EF82C8D893CA2C6727",
      "EEC427EA9E754FD9BE93D1C6C72F2F2E",
      "92BC895A020E452D926B6F64E985A197",
      "15FA7E0A9F2A41269B8B6CF7C52BD11D",
      "BCC8053D365143A18B033850EFE56F3C",
      "96730C617F4C40729D730D97395FD620",
      "2578D0D1427A4FE384544CA498301E40",
      "0A871CCD40364A03AD2F0FD24AEAB4E5",
      "640C685B4AE14A239A138C5A8876A1C9",
      "A7C8B9475B884B9593E3B9ED3F3B828C",
      "3844E4C9C5FA4ABAB3034FD6D4BE596E",
      "19193BC7942C41AEBA8147BD1778F35E",
      "AC218DF3AE43488C9E64E1AA551D2522",
      "CA289ED4F1A84A9EB6CDD822846FD745",
      "C24A87B33E3740B3B01254DC776F1EFE",
      "E2DECCA6300E4815B466C62C66E9D3AF",
      "ACD5116613FF40C9BC5EF7447CBEBBCB",
      "213B74E721ED457C9BE735C038C7CB95",
      "B70DE4074ABC49C69B0D8729D9212982",
      "6323EE2C82E94CC4BB83779DDFDDD6F5",
      "EECAA9639E054096B4A0F98E6845E963",
      "74F9BBD18C9143AE8547D078C9FAB456",
      "B1B179DB316A4178B0A5A57F00185A27",
      "E1F075B555A24EA8BF0E945CE580254F",
      "6C280692633D4498BD9CFEA7989138AB",
      "5BD6BED0CD794A078AA42476F47ECF46",
      "A2294A4CD0B14DE4912C1B530218AB57",
      "797EE3B1B9624E369B3E45FA0C10757C",
      "5EFB53D35BAE468B851CD388BEA46B30",
      "1E764495A5134823B30060D83FD6A2F9",
      "5698BACD2A7A45D6AC3FA60EAB3E6566",
      "8EF041900FE2428AABD404063AB979B6",
      "7348E620D27E40D1868C54247B5DE8D1",
      "E0849D6A48B84B8D9C135488C4C1DDF4",
      "A7AE79CD9E144A15B5B34B455AFB66D9",
      "B2A302B6DCD54F92AD0912A37A1D4850",
      "5ED4936039F64E9E99F703F8F46A1DB1",
      "E4F3367A4A6C4956875CAEF580692564",
      "5D6C6E00F2114D678570FC877B6F44BC",
      "D1AEF724041942BCAAC8CADBBDFD5EF4",
      "51CE2CDC8D3F49C7BF707B8E0A4BD15F",
      "CFACA4085F044622A1D4E00F7DB9C71E",
      "0EB9440796664EEBBAD9BE8B4DAE660F",
      "5CDF0A9CB3FC49BEB83D361B4CEE4082",
      "929D27BAADFE4F2F9194EF8E29B8B869",
      "26D7CD4E033242148E2724D3D054B4D3",
      "8CFAF78BCFFB41E6A3ED838D9EC2FD7C",
      "FF5EFC69BB8F4E56839A43A8477AE58F",
      "C57EC6AAFE22490082F06FD8DCBE2E1C",
      "471932C33B604C3099070F4106EE5024",
      "A274904C583C4290BB734B7F1875AB82",
      "9501CD77346746B39A98460EF44376D2",
      "A93B9C603CAA498AA19454C75B0826F8",
      "A1B184A401B546EEB7D09CDD35C47911",
      "D2ECF1B2A7FC45DB9D916996CE9BE9C9",
      "F0A8BC4265E44C47A127AEC1975F4C89",
      "56C80A4B7A774848824BA91BBB1065D5",
      "FA745E2F3D0B406FA6C0F3ECE4C5F5D4",
      "13558E4C883D4E1788160C87AB6F61BC",
      "9F7E6D76DB664D44AE13769DBF007571",
      "9344B56F71AA4BE1BB0A5DB6A75825B8",
      "9FC5E87F173747AB840D966D4238290C",
      "475E03B3E64740C5856F6E33EE8301FA",
      "8214ECDE53F04AFC95243E10B2C7BBD4",
      "4625FCFBF49747ACA31F31FA3101F1D5",
      "A3605A9B20E74483A7F4B8EF64AB72D6",
      "3D7E7F7C1487472A9690CDD361C6B26F",
      "D2794EA7FB7B4B52BA2CE4681E2D9DFB",
      "F0B2EAE5414249748F2986CC1EE78340",
      "F0CC0E7AEC0A401A94E8A63C9AC4F4D3",
      "73B2E56B742C40398F649727233DBD87",
      "B3441B45957C4BDFABBC2EA1E37FD31D",
      "D9520A9B3DEA453CB8F6A3A5CA1C9FEB",
      "C455246EFC1C402296B1C2249D00B04D",
      "3C3A0C482534454884151CCD97FCD561",
      "38C9CE2A4E06401DABF942E1D0224E87",
      "F356C4CED1584EBF812912F2D926066B",
      "E5B978ACDB914BA0AE53ED501B6F2600",
      "C3A6C9422EB64F5A8B9F82C4AE4FC928",
      "2F80CC4282644DDEB6DD5E57F6BDF9C1",
      "FBA0C040106A44CA93F48A6E112FB14E",
      "46A05975EE8A43DB82BA9D6C477D5756",
      "AABF8B3AD8AC4839BF103B1BFB3E9473",
      "5804B3F85FAB49B6B0B8B2686F4F5512",
      "1DA22B3D6F12458290F8549165B490EC",
      "8E5FA57F5168436BA998A70CB2C7F259",
      "8651DCAB4D714EF6AA747BB8F50719BA",
      "8EFCE40BCB74478B8254CEB594CE8774",
      "2D700440AC084B99AD123528BAE67D29",
      "9E212D4ED3C64493B631EE15D0A62AF7",
      "42A80AD3F88443F785C005BAF2121E01",
      "97F00180EC8442B3A1CB67E6349D7BDE",
      "AE7AC22E64D746B69F970EA1EC65DB05",
      "7937E5F0640240FDBBF9B158F45F4F6C",
      "FE597F427BF84EC783435F4471520403",
      "F3F07BFCCDDF48E79ED239F0111E4710",
      "34682D3EB66141ECACC8796C9D3A42B8",
      "4245947FE6244E37982F46D2FA46D74E",
      "30F30E1181BA43FE99706038200EFEBF",
      "40F45DEED2D84BA58B223F556FA25617",
      "DA0772F930654164AEC517CC6CDC5DBA",
      "3D1EF44A78D34058A32CB1C658AE90FE",
      "DCF6BD55DE71421CA49C6FF4F3B2D1FC",
      "EED63D7ABCB24E7E9AC1C4AA7C3ADC40",
      "D98433ED30FF48CDA8D2A84E846EC2DF",
    };

    // Ids for the property and itemtype item types
    private static HashSet<string> _dataModelIds = new HashSet<string>()
    {
      "D98433ED30FF48CDA8D2A84E846EC2DF",
      "26D7CD4E033242148E2724D3D054B4D3",
      "450906E86E304F55A34B3C0D65C097EA"
    };

    public int DefaultInstallOrder(ItemReference itemRef)
    {
      switch (itemRef.Type)
      {
        case "ItemType":
          // Prioritize core types (e.g. ItemType, List, etc.)
          if (_dataModelIds.Contains(itemRef.Unique))
            return -20;
          else if (_coreItemTypeIds.Contains(itemRef.Unique))
            return -10;
          break;
        case "RelationshipType":
          // Prioritize core metadata types (e.g. Member)
          if (_dataModelIds.Contains(itemRef.Unique))
            return -20;
          else if (_coreRelationshipTypeIds.Contains(itemRef.Unique))
            return -10;
          break;
        case InstallItem.ScriptType:
          if (itemRef.Unique.Split(' ').Concat(itemRef.KeyedName.Split(' ')).Any(p => _dataModelIds.Contains(p)))
            return -15;
          if (itemRef.Unique.Split(' ').Concat(itemRef.KeyedName.Split(' ')).Any(p => _coreItemTypeIds.Contains(p) || _coreRelationshipTypeIds.Contains(p)))
            return -5;
          var order = DefaultInstallOrder(itemRef.Unique.Split(':')[0]);
          if (order < 120)
            return order + 1;
          break;
      }
      var typeOrder = DefaultInstallOrder(itemRef.Type);
      if (FirstOfGroup?.Contains(itemRef.Unique ?? "") == true
        || string.Equals(itemRef.KeyedName, itemRef.Type, StringComparison.OrdinalIgnoreCase))
        return typeOrder - 1;
      return typeOrder;
    }

    private static int DefaultInstallOrder(string type)
    {
      switch (type)
      {
        case "List": return 10;
        case "Sequence": return 20;
        case "Revision": return 30;
        case "Variable": return 40;
        case "Identity": return 50;
        case "Member": return 60;
        case "User": return 70;
        case "Permission": return 80;
        case "Method": return 90;
        case "EMail Message": return 100;
        case "Action": return 110;
        case "Report": return 120;
        case "Form": return 130;
        case "Workflow Map": return 140;
        case "Life Cycle Map": return 150;
        case "Grid": return 160;
        case "xPropertyDefinition": return 170;
        case "Permission_ItemClassification": return 180;
        case "Permission_PropertyValue": return 190;
        case "CommandBarShortcut": return 200;
        case "CommandBarSeparator": return 210;
        case "CommandBarMenuSeparator": return 220;
        case "CommandBarDropDown": return 230;
        case "CommandBarCheckbox": return 240;
        case "CommandBarMenuCheckbox": return 250;
        case "CommandBarEdit": return 260;
        case "CommandBarButton": return 270;
        case "CommandBarMenu": return 280;
        case "CommandBarMenuButton": return 290;
        case "CommandBarSection": return 300;
        case "PresentationConfiguration": return 310;
        case "ItemType": return 320;
        case "RelationshipType": return 330;
        case "Morphae": return 340;
        case "Field": return 350;
        case "Property": return 360;
        case "View": return 370;
        case "SQL": return 380;
        case "Metric": return 390;
        case "Chart": return 400;
        case "Dashboard": return 410;
        case "ac_PolicyAccessItemAttribute": return 420;
        case "mp_PolicyAccessEnvAttribute": return 430;
        case "mp_MacPolicy": return 440;
        case "qry_QueryDefinition": return 450;
        case "dr_RelationshipFamily": return 460;
        case "dac_DomainDefinition": return 470;
        case InstallItem.ScriptType: return 700;
      }

      if (type.EndsWith("type", StringComparison.OrdinalIgnoreCase))
        return 9000;
      return 9999;
    }

    public enum CycleState
    {
      NoCycle,
      UnresolvedCycle,
      ResolvedCycle
    }
  }
}
