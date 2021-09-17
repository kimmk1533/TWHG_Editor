using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

// https://bravenewmethod.com/2014/09/13/lightweight-csv-reader-for-unity/#comment-7111
public static class CSVUtility
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file, FileFormat format = FileFormat.csv)
    {
        string path = Path.Combine(Application.dataPath, file) + "." + format.ToString();
        string data = new StreamReader(path).ReadToEnd();

        string[] lines = Regex.Split(data, LINE_SPLIT_RE);

        List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
        if (lines.Length <= 1)
            return list;

        string[] header = Regex.Split(lines[0], SPLIT_RE);

        for (int i = 1; i < lines.Length; ++i)
        {
            string[] values = Regex.Split(lines[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "")
                continue;

            var entry = new Dictionary<string, object>();
            for (int j = 0; j < header.Length && j < values.Length; ++j)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;

                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }

            list.Add(entry);
        }

        return list;
    }
    public static void Write(string file, in List<Dictionary<string, object>> data)
    {
        Write(file, FileFormat.csv, data);
    }
    public static void Write(string file, FileFormat format, in List<Dictionary<string, object>> data)
    {
        string path = Path.Combine(Application.dataPath, file) + "." + format.ToString();

        StreamWriter writer = null;
        try
        {
            writer = new StreamWriter(path);

            string key;
            object item;

            var enumerator = data[0].Keys.GetEnumerator();

            #region ≈∞ ¿˙¿Â
            for (int i = 0; i < data[0].Count - 1; ++i)
            {
                enumerator.MoveNext();
                key = enumerator.Current;

                writer.Write(key);
                writer.Write(',');
            }
            enumerator.MoveNext();
            key = enumerator.Current;
            writer.WriteLine(key);
            #endregion

            for (int i = 0; i < data.Count; ++i)
            {
                enumerator = data[i].Keys.GetEnumerator();

                for (int j = 0; j < data[i].Count - 1; ++j)
                {
                    enumerator.MoveNext();
                    key = enumerator.Current;
                    item = data[i][key];

                    writer.Write(item);
                    writer.Write(',');
                }

                enumerator.MoveNext();
                key = enumerator.Current;
                item = data[i][key];

                writer.WriteLine(item);
            }

            writer.Close();
            writer.Dispose();
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        finally
        {
            if (null != writer)
            {
                writer.Close();
            }
        }
    }

    public enum FileFormat
    {
        csv,
        txt,
    }
}
