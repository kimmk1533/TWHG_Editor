using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected E_EnemyType m_Type;
    [SerializeField]
    protected float m_Speed;

    protected Vector2 m_InitPos;

    #region Linear
    protected List<Vector2> m_WayPointList;

    protected int m_WayPointCount;
    protected bool m_Revert;
    #endregion
    #region Circular
    protected Vector2 m_Center;

    protected float m_Degree;
    #endregion

    #region 내부 컴포넌트
    protected MyRigidBody m_Rigidbody;
    protected EnemyCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __EditManager M_Edit => __EditManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
    protected void Move()
    {
        switch (m_Type)
        {
            case E_EnemyType.Linear:
                LinearMove();
                break;
            case E_EnemyType.LinearRepeat:
                LinearRepeatMove();
                break;
            case E_EnemyType.Circular:
                CircularMove();
                break;
        }
    }
    protected void LinearMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_WayPointList[m_WayPointCount], m_Speed * Time.deltaTime);

        if (CloseTarget(m_WayPointList[m_WayPointCount], 0.05f))
        {
            if (0 <= m_WayPointCount && m_WayPointCount <= m_WayPointList.Count - 1)
                if (!m_Revert)
                {
                    if (m_WayPointCount < m_WayPointList.Count - 1)
                    {
                        ++m_WayPointCount;
                    }
                    else
                    {
                        m_Revert = !m_Revert;
                    }
                }
                else
                {
                    if (m_WayPointCount > 0)
                    {
                        --m_WayPointCount;
                    }
                    else
                    {
                        m_Revert = !m_Revert;
                    }
                }
        }
    }
    protected void LinearRepeatMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_WayPointList[m_WayPointCount], m_Speed * Time.deltaTime);

        if (CloseTarget(m_WayPointList[m_WayPointCount], 0.05f))
        {
            ++m_WayPointCount;
            if (m_WayPointCount >= m_WayPointList.Count)
            {
                m_WayPointCount = 0;
            }
        }
    }
    protected void CircularMove()
    {
        m_Degree += m_Speed;

        float radius = Vector2.Distance(transform.position, m_Center);
        float radian = m_Degree * Mathf.Deg2Rad;

        Vector2 newPos = new Vector2();
        newPos.x = radius * Mathf.Cos(radian);
        newPos.y = radius * Mathf.Sin(radian);
        transform.position = m_Center + newPos;
    }

    protected bool CloseTarget(Vector3 targetPos, float distance)
    {
        return Vector3.Distance(targetPos, transform.position) <= distance;
    }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        if (null == m_Rigidbody)
        {
            m_Rigidbody = GetComponent<MyRigidBody>();
        }
        if (null == m_Collider)
        {
            m_Collider = GetComponentInChildren<EnemyCollider>();
            m_Collider.__Initialize(this);
        }

        if (null == m_WayPointList)
        {
            m_WayPointList = new List<Vector2>();
        }
    }

    public void AddWayPoint(Vector2 wayPoint)
    {
        m_WayPointList.Add(wayPoint);
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        m_InitPos = transform.position;

        switch (m_Type)
        {
            case E_EnemyType.Linear:
            case E_EnemyType.LinearRepeat:
                m_WayPointCount = 0;
                m_Revert = false;
                break;
            case E_EnemyType.Circular:
                Vector2 v = (Vector2)transform.position - m_Center;
                m_Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                break;
        }
    }
    public void OnPlayExit()
    {
        transform.position = m_InitPos;
    }
    #endregion
    #region 유니티 콜백 함수
    void Update()
    {
        if (M_Edit.isPlayMode)
        {
            Move();
        }
    }
    #endregion
}
