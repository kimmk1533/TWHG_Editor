using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZoneCollider : MonoBehaviour, IEraserable, IClickedObject
{
    protected GravityZone m_GravityZone;

    #region 내부 컴포넌트
    protected BoxCollider2D m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니저
    protected GravityZoneManager M_GravityZone => GravityZoneManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public GravityZone gravityZone { get => m_GravityZone; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize(GravityZone gravityZone)
    {
        m_GravityZone = gravityZone;
    }

    public void Erase()
    {
        M_GravityZone.DespawnGravityZone(m_GravityZone);
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
    #region 유니티 콜백 함수
    void Awake()
    {
        
    }
    void Update()
    {
        
    }
    #endregion
}
