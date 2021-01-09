using System;
using System.Security.Cryptography;

namespace RSAPPK.Cryptography
{
    // <summary>Utilizes AES cryptography to secure the data.</summary>
    public class AesDataCryptographer
    {
        #region Fields

        private readonly AesCryptoServiceProvider aes;

        #endregion

        #region Properties

        /// <summary>Gets the initialization vector.</summary>
        public byte[] InitializationVector => aes.IV;

        /// <summary>Gets the key.</summary>
        public byte[] Key => aes.Key;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="AesDataCryptographer" /> class.</summary>
        /// <exception cref="System.TypeInitializationException">EDAPI.AesDataCryptographer</exception>
        public AesDataCryptographer()
        {
            try
            {
                aes = new AesCryptoServiceProvider();
                aes.GenerateKey();
                aes.GenerateIV();
            }
            catch (Exception ex)
            {
                throw new TypeInitializationException("AesRsaCryptographer.AesDataCryptographer", ex);
            }
        }

        /// <summary>Initializes a new instance of the <see cref="AesDataCryptographer" /> class.</summary>
        /// <param name="key">The key.</param>
        /// <param name="iv">The iv.</param>
        /// <exception cref="System.ArgumentNullException">The key is null or the iv (initialization vector) is null.</exception>
        /// <exception cref="System.ArgumentException">The key cannot be empty or the iv (initialization vector) cannot be empty.</exception>
        /// <exception cref="System.TypeInitializationException">EDAPI.AesDataCryptographer</exception>
        public AesDataCryptographer(byte[] key, byte[] iv)
        {
            if (key == null) throw new ArgumentNullException(nameof(key), @"The key cannot be null.");
            if (key.Length == 0) throw new ArgumentException(@"The key cannot be empty.", nameof(key));

            if (iv == null) throw new ArgumentNullException(nameof(iv), @"The iv (initialization vector) cannot be null.");
            if (iv.Length == 0) throw new ArgumentException(@"The iv (initialization vector) cannot be empty.", nameof(iv));

            try
            {
                aes = new AesCryptoServiceProvider
                {
                    Key = key,
                    IV = iv
                };
            }
            catch (Exception ex)
            {
                throw new TypeInitializationException("AesRsaCryptographer.AesDataCryptographer", ex);
            }
        }

        #endregion

        #region Methods

        /// <summary>Decrypts the data.</summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <returns>The decrypted binary data.</returns>
        public byte[] DecryptData(byte[] encryptedData)
        {
            byte[] returnValue;

            using (var decryptor = aes.CreateDecryptor())
            {
                returnValue = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            }

            return returnValue;
        }

        /// <summary>Encrypts the data.</summary>
        /// <param name="rawData">The raw data.</param>
        /// <returns>The encrypted binary data.</returns>
        public byte[] EncryptData(byte[] rawData)
        {
            byte[] returnValue;

            using (var encryptor = aes.CreateEncryptor())
            {
                returnValue = encryptor.TransformFinalBlock(rawData, 0, rawData.Length);
            }

            return returnValue;
        }

        #endregion
    }
}
