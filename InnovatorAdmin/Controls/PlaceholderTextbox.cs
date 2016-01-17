using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  /// <summary>
  /// Represents a Windows text box control with placeholder.
  /// </summary>
  public class PlaceholderTextBox : TextBox
  {
    #region Properties

    char _passwordChar;
    string _placeholderText = DEFAULT_PLACEHOLDER;
    bool _isPlaceholderActive;


    /// <summary>
    /// Gets a value indicating whether the Placeholder is active.
    /// </summary>
    [Browsable(false)]
    public bool IsPlaceholderActive
    {
      get
      {
        return _isPlaceholderActive;
      }
      private set
      {
        if (value != _isPlaceholderActive)
        {
          _isPlaceholderActive = value;

          base.PasswordChar = _isPlaceholderActive ? default(char) : _passwordChar;
          OnPlaceholderActiveChanged(value);
        }
      }
    }


    /// <summary>
    /// Gets or sets the placeholder in the PlaceholderTextBox.
    /// </summary>
    [Description("The placeholder associated with the control."), Category("Placeholder"), DefaultValue(DEFAULT_PLACEHOLDER)]
    public string PlaceholderText
    {
      get { return _placeholderText; }
      set
      {
        _placeholderText = value;

        // Only use the new value if the placeholder is active.
        if (this.IsPlaceholderActive)
          this.Text = value;
      }
    }


    /// <summary>
    /// Gets or sets the current text in the TextBox.
    /// </summary>
    [Browsable(false)]
    public override string Text
    {
      get
      {
        // Check 'IsPlaceholderActive' to avoid this if-clause when the text is the same as the placeholder but actually it's not the placeholder.
        // Check 'base.Text == this.Placeholder' because in some cases IsPlaceholderActive changes too late although it isn't the placeholder anymore.
        // If you want to get the Text Property and it still contains the placeholder, an empty string will return.
        if (this.IsPlaceholderActive && base.Text == this.PlaceholderText)
          return String.Empty;

        return base.Text;
      }
      set { base.Text = value; }
    }

    /// <summary>
    /// Gets or sets the foreground color of the control.
    /// </summary>
    public override Color ForeColor
    {
      get
      {
        // We have to differentiate whether the system is asking for the ForeColor to draw it
        // or the developer is asking for the color.
        // && Environment.StackTrace.Contains("System.Windows.Forms.Control.InitializeDCForWmCtlColor(IntPtr dc, Int32 msg)")
        if (this.IsPlaceholderActive)
          return Color.Gray;

        return base.ForeColor;
      }
      set
      {
        base.ForeColor = value;
      }
    }

    public new char PasswordChar
    {
      get
      {
        return _passwordChar;
      }
      set
      {
        _passwordChar = value;
        if (!IsPlaceholderActive)
          base.PasswordChar = value;

      }
    }

    /// <summary>
    /// Occurs when the value of the IsPlaceholderActive property has changed.
    /// </summary>
    [Description("Occurs when the value of the IsPlaceholderInside property has changed.")]
    public event EventHandler<PlaceholderActiveChangedEventArgs> PlaceholderActiveChanged;

    #endregion


    #region Global Variables

    /// <summary>
    /// Specifies the default placeholder text.
    /// </summary>
    const string DEFAULT_PLACEHOLDER = "<Input>";

    /// <summary>
    /// Flag to avoid the TextChanged Event. Don't access directly, use Method 'ActionWithoutTextChanged(Action act)' instead.
    /// </summary>
    bool avoidTextChanged;

    #endregion


    #region Constructor

    /// <summary>
    /// Initializes a new instance of the PlaceholderTextBox class.
    /// </summary>
    public PlaceholderTextBox()
    {
      // Through this line the default placeholder gets displayed in designer
      base.Text = this.PlaceholderText;

      SubscribeEvents();

      // Set Default
      this.IsPlaceholderActive = true;
    }

    #endregion


    #region Functions

    /// <summary>
    /// Inserts placeholder, assigns placeholder style and sets cursor to first position.
    /// </summary>
    public void Reset()
    {
      this.IsPlaceholderActive = true;

      ActionWithoutTextChanged(() => this.Text = this.PlaceholderText);
      this.Select(0, 0);
    }

    /// <summary>
    /// Run an action with avoiding the TextChanged event.
    /// </summary>
    /// <param name="act">Specifies the action to run.</param>
    private void ActionWithoutTextChanged(Action act)
    {
      avoidTextChanged = true;

      act.Invoke();

      avoidTextChanged = false;
    }

    /// <summary>
    /// Subscribe necessary Events.
    /// </summary>
    private void SubscribeEvents()
    {
      this.TextChanged += PlaceholderTextBox_TextChanged;
    }

    #endregion


    #region Events

    private void PlaceholderTextBox_TextChanged(object sender, EventArgs e)
    {
      // Check flag
      if (avoidTextChanged) return;

      // Run code with avoiding recursive call
      ActionWithoutTextChanged(delegate
      {
        // If the Text is empty, insert placeholder and set cursor to to first position
        if (String.IsNullOrEmpty(this.Text))
        {
          Reset();
          return;
        }

        // If the placeholder is active, revert state to a usual TextBox
        if (this.IsPlaceholderActive)
        {
          this.IsPlaceholderActive = false;

          // Throw away the placeholder but leave the new typed char
          this.Text = this.Text.Replace(this.PlaceholderText, String.Empty);

          // Set Selection to last position
          this.Select(this.TextLength, 0);
        }
      });

      this.Font = this.Font;
    }

    protected override void OnGotFocus(EventArgs e)
    {
      // Without this line it would highlight the placeholder when getting focus
      this.Select(0, 0);
      base.OnGotFocus(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      // When you click on the placerholderTextBox and the placerholder is active, jump to first position
      if (this.IsPlaceholderActive)
        Reset();

      base.OnMouseDown(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      // Prevents that the user can go through the placeholder with arrow keys
      if (IsPlaceholderActive && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
        e.Handled = true;

      base.OnKeyDown(e);
    }

    protected virtual void OnPlaceholderActiveChanged(bool newValue)
    {
      if (PlaceholderActiveChanged != null)
        PlaceholderActiveChanged(this, new PlaceholderActiveChangedEventArgs(newValue));
    }

    #endregion
  }

  /// <summary>
  /// Provides data for the PlaceholderActiveChanged event.
  /// </summary>
  public class PlaceholderActiveChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Initializes a new instance of the PlaceholderInsideChangedEventArgs class.
    /// </summary>
    /// <param name="newValue">The new value of the IsPlaceholderInside Property.</param>
    public PlaceholderActiveChangedEventArgs(bool newValue)
    {
      this.NewValue = newValue;
    }

    /// <summary>
    /// Gets the new value of the IsPlaceholderActive property.
    /// </summary>
    public bool NewValue { get; private set; }
  }
}
