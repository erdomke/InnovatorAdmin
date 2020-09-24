using InnovatorAdmin.Editor;
using System;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class UiCommand
  {
    private Action _handler;
    private IconInfo _icon;

    public string Description { get; }
    public Func<KeyEventArgs, bool> KeyFilter { get; private set; }
    public string Title { get; }

    public UiCommand(string title, string description)
    {
      Title = title;
      Description = description;
    }

    public void AddToGrid(System.Windows.Controls.Grid grid)
    {
      grid.RowDefinitions.Add(new System.Windows.Controls.RowDefinition());

      var button = new System.Windows.Controls.Button();
      if (_icon.Wpf == null)
      {
        button.Content = Title;
      }
      else
      {
        var panel = new System.Windows.Controls.StackPanel
        {
          Orientation = System.Windows.Controls.Orientation.Horizontal
        };
        panel.Children.Add(new System.Windows.Controls.Image()
        {
          Source = _icon.Wpf,
          Margin = new System.Windows.Thickness(0, 0, 6, 0)
        });
        panel.Children.Add(new System.Windows.Controls.TextBlock(new System.Windows.Documents.Run(Title)));
        button.Content = panel;
      }
      button.Click += (sender, e) => {
        try
        {
          _handler();
        }
        catch (Exception ex)
        {
          Utils.HandleError(ex);
        }
      };
      button.Padding = new System.Windows.Thickness(6);
      System.Windows.Controls.Grid.SetRow(button, grid.RowDefinitions.Count - 1);
      System.Windows.Controls.Grid.SetColumn(button, 0);
      grid.Children.Add(button);

      var textBlock = new System.Windows.Controls.TextBlock(new System.Windows.Documents.Run(Description))
      {
        TextWrapping = System.Windows.TextWrapping.Wrap,
        VerticalAlignment = System.Windows.VerticalAlignment.Center,
        Margin = new System.Windows.Thickness(6)
      };
      System.Windows.Controls.Grid.SetRow(textBlock, grid.RowDefinitions.Count - 1);
      System.Windows.Controls.Grid.SetColumn(textBlock, 1);
      grid.Children.Add(textBlock);
    }

    public UiCommand Bind(ToolStripItem item, Func<KeyEventArgs, bool> keyFilter, Action callback)
    {
      _handler = callback;
      KeyFilter = keyFilter;
      item.Text = Title;
      item.ToolTipText = Description ?? item.ToolTipText;
      if (_icon.Gdi != null)
      {
        item.Image = _icon.Gdi;
        item.ImageScaling = ToolStripItemImageScaling.None;
        item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
      }
      item.Click += Item_Click;
      return this;
    }

    public void Execute()
    {
      _handler();
    }

    private void Item_Click(object sender, EventArgs e)
    {
      try
      {
        _handler();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public UiCommand Bind<T>(ToolStripItem item, Form form, Func<KeyEventArgs, bool> keyFilter, Action<T> callback)
    {
      return Bind(item, keyFilter, () =>
      {
        var ctrl = form.FindFocusedControl().ParentsAndSelf().OfType<T>().FirstOrDefault();
        if (ctrl != null)
        {
          callback.Invoke(ctrl);
        }
      });
    }

    public UiCommand WithIcon(IconInfo icon)
    {
      _icon = icon;
      return this;
    }
  }
}
