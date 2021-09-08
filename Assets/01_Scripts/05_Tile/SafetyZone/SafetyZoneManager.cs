using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneManager : ObjectManager<SafetyZoneManager, SafetyZone>
{
    protected int m_SafetyZoneCount;

    protected List<SafetyZone> m_SafetyZoneList;
    protected List<SafetyZone> m_FinishZoneList;

    #region 내부 프로퍼티
    #region 매니져
    protected StageManager M_Stage => StageManager.Instance;
    #endregion

    #endregion
    #region 외부 프로퍼티
    public int SafetyZoneCount => m_SafetyZoneCount;
    #endregion
    #region 내부 함수
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        #region 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
        #endregion

        m_SafetyZoneCount = 0;

        // 풀 사이즈 설정
        m_PoolSize = M_Game.width * M_Game.height;

        // 안전지역 풀 원본
        SafetyZone safetyZone = M_Resources.GetGameObject<SafetyZone>("Tile", "SafetyZone");
        // 안전지역 풀 생성
        AddPool("SafetyZone", safetyZone, transform);

        // 관리 리스트 초기화
        if (null == m_SafetyZoneList)
        {
            m_SafetyZoneList = new List<SafetyZone>();
        }
        if (null == m_FinishZoneList)
        {
            m_FinishZoneList = new List<SafetyZone>();
        }
    }
    public override void __Finalize()
    {
        //ClearSafetyZone();
    }

    public SafetyZone SpawnSafetyZone()
    {
        SafetyZone safetyZone = GetPool("SafetyZone").Spawn();
        m_SafetyZoneList.Add(safetyZone);
        ++m_SafetyZoneCount;
        M_Edit.AddSafetyZoneOption(m_SafetyZoneCount);
        return safetyZone;
    }
    public void DespawnSafetyZone(SafetyZone safetyZone)
    {
        M_Edit.RemoveSafetyZoneOption(m_SafetyZoneCount);
        --m_SafetyZoneCount;
        m_SafetyZoneList.Remove(safetyZone);
        GetPool("SafetyZone").DeSpawn(safetyZone);

        for (int i = 0; i < m_SafetyZoneList.Count; ++i)
        {
            m_SafetyZoneList[i].SetText(safetyZone.safetyZoneCount);
        }
    }

    public void SelectFinishZone(int index)
    {
        if (index < 0 || index > m_SafetyZoneList.Count)
            return;

        m_FinishZoneList.Add(m_SafetyZoneList[index - 1]);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {
        //ClearSafetyZone();

        //CreateSafetyZone();

        // 추후 수정
        //m_StartPoint = m_ColliderList[0];

        //m_EndPoint = m_ColliderList[m_ColliderList.Count - 1];
    }
    public override void OnPlayExit()
    {

    }
    #endregion

    #region 기존 함수
    /*void AddSafetyZone(int width, int height)
    {
        int w = width;
        int h = height;

        Vector2 index = new Vector2(w, h);

        SafetyZone safetyZone = GetPool("SafetyZone").Spawn();
        SafetyZoneCollider collider = safetyZone.GetComponent<SafetyZoneCollider>();
        collider.__Initialize();

        safetyZone.gameObject.SetActive(true);
        m_SafetyZoneList.Add(safetyZone);
        m_ColliderList.Add(collider);

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

            collider.m_Collider.SetPath(0, vertexs);
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

        collider.m_Collider.SetPath(0, vertexs);

        Debug.DrawLine(vertexs[0], collider.GetCenter(), Color.red, 5f);
    }*/
    /*public void CreateSafetyZone()
    {
        for (int y = 0; y < M_Game.m_height; ++y)
        {
            for (int x = 0; x < M_Game.m_width; ++x)
            {
                // 현재 칸이 안전 구역일 때
                if (M_Stage.m_Stage[y, x] == E_TileType.SafetyZone)
                {
                    bool flag = false;
                    for (int index = 0; index < m_ColliderList.Count; ++index)
                    {
                        if (m_ColliderList[index].m_Indices.Contains(new Vector2(x, y)))
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
    }*/
    /*public void ClearSafetyZone()
    {
        for (int i = 0; i < m_SafetyZoneList.Count; ++i)
        {
            GetPool("SafetyZone").DeSpawn(m_SafetyZoneList[i]);
        }

        m_SafetyZoneList.Clear();
        m_ColliderList.Clear();

        M_Edit.dropdown_first.ClearOptions();
        M_Edit.dropdown_last.ClearOptions();
    }*/
    #endregion
}
