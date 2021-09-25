using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceZone : MonoBehaviour
{
    protected Tile m_Tile;

    [SerializeField, ReadOnly]
    protected float m_Drag;

    #region 내부 컴포넌트
    protected IceZoneCollider m_Collider;
    #endregion
    #region 외부 프로퍼티
    public Tile tile { get => m_Tile; }
    public float drag { get => m_Drag; set => m_Drag = value; }
    #endregion
    #region 외부 함수
    public void __Initialize(Tile tile)
    {
        m_Tile = tile;

        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<IceZoneCollider>();
            m_Collider.__Initialize(this);
        }
    }
    #endregion
}
