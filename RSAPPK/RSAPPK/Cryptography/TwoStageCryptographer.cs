using System;
using System.Security.Cryptography;
using System.Text;

namespace RSAPPK.Cryptography
{
    /// <summary>A RSA key cryptographer and a AES data cryptographer.</summary>
    public class TwoStageCryptographer
    {
        #region Fields

        private AesDataCryptographer dataCryptographer;
        private int encryptedKeySize = 128;
        private int initializationVectorSize = 16;
        private RsaKeyCryptographer keyCryptographer;
        private int keySize = 32;

        #endregion

        #region Properties

        public string PublicPrivateKeyXml { get; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TwoStageCryptographer" /> class.</summary>
        public TwoStageCryptographer(string publicPrivateKeyXml)
        {
            PublicPrivateKeyXml = publicPrivateKeyXml;
        }

        #endregion

        #region Methods

        /// <summary>Decrypts the specified encrypted value.</summary>
        /// <param name="encryptedValue">The encrypted value.</param>
        /// <returns>The decrypted string.</returns>
        public string Decrypt(string encryptedValue)
        {
            try
            {
                byte[] rawData = Convert.FromBase64String(encryptedValue);

                byte[] key = new byte[keySize];
                byte[] iv = new byte[initializationVectorSize];
                byte[] encryptedKey = new byte[encryptedKeySize];
                byte[] encryptedData = new byte[rawData.Length - encryptedKeySize];

                Buffer.BlockCopy(rawData, 0, encryptedKey, 0, encryptedKey.Length);
                Buffer.BlockCopy(rawData, encryptedKey.Length, encryptedData, 0, encryptedData.Length);

                keyCryptographer = new RsaKeyCryptographer(PublicPrivateKeyXml);

                byte[] decryptedKey = keyCryptographer.DecryptKey(encryptedKey);

                Buffer.BlockCopy(decryptedKey, 0, key, 0, key.Length);
                Buffer.BlockCopy(decryptedKey, key.Length, iv, 0, iv.Length);

                dataCryptographer = new AesDataCryptographer(key, iv);

                byte[] decryptedData = dataCryptographer.DecryptData(encryptedData);

                string decryptedValue = Encoding.UTF8.GetString(decryptedData);

                return decryptedValue;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("An error occurred during the decryption process.", ex);
            }
        }

        /// <summary>Encrypts the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The encrypted string.</returns>
        public string Encrypt(string value)
        {
            try
            {
                dataCryptographer = new AesDataCryptographer();

                byte[] keyAndInitializationVectorBuffer = new byte[keySize + initializationVectorSize];

                Buffer.BlockCopy(dataCryptographer.Key, 0, keyAndInitializationVectorBuffer, 0, keySize);
                Buffer.BlockCopy(dataCryptographer.InitializationVector, 0, keyAndInitializationVectorBuffer, keySize, initializationVectorSize);

                keyCryptographer = new RsaKeyCryptographer(PublicPrivateKeyXml);

                byte[] encryptedKey = keyCryptographer.EncryptKey(keyAndInitializationVectorBuffer);

                byte[] rawData = Encoding.UTF8.GetBytes(value);

                byte[] encryptedData = dataCryptographer.EncryptData(rawData);

                byte[] encryptedKeyAndEncryptedData = new byte[encryptedKey.Length + encryptedData.Length];

                Buffer.BlockCopy(encryptedKey, 0, encryptedKeyAndEncryptedData, 0, encryptedKey.Length);
                Buffer.BlockCopy(encryptedData, 0, encryptedKeyAndEncryptedData, encryptedKey.Length, encryptedData.Length);

                var encryptedValue = Convert.ToBase64String(encryptedKeyAndEncryptedData);

                return encryptedValue;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("An error occurred during the encryption process.", ex);
            }
        }

        #endregion
    }
}
