namespace CertUtil.Commons.Controls.Validation
{
  public class DirectoryExistsValidationRule : TextRequiredValidationRule
  {
    public override bool IsValid(Control control)
    {
      if (base.IsValid(control))
      {
        if (Directory.Exists(control.Text)) return true;

        InvalidateControl(control);

        return false;
      }

      return false;
    }
  }
}