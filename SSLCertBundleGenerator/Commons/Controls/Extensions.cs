namespace SSLCertBundleGenerator.Commons.Controls
{
  using System.ComponentModel;
  using SSLCertBundleGenerator.Commons.Controls.Validation.Impl;

  public static class Extensions
  {
    public static void InvokeIfRequired(this Control control, MethodInvoker action)
    {
      ((ISynchronizeInvoke)control).InvokeIfRequired(action);
    }

    public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
    {
      if (obj.InvokeRequired)
        obj.Invoke(action, Array.Empty<object>());
      else
        action();
    }

    public static bool IsValid(this Control control, IControlValidationRule rule)
    {
      return rule.IsValid(control);
    }

    public static ToolTip SetToolTip(this Control control, string caption)
    {
      var toolTip = new ToolTip();

      toolTip.SetToolTip(control, caption);

      return toolTip;
    }

    public static void ToogleUseSystemPasswordChar(this TextBox control)
    {
      control.UseSystemPasswordChar = !control.UseSystemPasswordChar;
    }
  }
}