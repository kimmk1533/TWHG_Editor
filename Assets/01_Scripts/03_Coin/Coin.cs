using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    #region 내부 컴포넌트
    protected CoinCollider m_Collider;
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
            m_Collider = GetComponentInChildren<CoinCollider>();
            m_Collider.__Initialize(this);
        }
    }
    public void __Finalize()
    {

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
