namespace Infrastructure.Cryptography
{
    public interface ICryptographyService
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
