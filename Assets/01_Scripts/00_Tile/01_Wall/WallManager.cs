using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class WallManager : ObjectManager<WallManager, Wall>, ISaveHandler, ILoadHandler
{
    // 관리 리스트
    protected List<Wall> m_WallList;

    #region 내부 프로퍼티
    #region 매니져
    protected TileManager M_Tile => TileManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    protected void ClearWall()
    {
        for (int i = 0; i < m_WallList.Count; ++i)
        {
            m_WallList[i].collider.EraseTile();
        }

        m_WallList.Clear();
    }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        #region 이벤트 링크
        M_Game.OnEnterPlayMode += OnEnterPlayMode;
        M_Game.OnExitPlayMode += OnExitPlayMode;
        #endregion

        // 풀 사이즈 설정
        m_PoolSize = M_Game.width * M_Game.height;

        // 벽 풀 원본
        Wall wall = M_Resources.GetGameObject<Wall>("Tile", "Wall");
        // 벽 풀 생성
        AddPool("Wall", wall, transform);

        // 관리 리스트 초기화
        if (null == m_WallList)
        {
            m_WallList = new List<Wall>();
        }
    }
    public override void __Finalize()
    {
        ClearWall();

        base.__Finalize();
    }

    public Wall SpawnWall()
    {
        // 스폰
        Wall wall = GetPool("Wall").Spawn();
        // 관리 리스트에 추가
        m_WallList.Add(wall);

        return wall;
    }
    public void DespawnWall(Wall wall)
    {
        m_WallList.Remove(wall);
        GetPool("Wall").DeSpawn(wall);
    }

    public void Save(XmlWriter writer)
    {
        // 주석
        writer.WriteComment("벽");
        // 벽 리스트 시작
        writer.WriteStartElement("WallList");

        #region 갯수
        // 벽 갯수 시작
        writer.WriteStartAttribute("Count");
        // 벽 갯수 입력
        writer.WriteValue(m_WallList.Count);
        // 벽 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 벽
        foreach (var wall in m_WallList)
        {
            // 벽 시작
            writer.WriteStartElement("Wall");

            #region 인덱스
            // 벽 인덱스 시작
            writer.WriteStartElement("Index");
            // 벽 인덱스 입력
            writer.WriteValue(wall.tile.index);
            // 벽 인덱스 끝
            writer.WriteEndElement();
            #endregion
            #region 색깔
            // 벽 색깔 시작
            writer.WriteStartElement("Color");
            // 벽 색깔 입력
            writer.WriteColor(wall.tile.color);
            // 벽 색깔 끝
            writer.WriteEndElement();
            #endregion

            // 벽 끝
            writer.WriteEndElement();
        }
        #endregion

        // 벽 리스트 끝
        writer.WriteEndElement();
    }
    public void Load(XmlReader reader)
    {
        if (reader.LoadToElement("WallList"))
        {
            int count;
            string count_str = reader.GetAttribute("Count");
            if (!int.TryParse(count_str, out count))
            {
                count = 0;
            }

            for (int i = 0; i < count; ++i)
            {
                if (reader.LoadToElement("Wall"))
                {
                    int index = 0;

                    if (reader.LoadToElement("Index"))
                    {
                        reader.ReadStartElement("Index");
                        if (!int.TryParse(reader.Value, out index))
                        {
                            index = -1;
                        }
                        if (index < 0 || index >= M_Tile.tileList.Count)
                        {
                            Debug.LogError("Wall 불러오기 중 타일 인덱스 오류");
                            continue;
                        }
                    }

                    Tile tile = M_Tile.tileList[index];
                    tile.SetType(E_TileType.Wall);

                    Vector3 spawnPoint = tile.transform.position;
                    spawnPoint.z = 5f;

                    if (reader.LoadToElement("Color"))
                    {
                        tile.color = reader.ReadColor("Color");
                    }

                    // 스폰
                    Wall wall = SpawnWall();
                    // 위치 설정
                    wall.transform.position = spawnPoint;
                    // 초기화
                    wall.__Initialize(tile);
                    // 활성화
                    wall.gameObject.SetActive(true);
                }
            }
        }
    }
    #endregion
    #region 이벤트 함수
    public void OnEnterPlayMode()
    {

    }
    public void OnExitPlayMode()
    {

    }
    #endregion

    public enum E_WallDirection
    {
        Left,
        Right,
        Top,
        Bottom,

        Max
    }
}