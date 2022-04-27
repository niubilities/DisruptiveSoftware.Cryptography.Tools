namespace CertUtil
{

    public static class CertUtilConstants
    {
        // Object Formats.
        public static readonly Dictionary<string, CryptographicObjectType> CryptographicObjects = new()
        {
            { "Public Key Certificate", CryptographicObjectType.PublicKey },
            { "Private Key (no password)", CryptographicObjectType.PrivateKey }
        };

        // Private Key Formats.
        public static readonly Dictionary<string, string> PrivateKeyFormats = new()
        {
            { "Base64 DER encoded (*.key)", "*.key" },
            { "Binary DER encoded (*.key)", "*.key" }
        };

        // Public Key Certificate Formats.
        public static readonly Dictionary<string, string> PublicKeyCertificatesFormats = new()
        {
            { "Base64 DER encoded (*.pem)", "*.pem" },
            { "Binary DER encoded (*.crt)", "*.crt" },
            { "Binary DER encoded (*.cer)", "*.cer" }
        };
    }

    public enum CryptographicObjectType
    {
        PublicKey,

        PrivateKey
    }
}