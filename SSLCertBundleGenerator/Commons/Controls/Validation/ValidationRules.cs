namespace SSLCertBundleGenerator.Commons.Controls.Validation
{
  using SSLCertBundleGenerator.Commons.Controls.Validation.Impl;

  public static class ValidationRules
  {
    public static IControlValidationRule DirectoryExists => new DirectoryExistsValidationRule();
    public static IControlValidationRule Required => new TextRequiredValidationRule();
  }
}