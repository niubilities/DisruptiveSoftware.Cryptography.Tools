namespace SSLCertBundleGenerator.Extensions
{
  using DisruptiveSoftware.Cryptography.X509;

  public static class X509CertificateBuilderExtensions
  {
    public static T WithSerialNumberConfiguration<T>(this T certificateBuilder, bool randomSerialNumber,
      long serialNumber) where T : X509CertificateBuilder
    {
      if (randomSerialNumber) return (T)certificateBuilder.WithRandomSerialNumber();

      return (T)certificateBuilder.SetSerialNumber(serialNumber);
    }
  }
}