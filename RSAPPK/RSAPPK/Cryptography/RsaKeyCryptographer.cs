using System;
using System.Security.Cryptography;

namespace RSAPPK.Cryptography
{
    /// <summary>Utilizes RSA cryptography to secure the key and initialization vector of an AES data cryptographer.</summary>
    public class RsaKeyCryptographer
    {
        #region Fields

        private readonly RSACryptoServiceProvider rsa;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="RsaKeyCryptographer" /> class.</summary>
        /// <param name="publicPrivateKeyXml">The public private key XML.</param>
        /// <exception cref="System.TypeInitializationException">EDAPI.RsaKeyCryptographer</exception>
        public RsaKeyCryptographer(string publicPrivateKeyXml)
        {
            try
            {
                rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicPrivateKeyXml);
            }
            catch (Exception ex)
            {
                throw new TypeInitializationException("AesRsaCryptographer.RsaKeyCryptographer", ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>Decrypts the specified encrypted information.</summary>
        /// <param name="encryptedBytes">The encrypted data bytes to decrypt.</param>
        /// <returns>The decrypted data.</returns>
        /// <exception cref="CryptographicException">An error occurred during the key decryption process.</exception>
        public byte[] DecryptKey(byte[] encryptedBytes)
        {
            try
            {
                return rsa.Decrypt(encryptedBytes, false);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("An error occurred during the key decryption process.", ex);
            }
        }

        /// <summary>Encrypts the specified information.</summary>
        /// <param name="bytes">The data bytes to encrypt.</param>
        /// <returns>The encrypted data.</returns>
        /// <exception cref="CryptographicException">An error occurred during the key encryption process.</exception>
        public byte[] EncryptKey(byte[] bytes)
        {
            try
            {
                return rsa.Encrypt(bytes, false);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("An error occurred during the key encryption process.", ex);
            }
        }

        #endregion
    }
}
