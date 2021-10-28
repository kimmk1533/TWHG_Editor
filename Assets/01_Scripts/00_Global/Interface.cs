using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public interface IEraserableObject
{
	void EraseObject();
}
public interface IEraserableTile
{
	void EraseTile(E_ObjectType currentType = E_ObjectType.None);
}
public interface IClickedObject
{
	SpriteRenderer GetSpriteRenderer();
	GameObject GetGameObject();
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