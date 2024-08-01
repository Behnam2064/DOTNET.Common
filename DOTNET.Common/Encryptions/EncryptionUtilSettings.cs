using System.Text;


namespace DOTNET.Common.Encryptions
{
    public class EncryptionUtilSettings
    {
        public byte[] KeyBytes { get; init; }
        public byte[] IVBytes { get; init; }

        public EncryptionUtilSettings(byte[] key, byte[] iv)
        {
            if (key.Length != 16)
                throw new ArgumentOutOfRangeException(nameof(key));

            if (iv.Length != 16)
                throw new ArgumentOutOfRangeException(nameof(key));

            KeyBytes = key;
            IVBytes = iv;
        }

        public EncryptionUtilSettings(string Key, string Iv) : this(Encoding.UTF8.GetBytes(Key), Encoding.UTF8.GetBytes(Iv))
        {

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