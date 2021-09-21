using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected List<RopeElement> m_ElementList;

    [SerializeField]
    protected float m_Mass;
    
    #region 내부 컴포넌트
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public float mass { get => m_Mass; }
    public int maxIndex { get => m_ElementList.Count - 1; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        m_ElementList = new List<RopeElement>();

        this.GetComponentsInChildren<RopeElement>(true, m_ElementList);
        for (int i = 0; i < m_ElementList.Count; ++i)
        {
            m_ElementList[i].index = i;

            m_ElementList[i].gameObject.SetActive(true);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_ElementList[maxIndex]?.rigidBody2D.AddForce(Vector2.right * m_Mass);
        }
    }
    #endregion
}
