using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZone : MonoBehaviour
{
    protected Tile m_Tile;
    protected int m_SafetyZoneCount;
    [SerializeField, ReadOnly]
    protected bool m_IsFinishZone;

    #region 내부 컴포넌트
    protected SafetyZoneAnimator m_Animator;
    protected SafetyZoneCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니저
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public Tile tile { get => m_Tile; }
    public Vector3 SpawnPoint => transform.position;
    public BoxCollider2D Collider => m_Collider.Collider;

    public int safetyZoneCount => m_SafetyZoneCount;
    public bool isFinishZone
    {
        get => m_IsFinishZone;
        set => m_IsFinishZone = value;
    }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize(Tile tile)
    {
        m_Tile = tile;

        m_SafetyZoneCount = M_SafetyZone.safetyZoneIndex;

        if (null == m_Animator)
        {
            m_Animator = transform.GetComponentInChildren<SafetyZoneAnimator>();
        }
        m_Animator.__Initialize(this);
        if (null == m_Collider)
        {
            m_Collider = transform.GetComponentInChildren<SafetyZoneCollider>();
        }
        m_Collider.__Initialize(this);
    }

    public void SetText(int standard)
    {
        if (m_SafetyZoneCount < standard)
            return;

        --m_SafetyZoneCount;
        m_Animator.text.text = m_SafetyZoneCount.ToString();
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {

    }
    public void OnPlayExit()
    {

    }
    #endregion
    #region 유니티 콜백 함수
    #endregion
}
