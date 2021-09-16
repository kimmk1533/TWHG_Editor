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
    [SerializeField, ReadOnly]
    protected List<EnemyGizmo> m_WayPointList;

    protected int m_WayPointCount;
    protected bool m_Revert;
    #endregion
    #region Circular
    [SerializeField, ReadOnly]
    protected EnemyGizmo m_Center;

    protected float m_Degree;
    #endregion

    #region 내부 컴포넌트
    protected MyRigidBody m_Rigidbody;
    protected EnemyCollider m_Collider;
    #endregion
    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;

    protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public E_EnemyType type { get => m_Type; set => m_Type = value; }
    public float speed { get => m_Speed; set => m_Speed = value; }
    public List<EnemyGizmo> wayPointList { get => m_WayPointList; set => m_WayPointList = value; }
    public EnemyGizmo center { get => m_Center; set => m_Center = value; }
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
        transform.position = Vector3.MoveTowards(
            transform.position,
            m_WayPointList[m_WayPointCount].transform.position,
            m_Speed * Time.deltaTime);

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
        transform.position = Vector3.MoveTowards(
            transform.position,
            m_WayPointList[m_WayPointCount].transform.position,
            m_Speed * Time.deltaTime);

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

        float radius = Vector2.Distance(transform.position, m_Center.transform.position);
        float radian = m_Degree * Mathf.Deg2Rad;

        Vector3 newPos = new Vector3();
        newPos.x = radius * Mathf.Cos(radian);
        newPos.y = radius * Mathf.Sin(radian);
        transform.position = m_Center.transform.position + newPos;
    }

    protected bool CloseTarget(Vector3 targetPos, float distance)
    {
        return Vector3.Distance(targetPos, transform.position) <= distance;
    }
    protected bool CloseTarget(EnemyGizmo waypoint, float distance)
    {
        return CloseTarget(waypoint.transform.position, distance);
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
            m_WayPointList = new List<EnemyGizmo>();
        }

        if (null == m_Center)
        {
            m_Center = M_EnemyGizmo.SpawnGizmo();
            m_Center.transform.position = new Vector3(0f, 0f, 5f);
            m_Center.gameObject.SetActive(false);
        }
    }

    public void AddWayPoint()
    {
        EnemyGizmo wayPoint = M_EnemyGizmo.SpawnGizmo();
        wayPoint.transform.position = new Vector3(0f, 0f, 5f);

        wayPoint.index = m_WayPointList.Count;
        wayPoint.text.text = (wayPoint.index + 1).ToString();
        m_WayPointList.Add(wayPoint);
    }
    public void RemoveWayPoint(int index)
    {
        if (index < 0 || index >= m_WayPointList.Count)
            return;

        EnemyGizmo wayPoint = m_WayPointList[index];
        M_EnemyGizmo.DespawnGizmo(wayPoint);
        m_WayPointList.RemoveAt(index);

        for (int i = index; i < m_WayPointList.Count; ++i)
        {
            m_WayPointList[i].index = i;
            m_WayPointList[i].text.text = (i + 1).ToString();
        }
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
                Vector3 v = transform.position - m_Center.transform.position;
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
        if (M_Game.isPlayMode)
        {
            Move();
        }
    }
    #endregion
}
