namespace DisruptiveSoftware.Cryptography.Tests.Utils
{
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security;
    using DisruptiveSoftware.Cryptography.Extensions;
    using DisruptiveSoftware.Cryptography.Tests.Extensions;
    using DisruptiveSoftware.Cryptography.Tests.TestData;
    using DisruptiveSoftware.Cryptography.Utils;
    using NUnit.Framework;
    using Shouldly;

    [TestFixture]
    public class CertificateUtilsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="file">e.x.  @"Files\test.pdf"</param>
        /// <returns></returns>
        public static string GetFilePath(string file)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, file);
        }

        private static byte[] GetResource(string resourceFullPathName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceFullPathName);

            if (stream != null)
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                return buffer;
            }

            return null;
        }

        private const string Test1NoBinaryFormat = "test1nobinaryformat";
        private const string Test1BinaryFormat = "test1binaryformat";
        private const string Test1ClientAuthenticationNoBinaryFormat = "test1_clientAuthentication_nobinaryformat";

        private static byte[] GetResourceFromTestData(string resourceName)
        {
            return GetResource($"DisruptiveSoftware.Cryptography.Tests.TestData.{resourceName}");
        }

        private static byte[] DataCaCrtBinaryFormat =>
            GetResourceFromTestData($"{Test1BinaryFormat}.caCertificate.crt");

        private static byte[] DataSslCrtBinaryFormat =>
            GetResourceFromTestData($"{Test1BinaryFormat}.sslCertificate.crt");

        private static byte[] DataCaCerBinaryFormat =>
            GetResourceFromTestData($"{Test1BinaryFormat}.caCertificate.cer");

        private static byte[] DataSslCerBinaryFormat =>
            GetResourceFromTestData($"{Test1BinaryFormat}.sslCertificate.cer");

        private static byte[] DataCaP12NoBinaryFormat =>
            GetResourceFromTestData($"{Test1NoBinaryFormat}.caCertificate.p12");
        private static byte[] DataSslP12NoBinaryFormat =>
            GetResourceFromTestData($"{Test1NoBinaryFormat}.sslCertificate.p12");
        private static byte[] DataSnk => GetResourceFromTestData("test.snk");

        private static SecureString GetSecureString => "123456".ToSecureString();

        [Test]
        public void ExportPrivateKey()
        {
            // Arrange

            var caCertificateData = DataCaP12NoBinaryFormat;
            var certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPrivateKey(
                caCertificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaP12NoBinaryFormatPrivateKey);
        }

        [Test]
        public void ExportPrivateKeyAsXMLString()
        {
            // Arrange

            var certificateData = DataCaP12NoBinaryFormat;
            var certificatePassword = GetSecureString;

            var result = CertificateUtils.ExportPrivateKeyAsXMLString(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaP12NoBinaryFormatPrivateKeyXml);
        }

        [Test]
        public void ExportPrivateKeyToPEM1()
        {
            // Arrange

            var certificateData = DataCaP12NoBinaryFormat;
            var certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPrivateKeyToPEM(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaP12NoBinaryFormatPrivateKeyPEM);
        }

        [Test]
        public void ExportPublicKeyCertificate()
        {
            // Arrange

            var certificateData = DataCaP12NoBinaryFormat;
            var certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificate(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaP12NoBinaryFormatPublicKey);
        }

        [Test]
        public void ExportPublicKeyCertificateToBase64()
        {
            // Arrange

            var certificateData = DataCaCrtBinaryFormat;
            var certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToBase64(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaCrtBinaryFormatPublicKeyBase64);
        }

        [Test]
        public void ExportPublicKeyCertificateToPEM()
        {
            // Arrange
            //need a file without password
            var certificateData = DataCaCerBinaryFormat;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData);

            // Assert
            result.ShouldBe(ResultConstants.DataCaCrtOrCerDataBinaryFormatPublicKeyPEM);

            certificateData = DataCaCrtBinaryFormat;

            result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData);

            result.ShouldBe(ResultConstants.DataCaCrtOrCerDataBinaryFormatPublicKeyPEM);
            //

            certificateData = DataSslCerBinaryFormat;

            result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData);

            result.ShouldBe(ResultConstants.DataSslCerDataBinaryFormatPublicKeyPEM);
        }

        [Test]
        public void ExportPublicKeyCertificateToPEM1()
        {
            // Arrange

            var certificateData = DataCaP12NoBinaryFormat;
            var certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataCaP12NoBinaryFormatPublicKeyPEM);

            certificateData = DataSslP12NoBinaryFormat;

            // Act
            result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldBe(ResultConstants.DataSslP12NoBinaryFormatPublicKeyPEM);
        }

        [Test]
        public void ExportPublicKeyToPEM()
        {
            // Arrange   
            //need a file without password
            var certificateData = DataSslCrtBinaryFormat;

            // Act
            var result = CertificateUtils.ExportPublicKeyToPEM(
                certificateData);

            // Assert 

            result.ShouldBe(ResultConstants.DataSslCrtBinaryFormatPublicKeyPEM);
        }

        [Test]
        public void ExportSnkPrivateKey()
        {
            // Arrange

            var certificateData = DataSnk;

            // Act
            var result = CertificateUtils.ExportSnkPrivateKey(
                certificateData);

            // Assert
            result.ShouldBe(ResultConstants.DataSnkPrivateKey);
        }

        [Test]
        public void ExportSnkPrivateKeyToPEM()
        {
            // Arrange

            var snkCertificateData = DataSnk;

            // Act
            var result = CertificateUtils.ExportSnkPrivateKeyToPEM(
                snkCertificateData);

            result.ShouldBe(
                ResultConstants.DataSnkPrivateKeyPEM
            );
        }

        [Test]
        public void ExportSnkPublicKeyCertificate()
        {
            // Arrange

            var snkCertificateData = DataSnk;

            // Act
            var result = CertificateUtils.ExportSnkPublicKeyCertificate(
                snkCertificateData);

            // Assert
            result.ShouldBe(ResultConstants.DataSnkPublicKeyYyyFormat);
        }

        [Test]
        public void ExportSnkPublicKeyCertificateToPEM()
        {
            // Arrange

            var certificateData = DataSnk;

            // Act
            var result = CertificateUtils.ExportSnkPublicKeyToPEM(
                certificateData);

            result.ShouldBe(ResultConstants.DataSnkPublicKeyPEM);
        }

        [Test]
        public void GetPublicKey()
        {
            // Arrange

            var snkData = DataSnk;

            // Act
            var result = CertificateUtils.GetPublicKeyPkcs1(
                snkData);

            // Assert 
            result.ShouldBe(ResultConstants.DataSnkPublicKeyPkcs1Format);
        }

        [Test]
        public void GetPublicKeyToken()
        {
            // Arrange

            var snkPublicKey = CertificateUtils.GetPublicKeyPkcs1(
                DataSnk);

            // Act
            var result = CertificateUtils.GetPublicKeyToken(
                snkPublicKey);

            // Assert
            result.ShouldBe("6CADB13A5BD8AAED");
        }
    }
}