namespace RSAPPK.Database
{
    /// <summary>Represents a RSA public private key pair (PPK).</summary>
    public class RsaPpks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string RsaPpkXml { get; set; }
    }
}
