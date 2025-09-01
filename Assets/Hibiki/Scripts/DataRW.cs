using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class DataRW : MonoBehaviour
{
    /// <summary>
    /// Encrypt and save data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="saveData"></param>
    /// <param name="path"></param>
    public static void Save<T>(T saveData, string path)
    {
        string saveFilePath = Application.persistentDataPath + "/" + path + ".bytes";
        Debug.Log(saveFilePath);
        string jsonString = JsonUtility.ToJson(saveData);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

        byte[] arrEncrypted = AesEncrypt(bytes);
        FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
        try
        {
            file.Write(arrEncrypted, 0, arrEncrypted.Length);

        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }


    }

    /// <summary>
    /// Load data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public static T Load<T>(string path) where T : new()
    {
        string saveFilePath = Application.persistentDataPath + "/" + path + ".bytes";

        if (File.Exists(saveFilePath))
        {
            FileStream file = new FileStream(saveFilePath, FileMode.Open, FileAccess.Read);
            try
            {
                byte[] arrRead = File.ReadAllBytes(saveFilePath);
                byte[] arrDecrypt = AesDecrypt(arrRead);
                string decryptStr = Encoding.UTF8.GetString(arrDecrypt);
                T saveData = JsonUtility.FromJson<T>(decryptStr);

                return saveData;

            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
        else
        {
            Debug.Log("No data exist");
            return new T();
        }
    }


    //Get AesManaged manager
    public static AesManaged GetAesManager()
    {
        string aesIv = "h34fiewrfHFh390w";
        string aesKey = "h34fiewrfHFh390w";

        AesManaged aes = new AesManaged();
        aes.KeySize = 128;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.IV = Encoding.UTF8.GetBytes(aesIv);
        aes.Key = Encoding.UTF8.GetBytes(aesKey);
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }

    //AES Encryption
    public static byte[] AesEncrypt(byte[] byteText)
    {
        AesManaged aes = GetAesManager();

        byte[] encryptText = aes.CreateEncryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return encryptText;
    }

    //Restore AES
    public static byte[] AesDecrypt(byte[] byteText)
    {
        AesManaged aes = GetAesManager();

        byte[] decryptText = aes.CreateDecryptor().TransformFinalBlock(byteText, 0, byteText.Length);

        return decryptText;
    }
}
