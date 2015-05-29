using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class ImportXsltHelper : XmlCompletionDataProvider
  {
    public ImportXsltHelper() : base(LoadSchema()) { }

    public override void HandleTextEntered(EditorControl control, string insertText)
    {
      var text = control.Editor.Text.Substring(0, control.Editor.CaretOffset);

      switch (insertText[0])
      {
        case ':':
          if (XmlParser.IsInsideAttributeValue(text, text.Length))
          {
            var namespaces = XmlParser.GetNamespacesInScope(text);
            var i = text.Length - 2;
            while (Char.IsLetterOrDigit(text[i]) || text[i] == '_') i--;
            string namespaceUri;
            if (namespaces != null && namespaces.TryGetValue(text.Substring(i + 1).TrimEnd(':'), out namespaceUri))
            {
              var methods = ArasXsltExtensions.GetExtensionMethods(namespaceUri).OrderBy(m => m.Name);
              control.ShowCompletionWindow(from m in methods
                                           group m by m.Name into Group
                                           select new BasicCompletionData(Group.Key, Group.Select(MethodToString).Aggregate((p, c) => p + Environment.NewLine + c)), 0);
            }
          }
          break;
        default:
          base.HandleTextEntered(control, insertText);
          break;
      }
    }

    private static string MethodToString(System.Reflection.MethodInfo method)
    {
      return (method.ReturnType == null ? "void" : method.ReturnType.Name)
        + " " 
        + method.Name
        + "(" 
        + method.GetParameters()
                .GroupConcat(", ", p => (p.IsOut ? "out " : "") + p.ParameterType.Name + " " + p.Name + (p.IsOptional ? " = " + (p.DefaultValue ?? "null").ToString() : ""))
        + ")";
    }

    private static XmlSchema LoadSchema()
    {
      var schema = new XmlSchema();
      using (var stream = typeof(XmlCompletionDataProvider).Assembly.GetManifestResourceStream("Aras.Tools.InnovatorAdmin.Editor.xslt.xsd"))
      {
        using (var reader = XmlReader.Create(stream))
        {
          schema = XmlSchema.Read(reader, new ValidationEventHandler(SchemaValidation));
          schema.Compile(new ValidationEventHandler(SchemaValidation));
        }
      }
      return schema;
    }

    /// <summary>
    /// Handler for schema validation errors.
    /// </summary>
    private static void SchemaValidation(object source, ValidationEventArgs e)
    {
      // Do nothing.
    }
  }

}
