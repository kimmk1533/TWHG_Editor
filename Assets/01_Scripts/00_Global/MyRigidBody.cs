using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MyRigidBody : MonoBehaviour
{
    // 기본 중력
    public static readonly float Gravity = -9.8f;

    public float m_TempSpeed = 1f;

    // 질량
    [SerializeField]
    protected float m_Mass = 1f;
    // 공기 저항
    [SerializeField]
    protected float m_Drag = 0f;
    // 중력 사용 여부
    [SerializeField]
    protected bool m_UseGravity = false;
    // 가해지는 중력
    [SerializeField]
    protected float m_Gravity = Gravity;

    #region 내부 정보
    [Space(15)]
    // 속력
    [SerializeField, ReadOnly]
    protected float m_Speed;
    // 저항 속도
    [SerializeField, ReadOnly]
    protected float m_DragSpeed;
    //// 종단 속도
    //[SerializeField, ReadOnly]
    //protected float m_TerminalSpeed;
    // 속도
    [SerializeField, ReadOnly]
    protected Vector2 m_Velocity;
    // 가속도
    [SerializeField, ReadOnly]
    protected Vector2 m_Acceleration;
    // 힘 (작용)
    [SerializeField, ReadOnly]
    protected Vector2 m_Force;
    // 저항 (반작용)
    [SerializeField, ReadOnly]
    protected Vector2 m_DragForce;
    #endregion

    #region 외부 프로퍼티
    public float mass { get => m_Mass; set => m_Mass = value; }
    public bool useGravity { get => m_UseGravity; set => m_UseGravity = value; }
    #endregion
    #region 내부 함수
    protected Vector2 CalculateDrag(Vector2 force)
    {
        const float p = 1f;
        const float A = 1f;
        float v = force.magnitude;
        float Cd = m_Drag * 0.01f;
        Vector2 dir = force.normalized;

        Vector2 Fd = -0.5f * p * v * v * A * Cd * dir;

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
        #region 테스트
        AddForce(Vector2.right * Input.GetAxisRaw("Horizontal") * m_TempSpeed);
        AddForce(Vector2.up * Input.GetAxisRaw("Vertical") * m_TempSpeed);
        #endregion
        #region 힘 적용
        if (m_UseGravity)
        {
            Vector2 gravity = Vector2.up * m_Gravity * m_Mass;

            AddForce(gravity);
        }

        if (m_Force.magnitude < CalculateDrag(m_Force).magnitude)
        {
            m_Force = Vector2.zero;
        }
        if (m_Velocity.magnitude < CalculateDrag(m_Velocity).magnitude)
        {
            m_Velocity = Vector2.zero;
        }

        if (m_Velocity.magnitude > 0)
        {
            m_DragForce = CalculateDrag(m_Velocity);

            AddForce(m_DragForce);
        }
        #endregion
        #region 계산
        m_Acceleration = m_Force / m_Mass;
        m_Velocity += m_Acceleration * Time.deltaTime;
        #endregion
        #region 예외처리
        //if (m_Velocity.magnitude > m_TerminalSpeed)
        //{
        //    m_Velocity = m_Velocity.normalized * m_TerminalSpeed;
        //}
        if (m_Velocity.magnitude < 0.01f)
        {
            m_Velocity = Vector2.zero;
        }
        #endregion
        #region 이동
        transform.localPosition += (Vector3)m_Velocity * Time.deltaTime;
        #endregion
        #region 초기화
        m_Speed = m_Velocity.magnitude;
        m_DragSpeed = m_DragForce.magnitude;
        //m_TerminalSpeed = Mathf.Sqrt(Mathf.Abs((m_Mass * m_Gravity) / (m_Drag * 0.5f)));

        m_Force = Vector2.zero;
        #endregion
    }
    #endregion
}
