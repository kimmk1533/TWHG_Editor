using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZone : MonoBehaviour
{

    #region 내부 컴포넌트
    protected SafetyZoneCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        if (null == m_Collider)
        {
            m_Collider = transform.GetComponentInChildren<SafetyZoneCollider>();
            m_Collider.__Initialize();
        }
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
