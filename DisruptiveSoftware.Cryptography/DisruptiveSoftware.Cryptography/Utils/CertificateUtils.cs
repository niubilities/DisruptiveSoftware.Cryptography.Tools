﻿namespace DisruptiveSoftware.Cryptography.Utils
{
  using System.Security;
  using System.Security.Cryptography;
  using System.Security.Cryptography.X509Certificates;
  using System.Text;
  using DisruptiveSoftware.Cryptography.Extensions;
  using Org.BouncyCastle.OpenSsl;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  public static class CertificateUtils
  {
    public static byte[]? ExportPrivateKey(byte[] certificateData, SecureString? certificatePassword)
    {
      var privateKey = ExportPrivateKeyToPEM(certificateData, certificatePassword);

      // Certificate does not have a private key.
      if (privateKey.IsNullOrEmpty()) return null;

      var stringBuilder = new StringBuilder();

      foreach (var pemLine in privateKey!.Split('\n'))
      {
        // Trim padding CR and white spaces.
        var line = pemLine.TrimEnd('\r').Trim();

        // Skip directives and empty lines.
        if (!(line.Contains("BEGIN RSA PRIVATE KEY") || line.Contains("END RSA PRIVATE KEY") ||
              line.Length == 0))
          stringBuilder.Append(line);
      }

      // Decode Base64 to DER.
      return Convert.FromBase64String(stringBuilder.ToString());
    }

    public static string? ExportPrivateKeyAsXMLString(byte[] certificateData, SecureString certificatePassword)
    {
      var x509Certificate2 = new X509Certificate2(
        certificateData,
        certificatePassword,
        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
      );

      using var rsa = x509Certificate2.GetRSAPrivateKey();

      return rsa?.ToXmlString(true);
    }

    public static string? ExportPrivateKeyToPEM(RSA rsa)
    {
      using var textWriter = new StringWriter();
      var asymmetricCipherKeyPair = DotNetUtilities.GetRsaKeyPair(rsa);
      var pemWriter = new PemWriter(textWriter);
      pemWriter.WriteObject(asymmetricCipherKeyPair.Private);

      return pemWriter.Writer.ToString();
    }

    public static string? ExportPrivateKeyToPEM(byte[] certificateData, SecureString? certificatePassword)
    {
      using var x509Certificate2 = new X509Certificate2(
        certificateData,
        certificatePassword,
        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
      );

      if (!x509Certificate2.HasPrivateKey) return null;

      using var rsa = x509Certificate2.GetRSAPrivateKey()!;

      return ExportPrivateKeyToPEM(rsa);
    }

    public static byte[]? ExportPublicKeyCertificate(byte[] certificateData, SecureString? certificatePassword)
    {
      using var x509Certificate2 = new X509Certificate2(certificateData, certificatePassword);

      return x509Certificate2.Export(X509ContentType.Cert);
    }

    public static string ExportPublicKeyCertificateToBase64(byte[] certificateData,
      SecureString? certificatePassword)
    {
      return Convert.ToBase64String(
        ExportPublicKeyCertificate(certificateData, certificatePassword) ?? Array.Empty<byte>());
    }

    public static string? ExportPublicKeyCertificateToPEM(byte[] certificateData)
    {
      using var textWriter = new StringWriter();
      var x509CertificateParser = new X509CertificateParser();
      var x509Certificate = x509CertificateParser.ReadCertificate(certificateData);
      var pemWriter = new PemWriter(textWriter);
      pemWriter.WriteObject(x509Certificate);

      return pemWriter.Writer?.ToString();
    }

    public static string? ExportPublicKeyCertificateToPEM(byte[] certificateData, SecureString? certificatePassword)
    {
      var stringBuilder = new StringBuilder();

      stringBuilder.AppendLine("-----BEGIN CERTIFICATE-----");

      stringBuilder.AppendLine(
        Convert.ToBase64String(
          ExportPublicKeyCertificate(certificateData, certificatePassword) ?? Array.Empty<byte>(),
          Base64FormattingOptions.InsertLineBreaks));

      stringBuilder.AppendLine("-----END CERTIFICATE-----");

      return stringBuilder.ToString();
    }

    public static string? ExportPublicKeyToPEM(RSA rsa)
    {
      using var textWriter = new StringWriter();
      var asymmetricCipherKeyPair = DotNetUtilities.GetRsaKeyPair(rsa);
      var pemWriter = new PemWriter(textWriter);
      pemWriter.WriteObject(asymmetricCipherKeyPair.Public);

      return pemWriter.Writer?.ToString();
    }

    public static string? ExportPublicKeyToPEM(byte[] certificateData)
    {
      using var textWriter = new StringWriter();
      var x509CertificateParser = new X509CertificateParser();

      var x509Certificate = x509CertificateParser.ReadCertificate(certificateData);
      var asymmetricKeyParameter = x509Certificate.GetPublicKey();
      var pemWriter = new PemWriter(textWriter);
      pemWriter.WriteObject(asymmetricKeyParameter);

      return pemWriter.Writer?.ToString();
    }

    public static byte[]? ExportSnkPrivateKey(byte[] snkCertificateData)
    {
      var privateKey = ExportSnkPrivateKeyToPEM(snkCertificateData);

      if (privateKey.IsNullOrEmpty()) return null;

      var stringBuilder = new StringBuilder();

      foreach (var pemLine in privateKey?.Split('\n')!)
      {
        // Trim padding CR and white spaces.
        var line = pemLine.TrimEnd('\r').Trim();

        // Skip directives and empty lines.
        if (!(line.Contains("BEGIN RSA PRIVATE KEY") || line.Contains("END RSA PRIVATE KEY") ||
              line.Length == 0))
          stringBuilder.Append(line);
      }

      // Decode Base64 to DER.
      return Convert.FromBase64String(stringBuilder.ToString());
    }

    public static string? ExportSnkPrivateKeyToPEM(byte[] snkCertificateData)
    {
      return ExportSnk(snkCertificateData, ExportPrivateKeyToPEM);
    }

    public static byte[]? ExportSnkPublicKeyCertificate(byte[] snkCertificateData)
    {
      var publicKey = ExportSnkPublicKeyToPEM(snkCertificateData);

      if (publicKey.IsNullOrEmpty()) return null;

      var stringBuilder = new StringBuilder();

      foreach (var line in from pemLine in publicKey?.Split('\n') // Trim padding CR and white spaces.
               let line = pemLine.TrimEnd('\r').Trim() // Skip directives and empty lines.
               where !(line.Contains("-----BEGIN PUBLIC KEY-----") || line.Contains("-----END PUBLIC KEY-----") ||
                       line.Length == 0)
               select line)
        stringBuilder.Append(line);

      // Decode Base64 to DER.
      return Convert.FromBase64String(stringBuilder.ToString());
    }

    public static string? ExportSnkPublicKeyToPEM(byte[] snkCertificateData)
    {
      return ExportSnk(snkCertificateData, ExportPublicKeyToPEM);
    }

    public static byte[] GetPublicKeyPkcs1(byte[] snkData)
    {
      //return ExportSnk(snkData, rsa => rsa.ExportCspBlob(false));
      return ExportSnk(snkData, rsa => rsa.ExportRSAPublicKey());
    }

    public static byte[] GetPublicKeyToken(byte[] snkPublicKey)
    {
      using var csp = HashAlgorithm.Create("SHA1");
      //new SHA1CryptoServiceProvider();
      var hash = csp.ComputeHash(snkPublicKey);

      var token = new byte[8];

      for (var i = 0; i < 8; i++) token[i] = hash[hash.Length - i - 1];

      return token;
    }

    /// <summary>
    ///   Exports the snk.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="snkData">The snk data.</param>
    /// <param name="processor">The processor.</param>
    /// <returns>A <typeparamref name="T"></typeparamref></returns>
    private static T ExportSnk<T>(byte[] snkData, Func<RSA, T> processor)
    {
      using RSACryptoServiceProvider rsa = new();
      rsa.ImportCspBlob(snkData);
      rsa.PersistKeyInCsp = true;
      //X509Certificate2 certificate2 = new X509Certificate2(rsa.ExportCspBlob(true));
      //var certrsa = certificate2.GetRSAPrivateKey();

      return processor(rsa);
    }
  }
}