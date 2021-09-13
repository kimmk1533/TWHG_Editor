using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    protected float m_Speed;

    [SerializeField, ReadOnly]
    protected Vector3 m_InitPos;
    [SerializeField]
    protected Vector3 m_SpawnPos;
    [SerializeField]
    protected bool m_IsSafe;
    [SerializeField]
    protected bool m_CanMove;

    #region 내부 컴포넌트
    protected MyRigidBody m_RigidBody;
    protected PlayerAnimator m_Animator;
    protected PlayerCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    #endregion

    protected bool canMove => !M_Edit.isEditMode && m_CanMove;
    protected Vector2 size => m_Collider.size;
    protected Vector2 halfSize => size * 0.5f;
    #endregion
    #region 외부 프로퍼티
    public bool isSafe { get => m_IsSafe; set => m_IsSafe = value; }
    public Vector3 spawnPos { get => m_SpawnPos; set => m_SpawnPos = value; }
    #endregion
    #region 내부 함수
    void Move()
    {
        if (Input.anyKey)
        {
            float halfSize = (transform.lossyScale.x + transform.lossyScale.y) * 0.5f * 0.5f;
            float rayAdjust = halfSize - 0.05f;

            float xDir = Input.GetAxisRaw("Horizontal");
            float yDir = Input.GetAxisRaw("Vertical");

            Vector2 xVec = new Vector2(xDir, 0f);
            Vector2 yVec = new Vector2(0f, yDir);

            int layer = 1 << LayerMask.NameToLayer("Wall");

            Vector2 pos = transform.position;

            RaycastHit2D[] raycastHits = new RaycastHit2D[4];

            float xMove = m_Speed * Time.deltaTime;
            float yMove = m_Speed * Time.deltaTime;

            raycastHits[0] = Physics2D.Raycast(pos + yVec * halfSize + (Vector2.right * rayAdjust), yVec, yMove, layer);
            raycastHits[1] = Physics2D.Raycast(pos + yVec * halfSize + (Vector2.left * rayAdjust), yVec, yMove, layer);
            raycastHits[2] = Physics2D.Raycast(pos + xVec * halfSize + (Vector2.up * rayAdjust), xVec, xMove, layer);
            raycastHits[3] = Physics2D.Raycast(pos + xVec * halfSize + (Vector2.down * rayAdjust), xVec, xMove, layer);

            // 상, 하
            if (raycastHits[0].transform != null)
            {
                if (raycastHits[0].distance <= yMove)
                {
                    yMove = raycastHits[0].distance;
                }
            }
            if (raycastHits[1].transform != null)
            {
                if (raycastHits[1].distance <= yMove)
                {
                    yMove = raycastHits[1].distance;
                }
            }
            if (raycastHits[2].transform != null)
            {
                if (raycastHits[2].distance <= xMove)
                {
                    xMove = raycastHits[2].distance;
                }
            }
            if (raycastHits[3].transform != null)
            {
                if (raycastHits[3].distance <= xMove)
                {
                    xMove = raycastHits[3].distance;
                }
            }

            Vector2 temp = new Vector2(xDir * xMove, yDir * yMove);

            transform.Translate(temp);
        }
    }
    void ClampPos()
    {
        // 뷰포트 기준 좌측 하단의 좌표를 월드좌표로 변환 (최솟값)
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        // 뷰포트 기준 우측 상단의 좌표를 월드좌표로 변환 (최댓값)
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
        // 플레이어 월드좌표
        Vector2 PlayerPos = transform.position;

        // 플레이어의 월드좌표를 자신의 사이즈까지 고려하여 화면에 맞게 변환 
        PlayerPos.x = Mathf.Clamp(PlayerPos.x, min.x + 0.5f, max.x - 0.5f);
        PlayerPos.y = Mathf.Clamp(PlayerPos.y, min.y + 0.5f, max.y - 0.5f);

        // 제한한 위치로 이동
        transform.position = PlayerPos;
    }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        #region 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        M_Player.OnPlayerRespawn += Respawn;
        #endregion

        if (null == m_RigidBody)
        {
            m_RigidBody = GetComponent<MyRigidBody>();
        }
        if (null == m_Animator)
        {
            m_Animator = GetComponentInChildren<PlayerAnimator>();
            m_Animator.__Initialize(this);
        }
        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<PlayerCollider>();
            m_Collider.__Initialize(this);
        }

        m_CanMove = M_Edit.isEditMode;

        gameObject.SetActive(false);
    }

    public void Death()
    {
        m_CanMove = false;
        m_Animator.Death();
    }
    public void Respawn()
    {
        transform.position = m_SpawnPos;
        gameObject.SetActive(true);
        m_CanMove = true;
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        m_InitPos = m_SpawnPos = transform.position;
        gameObject.SetActive(true);
    }
    public void OnPlayExit()
    {
        transform.position = m_InitPos;
    }
    #endregion
    #region 유니티 콜백 함수
    protected void FixedUpdate()
    {
        if (canMove)
        {
            Vector2 force = Vector2.zero;

            force += Vector2.right * Input.GetAxisRaw("Horizontal") * m_Speed;
            force += Vector2.up * Input.GetAxisRaw("Vertical") * m_Speed;

            m_RigidBody.AddForce(force);
        }
    }
    /*protected void LateUpdate()
    {
        ClampPos();
    }*/
    #endregion
}
