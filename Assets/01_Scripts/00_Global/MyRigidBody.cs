using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MyRigidBody : MonoBehaviour
{
    // 기본 중력
    public static readonly float Gravity = -9.8f;

    // 질량
    [SerializeField]
    protected float m_Mass = 1f;
    // 항력 계수
    [SerializeField]
    protected float m_Drag = 0f;
    // 중력 사용 여부
    [SerializeField]
    protected bool m_UseGravity = false;
    // 가해지는 중력
    [SerializeField]
    protected float m_Gravity = Gravity;
    // 물리 사용 여부
    [SerializeField]
    protected bool m_IsKinematic = false;

    #region 내부 정보
    [Space(15)]
    // 속력
    [SerializeField, ReadOnly]
    protected float m_Speed;
    // 저항 속도
    [SerializeField, ReadOnly]
    protected float m_DragSpeed;
    // 속도
    [SerializeField, ReadOnly]
    protected Vector2 m_Velocity;
    // 가속도
    protected Vector2 m_Acceleration;
    // 힘 (작용)
    protected Vector2 m_Force;
    // 저항 (반작용)
    protected Vector2 m_DragForce;
    #endregion

    #region 외부 프로퍼티
    public float mass { get => m_Mass; set => m_Mass = value; }
    public bool useGravity { get => m_UseGravity; set => m_UseGravity = value; }
    public bool isKinematic { get => m_IsKinematic; set => m_IsKinematic = value; }
    #endregion
    #region 내부 함수
    protected Vector2 CalculateDrag(Vector2 force)
    {
        if (force.magnitude <= 0f)
            return Vector2.zero;

        // 밀도
        const float p = 1f;
        // 면적
        const float A = 1f;
        // 속도의 속력
        float v = force.magnitude;
        // 항력 계수
        float Cd = m_Drag;
        // 속도의 방향
        Vector2 dir = force.normalized;

        // 항력 방정식
        Vector2 Fd = -0.105f * p * v * v * A * Cd * dir;

        return Fd;
    }
    #endregion
    #region 외부 함수
    public void AddForce(Vector2 force)
    {
        m_Force += force;
    }
    public void AddForce(float force, Vector2 dir)
    {
        AddForce(dir * force);
    }
    #endregion
    #region 유니티 콜백 함수
    void FixedUpdate()
    {
        #region 중력
        if (m_UseGravity && !m_IsKinematic)
        {
            Vector2 gravity = Vector2.up * m_Gravity * m_Mass;

            AddForce(gravity);
        }
        #endregion
        #region 계산
        m_Acceleration = m_Force / m_Mass;
        m_Velocity += m_Acceleration * Time.deltaTime;
        #endregion
        #region 예외처리
        if (m_Velocity.magnitude > 0)
        {
            m_DragForce = CalculateDrag(m_Velocity) * Time.deltaTime;

            if (m_Velocity.magnitude <= m_DragForce.magnitude)
            {
                m_Velocity = Vector2.zero;
            }
            else
            {
                m_Velocity += m_DragForce;
            }
        }
        else
        {
            m_DragForce = Vector2.zero;
        }

        if (m_Velocity.magnitude < 0.01f)
        {
            m_Velocity = Vector2.zero;
        }
        #endregion
        #region 이동
        if (!m_IsKinematic)
        {
            transform.localPosition += (Vector3)m_Velocity * Time.deltaTime;
        }
        else
        {
            transform.localPosition += (Vector3)m_Force * Time.deltaTime;
        }
        #endregion
        #region 초기화
        if (!m_IsKinematic)
        {
            m_Speed = m_Velocity.magnitude;
        }
        else
        {
            m_Velocity = Vector2.zero;
            m_Speed = m_Force.magnitude;
        }

        m_DragSpeed = m_DragForce.magnitude;
        m_Force = Vector2.zero;
        #endregion
    }
    #endregion
}
