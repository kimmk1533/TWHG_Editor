using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class GravityZoneManager : ObjectManager<GravityZoneManager, GravityZone>, ISaveHandler, ILoadHandler
{
    [SerializeField]
    protected List<GravityZone> m_GravityZoneList;

    #region 내부 컴포넌트
    #region 매니저
    protected TileManager M_Tile => TileManager.Instance;
    #endregion
    #endregion
    #region 내부 프로퍼티
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 내부 함수
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

        // 중력구역 풀 원본
        GravityZone gravityZone = M_Resources.GetGameObject<GravityZone>("Tile", "GravityZone");
        // 중력구역 풀 생성
        AddPool("GravityZone", gravityZone, transform);

        // 관리 리스트 초기화
        if (null == m_GravityZoneList)
        {
            m_GravityZoneList = new List<GravityZone>();
        }
    }
    public override void __Finalize()
    {
        base.__Finalize();

    }

    public GravityZone SpawnGravityZone()
    {
        GravityZone gravityZone = GetPool("GravityZone").Spawn();
        m_GravityZoneList.Add(gravityZone);
        return gravityZone;
    }
    public void DespawnGravityZone(GravityZone gravityZone)
    {
        m_GravityZoneList.Remove(gravityZone);
        GetPool("GravityZone").DeSpawn(gravityZone);
    }

    public void Save(XmlWriter writer)
    {
        // 주석
        writer.WriteComment("중력구역");
        // 중력구역 리스트 시작
        writer.WriteStartElement("GravityZoneList");

        #region 갯수
        // 중력구역 갯수 시작
        writer.WriteStartAttribute("Count");
        // 중력구역 갯수 입력
        writer.WriteValue(m_GravityZoneList.Count);
        // 중력구역 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 중력구역
        foreach (var gravityZone in m_GravityZoneList)
        {
            // 중력구역 시작
            writer.WriteStartElement("GravityZone");

            #region 인덱스
            // 중력구역 인덱스 시작
            writer.WriteStartElement("Index");
            // 중력구역 인덱스 입력
            writer.WriteValue(gravityZone.tile.index);
            // 중력구역 인덱스 끝
            writer.WriteEndElement();
            #endregion
            #region 중력
            // 중력구역 중력 시작
            writer.WriteStartElement("Gravity");
            // 중력구역 중력 입력
            writer.WriteValue(gravityZone.gravity);
            // 중력구역 중력 끝
            writer.WriteEndElement();
            #endregion

            // 중력구역 끝
            writer.WriteEndElement();
        }
        #endregion

        // 중력구역 리스트 끝
        writer.WriteEndElement();
    }
    public void Load(XmlReader reader)
    {
        if (reader.LoadToElement("GravityZoneList"))
        {
            int count;
            string count_str = reader.GetAttribute("Count");
            if (!int.TryParse(count_str, out count))
            {
                count = 0;
            }

            for (int i = 0; i < count; ++i)
            {
                if (reader.LoadToElement("GravityZone"))
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
                            Debug.LogError("GravityZone 불러오기 중 타일 인덱스 오류");
                            continue;
                        }
                    }

                    Tile tile = M_Tile.tileList[index];
                    tile.SetType(E_TileType.GravityZone);

                    Vector3 spawnPoint = (Vector2)tile.transform.position;

                    Vector2 gravity = MyPhysics.Physics2D.gravity;
                    if (reader.LoadToElement("Gravity"))
                    {
                        gravity = reader.ReadVector("Gravity");
                    }

                    // 스폰
                    GravityZone gravityZone = SpawnGravityZone();
                    // 위치 설정
                    gravityZone.transform.position = spawnPoint;
                    // 초기화
                    gravityZone.__Initialize(tile);
                    // 중력 설정
                    gravityZone.gravity = gravity;
                    // 활성화
                    gravityZone.gameObject.SetActive(true);
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
}
