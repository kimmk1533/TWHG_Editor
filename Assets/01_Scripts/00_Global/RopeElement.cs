using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MyRigidBody2D))]
public class RopeElement : MonoBehaviour
{
    protected Rope m_Rope;
    protected int m_Index;

    [SerializeField]
    protected RopeElement m_LinkedElement;

    #region 내부 컴포넌트
    protected MyRigidBody2D m_Rigidbody;
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    public int index { get => m_Index; set => m_Index = value; }
    public RopeElement linkedElement { get => m_LinkedElement; set => m_LinkedElement = value; }
    public MyRigidBody2D rigidBody2D { get => m_Rigidbody; }
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public void AddForce(int n, Vector2 force)
    {
        m_Rigidbody.AddForce(n * force * m_Rope.mass);

        m_LinkedElement?.AddForce(n - 1, force);
    }
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        Transform parent = transform.parent;

        m_Rope = parent.GetComponent<Rope>();
        m_Rigidbody = GetComponent<MyRigidBody2D>();
    }
    void FixedUpdate()
    {
        if (null != m_LinkedElement &&
            m_Rigidbody.velocity.magnitude != 0f)
        {
            m_Rigidbody.AddForce((m_Index - 1) * (Vector2)m_LinkedElement.m_Rigidbody.velocity);
        }
    }
    #endregion
}
