using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class XMLUtility
{
    #region 확장 메서드
    public static void WriteVector(this XmlWriter writer, Vector3 value)
    {
        // x 시작
        writer.WriteStartElement("x");
        // x 입력
        writer.WriteValue(value.x);
        // x 끝
        writer.WriteEndElement();

        // y 시작
        writer.WriteStartElement("y");
        // y 입력
        writer.WriteValue(value.y);
        // y 끝
        writer.WriteEndElement();

        // z 시작
        writer.WriteStartElement("z");
        // z 입력
        writer.WriteValue(value.z);
        // z 끝
        writer.WriteEndElement();
    }
    public static void WriteColor(this XmlWriter writer, Color value)
    {
        // r 시작
        writer.WriteStartElement("r");
        // r 입력
        writer.WriteValue(value.r);
        // r 끝
        writer.WriteEndElement();

        // g 시작
        writer.WriteStartElement("g");
        // g 입력
        writer.WriteValue(value.g);
        // g 끝
        writer.WriteEndElement();

        // b 시작
        writer.WriteStartElement("b");
        // b 입력
        writer.WriteValue(value.b);
        // b 끝
        writer.WriteEndElement();

        // a 시작
        writer.WriteStartElement("a");
        // a 입력
        writer.WriteValue(value.a);
        // a 끝
        writer.WriteEndElement();
    }
    public static Vector3 ReadVector(this XmlReader reader, string name)
    {
        Vector3 value = new Vector3();

        if (!reader.IsStartElement(name))
        {
            reader.LoadToElement(name);
        }

        if (reader.LoadToElement("x"))
        {
            reader.ReadStartElement("x");
            if (!float.TryParse(reader.Value, out value.x))
            {
                value.x = 0f;
            }
        }
        if (reader.LoadToElement("y"))
        {
            reader.ReadStartElement("y");
            if (!float.TryParse(reader.Value, out value.y))
            {
                value.y = 0f;
            }
        }
        if (reader.LoadToElement("z"))
        {
            reader.ReadStartElement("z");
            if (!float.TryParse(reader.Value, out value.z))
            {
                value.z = 0f;
            }
        }

        return value;
    }
    public static Color ReadColor(this XmlReader reader, string name)
    {
        Color value = new Color();

        if (!reader.IsStartElement(name))
        {
            reader.LoadToElement(name);
        }

        if (reader.LoadToElement("r"))
        {
            reader.ReadStartElement("r");
            if (!float.TryParse(reader.Value, out value.r))
            {
                value.r = 0f;
            }
        }
        if (reader.LoadToElement("g"))
        {
            reader.ReadStartElement("g");
            if (!float.TryParse(reader.Value, out value.g))
            {
                value.g = 0f;
            }
        }
        if (reader.LoadToElement("b"))
        {
            reader.ReadStartElement("b");
            if (!float.TryParse(reader.Value, out value.b))
            {
                value.b = 0f;
            }
        }
        if (reader.LoadToElement("a"))
        {
            reader.ReadStartElement("a");
            if (!float.TryParse(reader.Value, out value.a))
            {
                value.a = 0f;
            }
        }

        return value;
    }

    public static bool LoadToElement(this XmlReader reader, string name)
    {
        while (reader.Read())
        {
            if (reader.IsStartElement(name))
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}