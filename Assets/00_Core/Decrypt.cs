using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

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
