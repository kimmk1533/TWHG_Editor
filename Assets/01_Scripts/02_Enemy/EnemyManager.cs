using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class EnemyManager : ObjectManager<EnemyManager, Enemy>, ISaveHandler, ILoadHandler
{
    protected List<Enemy> m_EnemyList;

    #region 외부 프로퍼티
    public List<Enemy> enemyList { get => m_EnemyList; }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        M_Game.OnEnterPlayMode += OnPlayEnter;
        M_Game.OnExitPlayMode += OnPlayExit;

        // 적 풀 원본
        Enemy enemy = M_Resources.GetGameObject<Enemy>("Enemy", "Enemy");
        // 적 풀 생성
        AddPool("Enemy", enemy, transform);

        if (null == m_EnemyList)
        {
            m_EnemyList = new List<Enemy>();
        }
    }
    public override void __Finalize()
    {
        base.__Finalize();
    }

    public Enemy SpawnEnemy()
    {
        Enemy enemy = GetPool("Enemy").Spawn();
        enemy.__Initialize();
        enemy.gameObject.SetActive(true);
        m_EnemyList.Add(enemy);

        return enemy;
    }
    public void DespawnEnemy(Enemy enemy)
    {
        m_EnemyList.Remove(enemy);
        GetPool("Enemy").DeSpawn(enemy);
    }

    public void Save(XmlWriter writer)
    {
        // 주석
        writer.WriteComment("적");
        // 적 리스트 시작
        writer.WriteStartElement("EnemyList");

        #region 갯수
        // 적 갯수 시작
        writer.WriteStartAttribute("Count");
        // 적 갯수 입력
        writer.WriteValue(m_EnemyList.Count);
        // 적 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 적
        foreach (var enemy in m_EnemyList)
        {
            // 적 시작
            writer.WriteStartElement("Enemy");

            #region 위치
            // 적 위치 시작
            writer.WriteStartElement("Position");
            // 적 위치 입력
            writer.WriteVector(enemy.transform.position);
            // 적 위치 끝
            writer.WriteEndElement();
            #endregion
            #region 타입
            // 적 타입 시작
            writer.WriteStartElement("Type");
            // 적 타입 입력
            writer.WriteValue((int)enemy.type);
            // 적 타입 끝
            writer.WriteEndElement();
            #endregion
            #region 속도
            // 적 속도 시작
            writer.WriteStartElement("Speed");
            // 적 속도 입력
            writer.WriteValue(enemy.speed);
            // 적 속도 끝
            writer.WriteEndElement();
            #endregion

            #region 웨이포인트 기즈모 리스트
            // 적 웨이포인트 기즈모 리스트 시작
            writer.WriteStartElement("WayPointList");

            #region 갯수
            // 적 웨이포인트 기즈모 갯수 시작
            writer.WriteStartAttribute("Count");
            // 적 웨이포인트 기즈모 갯수 입력
            writer.WriteValue(enemy.wayPointList.Count);
            // 적 웨이포인트 기즈모 갯수 끝
            writer.WriteEndAttribute();
            #endregion
            #region 웨이포인트 기즈모
            foreach (var gizmo in enemy.wayPointList)
            {
                // 웨이포인트 기즈모 시작
                writer.WriteStartElement("WayPoint");

                #region 위치
                // 적 웨이포인트 기즈모 위치 시작
                writer.WriteStartElement("Position");
                // 적 웨이포인트 기즈모 위치 입력
                writer.WriteVector(gizmo.transform.position);
                // 적 웨이포인트 기즈모 위치 끝
                writer.WriteEndElement();
                #endregion

                // 웨이포인트 기즈모 끝
                writer.WriteEndElement();
            }
            #endregion

            // 적 웨이포인트 기즈모 리스트 끝
            writer.WriteEndElement();
            #endregion
            #region 중심점 기즈모
            // 적 중심점 기즈모 시작
            writer.WriteStartElement("Center");

            #region 위치
            // 적 중심점 기즈모 위치 시작
            writer.WriteStartElement("Position");
            // 적 중심점 기즈모 위치 입력
            writer.WriteVector(enemy.center.transform.position);
            // 적 중심점 기즈모 위치 끝
            writer.WriteEndElement();
            #endregion

            // 적 중심점 기즈모 끝
            writer.WriteEndElement();
            #endregion

            // 적 끝
            writer.WriteEndElement();
        }
        #endregion

        // 적 리스트 끝
        writer.WriteEndElement();
    }
    public void Load(XmlReader reader)
    {
        if (reader.LoadToElement("EnemyList"))
        {
            int count;
            string count_str = reader.GetAttribute("Count");
            if (!int.TryParse(count_str, out count))
            {
                count = 0;
            }

            for (int i = 0; i < count; ++i)
            {
                Enemy enemy = SpawnEnemy();

                if (reader.LoadToElement("Enemy"))
                {
                    if (reader.LoadToElement("Position"))
                    {
                        enemy.transform.position = reader.ReadVector("Position");
                    }
                    if (reader.LoadToElement("Type"))
                    {
                        int type;
                        reader.ReadStartElement("Type");
                        if (!int.TryParse(reader.Value, out type))
                        {
                            type = -1;
                        }
                        enemy.type = (E_EnemyType)type;
                    }
                    if (reader.LoadToElement("Speed"))
                    {
                        float speed;
                        reader.ReadStartElement("Speed");
                        if (!float.TryParse(reader.Value, out speed))
                        {
                            speed = 0.01f;
                        }
                        enemy.speed = speed;
                    }
                    if (reader.LoadToElement("WayPointList"))
                    {
                        int wayPointCount;
                        string wayPointCount_str = reader.GetAttribute("Count");
                        if (!int.TryParse(wayPointCount_str, out wayPointCount))
                        {
                            wayPointCount = 0;
                        }

                        for (int j = 0; j < wayPointCount; ++j)
                        {
                            if (reader.LoadToElement("WayPoint"))
                            {
                                enemy.AddWayPoint();
                                enemy.wayPointList[j].gameObject.SetActive(false);

                                if (reader.LoadToElement("Position"))
                                {
                                    enemy.wayPointList[j].transform.position = reader.ReadVector("Position");
                                }
                            }
                        }
                    }
                    if (reader.LoadToElement("Center"))
                    {
                        if (reader.LoadToElement("Position"))
                        {
                            enemy.center.transform.position = reader.ReadVector("Position");
                        }
                    }
                }
            }
        }
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {
        foreach (var item in m_EnemyList)
        {
            item.OnPlayEnter();
        }
    }
    public override void OnPlayExit()
    {
        foreach (var item in m_EnemyList)
        {
            item.OnPlayExit();
        }
    }
    #endregion
}