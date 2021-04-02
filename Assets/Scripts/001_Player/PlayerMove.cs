using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float Speed;
    const float Size = 1f;

    Animator animator;

    __GameManager m_Game;
    __EditManager m_Edit;

    private void Awake()
    {
        m_Game = __GameManager.Instance;
        m_Edit = __EditManager.Instance;
    }
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!m_Edit.isEdit)
        {
            Move();

            ClampPos();
        }
    }

    void Move()
    {
        if (Input.anyKey && !animator.GetBool("Death"))
        {
            float halfSize = (Size * (transform.lossyScale.x + transform.lossyScale.y) * 0.5f) * 0.5f;
            float rayAdjust = halfSize - 0.05f;

            float xDir = Input.GetAxisRaw("Horizontal");
            float yDir = Input.GetAxisRaw("Vertical");

            Vector2 xVec = new Vector2(xDir, 0f);
            Vector2 yVec = new Vector2(0f, yDir);

            int layer = 1 << LayerMask.NameToLayer("Wall");

            Vector2 pos = transform.position;

            RaycastHit2D[] raycastHits = new RaycastHit2D[4];

            float xMove = Speed * Time.deltaTime;
            float yMove = Speed * Time.deltaTime;

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
}