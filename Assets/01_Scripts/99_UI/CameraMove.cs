using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    protected Camera m_Camera;

    [SerializeField]
    protected Canvas m_Canvas;
    protected RectTransform m_CanvasRectTransform;

    [SerializeField]
    protected float m_ScrollDelta;
    [SerializeField]
    protected float m_MoveSpeed;

    protected float m_Width;
    protected float m_Height;

    #region 이동
    protected bool m_IsMove;
    protected float m_StartZ;
    protected Vector3 m_StartPos;
    #endregion
    #region 스케일
    protected float m_MinSize;
    #endregion

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    #endregion

    protected float unit => m_Canvas.referencePixelsPerUnit;
    #endregion
    #region 내부 함수
    protected void Move()
    {
        #region 마우스로 이동
        if (Input.GetMouseButtonDown((int)E_InputButton.Middle))
        {
            if (M_Edit.IsPointerOverUIObject())
                return;

            m_StartPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            m_IsMove = true;
        }

        if (m_IsMove)
        {
            Vector3 pos = m_StartPos + (transform.position - m_Camera.ScreenToWorldPoint(Input.mousePosition));
            pos.z = m_StartZ;
            transform.position = pos;

            if (Input.GetMouseButtonUp((int)E_InputButton.Middle))
            {
                m_IsMove = false;
            }
        }
        #endregion
        #region 키보드로 이동
        if (M_Edit.isEditMode)
        {
            Vector3 dir = new Vector2();
            dir.x = Input.GetAxisRaw("Horizontal");
            dir.y = Input.GetAxisRaw("Vertical");

            transform.position += dir * m_MoveSpeed * Time.deltaTime;
        }
        #endregion

        if (M_Game.isPlayMode)
        {
            // 플레이어 찾아서 이동 시 3초 뒤에 플레이어에게 돌아가도록 구현
        }

        ClampMove();
    }
    protected void Scale()
    {
        if (M_Edit.IsPointerOverUIObject() ||
            Input.mouseScrollDelta.magnitude == 0f)
            return;

        float orhoSize = m_Camera.orthographicSize - Input.mouseScrollDelta.y * m_ScrollDelta;
        if (orhoSize <= m_MinSize)
        {
            m_Camera.orthographicSize = m_MinSize;
            return;
        }

        m_Camera.orthographicSize = orhoSize;
        ClampScale();
    }
    protected void ClampMove()
    {
        Vector2 cameraPos = m_Camera.transform.position;
        Vector2 canvasPos = m_Canvas.transform.position;

        Vector2 screenSize = new Vector2(m_Width, m_Height);
        Vector2 cameraSize = (Vector2)m_Camera.ScreenToWorldPoint(screenSize) - cameraPos;
        Vector2 canvasSize = m_CanvasRectTransform.sizeDelta * 0.5f / m_Canvas.referencePixelsPerUnit;

        // 하
        if (cameraPos.y - cameraSize.y < canvasPos.y - canvasSize.y)
            cameraPos.y = canvasPos.y - canvasSize.y + cameraSize.y;
        // 상
        if (cameraPos.y + cameraSize.y > canvasPos.y + canvasSize.y)
            cameraPos.y = canvasPos.y + canvasSize.y - cameraSize.y;
        // 우
        if (cameraPos.x + cameraSize.x > canvasPos.x + canvasSize.x)
            cameraPos.x = canvasPos.x + canvasSize.x - cameraSize.x;
        // 좌
        if (cameraPos.x - cameraSize.x < canvasPos.x - canvasSize.x)
            cameraPos.x = canvasPos.x - canvasSize.x + cameraSize.x;

        m_Camera.transform.position = cameraPos;
    }
    protected void ClampScale()
    {
        Vector2 cameraPos = m_Camera.transform.position;

        Vector2 screenSize = new Vector2(m_Width, m_Height);
        Vector2 cameraSize = (Vector2)m_Camera.ScreenToWorldPoint(screenSize) - cameraPos;
        Vector2 canvasSize = m_CanvasRectTransform.sizeDelta * 0.5f / unit;

        if (cameraSize.x >= canvasSize.x &&
            cameraSize.y >= canvasSize.y)
        {
            m_Camera.orthographicSize = Mathf.Min(canvasSize.x, canvasSize.y);
        }
    }
    protected void Clamp()
    {
        Vector2 cameraPos = m_Camera.transform.position;
        Vector2 canvasPos = m_Canvas.transform.position;

        Vector2 screenSize = new Vector2(m_Width, m_Height);
        Vector2 cameraSize = (Vector2)m_Camera.ScreenToWorldPoint(screenSize) - cameraPos;
        Vector2 canvasSize = m_CanvasRectTransform.sizeDelta * 0.5f / unit;

        if (cameraSize.x > canvasSize.x)
        {
            cameraPos.x = canvasPos.x;
        }
        if (cameraSize.y > canvasSize.y)
        {
            cameraPos.y = canvasPos.y;
        }

        m_Camera.transform.position = cameraPos;
    }
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        if (null == m_Camera)
        {
            m_Camera = GetComponent<Camera>();
            if (null == m_Camera)
            {
                m_Camera = Camera.main;
            }
        }
        m_CanvasRectTransform = m_Canvas.GetComponent<RectTransform>();

        m_StartZ = transform.position.z;
        m_MinSize = m_ScrollDelta;

        m_Width = m_Camera.pixelWidth;
        m_Height = m_Camera.pixelHeight;
    }
    void Update()
    {
        Scale();
        Move();
    }
    void LateUpdate()
    {
        Clamp();
    }
    #endregion
}
