using DOTNET.Common.Interfaces;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;


namespace DOTNET.Common.Encryptions
{

    public class EncryptionUtil : ISecurityText
    {
        //private static byte[] key = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes key
        //private static byte[] iv = Encoding.UTF8.GetBytes("1234567890123456"); // 16 bytes IV
        public static EncryptionUtilSettings? settings;
        public EncryptionUtil(EncryptionUtilSettings set)
        {
            settings = set;
        }
        public EncryptionUtil()
        {

        }

        public string EncryptText(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = settings.KeyBytes;
                aesAlg.IV = settings.IVBytes;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }


        }

        public string DecryptText(string text)
        {
            byte[] cipherText = Convert.FromBase64String(text);
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = settings.KeyBytes;
                aesAlg.IV = settings.IVBytes;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

}



/*
 Android or java side


import javax.crypto.Cipher;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import android.util.Base64;

import java.security.Key;

public class EncryptionUtil {

   private static String encryptedBase64 = "ENCRYPTED_STRING_FROM_NET";
    private static String key = "1234567890123456"; // 16 bytes key
    private static String initVector = "1234567890123456"; // 16 bytes IV


    public static String encrypt(String data) throws Exception {
        IvParameterSpec iv = new IvParameterSpec(initVector.getBytes("UTF-8"));
        SecretKeySpec skeySpec = new SecretKeySpec(key.getBytes("UTF-8"), "AES");

        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
        cipher.init(Cipher.ENCRYPT_MODE, skeySpec, iv);

        byte[] encrypted = cipher.doFinal(data.getBytes());
        return Base64.encodeToString(encrypted, Base64.DEFAULT);
    }

    public static String decrypt(String encryptedData) throws Exception {
        IvParameterSpec iv = new IvParameterSpec(initVector.getBytes("UTF-8"));
        SecretKeySpec skeySpec = new SecretKeySpec(key.getBytes("UTF-8"), "AES");

        Cipher cipher = Cipher.getInstance("AES/CBC/PKCS5PADDING");
        cipher.init(Cipher.DECRYPT_MODE, skeySpec, iv);

        byte[] original = cipher.doFinal(Base64.decode(encryptedData, Base64.DEFAULT));
        return new String(original);
    }
}




 */