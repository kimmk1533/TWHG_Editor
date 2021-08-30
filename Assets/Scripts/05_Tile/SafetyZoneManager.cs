using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneManager : Singleton<SafetyZoneManager>
{
    public List<GameObject> m_SafetyZones;
    public List<SafetyZoneCollider> m_Colliders;
    public SafetyZoneCollider m_StartPoint;
    public SafetyZoneCollider m_EndPoint;

    GameObject m_Origin;

    int m_PoolSize;
    MemoryPool m_Pool;

    InGameManager M_Game;
    EditManager M_Edit;
    ResourcesManager M_Resources;
    protected StageManager M_Stage => StageManager.Instance;

    protected void Awake()
    {
        M_Game = InGameManager.Instance;
        M_Edit = EditManager.Instance;
        M_Resources = ResourcesManager.Instance;

        m_PoolSize = 100;
    }

    public void OnPlayEnter()
    {
        ClearSafetyZone();

        CreateSafetyZone();

        // 추후 수정
        m_StartPoint = m_Colliders[0];

        m_EndPoint = m_Colliders[m_Colliders.Count - 1];
    }
    public void OnPlayExit()
    {

    }

    public void __Initialize()
    {
        m_SafetyZones = new List<GameObject>();
        m_Colliders = new List<SafetyZoneCollider>();

        m_Origin = new GameObject("SafetyZone", typeof(SafetyZoneCollider), typeof(PolygonCollider2D));
        m_Origin.tag = "SafetyZone";
        m_Origin.transform.parent = transform;

        m_Pool = new MemoryPool(m_Origin, m_PoolSize, transform);

        Destroy(m_Origin);
    }
    public void __Finalize()
    {
        ClearSafetyZone();
    }

    void AddSafetyZone(int width, int height)
    {
        int w = width;
        int h = height;

        GameObject SafetyZone;
        SafetyZoneCollider collider;
        Vector2 index = new Vector2(w, h);

        SafetyZone = m_Pool.Spawn();

        collider = SafetyZone.GetComponent<SafetyZoneCollider>();
        collider.__Initialize();

        SafetyZone.SetActive(true);
        m_SafetyZones.Add(SafetyZone);
        m_Colliders.Add(collider);

        Vector2 vertex = index - M_Game.m_StandardPos;
        List<Vector2> vertexs = new List<Vector2>();

        E_TileType[,] stage = (E_TileType[,])M_Stage.m_Stage.Clone();

        Stack<Vector2> history = new Stack<Vector2>();

        while (true)
        {
            // 콜라이더에 현재 위치 추가
            if (!collider.m_Indices.Contains(index))
            {
                collider.m_Indices.Add(index);
            }

            // 현재 위치 막음
            stage[h, w] = E_TileType.Wall;

            bool flag = false;

            if (w + 1 < M_Game.m_width && !flag)
            {
                // 오른쪽이 길일 경우
                if (stage[h, w + 1] == E_TileType.SafetyZone)
                {
                    flag = true;

                    // 이전 위치 기억
                    history.Push(index);
                    // 이동
                    ++index.x;
                }
            }
            if (h + 1 < M_Game.m_height && !flag)
            {
                // 위쪽이 길일 경우
                if (stage[h + 1, w] == E_TileType.SafetyZone)
                {
                    flag = true;

                    // 이전 위치 기억
                    history.Push(index);
                    // 이동
                    ++index.y;
                }
            }
            if (w - 1 >= 0 && !flag)
            {
                // 왼쪽이 길일 경우
                if (stage[h, w - 1] == E_TileType.SafetyZone)
                {
                    flag = true;

                    // 이전 위치 기억
                    history.Push(index);
                    // 이동
                    --index.x;
                }
            }
            if (h - 1 >= 0 && !flag)
            {
                // 아래쪽이 길일 경우
                if (stage[h - 1, w] == E_TileType.SafetyZone)
                {
                    flag = true;

                    // 이전 위치 기억
                    history.Push(index);
                    // 이동
                    --index.y;
                }
            }
            if (!flag)
            {
                if (history.Count == 0)
                    break;

                // 이전 위치로 이동
                index = history.Pop();
            }

            w = (int)index.x;
            h = (int)index.y;
        }

        if (collider.m_Indices.Count == 1)
        {
            vertexs.Add(vertex);
            vertexs.Add(vertex + Vector2.right);
            vertexs.Add(vertex + Vector2.one);
            vertexs.Add(vertex + Vector2.up);
            vertexs.Add(vertex);

            collider.m_Polygon.SetPath(0, vertexs);
            return;
        }

        stage = (E_TileType[,])M_Stage.m_Stage.Clone();

        Vector2 dir, last_dir;
        dir = last_dir = Vector2.right;

        int error_count = 0;

        while (true)
        {
            if (dir == Vector2.right)
                dir = Vector2.down;
            else if (dir == Vector2.down)
                dir = Vector2.left;
            else if (dir == Vector2.left)
                dir = Vector2.up;
            else if (dir == Vector2.up)
                dir = Vector2.right;

            while (true)
            {
                int x = (int)(index + dir).x;
                int y = (int)(index + dir).y;

                if (x < 0 || y < 0 ||
                    x > M_Game.m_width - 1 ||
                    y > M_Game.m_height - 1)
                {
                    if (dir == Vector2.right)
                    {
                        dir = Vector2.up;
                    }
                    else if (dir == Vector2.up)
                    {
                        dir = Vector2.left;
                    }
                    else if (dir == Vector2.left)
                    {
                        dir = Vector2.down;
                    }
                    else if (dir == Vector2.down)
                    {
                        dir = Vector2.right;
                    }
                    continue;
                }

                if (stage[y, x] == E_TileType.SafetyZone)
                    break;

                if (dir == Vector2.right)
                {
                    dir = Vector2.up;
                }
                else if (dir == Vector2.up)
                {
                    dir = Vector2.left;
                }
                else if (dir == Vector2.left)
                {
                    dir = Vector2.down;
                }
                else if (dir == Vector2.down)
                {
                    dir = Vector2.right;
                }

                ++error_count;
                if (error_count >= 1000000)
                {
                    Debug.LogError("무한 루프 오류");
                    return;
                }
            }

            // 조건문 추후 수정
            vertex = index - M_Game.m_StandardPos;
            if (last_dir == Vector2.right)
            {
                if (dir == Vector2.down)
                {
                    //if (!vertexs.Contains(vertex))
                        vertexs.Add(vertex);
                }
                if (dir == Vector2.up)
                {
                    //if (!vertexs.Contains(vertex + Vector2.right))
                        vertexs.Add(vertex + Vector2.right);
                }
                if (dir == Vector2.left)
                {
                    //if (!vertexs.Contains(vertex + Vector2.right))
                        vertexs.Add(vertex + Vector2.right);
                    //if (!vertexs.Contains(vertex + Vector2.one))
                        vertexs.Add(vertex + Vector2.one);
                }
            }
            if (last_dir == Vector2.up)
            {
                if (dir == Vector2.right)
                {
                    //if (!vertexs.Contains(vertex + Vector2.right))
                        vertexs.Add(vertex + Vector2.right);
                }
                if (dir == Vector2.left)
                {
                    //if (!vertexs.Contains(vertex + Vector2.one))
                        vertexs.Add(vertex + Vector2.one);
                }
                if (dir == Vector2.down)
                {
                    //if (!vertexs.Contains(vertex + Vector2.one))
                        vertexs.Add(vertex + Vector2.one);
                    //if (!vertexs.Contains(vertex + Vector2.up))
                        vertexs.Add(vertex + Vector2.up);
                }
            }
            if (last_dir == Vector2.left)
            {
                if (dir == Vector2.up)
                {
                    //if (!vertexs.Contains(vertex + Vector2.one))
                        vertexs.Add(vertex + Vector2.one);
                }
                if (dir == Vector2.down)
                {
                    //if (!vertexs.Contains(vertex + Vector2.up))
                        vertexs.Add(vertex + Vector2.up);
                }
                if (dir == Vector2.right)
                {
                    //if (!vertexs.Contains(vertex + Vector2.up))
                        vertexs.Add(vertex + Vector2.up);
                    //if (!vertexs.Contains(vertex))
                        vertexs.Add(vertex);
                }
            }
            if (last_dir == Vector2.down)
            {
                if (dir == Vector2.left)
                {
                    //if (!vertexs.Contains(vertex + Vector2.up))
                        vertexs.Add(vertex + Vector2.up);
                }
                if (dir == Vector2.right)
                {
                    //if (!vertexs.Contains(vertex))
                        vertexs.Add(vertex);
                }
                if (dir == Vector2.up)
                {
                    //if (!vertexs.Contains(vertex))
                        vertexs.Add(vertex);
                    //if (!vertexs.Contains(vertex + Vector2.right))
                        vertexs.Add(vertex + Vector2.right);
                }
            }

            last_dir = dir;

            if (vertexs.Count > 1)
            {
                if (vertexs.Contains(new Vector2(width, height) - M_Game.m_StandardPos))
                    break;
            }

            index += dir;

            if (error_count >= 1000000)
            {
                Debug.LogError("무한 루프 오류");
                return;
            }
        }

        vertexs.RemoveAt(vertexs.IndexOf(new Vector2(width, height) - M_Game.m_StandardPos));
        vertexs.Insert(0, new Vector2(width, height) - M_Game.m_StandardPos);
        vertexs.Add(new Vector2(width, height) - M_Game.m_StandardPos);

        collider.m_Polygon.SetPath(0, vertexs);

        Debug.DrawLine(vertexs[0], collider.GetCenter(), Color.red, 5f);
    }

    public void CreateSafetyZone()
    {
        for (int y = 0; y < M_Game.m_height; ++y)
        {
            for (int x = 0; x < M_Game.m_width; ++x)
            {
                // 현재 칸이 안전 구역일 때
                if (M_Stage.m_Stage[y, x] == E_TileType.SafetyZone)
                {
                    bool flag = false;
                    for (int index = 0; index < m_Colliders.Count; ++index)
                    {
                        if (m_Colliders[index].m_Indices.Contains(new Vector2(x, y)))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                        continue;
                    AddSafetyZone(x, y);
                }
            }
        }

        // 추후 수정... 원인을 모르겠음...
        // M_Edit.UpdateSafetyZoneOption();
    }
    public void ClearSafetyZone()
    {
        for (int i = 0; i < m_SafetyZones.Count; ++i)
        {
            m_Pool.DeSpawn(m_SafetyZones[i].gameObject);
        }

        m_SafetyZones.Clear();
        m_Colliders.Clear();

        M_Edit.dropdown_first.ClearOptions();
        M_Edit.dropdown_last.ClearOptions();
    }
}
