using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class TreeListView : BrightIdeasSoftware.TreeListView
  {
    public event EventHandler<ModelDoubleClickEventArgs> ModelDoubleClick;

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
      base.OnMouseDoubleClick(e);

      var model = this.OlvHitTest(e.X, e.Y).RowObject;
      if (model != null && ModelDoubleClick != null)
        ModelDoubleClick.Invoke(this, new ModelDoubleClickEventArgs(e, model));
    }
  }

  public class ModelDoubleClickEventArgs : MouseEventArgs
  {
    private object _model;
    public object Model { get { return _model; } }

    public ModelDoubleClickEventArgs(MouseEventArgs e, object model)
      : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
    {
      _model = model;
    }
  }
}
