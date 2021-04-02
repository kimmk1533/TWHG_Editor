using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearRoad : EnemyRoad
{
    public List<Vector2> WayPoints;
    public bool Repeat;

    int WayPointCount;
    bool Revert;

    private void Start()
    {
        WayPointCount = 0;
        Revert = false;
    }

    public void AddWayPoint(Vector2 wayPoint)
    {
        if (WayPoints == null)
            WayPoints = new List<Vector2>();

        WayPoints.Add(wayPoint);
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

        WayPointCount = 0;
        Revert = false;
    }
    public override void OnPlayExit()
    {
        base.OnPlayExit();

    }

    protected override void Move()
    {
        switch (Repeat)
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
        Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, WayPoints[WayPointCount], Speed * Time.deltaTime);

        if (CloseTarget(WayPoints[WayPointCount], 0.05f))
        {
            if (0 <= WayPointCount && WayPointCount <= WayPoints.Count - 1)
                if (!Revert)
                {
                    if (WayPointCount < WayPoints.Count - 1)
                    {
                        ++WayPointCount;
                    }
                    else
                    {
                        Revert = !Revert;
                    }
                }
                else
                {
                    if (WayPointCount > 0)
                    {
                        --WayPointCount;
                    }
                    else
                    {
                        Revert = !Revert;
                    }
                }
        }
    }
    void LinearRepeatMove()
    {
        Enemy.transform.position = Vector3.MoveTowards(Enemy.transform.position, WayPoints[WayPointCount], Speed * Time.deltaTime);

        if (CloseTarget(WayPoints[WayPointCount], 0.05f))
        {
            ++WayPointCount;
            if (WayPointCount >= WayPoints.Count)
            {
                WayPointCount = 0;
            }
        }
    }
    protected virtual bool CloseTarget(Vector3 targetPos, float distance)
    {
        return Vector3.Distance(targetPos, Enemy.transform.position) <= distance;
    }
}
