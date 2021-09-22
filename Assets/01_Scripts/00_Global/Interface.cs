using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public interface IEraserable
{
    void Erase();
}
public interface IClickedObject
{
    GameObject GetGameObject();
    SpriteRenderer GetSpriteRenderer();
    E_ObjectType GetObjectType();
}
public interface ISaveHandler
{
    void Save(XmlWriter writer);
}
public interface ILoadHandler
{
    void Load(XmlReader reader);
}