using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour
{
    protected Tile m_Tile;

    [SerializeField, ReadOnly]
    protected Vector2 m_Gravity;

    #region 내부 컴포넌트
    protected GravityZoneCollider m_Collider;
    #endregion
    #region 외부 프로퍼티
    public Tile tile { get => m_Tile; }
    public Vector2 gravity { get => m_Gravity; set => m_Gravity = value; }
    #endregion
    #region 외부 함수
    public void __Initialize(Tile tile)
    {
        m_Tile = tile;

        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<GravityZoneCollider>();
            m_Collider.__Initialize(this);
        }
    }
    #endregion
}
