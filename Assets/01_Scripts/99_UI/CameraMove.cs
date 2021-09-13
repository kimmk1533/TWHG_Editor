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
    protected __EditManager M_Edit => __EditManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    protected void Move()
    {
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
    }
    protected void Scale()
    {
        if (M_Edit.IsPointerOverUIObject())
            return;

        float orhoSize = m_Camera.orthographicSize - Input.mouseScrollDelta.y;
        if (orhoSize < 0f)
        {
            m_Camera.orthographicSize = m_MinSize;
            return;
        }

        m_Camera.orthographicSize = orhoSize;
    }
    protected void ClampMove()
    {
        Vector2 cameraPos = m_Camera.transform.position;
        Vector2 canvasPos = m_Canvas.transform.position;

        Vector2 screenSize = new Vector2(m_Camera.pixelWidth, m_Camera.pixelHeight);
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

    }
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        if (null == m_Camera)
        {
            m_Camera = GetComponent<Camera>();
        }
        m_CanvasRectTransform = m_Canvas.GetComponent<RectTransform>();

        m_StartZ = transform.position.z;
        m_MinSize = m_Camera.orthographicSize % 1f;
        if (m_MinSize == 0f)
        {
            m_MinSize = 0.01f;
        }
    }
    void Update()
    {
        Move();
        Scale();
    }
    void LateUpdate()
    {
        ClampScale();
        ClampMove();
    }
    #endregion
}
