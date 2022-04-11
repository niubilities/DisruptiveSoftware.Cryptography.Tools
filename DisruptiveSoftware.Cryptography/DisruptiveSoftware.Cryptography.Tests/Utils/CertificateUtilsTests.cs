using DisruptiveSoftware.Cryptography.Utils;
using NUnit.Framework;
using Shouldly;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Linq.Expressions;



namespace DisruptiveSoftware.Cryptography.Tests.Utils
{
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Cryptography;
    using NUnit.Framework.Internal;

    [TestFixture]
    public class CertificateUtilsTests
    {
        [SetUp]
        public void Setup()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">e.x.  @"Files\test.pdf"</param>
        /// <returns></returns>
        public static string GetFilePath(string file)
        {
            return Path.Combine(TestContext.CurrentContext.TestDirectory, file);
        }

        public static byte[] GetResource(string resouceFullPathName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resouceFullPathName);

            if (stream != null)
            {

                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
            return null;
        }
        public static byte[] GetResourceFromTestData(string resouceName)
        {
            return GetResource($"DisruptiveSoftware.Cryptography.Tests.TestData.{resouceName}");
        }


        private static byte[] GetCaCertificateData => GetResourceFromTestData("caCertificate.p12");
        private static byte[] GetSslCertificateData => GetResourceFromTestData("sslCertificate.p12");
        private static byte[] GetSnkCertificateData => GetResourceFromTestData("test.snk");
        private static SecureString GetSecureString => "123456".Aggregate(new SecureString(), (seed, b) =>
            {
                seed.AppendChar(b);
                return seed;
            });
        [Test]
        public void ExportPrivateKey_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] caCertificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;
            // Act
            var result = CertificateUtils.ExportPrivateKey(
                caCertificateData,
                certificatePassword);


            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPrivateKeyAsXMLString_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] certificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;


            var result = CertificateUtils.ExportPrivateKeyAsXMLString(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPrivateKeyToPEM_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange

            byte[] certificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;


            // Act
            var result = CertificateUtils.ExportPrivateKeyToPEM(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPublicKeyCertificate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] certificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;


            // Act
            var result = CertificateUtils.ExportPublicKeyCertificate(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPublicKeyCertificateToBase64_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] certificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToBase64(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPublicKeyCertificateToPEM_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            //need a file without password
            byte[] certificateData = GetSslCertificateData;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData);

            // Assert
            //result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPublicKeyCertificateToPEM_StateUnderTest_ExpectedBehavior1()
        {
            // Arrange

            byte[] certificateData = GetCaCertificateData;
            SecureString certificatePassword = GetSecureString;

            // Act
            var result = CertificateUtils.ExportPublicKeyCertificateToPEM(
                certificateData,
                certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
            //result.ShouldBe("");

            certificateData = GetSslCertificateData;

            // Act
            result = CertificateUtils.ExportPublicKeyCertificateToPEM(
              certificateData,
              certificatePassword);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportPublicKeyToPEM_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            //need a file without password
            byte[] certificateData = GetCaCertificateData;

            // Act
            var result = CertificateUtils.ExportPublicKeyToPEM(
                certificateData);

            // Assert 

           // result.ShouldNotBeEmpty();

        }

        [Test]
        public void ExportSnkPrivateKey_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] certificateData = GetSnkCertificateData;

            // Act
            var result = CertificateUtils.ExportSnkPrivateKey(
                certificateData);

            // Assert
            result.ShouldNotBeEmpty();
            
        }

        [Test]
        public void ExportSnkPrivateKeyToPEM_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] snkCertificateData = GetSnkCertificateData;

            // Act
            var result = CertificateUtils.ExportSnkPrivateKeyToPEM(
                snkCertificateData);

            // Assert
            result.ShouldNotBeEmpty();
            result.ShouldBe($@"-----BEGIN RSA PRIVATE KEY-----
MIICWwIBAAKBgQDdaTOaR7btN54IqXoPNBwDdZrf5dFLHATDsq88KNpm2ZFwQkcZ
nWuFRhVw0CQgEEvEVN9huk8IKloZ8sKh71O5obXSsww6gIq8otEAdzsUlx0t8o8V
Hih3XFpWWkBRYtdE5QmGjU1J/KmQ/4PXKWUbAy03PKBEhoj/Uhc81gLh+QIDAQAB
AoGBAMzGkMBfFXNeXh46yLYpBsO4UI5FmoWyG0H4EBQ+4IgBL384/VNWgewYPppB
FzhEeh7SNGvJiXDO4the6t4kYDR2G5LFdaTkvbERJ8/Cpm3ydMb4zEKZ547xht82
A5CDr6juriDWDu22ip4iNPs7nsFcJcaKNBkExecOxTTs8IfFAkEA5pSXgEbJBe71
erfaIJpIs+r/B62n/kB7iOhgQ2ySKqIor3xpTs5oLVd24MQJf2YJc9cpzA956eKU
bKsYQ4qDXwJBAPXR1GOscVrWNPboTzLvMwN5KoXKK36lviH4NPTpyp4mVW6i0ihc
dhzHUqgnBAw38DYgX3gMBjb4IF17tB2+MacCQHqhln2jp/Ae6bGtrDXguD/wAFju
E8WWN91VcTUKviYsfiTurvc5sZBDzza1LDP0aZyRV2pu5LDuT3AIAuyQ81MCP3ro
b0lm70Z70/+gJ/lPoDIcYyaB7z1joa1abSAHxUdN42lt/6YulN/OyYVJ/LwfO/vU
M+fSG0lgxs33DBfTAQJAUP8KNyAFDtoIqr1rC1K2Dp0cINJ6y+WulBgJh7czkGYG
+/SmEjdGMlW+imO0keQNiMVZzKcgbVtAxOxNeWaYaQ==
-----END RSA PRIVATE KEY-----
");
        }

        [Test]
        public void ExportSnkPublicKeyCertificate_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] snkCertificateData = GetSnkCertificateData;

            // Act
            var result = CertificateUtils.ExportSnkPublicKeyCertificate(
                snkCertificateData);

            // Assert
            result.ShouldNotBeEmpty();
        }

        [Test]
        public void ExportSnkPublicKeyCertificateToPEM_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] certificateData = GetSnkCertificateData;

            // Act
            var result = CertificateUtils.ExportSnkPublicKeyToPEM(
                certificateData);

            // Assert
            result.ShouldNotBeEmpty();
            result.ShouldBe(@"-----BEGIN PUBLIC KEY-----
MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDdaTOaR7btN54IqXoPNBwDdZrf
5dFLHATDsq88KNpm2ZFwQkcZnWuFRhVw0CQgEEvEVN9huk8IKloZ8sKh71O5obXS
sww6gIq8otEAdzsUlx0t8o8VHih3XFpWWkBRYtdE5QmGjU1J/KmQ/4PXKWUbAy03
PKBEhoj/Uhc81gLh+QIDAQAB
-----END PUBLIC KEY-----
");
        }

        [Test]
        public void GetPublicKey_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] snkData = GetSnkCertificateData;

            // Act
            var result = CertificateUtils.GetPublicKey(
                snkData);

            // Assert 
            result.ShouldNotBeEmpty();
            
        }

        [Test]
        public void GetPublicKeyToken_StateUnderTest_ExpectedBehavior()
        {
            // Arrange

            byte[] snkPublicKey = null;

            // Act
            var result = CertificateUtils.GetPublicKeyToken(
                snkPublicKey);

            // Assert
            Assert.Fail();
        }
    }
}
