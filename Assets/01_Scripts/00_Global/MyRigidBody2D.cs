using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class MyRigidBody2D : MonoBehaviour
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
    // 콜라이더 자식 감지 여부
    [SerializeField]
    protected bool m_UseChildCollider = false;

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
    protected Vector2 m_CalculedDrag;
    // 충돌 계산할 콜라이더
    protected Collider2D m_Collider;
    // 충돌 계산시 사용될 레이어 마스크
    protected int m_LayerMask;
    #endregion

    #region 외부 프로퍼티
    public float mass { get => m_Mass; set => m_Mass = value; }
    public bool useGravity { get => m_UseGravity; set => m_UseGravity = value; }
    public bool isKinematic { get => m_IsKinematic; set => m_IsKinematic = value; }
    public Vector3 velocity { get => m_Velocity; set => m_Velocity = value; }
    public int layerMask { get => m_LayerMask; set => m_LayerMask = value; }
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
        float Cd = -0.003918f * m_Drag;
        // 속도의 방향
        Vector2 dir = force.normalized;

        // 항력 방정식
        Vector2 Fd = 0.5f * p * v * v * A * Cd * dir;

        return Fd;
    }
    protected void CheckCollision()
    {
        Vector2 center = m_Collider.bounds.center;
        Vector2 size = m_Collider.bounds.size;
        Vector2 halfSize = size * 0.5f;
        Vector2 dir;
        if (!m_IsKinematic)
        {
            if (m_Velocity.x > 0f)
                dir.x = 1f;
            else if (m_Velocity.x < 0f)
                dir.x = -1f;
            else
                dir.x = 0f;

            if (m_Velocity.y > 0f)
                dir.y = 1f;
            else if (m_Velocity.y < 0f)
                dir.y = -1f;
            else
                dir.y = 0f;
        }
        else
        {
            if (m_Force.x > 0f)
                dir.x = 1f;
            else if (m_Force.x < 0f)
                dir.x = -1f;
            else
                dir.x = 0f;

            if (m_Force.y > 0f)
                dir.y = 1f;
            else if (m_Force.y < 0f)
                dir.y = -1f;
            else
                dir.y = 0f;
        }
        float distance = (!m_IsKinematic ? m_Velocity.magnitude : m_Force.magnitude) * Time.deltaTime;
        Vector2[] vertices = new Vector2[4]
        {
            center + Vector2.left * halfSize.x + Vector2.up * halfSize.y,
            center + Vector2.right * halfSize.x + Vector2.up * halfSize.y,

            center + Vector2.left * halfSize.x + Vector2.down * halfSize.y,
            center + Vector2.right * halfSize.x + Vector2.down * halfSize.y
        };

        RaycastHit2D[] hits = new RaycastHit2D[8]
        {
            Physics2D.Raycast(vertices[0], new Vector2(dir.x, 0f), distance, m_LayerMask),
            Physics2D.Raycast(vertices[0], new Vector2(0f, dir.y), distance, m_LayerMask),

            Physics2D.Raycast(vertices[1], new Vector2(dir.x, 0f), distance, m_LayerMask),
            Physics2D.Raycast(vertices[1], new Vector2(0f, dir.y), distance, m_LayerMask),

            Physics2D.Raycast(vertices[2], new Vector2(dir.x, 0f), distance, m_LayerMask),
            Physics2D.Raycast(vertices[2], new Vector2(0f, dir.y), distance, m_LayerMask),

            Physics2D.Raycast(vertices[3], new Vector2(dir.x, 0f), distance, m_LayerMask),
            Physics2D.Raycast(vertices[3], new Vector2(0f, dir.y), distance, m_LayerMask)
        };

        if (!m_IsKinematic)
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (null != hits[i].transform &&
                    hits[i].transform != m_Collider.transform)
                {
                    Vector2 hit_dir = hits[i].point - vertices[i / 2];

                    if (i % 2 == 0)
                    {
                        if (dir.x < 0f)
                        {
                            if (hit_dir.x < -0.015f)
                            {
                                m_Velocity.x = 0f;
                            }
                            else
                            {
                                m_Velocity.x = hit_dir.x;
                            }
                        }
                        else if (dir.x > 0f)
                        {
                            if (hit_dir.x > 0.015f)
                            {
                                m_Velocity.x = 0f;
                            }
                            else
                            {
                                m_Velocity.x = hit_dir.x;
                            }
                        }
                    }
                    else
                    {
                        if (dir.y < 0f)
                        {
                            if (hit_dir.y < -0.015f)
                            {
                                m_Velocity.y = 0f;
                            }
                            else
                            {
                                m_Velocity.y = hit_dir.y;
                            }
                        }
                        else if (dir.y > 0f)
                        {
                            if (hit_dir.y > 0.015f)
                            {
                                m_Velocity.y = 0f;
                            }
                            else
                            {
                                m_Velocity.y = hit_dir.y;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < hits.Length; ++i)
            {
                if (null != hits[i].transform &&
                    hits[i].transform != m_Collider.transform)
                {
                    Vector2 hit_dir = hits[i].point - vertices[i / 2];

                    if (i % 2 == 0)
                    {
                        if (dir.x < 0f)
                        {
                            if (hit_dir.x < -0.015f)
                            {
                                m_Force.x = 0f;
                            }
                            else
                            {
                                m_Force.x = hit_dir.x;
                            }
                        }
                        else if (dir.x > 0f)
                        {
                            if (hit_dir.x > 0.015f)
                            {
                                m_Force.x = 0f;
                            }
                            else
                            {
                                m_Force.x = hit_dir.x;
                            }
                        }
                    }
                    else
                    {
                        if (dir.y < 0f)
                        {
                            if (hit_dir.y < -0.015f)
                            {
                                m_Force.y = 0f;
                            }
                            else
                            {
                                m_Force.y = hit_dir.y;
                            }
                        }
                        else if (dir.y > 0f)
                        {
                            if (hit_dir.y > 0.015f)
                            {
                                m_Force.y = 0f;
                            }
                            else
                            {
                                m_Force.y = hit_dir.y;
                            }
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region 외부 함수
    public void AddForce(Vector2 force)
    {
        m_Force += force;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        if (!m_UseChildCollider)
        {
            m_Collider = GetComponent<Collider2D>();
        }
        else
        {
            m_Collider = GetComponentInChildren<Collider2D>();
        }
    }
    void FixedUpdate()
    {
        #region 테스트
        //Vector2 force = Vector2.zero;

        //force += Vector2.right * Input.GetAxisRaw("Horizontal");
        //force += Vector2.up * Input.GetAxisRaw("Vertical");

        //AddForce(force);
        #endregion

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
        #region 공기저항
        if (m_Velocity.magnitude > 0)
        {
            m_CalculedDrag = CalculateDrag(m_Velocity);

            if (m_Velocity.magnitude <= m_CalculedDrag.magnitude)
            {
                m_Velocity = Vector2.zero;
            }
            else
            {
                m_Velocity += m_CalculedDrag;
            }
        }
        else
        {
            m_CalculedDrag = Vector2.zero;
        }
        #endregion
        #region 충돌
        CheckCollision();
        #endregion
        #region 선 예외처리
        if (!m_IsKinematic)
        {
            if (Mathf.Abs(m_Velocity.x) < 0.015f)
            {
                m_Velocity.x = 0f;
            }
            if (Mathf.Abs(m_Velocity.y) < 0.015f)
            {
                m_Velocity.y = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(m_Force.x) < 0.015f)
            {
                m_Force.x = 0f;
            }
            if (Mathf.Abs(m_Force.y) < 0.015f)
            {
                m_Force.y = 0f;
            }
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
        #region 후 예외처리
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

        m_DragSpeed = m_CalculedDrag.magnitude;
        m_Force = Vector2.zero;
        #endregion
    }
    #endregion
}
