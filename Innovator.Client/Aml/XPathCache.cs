using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Innovator.Client
{
  /// <summary>
  /// Implements a cache of XPath queries, for faster execution.
  /// </summary>
  /// <remarks>
  /// Discussed at http://weblogs.asp.net/cazzu/archive/2004/04/02/106667.aspx
  /// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
  /// </remarks>
  internal static class XPathCache
  {
    private const string XPathCache_BadSortObject = "Sort expression must be either a string or an XPathExpression";

    private static IDictionary<string, XPathExpression> _cache = new Dictionary<string, XPathExpression>();
    /// <summary>
    /// Initially a simple hashtable. In the future should 
    /// implement sliding expiration of unused expressions.
    /// </summary>
    private static IDictionary<string, XPathExpression> Cache
    {
      get
      {
        return XPathCache._cache;
      }
    }
    /// <summary>
    /// Retrieves a cached compiled expression, or a newly compiled one.
    /// </summary>
    private static XPathExpression GetCompiledExpression(string expression, XPathNavigator source)
    {
      XPathExpression xPathExpression;
      if (!XPathCache.Cache.TryGetValue(expression, out xPathExpression))
      {
        xPathExpression = source.Compile(expression);
        XPathCache.Cache[expression] = xPathExpression;
      }
      return xPathExpression.Clone();
    }

    /// <summary>
    /// Sets up the context for expression execution.
    /// </summary>
    private static XmlNamespaceManager PrepareContext(XPathNavigator source, XmlNamespaceManager context, XmlPrefix[] prefixes, XPathVariable[] variables)
    {
      XmlNamespaceManager xmlNamespaceManager = context;
      if (variables != null)
      {
        DynamicContext dynamicContext;
        if (xmlNamespaceManager != null)
        {
          dynamicContext = new DynamicContext(xmlNamespaceManager);
        }
        else
        {
          dynamicContext = new DynamicContext();
        }
        for (int i = 0; i < variables.Length; i++)
        {
          XPathVariable xPathVariable = variables[i];
          dynamicContext.AddVariable(xPathVariable.Name, xPathVariable.Value);
        }
        xmlNamespaceManager = dynamicContext;
      }
      if (prefixes != null)
      {
        if (xmlNamespaceManager == null)
        {
          xmlNamespaceManager = new XmlNamespaceManager(source.NameTable);
        }
        for (int j = 0; j < prefixes.Length; j++)
        {
          XmlPrefix xmlPrefix = prefixes[j];
          xmlNamespaceManager.AddNamespace(xmlPrefix.Prefix, xmlPrefix.NamespaceURI);
        }
      }
      return xmlNamespaceManager;
    }
    private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
    {
      if (sortExpression is string)
      {
        expression.AddSort(XPathCache.GetCompiledExpression((string)sortExpression, source), order, caseOrder, lang, dataType);
        return;
      }
      if (sortExpression is XPathExpression)
      {
        expression.AddSort(sortExpression, order, caseOrder, lang, dataType);
        return;
      }
      throw new XPathException(XPathCache_BadSortObject, null);
    }
    private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
    {
      XPathExpression xPathExpression;
      if (sortExpression is string)
      {
        xPathExpression = XPathCache.GetCompiledExpression((string)sortExpression, source);
      }
      else
      {
        if (!(sortExpression is XPathExpression))
        {
          throw new XPathException(XPathCache_BadSortObject, null);
        }
        xPathExpression = (XPathExpression)sortExpression;
      }
      xPathExpression.SetContext(context);
      expression.AddSort(xPathExpression, order, caseOrder, lang, dataType);
    }
    private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, IComparer comparer)
    {
      if (sortExpression is string)
      {
        expression.AddSort(XPathCache.GetCompiledExpression((string)sortExpression, source), comparer);
        return;
      }
      if (sortExpression is XPathExpression)
      {
        expression.AddSort(sortExpression, comparer);
        return;
      }
      throw new XPathException(XPathCache_BadSortObject, null);
    }
    private static void PrepareSort(XPathExpression expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlNamespaceManager context)
    {
      XPathExpression xPathExpression;
      if (sortExpression is string)
      {
        xPathExpression = XPathCache.GetCompiledExpression((string)sortExpression, source);
      }
      else
      {
        if (!(sortExpression is XPathExpression))
        {
          throw new XPathException(XPathCache_BadSortObject, null);
        }
        xPathExpression = (XPathExpression)sortExpression;
      }
      xPathExpression.SetContext(context);
      expression.AddSort(xPathExpression, comparer);
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source)
    {
      return source.Evaluate(XPathCache.GetCompiledExpression(expression, source));
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, null, variables));
      return source.Evaluate(compiledExpression);
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source, XmlNamespaceManager context)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(context);
      return source.Evaluate(compiledExpression);
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source, params XmlPrefix[] prefixes)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, prefixes, null));
      return source.Evaluate(compiledExpression);
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, context, null, variables));
      return source.Evaluate(compiledExpression);
    }
    /// <summary>
    /// Evaluates the given expression and returns the typed result.
    /// </summary>
    public static object Evaluate(string expression, XPathNavigator source, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, prefixes, variables));
      return source.Evaluate(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source)
    {
      return source.Select(XPathCache.GetCompiledExpression(expression, source));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, null, variables));
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlNamespaceManager context)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source, params XmlPrefix[] prefixes)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, prefixes, null));
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, context, null, variables));
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator Select(string expression, XPathNavigator source, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, prefixes, variables));
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression and sort.
    /// </summary>
    /// <remarks>
    /// See <see cref="M:System.Xml.XPath.XPathExpression.AddSort(System.Object,System.Collections.IComparer)" />.
    /// </remarks>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, comparer);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression and sort.
    /// </summary>
    /// <remarks>
    /// See <see cref="M:System.Xml.XPath.XPathExpression.AddSort(System.Object,System.Collections.IComparer)" />.
    /// </remarks>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, null, variables));
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, comparer);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(XPathCache.PrepareContext(source, null, null, variables));
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      compiledExpression.SetContext(context);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, params XmlPrefix[] prefixes)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context = XPathCache.PrepareContext(source, null, prefixes, null);
      compiledExpression.SetContext(context);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, comparer, context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XmlPrefix[] prefixes)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context = XPathCache.PrepareContext(source, null, prefixes, null);
      compiledExpression.SetContext(context);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context2 = XPathCache.PrepareContext(source, context, null, variables);
      compiledExpression.SetContext(context2);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context2);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context2 = XPathCache.PrepareContext(source, context, null, variables);
      compiledExpression.SetContext(context2);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, comparer, context2);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context = XPathCache.PrepareContext(source, null, prefixes, variables);
      compiledExpression.SetContext(context);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, order, caseOrder, lang, dataType, context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XPathNodeIterator SelectSorted(string expression, XPathNavigator source, object sortExpression, IComparer comparer, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      XPathExpression compiledExpression = XPathCache.GetCompiledExpression(expression, source);
      XmlNamespaceManager context = XPathCache.PrepareContext(source, null, prefixes, variables);
      compiledExpression.SetContext(context);
      XPathCache.PrepareSort(compiledExpression, source, sortExpression, comparer, context);
      return source.Select(compiledExpression);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator());
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source, params XPathVariable[] variables)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator(), variables);
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlNamespaceManager context)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator(), context);
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source, params XmlPrefix[] prefixes)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator(), prefixes);
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator(), context, variables);
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a list of nodes matching the XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodes(string expression, XmlNode source, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      XPathNodeIterator iterator = XPathCache.Select(expression, source.CreateNavigator(), prefixes, variables);
      return XmlNodeListFactory.CreateNodeList(iterator);
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression and sort.
    /// </summary>
    /// <remarks>
    /// See <see cref="M:System.Xml.XPath.XPathExpression.AddSort(System.Object,System.Collections.IComparer)" />.
    /// </remarks>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression and sort.
    /// </summary>
    /// <remarks>(object, IComparer)
    /// See <see cref="M:System.Xml.XPath.XPathExpression.AddSort(System.Object,System.Collections.IComparer)" />.
    /// </remarks>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, variables));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, variables));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, context));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, params XmlPrefix[] prefixes)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, prefixes));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, params XmlPrefix[] prefixes)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, prefixes));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, context, variables));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, context, variables));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, XmlSortOrder order, XmlCaseOrder caseOrder, string lang, XmlDataType dataType, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, order, caseOrder, lang, dataType, prefixes, variables));
    }
    /// <summary>
    /// Selects a node set using the specified XPath expression.
    /// </summary>
    public static XmlNodeList SelectNodesSorted(string expression, XmlNode source, object sortExpression, IComparer comparer, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      return XmlNodeListFactory.CreateNodeList(XPathCache.SelectSorted(expression, source.CreateNavigator(), sortExpression, comparer, prefixes, variables));
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source, params XPathVariable[] variables)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source, variables).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlNamespaceManager context)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source, context).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source, params XmlPrefix[] prefixes)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source, prefixes).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlNamespaceManager context, params XPathVariable[] variables)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source, context, variables).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
    /// <summary>
    /// Selects the first XmlNode that matches the XPath expression.
    /// </summary>
    public static XmlNode SelectSingleNode(string expression, XmlNode source, XmlPrefix[] prefixes, params XPathVariable[] variables)
    {
      IEnumerator enumerator = XPathCache.SelectNodes(expression, source, prefixes, variables).GetEnumerator();
      try
      {
        if (enumerator.MoveNext())
        {
          return (XmlNode)enumerator.Current;
        }
      }
      finally
      {
        IDisposable disposable = enumerator as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
      return null;
    }
  }

  /// <summary>
  /// Represents a mapping between a prefix and a namespace.
  /// </summary>
  internal class XmlPrefix
  {
    private string _prefix;
    private string _ns;
    /// <summary>
    /// Gets the prefix associated with the <see cref="P:Mvp.Xml.Common.XmlPrefix.NamespaceURI" />.
    /// </summary>
    public string Prefix
    {
      get
      {
        return this._prefix;
      }
    }
    /// <summary>
    /// Gets the namespace associated with the <see cref="P:Mvp.Xml.Common.XmlPrefix.Prefix" />.
    /// </summary>
    public string NamespaceURI
    {
      get
      {
        return this._ns;
      }
    }
    /// <summary>
    /// Creates the prefix mapping.
    /// </summary>
    /// <param name="prefix">Prefix associated with the namespace.</param>
    /// <param name="ns">Namespace to associate with the prefix.</param>
    public XmlPrefix(string prefix, string ns)
    {
      this._prefix = prefix;
      this._ns = ns;
    }
    /// <summary>
    /// Creates the prefix mapping, using atomized strings from the 
    /// <paramref name="nameTable" /> for faster lookups and comparisons.
    /// </summary>
    /// <param name="prefix">Prefix associated with the namespace.</param>
    /// <param name="ns">Namespace to associate with the prefix.</param>
    /// <param name="nameTable">The name table to use to atomize strings.</param>
    /// <remarks>
    /// This is the recommended way to construct this class, as it uses the 
    /// best approach to handling strings in XML.
    /// </remarks>
    public XmlPrefix(string prefix, string ns, XmlNameTable nameTable)
    {
      this._prefix = nameTable.Add(prefix);
      this._ns = nameTable.Add(ns);
    }
  }

  /// <summary>
  /// Represents a variable to use in dynamic XPath expression 
  /// queries.
  /// </summary>
  /// <remarks>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></remarks>
  internal struct XPathVariable
  {
    private string _name;
    private object _value;
    /// <summary>
    /// Gets the variable name.
    /// </summary>
    public string Name
    {
      get
      {
        return this._name;
      }
    }
    /// <summary>
    /// Gets the variable value.
    /// </summary>
    public object Value
    {
      get
      {
        return this._value;
      }
    }
    /// <summary>
    /// Initializes the new variable.
    /// </summary>
    /// <param name="name">The name to assign to the variable.</param>
    /// <param name="value">The variable value.</param>
    public XPathVariable(string name, object value)
    {
      this._name = name;
      this._value = value;
    }
    /// <summary>
    /// Checks equality of two variables. They are equal 
    /// if both their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Name" /> and their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Value" /> 
    /// are equal.
    /// </summary>
    public override bool Equals(object obj)
    {
      return this.Name == ((XPathVariable)obj).Name && this.Value == ((XPathVariable)obj).Value;
    }
    /// <summary>
    /// See <see cref="M:System.Object.GetHashCode" />.
    /// </summary>
    public override int GetHashCode()
    {
      return (this.Name + "." + this.Value.GetHashCode()).GetHashCode();
    }
    /// <summary>
    /// Checks equality of two variables. They are equal 
    /// if both their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Name" /> and their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Value" /> 
    /// are equal.
    /// </summary>
    public static bool operator ==(XPathVariable a, XPathVariable b)
    {
      return a.Equals(b);
    }
    /// <summary>
    /// Checks equality of two variables. They are not equal 
    /// if both their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Name" /> and their <see cref="P:Mvp.Xml.Common.XPath.XPathVariable.Value" /> 
    /// are different.
    /// </summary>
    public static bool operator !=(XPathVariable a, XPathVariable b)
    {
      return !a.Equals(b);
    }
  }

  /// <summary>
  /// Provides the evaluation context for fast execution and custom 
  /// variables resolution.
  /// </summary>
  /// <remarks>
  /// This class is responsible for resolving variables during dynamic expression execution.
  /// <para>Discussed in http://weblogs.asp.net/cazzu/archive/2003/10/07/30888.aspx</para>
  /// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
  /// </remarks>
  internal class DynamicContext : XsltContext
  {
    /// <summary>
    /// Represents a variable during dynamic expression execution.
    /// </summary>
    internal class DynamicVariable : IXsltContextVariable
    {
      private string _name;
      private object _value;
      private XPathResultType _type;
      XPathResultType IXsltContextVariable.VariableType
      {
        get
        {
          return this._type;
        }
      }
      bool IXsltContextVariable.IsLocal
      {
        get
        {
          return false;
        }
      }
      bool IXsltContextVariable.IsParam
      {
        get
        {
          return false;
        }
      }
      /// <summary>
      /// Initializes a new instance of the class.
      /// </summary>
      /// <param name="name">The name of the variable.</param>
      /// <param name="value">The value of the variable.</param>
      public DynamicVariable(string name, object value)
      {
        this._name = name;
        this._value = value;
        if (value is string)
        {
          this._type = XPathResultType.String;
          return;
        }
        if (value is bool)
        {
          this._type = XPathResultType.Boolean;
          return;
        }
        if (value is XPathNavigator)
        {
          this._type = XPathResultType.String;
          return;
        }
        if (value is XPathNodeIterator)
        {
          this._type = XPathResultType.NodeSet;
          return;
        }
        if (value is double)
        {
          this._type = XPathResultType.Number;
          return;
        }
        if (value is IConvertible)
        {
          try
          {
            this._value = Convert.ToDouble(value);
            this._type = XPathResultType.Number;
            return;
          }
          catch (FormatException)
          {
            this._type = XPathResultType.Any;
            return;
          }
          catch (OverflowException)
          {
            this._type = XPathResultType.Any;
            return;
          }
        }
        this._type = XPathResultType.Any;
      }
      object IXsltContextVariable.Evaluate(XsltContext context)
      {
        return this._value;
      }
    }
    private IDictionary<string, IXsltContextVariable> _variables = new Dictionary<string, IXsltContextVariable>();
    /// <summary>
    /// Same as <see cref="T:System.Xml.Xsl.XsltContext" />.
    /// </summary>
    public override bool Whitespace
    {
      get
      {
        return true;
      }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Mvp.Xml.Common.XPath.DynamicContext" /> class.
    /// </summary>
    public DynamicContext()
      : base(new NameTable())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Mvp.Xml.Common.XPath.DynamicContext" /> 
    /// class with the specified <see cref="T:System.Xml.NameTable" />.
    /// </summary>
    /// <param name="table">The NameTable to use.</param>
    public DynamicContext(NameTable table)
      : base(table)
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Mvp.Xml.Common.XPath.DynamicContext" /> class.
    /// </summary>
    /// <param name="context">A previously filled context with the namespaces to use.</param>
    public DynamicContext(XmlNamespaceManager context)
      : this(context, new NameTable())
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="T:Mvp.Xml.Common.XPath.DynamicContext" /> class.
    /// </summary>
    /// <param name="context">A previously filled context with the namespaces to use.</param>
    /// <param name="table">The NameTable to use.</param>
    public DynamicContext(XmlNamespaceManager context, NameTable table)
      : base(table)
    {
      object objB = table.Add("http://www.w3.org/XML/1998/namespace");
      object objB2 = table.Add("http://www.w3.org/2000/xmlns/");
      if (context != null)
      {
        foreach (string prefix in context)
        {
          string text = context.LookupNamespace(prefix);
          if (!object.Equals(text, objB) && !object.Equals(text, objB2))
          {
            base.AddNamespace(prefix, text);
          }
        }
      }
    }
    /// <summary>
    /// Implementation equal to <see cref="T:System.Xml.Xsl.XsltContext" />.
    /// </summary>
    public override int CompareDocument(string baseUri, string nextbaseUri)
    {
      return string.Compare(baseUri, nextbaseUri, false, CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// Same as <see cref="T:System.Xml.XmlNamespaceManager" />.
    /// </summary>
    public override string LookupNamespace(string prefix)
    {
      string text = this.NameTable.Get(prefix);
      if (text == null)
      {
        return null;
      }
      return base.LookupNamespace(text);
    }
    /// <summary>
    /// Same as <see cref="T:System.Xml.XmlNamespaceManager" />.
    /// </summary>
    public override string LookupPrefix(string uri)
    {
      string text = this.NameTable.Get(uri);
      if (text == null)
      {
        return null;
      }
      return base.LookupPrefix(text);
    }
    /// <summary>
    /// Same as <see cref="T:System.Xml.Xsl.XsltContext" />.
    /// </summary>
    public override bool PreserveWhitespace(XPathNavigator node)
    {
      return true;
    }
    /// <summary>
    /// Shortcut method that compiles an expression using an empty navigator.
    /// </summary>
    /// <param name="xpath">The expression to compile</param>
    /// <returns>A compiled <see cref="T:System.Xml.XPath.XPathExpression" />.</returns>
    public static XPathExpression Compile(string xpath)
    {
      return new XmlDocument().CreateNavigator().Compile(xpath);
    }
    /// <summary>
    /// Adds the variable to the dynamic evaluation context.
    /// </summary>
    /// <param name="name">The name of the variable to add to the context.</param>
    /// <param name="value">The value of the variable to add to the context.</param>
    /// <remarks>
    /// Value type conversion for XPath evaluation is as follows:
    /// <list type="table">
    /// 	<listheader>
    /// 		<term>CLR Type</term>
    /// 		<description>XPath type</description>
    /// 	</listheader>
    /// 	<item>
    /// 		<term>System.String</term>
    /// 		<description>XPathResultType.String</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>System.Double (or types that can be converted to)</term>
    /// 		<description>XPathResultType.Number</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>System.Boolean</term>
    /// 		<description>XPathResultType.Boolean</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>System.Xml.XPath.XPathNavigator</term>
    /// 		<description>XPathResultType.Navigator</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>System.Xml.XPath.XPathNodeIterator</term>
    /// 		<description>XPathResultType.NodeSet</description>
    /// 	</item>
    /// 	<item>
    /// 		<term>Others</term>
    /// 		<description>XPathResultType.Any</description>
    /// 	</item>
    /// </list>
    /// <note type="note">See the topic "Compile, Select, Evaluate, and Matches with 
    /// XPath and XPathExpressions" in MSDN documentation for additional information.</note>
    /// </remarks>
    /// <exception cref="T:System.ArgumentNullException">The <paramref name="value" /> is null.</exception>
    public void AddVariable(string name, object value)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      this._variables[name] = new DynamicContext.DynamicVariable(name, value);
    }
    /// <summary>
    /// See <see cref="T:System.Xml.Xsl.XsltContext" />. Not used in our implementation.
    /// </summary>
    public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] ArgTypes)
    {
      return null;
    }
    /// <summary>
    /// Resolves the dynamic variables added to the context. See <see cref="T:System.Xml.Xsl.XsltContext" />. 
    /// </summary>
    public override IXsltContextVariable ResolveVariable(string prefix, string name)
    {
      IXsltContextVariable result;
      this._variables.TryGetValue(name, out result);
      return result;
    }
  }

  /// <summary>
  /// Constructs <see cref="T:System.Xml.XmlNodeList" /> instances from 
  /// <see cref="T:System.Xml.XPath.XPathNodeIterator" /> objects.
  /// </summary>
  /// <remarks>See http://weblogs.asp.net/cazzu/archive/2004/04/14/113479.aspx. 
  /// <para>Author: Daniel Cazzulino, <a href="http://clariusconsulting.net/kzu">blog</a></para>
  /// <para>Contributors: Oleg Tkachenko, http://www.xmllab.net</para>
  /// </remarks>
  internal sealed class XmlNodeListFactory
  {
    private const string XmlNodeListFactory_IHasXmlNodeMissing = "The XPath query was not executed against an XmlNode-based object (such as an XmlDocument)";

    private class XmlNodeListIterator : XmlNodeList
    {
      private class XmlNodeListEnumerator : IEnumerator
      {
        private XmlNodeListFactory.XmlNodeListIterator _iterator;
        private int _position = -1;
        object IEnumerator.Current
        {
          get
          {
            return this._iterator[this._position];
          }
        }
        public XmlNodeListEnumerator(XmlNodeListFactory.XmlNodeListIterator iterator)
        {
          this._iterator = iterator;
        }
        void IEnumerator.Reset()
        {
          this._position = -1;
        }
        bool IEnumerator.MoveNext()
        {
          this._position++;
          this._iterator.ReadTo(this._position);
          return !this._iterator.Done || this._position < this._iterator.CurrentPosition;
        }
      }
      private XPathNodeIterator _iterator;
      private IList<XmlNode> _nodes = new List<XmlNode>();
      private bool _done;
      public override int Count
      {
        get
        {
          if (!this._done)
          {
            this.ReadToEnd();
          }
          return this._nodes.Count;
        }
      }
      /// <summary>
      /// Flags that the iterator has been consumed.
      /// </summary>
      private bool Done
      {
        get
        {
          return this._done;
        }
      }
      /// <summary>
      /// Current count of nodes in the iterator (read so far).
      /// </summary>
      private int CurrentPosition
      {
        get
        {
          return this._nodes.Count;
        }
      }
      public XmlNodeListIterator(XPathNodeIterator iterator)
      {
        this._iterator = iterator.Clone();
      }
      public override IEnumerator GetEnumerator()
      {
        return new XmlNodeListFactory.XmlNodeListIterator.XmlNodeListEnumerator(this);
      }
      public override XmlNode Item(int index)
      {
        if (index >= this._nodes.Count)
        {
          this.ReadTo(index);
        }
        if (index >= this._nodes.Count || index < 0)
        {
          return null;
        }
        return this._nodes[index];
      }
      /// <summary>
      /// Reads the entire iterator.
      /// </summary>
      private void ReadToEnd()
      {
        while (this._iterator.MoveNext())
        {
          IHasXmlNode hasXmlNode = this._iterator.Current as IHasXmlNode;
          if (hasXmlNode == null)
          {
            throw new ArgumentException(XmlNodeListFactory_IHasXmlNodeMissing);
          }
          this._nodes.Add(hasXmlNode.GetNode());
        }
        this._done = true;
      }
      /// <summary>
      /// Reads up to the specified index, or until the 
      /// iterator is consumed.
      /// </summary>
      private void ReadTo(int to)
      {
        while (this._nodes.Count <= to)
        {
          if (!this._iterator.MoveNext())
          {
            this._done = true;
            return;
          }
          IHasXmlNode hasXmlNode = this._iterator.Current as IHasXmlNode;
          if (hasXmlNode == null)
          {
            throw new ArgumentException(XmlNodeListFactory_IHasXmlNodeMissing);
          }
          this._nodes.Add(hasXmlNode.GetNode());
        }
      }
    }
    private XmlNodeListFactory()
    {
    }
    /// <summary>
    /// Creates an instance of a <see cref="T:System.Xml.XmlNodeList" /> that allows 
    /// enumerating <see cref="T:System.Xml.XmlNode" /> elements in the iterator.
    /// </summary>
    /// <param name="iterator">The result of a previous node selection 
    /// through an <see cref="T:System.Xml.XPath.XPathNavigator" /> query.</param>
    /// <returns>An initialized list ready to be enumerated.</returns>
    /// <remarks>The underlying XML store used to issue the query must be 
    /// an object inheriting <see cref="T:System.Xml.XmlNode" />, such as 
    /// <see cref="T:System.Xml.XmlDocument" />.</remarks>
    public static XmlNodeList CreateNodeList(XPathNodeIterator iterator)
    {
      return new XmlNodeListFactory.XmlNodeListIterator(iterator);
    }
  }
}
