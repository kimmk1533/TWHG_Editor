using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceZoneCollider : MonoBehaviour, IClickedObject, IEraserableTile
{
    protected IceZone m_IceZone;

    #region 내부 컴포넌트
    protected MyPhysics.BoxCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티 
    #region 매니저
    protected IceZoneManager M_IceZone => IceZoneManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public IceZone iceZone { get => m_IceZone; }
    #endregion
    #region 외부 함수
    public void __Initialize(IceZone iceZone)
    {
        m_IceZone = iceZone;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
        }
    }

    public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
    {
        if (currentType != E_ObjectType.IceZone)
        {
            M_IceZone.DespawnIceZone(m_IceZone);
        }
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return null;
    }
    public GameObject GetGameObject()
    {
        return m_IceZone.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.IceZone;
    }
    #endregion
}
