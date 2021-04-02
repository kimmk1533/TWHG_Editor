using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneManager : Singleton<SafetyZoneManager>
{
    public List<GameObject> SafetyZones;
    public List<SafetyZoneCollider> Colliders;
    public SafetyZoneCollider StartPoint;
    public SafetyZoneCollider EndPoint;

    GameObject origin;

    int PoolSize;
    MemoryPool Pool;

    __GameManager m_Game;
    __EditManager m_Edit;
    ResourcesManager m_Resources;

    protected void Awake()
    {
        m_Game = __GameManager.Instance;
        m_Edit = __EditManager.Instance;
        m_Resources = ResourcesManager.Instance;

        PoolSize = 100;
    }

    public void OnPlayEnter()
    {
        ClearSafetyZone();

        CreateSafetyZone();

        // 추후 수정
        StartPoint = Colliders[0];

        EndPoint = Colliders[Colliders.Count - 1];
    }
    public void OnPlayExit()
    {

    }

    public void __Initialize()
    {
        SafetyZones = new List<GameObject>();
        Colliders = new List<SafetyZoneCollider>();

        origin = new GameObject("SafetyZone", typeof(SafetyZoneCollider), typeof(PolygonCollider2D));
        origin.tag = "SafetyZone";
        origin.transform.parent = transform;

        Pool = new MemoryPool(origin, PoolSize, transform);

        Destroy(origin);
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

        SafetyZone = Pool.Spawn();

        collider = SafetyZone.GetComponent<SafetyZoneCollider>();
        collider.__Initialize();

        SafetyZone.SetActive(true);
        SafetyZones.Add(SafetyZone);
        Colliders.Add(collider);

        Vector2 vertex = index - m_Game.StandardPos;
        List<Vector2> vertexs = new List<Vector2>();

        E_StageType[,] stage = (E_StageType[,])m_Resources.Stage.Clone();

        Stack<Vector2> history = new Stack<Vector2>();

        while (true)
        {
            // 콜라이더에 현재 위치 추가
            if (!collider.indices.Contains(index))
            {
                collider.indices.Add(index);
            }

            // 현재 위치 막음
            stage[h, w] = E_StageType.Wall;

            bool flag = false;

            if (w + 1 < m_Game.width && !flag)
            {
                // 오른쪽이 길일 경우
                if (stage[h, w + 1] == E_StageType.SafetyZone)
                {
                    flag = true;

                    // 이전 위치 기억
                    history.Push(index);
                    // 이동
                    ++index.x;
                }
            }
            if (h + 1 < m_Game.height && !flag)
            {
                // 위쪽이 길일 경우
                if (stage[h + 1, w] == E_StageType.SafetyZone)
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
                if (stage[h, w - 1] == E_StageType.SafetyZone)
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
                if (stage[h - 1, w] == E_StageType.SafetyZone)
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

        if (collider.indices.Count == 1)
        {
            vertexs.Add(vertex);
            vertexs.Add(vertex + Vector2.right);
            vertexs.Add(vertex + Vector2.one);
            vertexs.Add(vertex + Vector2.up);
            vertexs.Add(vertex);

            collider.Polygon.SetPath(0, vertexs);
            return;
        }

        stage = (E_StageType[,])m_Resources.Stage.Clone();

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
                    x > m_Game.width - 1 ||
                    y > m_Game.height - 1)
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

                if (stage[y, x] == E_StageType.SafetyZone)
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
            vertex = index - m_Game.StandardPos;
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
                if (vertexs.Contains(new Vector2(width, height) - m_Game.StandardPos))
                    break;
            }

            index += dir;

            if (error_count >= 1000000)
            {
                Debug.LogError("무한 루프 오류");
                return;
            }
        }

        vertexs.RemoveAt(vertexs.IndexOf(new Vector2(width, height) - m_Game.StandardPos));
        vertexs.Insert(0, new Vector2(width, height) - m_Game.StandardPos);
        vertexs.Add(new Vector2(width, height) - m_Game.StandardPos);

        collider.Polygon.SetPath(0, vertexs);

        Debug.DrawLine(vertexs[0], collider.GetCenter(), Color.red, 5f);
    }

    public void CreateSafetyZone()
    {
        for (int y = 0; y < m_Game.height; ++y)
        {
            for (int x = 0; x < m_Game.width; ++x)
            {
                // 현재 칸이 안전 구역일 때
                if (m_Resources.Stage[y, x] == E_StageType.SafetyZone)
                {
                    bool flag = false;
                    for (int index = 0; index < Colliders.Count; ++index)
                    {
                        if (Colliders[index].indices.Contains(new Vector2(x, y)))
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
        //m_Edit.UpdateSafetyZoneOption();
    }
    public void ClearSafetyZone()
    {
        for (int i = 0; i < SafetyZones.Count; ++i)
        {
            Pool.DeSpawn(SafetyZones[i].gameObject);
        }

        SafetyZones.Clear();
        Colliders.Clear();

        m_Edit.dropdown_first.ClearOptions();
        m_Edit.dropdown_last.ClearOptions();
    }
}
