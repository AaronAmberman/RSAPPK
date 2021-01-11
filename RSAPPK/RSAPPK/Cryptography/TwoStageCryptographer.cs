using System;
using System.Security.Cryptography;
using System.Text;

namespace RSAPPK.Cryptography
{
    /// <summary>A RSA key cryptographer and a AES data cryptographer combined.</summary>
    public class TwoStageCryptographer
    {
        #region Fields

        private readonly int encryptedKeySize = 256;
        private readonly int initializationVectorSize = 16;
        private readonly int keySize = 32;

        #endregion

        #region Properties

        public string RsaPpkName { get; }

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="TwoStageCryptographer" /> class.</summary>
        /// <param name="psaPpkName">The name of the RSA PPK pair to use.</param>
        public TwoStageCryptographer(string rsaPpkName)
        {
            RsaPpkName = rsaPpkName;
        }

        #endregion

        #region Methods

        /// <summary>Decrypts the specified encrypted value.</summary>
        /// <param name="encryptedValue">The encrypted value.</param>
        /// <returns>The decrypted string.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException">When an error occurs during the decryption process.</exception>
        public string Decrypt(string encryptedValue)
        {
            byte[] convertedEncryptedValue = Convert.FromBase64String(encryptedValue);

            // copy the encrypted key out first
            byte[] encryptedKey = new byte[encryptedKeySize];

            Buffer.BlockCopy(convertedEncryptedValue, 0, encryptedKey, 0, encryptedKeySize);

            // copy the encrypted data out second
            byte[] encryptedData = new byte[convertedEncryptedValue.Length - encryptedKeySize];

            Buffer.BlockCopy(convertedEncryptedValue, encryptedKeySize, encryptedData, 0, encryptedData.Length);

            // decrypt the key third
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, new CspParameters
            {
                KeyContainerName = RsaPpkName
            });

            byte[] encryptedKeyAndIv = rsa.Decrypt(encryptedKey, false);

            // next decrypt the data
            byte[] key = new byte[32];
            byte[] iv = new byte[initializationVectorSize];

            // get key
            Buffer.BlockCopy(encryptedKeyAndIv, 0, key, 0, keySize);

            // get initialization vector
            Buffer.BlockCopy(encryptedKeyAndIv, keySize, iv, 0, initializationVectorSize);

            AesCryptoServiceProvider aes = new AesCryptoServiceProvider
            {
                IV = iv,
                Key = key
            };

            ICryptoTransform decryptor = aes.CreateDecryptor();

            byte[] encodedValue = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            decryptor.Dispose();

            string decryptedValue = Encoding.UTF8.GetString(encodedValue);

            return decryptedValue;
        }

        /// <summary>Encrypts the specified value.</summary>
        /// <param name="value">The value.</param>
        /// <returns>The encrypted string.</returns>
        /// <exception cref="System.Security.Cryptography.CryptographicException">When an error occurs during the decryption process.</exception>
        public string Encrypt(string value)
        {
            // first generate our AES Key and IV
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            aes.GenerateIV();
            aes.GenerateKey();

            byte[] keyAndIv = new byte[aes.IV.Length + aes.Key.Length];

            // copy them into an array together
            Buffer.BlockCopy(aes.Key, 0, keyAndIv, 0, aes.Key.Length);
            Buffer.BlockCopy(aes.IV, 0, keyAndIv, aes.Key.Length, aes.IV.Length);

            // encrypt the key
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048, new CspParameters
            {
                KeyContainerName = RsaPpkName
            });

            byte[] encryptedKeyAndIv = rsa.Encrypt(keyAndIv, false);

            // next encrypt the data
            byte[] encodedData = Encoding.UTF8.GetBytes(value);

            ICryptoTransform cryptoTransform = aes.CreateEncryptor();

            byte[] encryptedData = cryptoTransform.TransformFinalBlock(encodedData, 0, encodedData.Length);

            cryptoTransform.Dispose();

            byte[] encryptedKeyAndEncryptedData = new byte[encryptedKeyAndIv.Length + encryptedData.Length];

            // copy in the encrypted key first
            Buffer.BlockCopy(encryptedKeyAndIv, 0, encryptedKeyAndEncryptedData, 0, encryptedKeyAndIv.Length);

            // copy in the encrypted data second
            Buffer.BlockCopy(encryptedData, 0, encryptedKeyAndEncryptedData, encryptedKeyAndIv.Length, encryptedData.Length);

            // convert to a string and return
            string convertedEncryptedValue = Convert.ToBase64String(encryptedKeyAndEncryptedData);

            return convertedEncryptedValue;
        }

        #endregion
    }
}
