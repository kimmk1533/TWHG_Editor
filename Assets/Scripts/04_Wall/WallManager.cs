using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : ObjectManager<WallManager, Wall>
{
    List<Wall> m_Walls;
    LineRenderer m_Line;
    BoxCollider2D m_Collider2D;

    protected StageManager M_Stage => StageManager.Instance;

    protected override void Awake()
    {
        base.Awake();

        m_PoolSize = 100;
    }

    public override void OnPlayEnter()
    {

    }
    public override void OnPlayExit()
    {

    }

    public override void __Initialize()
    {
        m_Walls = new List<Wall>();

        base.__Initialize();
    }
    public override void __Finalize()
    {
        ClearWall();

        base.__Finalize();
    }

    void CreateWall_V(int width, int height)
    {
        Wall temp = GetPool("Wall").Spawn();

        m_Line = temp.GetComponentInChildren<LineRenderer>();
        m_Collider2D = temp.GetComponentInChildren<BoxCollider2D>();

        // Line
        Vector2 pos = new Vector2(width, height);
        pos.y -= 0.075f;
        m_Line.SetPosition(0, pos - M_Game.m_StandardPos);
        pos.y += 1.15f;
        m_Line.SetPosition(1, pos - M_Game.m_StandardPos);

        // Collider
        m_Collider2D.offset = (m_Line.GetPosition(0) + m_Line.GetPosition(1)) * 0.5f;
        float LineWidth = (m_Line.startWidth + m_Line.endWidth) * 0.5f;
        float LineHeight = Mathf.Abs(m_Line.GetPosition(0).y - m_Line.GetPosition(1).y);
        m_Collider2D.size = new Vector2(LineWidth, LineHeight);

        temp.gameObject.SetActive(true);
        m_Walls.Add(temp);
    }
    void CreateWall_H(int width, int height)
    {
        Wall temp = GetPool("Wall").Spawn();

        m_Line = temp.GetComponentInChildren<LineRenderer>();
        m_Collider2D = temp.GetComponentInChildren<BoxCollider2D>();

        Vector2 pos = new Vector2(width, height);
        pos.x -= 0.075f;
        m_Line.SetPosition(0, pos - M_Game.m_StandardPos);
        pos.x += 1.15f;
        m_Line.SetPosition(1, pos - M_Game.m_StandardPos);

        m_Collider2D.offset = (m_Line.GetPosition(0) + m_Line.GetPosition(1)) * 0.5f;

        float LineWidth = Mathf.Abs(m_Line.GetPosition(0).x - m_Line.GetPosition(1).x);
        float LineHeight = (m_Line.startWidth + m_Line.endWidth) * 0.5f;
        m_Collider2D.size = new Vector2(LineWidth, LineHeight);

        temp.gameObject.SetActive(true);
        m_Walls.Add(temp);
    }
    public void CreateWall()
    {
        for (int i = 0; i < M_Game.m_height; ++i)
        {
            for (int j = 0; j < M_Game.m_width; ++j)
            {
                if (M_Stage.m_Stage[i, j] != E_TileType.Wall)
                {
                    // 왼쪽
                    if (j - 1 >= 0)
                    {
                        if (M_Stage.m_Stage[i, j - 1] == E_TileType.Wall)
                        {
                            CreateWall_V(j, i);
                        }
                    }
                    // 위
                    if (i - 1 >= 0)
                    {
                        if (M_Stage.m_Stage[i - 1, j] == E_TileType.Wall)
                        {
                            CreateWall_H(j, i);
                        }
                    }
                    // 아래쪽
                    if (i + 1 < M_Game.m_height)
                    {
                        if (M_Stage.m_Stage[i + 1, j] == E_TileType.Wall)
                        {
                            CreateWall_H(j, i + 1);
                        }
                    }
                    // 오른쪽
                    if (j + 1 < M_Game.m_width)
                    {
                        if (M_Stage.m_Stage[i, j + 1] == E_TileType.Wall)
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
        for (int i = m_Walls.Count - 1; i >= 0; --i)
        {
            GetPool("Wall").DeSpawn(m_Walls[i]);
        }

        m_Walls.Clear();
    }
}