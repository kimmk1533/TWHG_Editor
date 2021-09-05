using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SafetyZone : MonoBehaviour
{
    protected int m_SafetyZoneCount;

    #region 내부 컴포넌트
    protected SafetyZoneCollider m_Collider;
    protected TextMeshPro m_Text;
    #endregion
    #region 내부 프로퍼티
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion
    #region 외부 프로퍼티
    public Vector3 SpawnPoint => transform.position;
    public BoxCollider2D Collider => m_Collider.Collider;
    public int SafetyZoneCount => m_SafetyZoneCount;
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        m_SafetyZoneCount = M_SafetyZone.SafetyZoneCount;

        if (null == m_Collider)
        {
            m_Collider = transform.GetComponentInChildren<SafetyZoneCollider>();
            m_Collider.__Initialize(this);
        }
        if (null == m_Text)
        {
            m_Text = transform.GetComponentInChildren<TextMeshPro>();
            m_Text.text = m_SafetyZoneCount.ToString();
        }
    }

    public void SetText(int standard)
    {
        if (m_SafetyZoneCount < standard)
            return;

        --m_SafetyZoneCount;
        m_Text.text = m_SafetyZoneCount.ToString();
    }
    #endregion
    #region 유니티 콜백 함수
    #endregion
}
