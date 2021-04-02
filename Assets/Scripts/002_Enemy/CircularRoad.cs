using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularRoad : EnemyRoad
{
    public Vector2 Center;

    float Degree;

    private void Start()
    {
        Vector2 v = (Vector2)Enemy.transform.position - Center;
        Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        if (!m_Edit.isEdit)
        {
            Move();
        }
    }

    public override void OnPlayEnter()
    {
        base.OnPlayEnter();

        Vector2 v = (Vector2)Enemy.transform.position - Center;
        Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
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
        Degree += Speed;

        float radius = Vector2.Distance(Enemy.transform.position, Center);
        float radian = Degree * Mathf.Deg2Rad;

        Vector2 newPos = new Vector2();
        newPos.x = radius * Mathf.Cos(radian);
        newPos.y = radius * Mathf.Sin(radian);
        Enemy.transform.position = newPos + Center;
    }
}
