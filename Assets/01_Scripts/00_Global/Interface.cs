using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEraserable
{
    void Erase();
}
public interface IObjectType
{
    GameObject GetGameObject();
    E_ObjectType GetObjectType();
}