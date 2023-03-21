using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public static class CoreActions
  {
    public static IEnumerable<string> GetActions(int version)
    {
      yield return "ActivateActivity";
      yield return "add";
      yield return "AddHistory";
      yield return "AddItem";
      yield return "ApplyUpdate";
      yield return "BuildProcessReport";
      yield return "CancelWorkflow";
      yield return "closeWorkflow";
      yield return "copy";
      yield return "copyAsIs";
      yield return "copyAsNew";
      yield return "create";
      yield return "delete";
      yield return "edit";
      yield return "EmailItem";
      yield return "EvaluateActivity";
      yield return "get";
      yield return "getAffectedItems";
      yield return "GetInheritedServerEvents";
      yield return "getItemAllVersions";
      yield return "GetItemConfig";
      yield return "getItemLastVersion";
      yield return "getItemNextStates";
      yield return "getItemRelationships";
      yield return "GetItemRepeatConfig";
      yield return "getItemWhereUsed";
      yield return "GetMappedPath";
      yield return "getPermissions";
      yield return "getRelatedItem";
      yield return "GetUpdateInfo";
      yield return "instantiateWorkflow";
      yield return "lock";
      yield return "merge";
      yield return "New Workflow Map";
      yield return "promoteItem";
      yield return "purge";
      yield return "recache";
      yield return "replicate";
      yield return "resetAllItemsAccess";
      yield return "resetItemAccess";
      yield return "resetLifecycle";
      yield return "setDefaultLifecycle";
      yield return "skip";
      yield return "startWorkflow";
      yield return "unlock";
      yield return "update";
      yield return "ValidateWorkflowMap";
      yield return "version";

      if (version < 10)
        yield return "checkImportedItemType";
      if (version < 11)
        yield return "exportItemType";
      if (version < 0 || version >= 10)
        yield return "VaultServerEvent";
      if (version < 0 || version >= 11)
      {
        yield return "GetInheritedServerEvents";
        yield return "getHistoryItems";
      }
    }
  }
}
