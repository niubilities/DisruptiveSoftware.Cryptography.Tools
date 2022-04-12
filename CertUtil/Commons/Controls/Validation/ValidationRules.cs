namespace CertUtil.Commons.Controls.Validation
{
    using CertUtil.Commons.Controls.Validation.Impl;

    public static class ValidationRules
    {
        public static IControlValidationRule DirectoryExists => new DirectoryExistsValidationRule();

        public static IControlValidationRule FileExists => new FileExistsValidationRule();
        public static IControlValidationRule Required => new TextRequiredValidationRule();
    }
}