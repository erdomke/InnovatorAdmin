//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Xml;
//using Innovator.Client;

//namespace InnovatorAdmin
//{
//  public class ImportProcessor : ICancelableProgressReporter
//  {
//    public event EventHandler<ActionCompleteEventArgs> ActionComplete;
//    public event EventHandler<ProgressChangedEventArgs> ProgressChanged;

//    private ConcurrentBag<ErrorEntry> _errors = new ConcurrentBag<ErrorEntry>();
//    private int _errorBlocks;
//    private IAsyncConnection _conn;
//    private DateTime _completeDate;
//    private IDataExtractor _extractor;
//    private CancellationTokenSource _cts;
//    private bool _cancel;

//    private int _globallyProcessed;
//    private int _maxCount;

//    public bool Cancel
//    {
//      get { return _cancel; }
//      set
//      {
//        if (value && !_cancel && _cts != null) _cts.Cancel();
//        _cancel = value;
//      }
//    }
//    public IDataExtractor CurrentExtractor { get { return _extractor; } }
//    public bool HasErrors { get { return _errors.Count > 0; } }

//    public ImportProcessor(IAsyncConnection conn)
//    {
//      _conn = conn;
//    }

//    public string GetLog()
//    {
//      var builder = new StringBuilder();
//      builder.Append("Import ").Append(_completeDate == DateTime.MinValue ? "still running" : (this.Cancel ? "canceled" : "completed")).Append(" on ").Append(_completeDate.ToString("s"));

//      if (_errors.Count > 0)
//      {
//        builder.Append(" with errors during ").Append(_errorBlocks).Append(" blocks").Append(Environment.NewLine).Append(Environment.NewLine);

//        foreach (var err in _errors)
//        {
//          builder.Append(err.Error).Append(Environment.NewLine).Append(err.Query).Append(Environment.NewLine).Append(Environment.NewLine);
//        }
//      }
//      else
//      {
//        builder.Append(" with no errors!");
//      }

//      return builder.ToString();
//    }

//    public void ProcessImport(IDataExtractor extractor, string xslt, int batchSize)
//    {
//      _extractor = extractor;
//      Exception error = null;
//      _completeDate = DateTime.MinValue;
//      _globallyProcessed = 0;

//      try
//      {
//        extractor.Reset();
//        using (_cts = new CancellationTokenSource())
//        {
//          var opts = new ParallelOptions();
//          opts.CancellationToken = _cts.Token;

//          Parallel.ForEach(GetXml(extractor, batchSize), opts, xml =>
//          {
//            int locallyProcessed = 0;
//            var doc = new XmlDocument();
//            doc.LoadXml(ArasXsltExtensions.Transform(xslt, xml, _conn));
//            IList<XmlElement> levelElements = new XmlElement[] { doc.DocumentElement };

//            while (!levelElements.Any(e => e.LocalName == "AML" || e.LocalName == "SQL"
//              || e.LocalName == "sql" || e.LocalName == "Item"))
//              levelElements = levelElements.Elements(e => true).ToList();

//            var isError = false;
//            for (var i = 0; i < levelElements.Count; i++)
//            {
//              Action<int, int> progressReporter = (curr, count) =>
//              {
//                var progress = (int)(((double)(count * i + curr) / (count * levelElements.Count)) * batchSize);
//                if (progress > locallyProcessed) Interlocked.Add(ref _globallyProcessed, progress - locallyProcessed);
//                locallyProcessed = progress;
//                SignalProgress(batchSize);
//              };

//              XmlNode result;
//              switch (levelElements[i].LocalName)
//              {
//                case "AML":
//                  result = _conn.CallAction("ApplyAML", levelElements[i], progressReporter);
//                  break;
//                case "SQL":
//                case "sql":
//                  result = _conn.CallAction("ApplySQL", levelElements[i], progressReporter);
//                  break;
//                case "Item":
//                  result = _conn.CallAction("ApplyItem", levelElements[i], progressReporter);
//                  break;
//                default:
//                  result = null;
//                  break;
//              }

//              if (_conn.GetError(result) != null)
//              {
//                _errors.Add(new ErrorEntry() { Error = result.OuterXml, Query = xml });
//                isError = true;
//                break;
//              }
//            }

//            if (isError) Interlocked.Increment(ref _errorBlocks);
//            Interlocked.Add(ref _globallyProcessed, batchSize - locallyProcessed);

//            SignalProgress(batchSize);
//            opts.CancellationToken.ThrowIfCancellationRequested();
//          });
//        }
//      }
//      catch (OperationCanceledException) { }
//      catch (Exception ex)
//      {
//        error = ex;
//      }
//      finally
//      {
//        _cts = null;
//      }

//      _completeDate = DateTime.Now;
//      OnActionComplete(new ActionCompleteEventArgs() { Exception = error });
//    }

//    private IEnumerable<string> GetXml(IDataExtractor extractor, int batchSize)
//    {
//      _maxCount = int.MaxValue;
//      while (!extractor.AtEnd)
//      {
//        var writer = new XmlDataWriter();
//        extractor.Write(batchSize, writer);
//        yield return writer.ToString();
//      }
//      _maxCount = extractor.NumProcessed;
//    }

//    private void SignalProgress(int batchSize)
//    {
//      OnProgressChanged(new ProgressChangedEventArgs(string.Format("{0} processed, {1} with errors", Math.Min(_globallyProcessed, _maxCount), _errorBlocks * batchSize), Math.Min(_globallyProcessed, _maxCount)));
//    }

//    protected virtual void OnActionComplete(ActionCompleteEventArgs e)
//    {
//      if (this.ActionComplete != null) ActionComplete(this, e);
//    }
//    protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
//    {
//      if (this.ProgressChanged != null) this.ProgressChanged.Invoke(this, e);
//    }

//    private class ErrorEntry
//    {
//      public string Error { get; set; }
//      public string Query { get; set; }
//    }
//  }
//}
