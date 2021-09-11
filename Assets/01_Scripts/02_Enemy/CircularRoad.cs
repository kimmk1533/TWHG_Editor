using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularRoad : EnemyRoad
{
    public Vector2 m_Center;

    float m_Degree;

    private void Start()
    {
        Vector2 v = (Vector2)m_Enemy.transform.position - m_Center;
        m_Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        if (!M_Edit.isEditMode)
        {
            Move();
        }
    }

    public override void OnPlayEnter()
    {
        base.OnPlayEnter();

        Vector2 v = (Vector2)m_Enemy.transform.position - m_Center;
        m_Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
    public override void OnPlayExit()
    {
        base.OnPlayExit();

    }

    protected override void Move()
    {
        CircularMove();
    }

    void CircularMove()
    {
        m_Degree += m_Speed;

        float radius = Vector2.Distance(m_Enemy.transform.position, m_Center);
        float radian = m_Degree * Mathf.Deg2Rad;

        Vector2 newPos = new Vector2();
        newPos.x = radius * Mathf.Cos(radian);
        newPos.y = radius * Mathf.Sin(radian);
        m_Enemy.transform.position = newPos + m_Center;
    }
}
