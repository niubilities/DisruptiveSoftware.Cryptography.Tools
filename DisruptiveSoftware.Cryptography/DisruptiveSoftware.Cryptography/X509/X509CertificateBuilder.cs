namespace DisruptiveSoftware.Cryptography.X509
{
  using System.Collections;
  using DisruptiveSoftware.Cryptography.Extensions;
  using Org.BouncyCastle.Asn1;
  using Org.BouncyCastle.Asn1.Pkcs;
  using Org.BouncyCastle.Asn1.X509;
  using Org.BouncyCastle.Crypto.Prng;
  using Org.BouncyCastle.Math;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.Utilities;
  using Org.BouncyCastle.X509;

  public abstract class X509CertificateBuilder
  {
    public X509CertificateBuilder()
    {
      X509V3CertificateGenerator = new X509V3CertificateGenerator();
      AttributesOids = new ArrayList();
      AttributesValues = new ArrayList();
    }

    protected IList AttributesOids { get; private set; }

    protected IList AttributesValues { get; private set; }

    protected int KeySize { get; private set; }

    protected BigInteger SerialNumber { get; private set; }
    protected X509V3CertificateGenerator X509V3CertificateGenerator { get; }

    public abstract X509CertificateBuilderResult Build();

    public virtual X509CertificateBuilder SetKeySize(uint keySize)
    {
      KeySize = (int)keySize;

      return this;
    }

    public virtual X509CertificateBuilder SetNotAfter(DateTime notAfter)
    {
      X509V3CertificateGenerator.SetNotAfter(notAfter);

      return this;
    }

    public virtual X509CertificateBuilder SetNotBefore(DateTime notBefore)
    {
      X509V3CertificateGenerator.SetNotBefore(notBefore);

      return this;
    }

    public virtual X509CertificateBuilder SetSerialNumber(long serialNumber)
    {
      SerialNumber = BigInteger.ValueOf(serialNumber);
      X509V3CertificateGenerator.SetSerialNumber(SerialNumber);

      return this;
    }

    public virtual X509CertificateBuilder SetSubjectDn(string? cn, string? ou, string? o, string? l, string? c)
    {
      var result = BuildX509Name(cn, ou, o, l, c);

      AttributesOids = result.Item1;
      AttributesValues = result.Item2;

      X509V3CertificateGenerator.SetSubjectDN(new X509Name(result.Item1, result.Item2));

      return this;
    }

    public virtual X509CertificateBuilder WithRandomSerialNumber()
    {
      SerialNumber = GetRandomSerialNumber();
      X509V3CertificateGenerator.SetSerialNumber(SerialNumber);

      return this;
    }

    protected static Tuple<IList, IList> BuildX509Name(string? cn, string? ou, string? o, string? l, string? c)
    {
      IList attributesOids = new ArrayList();
      IList attributesValues = new ArrayList();

      if (!c.IsNullOrEmpty()) AddAttribute(X509Name.C, c, attributesOids, attributesValues);

      if (!l.IsNullOrEmpty()) AddAttribute(X509Name.L, l, attributesOids, attributesValues);

      if (!o.IsNullOrEmpty()) AddAttribute(X509Name.O, o, attributesOids, attributesValues);

      if (!ou.IsNullOrEmpty()) AddAttribute(X509Name.OU, ou, attributesOids, attributesValues);

      if (!cn.IsNullOrEmpty()) AddAttribute(X509Name.CN, cn, attributesOids, attributesValues);

      return new Tuple<IList, IList>(attributesOids, attributesValues);
    }

    protected static BigInteger GetRandomSerialNumber()
    {
      return BigIntegers.CreateRandomInRange(
        BigInteger.One,
        BigInteger.ValueOf(long.MaxValue),
        new SecureRandom(new CryptoApiRandomGenerator())
      );
    }

    protected static string GetSignatureAlgorithm(int keySize)
    {
      if (keySize == Constants.RsaKeySize.KeySize1024) return PkcsObjectIdentifiers.Sha1WithRsaEncryption.Id;

      if (keySize == Constants.RsaKeySize.KeySize2048) return PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id;

      if (keySize == Constants.RsaKeySize.KeySize3072) return PkcsObjectIdentifiers.Sha384WithRsaEncryption.Id;

      if (keySize == Constants.RsaKeySize.KeySize4096) return PkcsObjectIdentifiers.Sha512WithRsaEncryption.Id;

      throw new Exception($"Unable to determine signature algorithm. Invalid private key size {keySize}.");
    }

    private static void AddAttribute(DerObjectIdentifier oid, string? value, IList attributesOids,
      IList attributesValues)
    {
      attributesOids.Add(oid);
      attributesValues.Add(value);
    }
  }
}