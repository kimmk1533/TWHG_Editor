using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// √‚√≥: https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=ekdlsk2124&logNo=220588249362
public static class Encrypt
{
    public static string EncryptData(string filepath)
    {
        if (!File.Exists(filepath))
            return null;

        string data = File.ReadAllText(filepath);

        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        byte[] toEmcryptArray = UTF8Encoding.UTF8.GetBytes(data);
        RijndaelManaged rDel = new RijndaelManaged();

        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;

        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateEncryptor();

        byte[] resultArray = cTransform.TransformFinalBlock(toEmcryptArray, 0, toEmcryptArray.Length);

        return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    }
}
