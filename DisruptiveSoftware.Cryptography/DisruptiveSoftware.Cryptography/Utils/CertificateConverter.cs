using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisruptiveSoftware.Cryptography.Utils
{
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    public enum FileFormat
    {
        Snk,
        P12,
        Pfx,
        Pem,
        Crt,
        Cer,
        Key

    }

    public interface Importor
    {
        byte[] Import(string path);
        byte[] Import(byte[] data, FileFormat format);
    }

    public class CertificateConverter
    {
        private static readonly Dictionary<string, FileFormat> FileFormats = new(StringComparer.OrdinalIgnoreCase);

        static CertificateConverter()
        {
            FileFormats.Add(".snk", FileFormat.Snk);
            FileFormats.Add(".p12", FileFormat.P12);
            FileFormats.Add(".pfx", FileFormat.Pfx);
            FileFormats.Add(".crt", FileFormat.Crt);
            FileFormats.Add(".cer", FileFormat.Cer);
            FileFormats.Add(".key", FileFormat.Key);
            FileFormats.Add(".pem", FileFormat.Pem);
        }
        private X509Certificate2 _x509Certificate2 = null;



        public CertificateConverter ImportFrom(string path,string? password)
        {
            if(!File.Exists(path))throw new FileNotFoundException("File must be existed and having access right",path);
            var extension = Path.GetExtension(path);

            if (string.IsNullOrWhiteSpace(extension))
            {
                throw new NotSupportedException($"The file must have a extension when importing from path or Using ImportFrom(byte[] data, FileFormat format). File:{path}");
            }

            var format = FileFormats[extension];

            return ImportFrom(File.ReadAllBytes(path), format,password);
        }

        public CertificateConverter ImportFrom(byte[] data, FileFormat format,string? password)
        {
            if (format == FileFormat.Snk)
            {
                using RSACryptoServiceProvider rsa = new();
                rsa.ImportCspBlob(data);
                _x509Certificate2 = new X509Certificate2(data);


            }

            return this;
        }

    }
}
