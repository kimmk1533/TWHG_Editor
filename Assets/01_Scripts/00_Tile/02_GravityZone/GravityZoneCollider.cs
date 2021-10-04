using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZoneCollider : MonoBehaviour, IEraserableTile, IClickedObject
{
    protected GravityZone m_GravityZone;

    #region 내부 컴포넌트
    protected MyPhysics.BoxCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니저
    protected GravityZoneManager M_GravityZone => GravityZoneManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public GravityZone gravityZone { get => m_GravityZone; }
    #endregion
    #region 외부 함수
    public void __Initialize(GravityZone gravityZone)
    {
        m_GravityZone = gravityZone;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
        }
    }

    public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
    {
        if (currentType != E_ObjectType.GravityZone)
        {
            M_GravityZone.DespawnGravityZone(m_GravityZone);
        }
    }
    public SpriteRenderer GetSpriteRenderer()
    {
        return null;
    }
    public GameObject GetGameObject()
    {
        return m_GravityZone.gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.GravityZone;
    }
    #endregion
}
