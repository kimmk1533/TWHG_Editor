using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : ObjectManager<WallManager>
{
    List<GameObject> Walls;
    LineRenderer Line;
    BoxCollider2D Collider2D;

    protected override void Awake()
    {
        base.Awake();

        ResourcesType = E_ResourcesType.Wall;

        PoolSize = 100;
    }

    public override void OnPlayEnter()
    {

    }
    public override void OnPlayExit()
    {

    }

    public override void __Initialize()
    {
        Walls = new List<GameObject>();

        base.__Initialize();
    }
    public override void __Finalize()
    {
        ClearWall();

        base.__Finalize();
    }

    void CreateWall_V(int width, int height)
    {
        GameObject temp = GetPool().Spawn();

        Line = temp.GetComponentInChildren<LineRenderer>();
        Collider2D = temp.GetComponentInChildren<BoxCollider2D>();

        // Line
        Vector2 pos = new Vector2(width, height);
        pos.y -= 0.075f;
        Line.SetPosition(0, pos - m_Game.StandardPos);
        pos.y += 1.15f;
        Line.SetPosition(1, pos - m_Game.StandardPos);

        // Collider
        Collider2D.offset = (Line.GetPosition(0) + Line.GetPosition(1)) * 0.5f;
        float LineWidth = (Line.startWidth + Line.endWidth) * 0.5f;
        float LineHeight = Mathf.Abs(Line.GetPosition(0).y - Line.GetPosition(1).y);
        Collider2D.size = new Vector2(LineWidth, LineHeight);

        temp.SetActive(true);
        Walls.Add(temp);
    }
    void CreateWall_H(int width, int height)
    {
        GameObject temp = GetPool().Spawn();

        Line = temp.GetComponentInChildren<LineRenderer>();
        Collider2D = temp.GetComponentInChildren<BoxCollider2D>();

        Vector2 pos = new Vector2(width, height);
        pos.x -= 0.075f;
        Line.SetPosition(0, pos - m_Game.StandardPos);
        pos.x += 1.15f;
        Line.SetPosition(1, pos - m_Game.StandardPos);

        Collider2D.offset = (Line.GetPosition(0) + Line.GetPosition(1)) * 0.5f;

        float LineWidth = Mathf.Abs(Line.GetPosition(0).x - Line.GetPosition(1).x);
        float LineHeight = (Line.startWidth + Line.endWidth) * 0.5f;
        Collider2D.size = new Vector2(LineWidth, LineHeight);

        temp.SetActive(true);
        Walls.Add(temp);
    }
    public void CreateWall()
    {
        for (int i = 0; i < m_Game.height; ++i)
        {
            for (int j = 0; j < m_Game.width; ++j)
            {
                if (m_Resources.Stage[i, j] != E_StageType.Wall)
                {
                    // 왼쪽
                    if (j - 1 >= 0)
                    {
                        if (m_Resources.Stage[i, j - 1] == E_StageType.Wall)
                        {
                            CreateWall_V(j, i);
                        }
                    }
                    // 위
                    if (i - 1 >= 0)
                    {
                        if (m_Resources.Stage[i - 1, j] == E_StageType.Wall)
                        {
                            CreateWall_H(j, i);
                        }
                    }
                    // 아래쪽
                    if (i + 1 < m_Game.height)
                    {
                        if (m_Resources.Stage[i + 1, j] == E_StageType.Wall)
                        {
                            CreateWall_H(j, i + 1);
                        }
                    }
                    // 오른쪽
                    if (j + 1 < m_Game.width)
                    {
                        if (m_Resources.Stage[i, j + 1] == E_StageType.Wall)
                        {
                            CreateWall_V(j + 1, i);
                        }
                    }
                }
            }
        }
    }
    public void ClearWall()
    {
        for (int i = Walls.Count - 1; i >= 0; --i)
        {
            GetPool().DeSpawn(Walls[i]);
        }

        Walls.Clear();
    }
}