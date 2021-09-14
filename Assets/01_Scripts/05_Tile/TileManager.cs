using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileManager : ObjectManager<TileManager, Tile>
{
    // 생성한 타일(BG) 리스트
    protected List<Tile> m_Tiles;

    // 타일(BG) 부모
    [SerializeField, ReadOnly(true)]
    protected GameObject m_TileParent;
    // 정렬 컴포넌트
    protected GridLayoutGroup m_GridLayoutGroup;

    #region 내부 프로퍼티
    #region 매니져
    protected StageManager M_Stage => StageManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    // 타일(BG) 전체 생성 함수
    void CreateTiles()
    {
        for (int i = 0; i < M_Game.height; ++i)
        {
            for (int j = 0; j < M_Game.width; ++j)
            {
                CreateTile(j, i);
            }
        }
    }
    // 타일(BG) 하나 생성 함수
    void CreateTile(int x, int y)
    {
        // 타일(BG) 스폰
        Tile tile = GetPool("Tile").Spawn();

        // 타일(BG) 설정
        tile.transform.SetParent(m_TileParent.transform);
        tile.transform.localScale = Vector3.one;
        tile.transform.localPosition = Vector3.zero;
        tile.__Initialize(x, y);
        tile.gameObject.SetActive(true);

        // 타일(BG) 이미지 설정
        Image image = tile.GetComponent<Image>();
        switch (M_Stage.stage[y, x])
        {
            case E_TileType.None:
                switch (tile.indexType)
                {
                    case E_TileIndexType.Odd:
                        image.color = M_Game.oddColor;
                        break;
                    case E_TileIndexType.Even:
                        image.color = M_Game.evenColor;
                        break;
                    default:
                        Debug.LogError("타일 홀짝 오류");
                        break;
                }
                break;
            case E_TileType.Wall:
                image.color = M_Game.wallColor;
                break;
            case E_TileType.SafetyZone:
                image.color = M_Game.safetyZoneColor;
                break;
            default:
                Debug.LogError("스테이지 값 오류");
                break;
        }

        // 관리 리스트에 추가
        m_Tiles.Add(tile);
    }
    // 타일(BG) 전체 삭제 함수
    void ClearTiles()
    {
        for (int i = m_Tiles.Count - 1; i >= 0; --i)
        {
            GetPool("Tile").DeSpawn(m_Tiles[i]);
        }

        m_Tiles.Clear();
    }

    //    void PlayerProcess()
    //    {
    //        if (null == M_SafetyZone.m_StartPoint)
    //        {
    //            M_Player.m_Player.transform.position = m_Pos;
    //            M_Player.m_InitPos = m_Pos;
    //        }
    //        else
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //            M_Player.m_InitPos = M_SafetyZone.m_StartPoint.GetCenter();
    //        }

    //        M_Player.m_Player.gameObject.SetActive(true);

    //        M_Edit.CurrentSelectedType = E_ObjectType.None;
    //        M_Edit.CurrentSelectedText.text = "Selected:" + "\n" + "None";
    //        M_Edit.CurrentSelectedImage.color = Color.white * 0f;
    //    }
    //    void EnemyProcess()
    //    {
    //        if (!M_Edit.AddPoint)
    //        {
    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear ||
    //                M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
    //            {
    //                if (M_Road.m_WayPoints.Count < 1)
    //                    return;
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                if (M_Road.m_WayPoints.Count < 1)
    //                    return;
    //            }

    //            M_Road.m_WayPoints.Insert(0, m_Pos);

    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear)
    //            {
    //                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), false);
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
    //            {
    //                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), true);
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                M_Road.CreateCircularRoad(M_Road.m_WayPoints[0], M_Road.m_WayPoints[1], float.Parse(M_Edit.input_EnemySpeed.text));
    //            }

    //            M_Road.m_WayPoints.Clear();
    //        }
    //        else
    //        {
    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                M_Road.m_WayPoints.Insert(0, m_Pos);
    //                M_Edit.PointButtonPressed();
    //            }
    //            else
    //            {
    //                M_Road.m_WayPoints.Add(m_Pos);
    //            }
    //        }
    //    }
    //    void CoinProcess()
    //    {
    //        M_Coin.SpawnCoin(m_Pos);
    //    }
    //    void SafetyZoneProcess()
    //    {
    //        M_SafetyZone.ClearSafetyZone();

    //        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
    //        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

    //        m_Image.color = M_Game.m_SafetyZone;
    //        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.SafetyZone;

    //        M_SafetyZone.CreateSafetyZone();

    //        if (M_SafetyZone.m_StartPoint != null)
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //        }

    //        M_Wall.ClearWall();
    //        M_Wall.CreateWall();
    //    }
    //    void WallProcess()
    //    {
    //        M_Wall.ClearWall();

    //        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
    //        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

    //        m_Image.color = M_Game.m_Wall;
    //        if (m_Index.x < 0 || m_Index.x >= M_Stage.m_Stage.GetLength(0))
    //            Debug.Log("인덱스 x : " + m_Index.x);
    //        if (m_Index.y < 0 || m_Index.y >= M_Stage.m_Stage.GetLength(1))
    //            Debug.Log("인덱스 y : " + m_Index.y);
    //        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.Wall;

    //        M_Wall.CreateWall();

    //        M_SafetyZone.ClearSafetyZone();
    //        M_SafetyZone.CreateSafetyZone();

    //        if (M_SafetyZone.m_StartPoint == null)
    //        {
    //            M_Player.m_Player.transform.position = M_Player.m_InitPos;
    //        }
    //        else
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //        }
    //    }

    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        // 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        // 관리 리스트 초기화
        if (null == m_Tiles)
        {
            m_Tiles = new List<Tile>();
        }

        // 정렬 설정
        if (null == m_GridLayoutGroup)
        {
            m_GridLayoutGroup = m_TileParent.GetComponent<GridLayoutGroup>();
        }
        if (m_GridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
            m_GridLayoutGroup.constraintCount = M_Game.width;
        else if (m_GridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
            m_GridLayoutGroup.constraintCount = M_Game.height;

        // 풀 사이즈 설정
        m_PoolSize = M_Game.width * M_Game.height;

        // 타일(BG) 풀 원본
        Tile tile = M_Resources.GetGameObject<Tile>("Tile", "Tile");
        // 타일(BG) 풀 생성
        AddPool("Tile", tile, transform);

        // 타일(BG) 생성
        CreateTiles();
    }
    public override void __Finalize()
    {
        // 타일(BG) 삭제
        ClearTiles();

        base.__Finalize();
    }

    public void Draw(GameObject obj, E_TileType type)
    {
        obj.GetComponent<Tile>()?.SetType(type);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {
        //// 타일(BG) 삭제
        //ClearTiles();

        //// 타일(BG) 생성
        //CreateTiles();
    }
    public override void OnPlayExit()
    {
        //// 타일(BG) 삭제
        //ClearTiles();

        //// 타일(BG) 생성
        //CreateTiles();
    }
    #endregion
    #region 유니티 콜백 함수
    //protected override void Awake()
    //{
    //    base.Awake();
    //}
    #endregion
}