namespace CertUtil.Commons.Controls.Validation
{
    public class FileExistsValidationRule : TextRequiredValidationRule
    {
        public override bool IsValid(Control control)
        {
            if (base.IsValid(control))
            {
                if (File.Exists(control.Text)) return true;

                InvalidateControl(control);

                return false;
            }

            return false;
        }
    }
}