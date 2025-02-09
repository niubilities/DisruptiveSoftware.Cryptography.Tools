﻿namespace SSLCertBundleGenerator.Commons.Controls.Validation
{
  using DisruptiveSoftware.Cryptography.Extensions;
  using SSLCertBundleGenerator.Commons.Controls.Validation.Impl;

  public class TextRequiredValidationRule : IControlValidationRule
  {
    public virtual bool IsValid(Control control)
    {
      if (control.Text.IsNullOrEmpty())
      {
        InvalidateControl(control);

        return false;
      }

      ValidateControl(control);

      return true;
    }

    protected virtual void InvalidateControl(Control control)
    {
      control.InvokeIfRequired(() => { control.BackColor = Constants.Controls.InvalidControlColor; });
    }

    protected virtual void ValidateControl(Control control)
    {
      control.InvokeIfRequired(() => { control.BackColor = default(Color); });
    }
  }
}