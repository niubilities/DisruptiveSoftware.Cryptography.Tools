﻿using SystemX509Certificates = System.Security.Cryptography.X509Certificates;

namespace DisruptiveSoftware.Cryptography.X509
{
  using System.Security;
  using DisruptiveSoftware.Cryptography.BouncyCastle.Extensions;
  using Org.BouncyCastle.Asn1;
  using Org.BouncyCastle.Asn1.X509;
  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Generators;
  using Org.BouncyCastle.Crypto.Operators;
  using Org.BouncyCastle.Crypto.Prng;
  using Org.BouncyCastle.Math;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  public class SslCertificateBuilder : X509CertificateBuilder
  {
    protected bool IsClientAuthKeyUsage { get; private set; }

    protected bool IsServerAuthKeyUsage { get; private set; }

    protected byte[] IssuerCertificate { get; private set; }
    protected SecureString? IssuerCertificatePassword { get; private set; }
    protected IList<string> SubjectAlternativeNames { get; private set; }

    public override X509CertificateBuilderResult Build()
    {
      var issuerX509Certificate2 = new SystemX509Certificates.X509Certificate2(
        IssuerCertificate,
        IssuerCertificatePassword,
        SystemX509Certificates.X509KeyStorageFlags.Exportable
      );

      var issuerSubjectDn = issuerX509Certificate2.ToX509Certificate().SubjectDN;

      X509V3CertificateGenerator.SetIssuerDN(issuerSubjectDn);

      // Generate Keys.
      var rsaKeyPairGenerator = new RsaKeyPairGenerator();

      rsaKeyPairGenerator.Init(
        new KeyGenerationParameters(new SecureRandom(new CryptoApiRandomGenerator()), KeySize));

      var asymmetricCipherKeyPair = rsaKeyPairGenerator.GenerateKeyPair();

      // Set Public Key.
      X509V3CertificateGenerator.SetPublicKey(asymmetricCipherKeyPair.Public);

      // Key Usage - for maximum interoperability, specify all four flags.
      var keyUsage = KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.KeyEncipherment |
                     KeyUsage.KeyAgreement;

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.KeyUsage,
        true,
        new KeyUsage(keyUsage)
      );

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.BasicConstraints,
        true,
        new BasicConstraints(false)
      );

      // Extended Key Usage.
      var extendedKeyUsage = new List<KeyPurposeID>();

      // Set TLS Web Server Authentication (1.3.6.1.5.5.7.3.1).
      if (IsServerAuthKeyUsage) extendedKeyUsage.Add(KeyPurposeID.IdKPServerAuth);

      // Set TLS Web Client Authentication (1.3.6.1.5.5.7.3.2).
      if (IsClientAuthKeyUsage) extendedKeyUsage.Add(KeyPurposeID.IdKPClientAuth);

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.ExtendedKeyUsage,
        true,
        new ExtendedKeyUsage(extendedKeyUsage)
      );

      // Set Subject Alternative Names.
      {
        var subjectAlternativeNames = new Asn1Encodable[SubjectAlternativeNames.Count];

        for (var i = 0; i < SubjectAlternativeNames.Count; i++)
          subjectAlternativeNames[i] = new GeneralName(GeneralName.DnsName, SubjectAlternativeNames[i]);

        X509V3CertificateGenerator.AddExtension(
          X509Extensions.SubjectAlternativeName,
          false,
          new DerSequence(subjectAlternativeNames)
        );
      }

      X509V3CertificateGenerator.AddExtension(
        X509Extensions.AuthorityKeyIdentifier,
        false,
        new AuthorityKeyIdentifier(
          SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(
            issuerX509Certificate2.GetPublicKeyAsAsymmetricKeyParameter()),
          new GeneralNames(new GeneralName(issuerSubjectDn)),
          new BigInteger(issuerX509Certificate2.GetSerialNumber())
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
        issuerX509Certificate2.GetPrivateKeyAsAsymmetricKeyParameter());

      // Generate X.509 Certificate.
      var x509Certificate = X509V3CertificateGenerator.Generate(signatureFactory);

      return new X509CertificateBuilderResult(x509Certificate, asymmetricCipherKeyPair.Private);
    }

    public SslCertificateBuilder SetClientAuthKeyUsage()
    {
      IsClientAuthKeyUsage = true;

      return this;
    }

    public SslCertificateBuilder SetIssuerCertificate(byte[] issuerCertificate,
      SecureString? issuerCertificatePassword)
    {
      IssuerCertificate = issuerCertificate;
      IssuerCertificatePassword = issuerCertificatePassword;

      return this;
    }

    public new SslCertificateBuilder SetKeySize(uint keySize)
    {
      base.SetKeySize(keySize);

      return this;
    }

    public new SslCertificateBuilder SetNotAfter(DateTime notAfter)
    {
      base.SetNotAfter(notAfter);

      return this;
    }

    public new SslCertificateBuilder SetNotBefore(DateTime notBefore)
    {
      base.SetNotBefore(notBefore);

      return this;
    }

    public new SslCertificateBuilder SetSerialNumber(long serialNumber)
    {
      base.SetSerialNumber(serialNumber);

      return this;
    }

    public SslCertificateBuilder SetServerAuthKeyUsage()
    {
      IsServerAuthKeyUsage = true;

      return this;
    }

    public SslCertificateBuilder SetSubjectAlternativeNames(IList<string> subjectAlternativeNames)
    {
      SubjectAlternativeNames = new List<string>(subjectAlternativeNames);

      return this;
    }

    public new SslCertificateBuilder SetSubjectDn(string? cn, string? ou, string? o, string? l, string? c)
    {
      base.SetSubjectDn(cn, ou, o, l, c);

      return this;
    }

    public new SslCertificateBuilder WithRandomSerialNumber()
    {
      base.WithRandomSerialNumber();

      return this;
    }
  }
}