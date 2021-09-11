using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    protected Camera m_Camera;

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
        if (Input.GetMouseButtonDown((int)E_InputButton.Middle) &&
            !M_Edit.IsPointerOverUIObject())
        {
            m_StartPos = m_Camera.ScreenToWorldPoint(Input.mousePosition);
            m_IsMove = true;
        }

        if (m_IsMove)
        {
            Vector3 pos = m_StartPos + (transform.position - m_Camera.ScreenToWorldPoint(Input.mousePosition));
            pos.z = m_StartZ;
            transform.position = pos;

            if (Input.GetMouseButtonUp((int)E_InputButton.Middle) ||
                M_Edit.IsPointerOverUIObject())
            {
                m_IsMove = false;
            }
        }
    }
    protected void Scale()
    {
        if (M_Edit.IsPointerOverUIObject())
            return;

        float scrollDelta = m_Camera.orthographicSize - Input.mouseScrollDelta.y;
        if (scrollDelta < 0f)
        {
            m_Camera.orthographicSize = m_MinSize;
            return;
        }

        m_Camera.orthographicSize -= Input.mouseScrollDelta.y;
    }
    protected void Clamp()
    {
        float unit = 100f / m_Camera.orthographicSize;
    }
    #endregion
    #region 유니티 콜백 함수
    void Awake()
    {
        if (null == m_Camera)
        {
            m_Camera = GetComponent<Camera>();
        }

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
        Clamp();
    }
    #endregion
}
