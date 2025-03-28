using UnityEngine;
using System.IO;
using System.Security.Cryptography;

public class EncryptToFile : MonoBehaviour
{
    void Start()
    {
        // Generate a new encryption key and initialization vector
        byte[] key = new byte[32];
        byte[] iv = new byte[16];
        using (RijndaelManaged aes = new RijndaelManaged())
        {
            aes.GenerateKey();
            aes.GenerateIV();
            key = aes.Key;
            iv = aes.IV;
        }

        // Encrypt a string and write it to a file
        string data = "Hello, world!";
        byte[] encryptedData = EncryptStringToBytes(data, key, iv);
        File.WriteAllBytes(Application.persistentDataPath + "/encrypted.dat", encryptedData);

        Debug.Log("Encrypted data written to file.");

        // Read the encrypted data from the file and decrypt it
        byte[] readData = File.ReadAllBytes(Application.persistentDataPath + "/encrypted.dat");
        string decryptedData = DecryptStringFromBytes(readData, key, iv);

        Debug.Log("Decrypted data: " + decryptedData);
    }

    // Encrypts a string using RijndaelManaged encryption
    byte[] EncryptStringToBytes(string data, byte[] key, byte[] iv)
    {
        byte[] encrypted;

        using (RijndaelManaged aes = new RijndaelManaged())
        {
            aes.Key = key;
            aes.IV = iv;

            // Create an encryptor to perform the stream transform
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            // Create the streams used for encryption
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        // Write the data to the stream
                        sw.Write(data);
                    }

                    encrypted = ms.ToArray();
                }
            }
        }

        return encrypted;
    }

    // Decrypts a byte array using RijndaelManaged encryption
    string DecryptStringFromBytes(byte[] data, byte[] key, byte[] iv)
    {
        string plaintext = null;

        using (RijndaelManaged aes = new RijndaelManaged())
        {
            aes.Key = key;
            aes.IV = iv;

            // Create a decryptor to perform the stream transform
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            // Create the streams used for decryption
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        plaintext = sr.ReadToEnd();
                    }
                }
            }
        }

        return plaintext;
    }
}
