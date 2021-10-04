using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// √‚√≥: https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=ekdlsk2124&logNo=220588249362
public static class Decrypt
{
    public static string DecryptData(string filepath)
    {
        if (!File.Exists(filepath))
            return null;

        string data = File.ReadAllText(filepath);

        byte[] keyArray = UTF8Encoding.UTF8.GetBytes("12345678901234567890123456789012");

        byte[] toEncryptArray = Convert.FromBase64String(data);

        RijndaelManaged rDel = new RijndaelManaged();
        rDel.Key = keyArray;
        rDel.Mode = CipherMode.ECB;

        rDel.Padding = PaddingMode.PKCS7;

        ICryptoTransform cTransform = rDel.CreateDecryptor();

        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        return UTF8Encoding.UTF8.GetString(resultArray);
    }
}
