using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearRoad : EnemyRoad
{
    public List<Vector2> m_WayPoints;
    public bool m_Repeat;

    int m_WayPointCount;
    bool m_Revert;

    private void Start()
    {
        m_WayPointCount = 0;
        m_Revert = false;
    }

    public void AddWayPoint(Vector2 wayPoint)
    {
        if (m_WayPoints == null)
            m_WayPoints = new List<Vector2>();

        m_WayPoints.Add(wayPoint);
    }

    private void Update()
    {
        if (!M_Edit.isEdit)
        {
            Move();
        }
    }

    public override void OnPlayEnter()
    {
        base.OnPlayEnter();

        m_WayPointCount = 0;
        m_Revert = false;
    }
    public override void OnPlayExit()
    {
        base.OnPlayExit();

    }

    protected override void Move()
    {
        switch (m_Repeat)
        {
            case true:
                LinearRepeatMove();
                break;
            case false:
                LinearMove();
                break;
        }
    }

    void LinearMove()
    {
        m_Enemy.transform.position = Vector3.MoveTowards(m_Enemy.transform.position, m_WayPoints[m_WayPointCount], m_Speed * Time.deltaTime);

        if (CloseTarget(m_WayPoints[m_WayPointCount], 0.05f))
        {
            if (0 <= m_WayPointCount && m_WayPointCount <= m_WayPoints.Count - 1)
                if (!m_Revert)
                {
                    if (m_WayPointCount < m_WayPoints.Count - 1)
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
    void LinearRepeatMove()
    {
        m_Enemy.transform.position = Vector3.MoveTowards(m_Enemy.transform.position, m_WayPoints[m_WayPointCount], m_Speed * Time.deltaTime);

        if (CloseTarget(m_WayPoints[m_WayPointCount], 0.05f))
        {
            ++m_WayPointCount;
            if (m_WayPointCount >= m_WayPoints.Count)
            {
                m_WayPointCount = 0;
            }
        }
    }
    protected virtual bool CloseTarget(Vector3 targetPos, float distance)
    {
        return Vector3.Distance(targetPos, m_Enemy.transform.position) <= distance;
    }
}
