using System.Collections;
using System.Collections.Generic;
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