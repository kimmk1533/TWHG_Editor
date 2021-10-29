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
public interface IClickerableObject
{
	E_ObjectType GetObjectType();
	GameObject GetGameObject();
	Renderer GetRenderer();

	T GetComponent<T>();
}
public interface ISaveHandler
{
	void SaveData(XmlWriter writer);
}
public interface ILoadHandler
{
	void LoadData(XmlReader reader);
}