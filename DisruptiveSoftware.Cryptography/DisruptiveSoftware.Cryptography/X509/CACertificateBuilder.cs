﻿namespace DisruptiveSoftware.Cryptography.X509
{
  using Org.BouncyCastle.Asn1.X509;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Generators;
  using Org.BouncyCastle.Crypto.Operators;
  using Org.BouncyCastle.Crypto.Prng;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  public class CaCertificateBuilder : X509CertificateBuilder
  {
    public override X509CertificateBuilderResult Build()
    {
      // Generate Keys.
      var rsaKeyPairGenerator = new RsaKeyPairGenerator();

      rsaKeyPairGenerator.Init(
        new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), KeySize));

      var asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();

      // Set Public Key.
      X509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);

      // Add Extensions.
      var keyUsage = KeyUsage.KeyCertSign | KeyUsage.CrlSign;

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.KeyUsage,
        true,
        new KeyUsage(keyUsage)
      );

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.BasicConstraints,
        true,
        new BasicConstraints(true)
      );

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.AuthorityKeyIdentifier,
        false,
        new AuthorityKeyIdentifier(
          SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public),
          new GeneralNames(new GeneralName(new X509Name(AttributesOids, AttributesValues))),
          SerialNumber
        )
      );

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.SubjectKeyIdentifier,
        false,
        new SubjectKeyIdentifier(
          SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(asymmetricCipherKeyPair.Public)
        )
      );

      var signatureFactory = new Asn1SignatureFactory(
        GetSignatureAlgorithm(KeySize),
        asymmetricCipherKeyPair.Private);

      // Generate X.509 Certificate.
      var x509Certificate = X509V3CertificateGenerator.Generate(signatureFactory);

      return new X509CertificateBuilderResult(x509Certificate, asymmetricCipherKeyPair.Private);
    }

    public override X509CertificateBuilder SetSubjectDn(string? cn, string? ou, string? o, string? l, string? c)
    {
      base.SetSubjectDn(cn, ou, o, l, c);

      var result = BuildX509Name(cn, ou, o, l, c);
      X509V3CertificateGenerator.SetIssuerDN(new X509Name(result.Item1, result.Item2));

      return this;
    }
  }
}