namespace WordKiller.Scripts.File.Encryption
{
    public interface IEncryption
    {
        public string Encrypt(string text);
        public string Decrypt(string text);
    }
}
