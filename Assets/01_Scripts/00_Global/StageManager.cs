using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    protected E_TileType[,] m_Stage;

    #region 내부 컴포넌트
    protected int width { get => M_Game.width; set => M_Game.width = value; }
    protected int height { get => M_Game.height; set => M_Game.height = value; }
    #endregion
    #region 내부 프로퍼티
    #region 매니저
    protected __GameManager M_Game => __GameManager.Instance;

    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion

    #endregion
    #region 외부 프로퍼티
    public E_TileType[,] stage { get => m_Stage; set => m_Stage = value; }
    #endregion
    #region 내부 함수
    protected void ResetStage()
    {
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                m_Stage[i, j] = E_TileType.None;
            }
        }
    }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        m_Stage = new E_TileType[height, width];

        ResetStage();
    }

    public void SaveStage(string stageName)
    {
        #region Tile
        string tilePath = Path.Combine("Stage", stageName + "_tile");

        List<Dictionary<string, object>> data = new List<Dictionary<string, object>>();

        for (int i = 1; i <= height; ++i)
        {
            Dictionary<string, object> widthData = new Dictionary<string, object>();

            widthData.Add("0", i.ToString());
            for (int j = 1; j <= width; ++j)
            {
                widthData.Add(j.ToString(), (int)m_Stage[i - 1, j - 1]);
            }

            data.Add(widthData);
        }

        CSVUtility.Write(tilePath, in data);
        #endregion
        #region Object
        string objectPath = Path.Combine(Application.dataPath, "Stage", stageName + "_object.xml");

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(objectPath, settings);

        // 루트 시작
        writer.WriteStartElement("root");

        #region 버젼
        //// 주석
        //writer.WriteComment("버젼");
        //// 버젼 시작
        //writer.WriteStartElement("version");
        //// 버젼 끝
        //writer.WriteEndElement();
        #endregion
        #region 플레이어
        // 주석
        writer.WriteComment("플레이어");
        // 플레이어 시작
        writer.WriteStartElement("Player");

        #region 위치
        // 플레이어 위치 시작
        writer.WriteStartElement("Position");

        #region x
        // 플레이어 위치 x 시작
        writer.WriteStartElement("x");
        // 플레이어 위치 x 입력
        writer.WriteValue(M_Player.playerPos.x);
        // 플레이어 위치 x 끝
        writer.WriteEndElement();
        #endregion
        #region y
        // 플레이어 위치 y 시작
        writer.WriteStartElement("y");
        // 플레이어 위치 y 입력
        writer.WriteValue(M_Player.playerPos.y);
        // 플레이어 위치 y 끝
        writer.WriteEndElement();
        #endregion
        #region z
        // 플레이어 위치 z 시작
        writer.WriteStartElement("z");
        // 플레이어 위치 z 입력
        writer.WriteValue(M_Player.playerPos.z);
        // 플레이어 위치 z 끝
        writer.WriteEndElement();
        #endregion

        // 플레이어 위치 끝
        writer.WriteEndElement();
        #endregion
        #region 활성화
        // 플레이어 활성화 시작
        writer.WriteStartElement("Active");
        // 플레이어 활성화 입력
        writer.WriteValue(M_Player.playerActive);
        // 플레이어 활성화 끝
        writer.WriteEndElement();
        #endregion

        // 플레이어 끝
        writer.WriteEndElement();
        #endregion
        #region 적 리스트
        // 주석
        writer.WriteComment("적");
        // 적 리스트 시작
        writer.WriteStartElement("EnemyList");

        #region 갯수
        // 적 갯수 시작
        writer.WriteStartAttribute("Count");
        // 적 갯수 입력
        writer.WriteValue(M_Enemy.enemyList.Count);
        // 적 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 적
        foreach (var enemy in M_Enemy.enemyList)
        {
            // 적 시작
            writer.WriteStartElement("Enemy");

            #region 위치
            // 적 위치 시작
            writer.WriteStartElement("Position");

            #region x
            // 적 위치 x 시작
            writer.WriteStartElement("x");
            // 적 위치 x 입력
            writer.WriteValue(enemy.transform.position.x);
            // 적 위치 x 끝
            writer.WriteEndElement();
            #endregion
            #region y
            // 적 위치 y 시작
            writer.WriteStartElement("y");
            // 적 위치 y 입력
            writer.WriteValue(enemy.transform.position.y);
            // 적 위치 y 끝
            writer.WriteEndElement();
            #endregion
            #region z
            // 적 위치 z 시작
            writer.WriteStartElement("z");
            // 적 위치 z 입력
            writer.WriteValue(enemy.transform.position.z);
            // 적 위치 z 끝
            writer.WriteEndElement();
            #endregion

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
                // 적 웨이포인트 기즈모 시작
                writer.WriteStartElement("Position");

                #region x
                // 적 웨이포인트 기즈모 x 시작
                writer.WriteStartElement("x");
                // 적 웨이포인트 기즈모 x 입력
                writer.WriteValue(gizmo.transform.position.x);
                // 적 웨이포인트 기즈모 x 끝
                writer.WriteEndElement();
                #endregion
                #region y
                // 적 웨이포인트 기즈모 y 시작
                writer.WriteStartElement("y");
                // 적 웨이포인트 기즈모 y 입력
                writer.WriteValue(gizmo.transform.position.y);
                // 적 웨이포인트 기즈모 y 끝
                writer.WriteEndElement();
                #endregion
                #region z
                // 적 웨이포인트 기즈모 z 시작
                writer.WriteStartElement("z");
                // 적 웨이포인트 기즈모 z 입력
                writer.WriteValue(gizmo.transform.position.z);
                // 적 웨이포인트 기즈모 z 끝
                writer.WriteEndElement();
                #endregion

                // 적 위치 끝
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
            // 적 중심점 기즈모 시작
            writer.WriteStartElement("Position");

            #region x
            // 적 중심점 기즈모 x 시작
            writer.WriteStartElement("x");
            // 적 중심점 기즈모 x 입력
            writer.WriteValue(enemy.center.transform.position.x);
            // 적 중심점 기즈모 x 끝
            writer.WriteEndElement();
            #endregion
            #region y
            // 적 중심점 기즈모 y 시작
            writer.WriteStartElement("y");
            // 적 중심점 기즈모 y 입력
            writer.WriteValue(enemy.center.transform.position.y);
            // 적 중심점 기즈모 y 끝
            writer.WriteEndElement();
            #endregion
            #region z
            // 적 중심점 기즈모 z 시작
            writer.WriteStartElement("z");
            // 적 중심점 기즈모 z 입력
            writer.WriteValue(enemy.center.transform.position.z);
            // 적 중심점 기즈모 z 끝
            writer.WriteEndElement();
            #endregion

            // 적 위치 끝
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
        #endregion
        #region 코인 리스트
        // 주석
        writer.WriteComment("코인");
        // 코인 리스트 시작
        writer.WriteStartElement("CoinList");

        #region 갯수
        // 코인 갯수 시작
        writer.WriteStartAttribute("Count");
        // 코인 갯수 입력
        writer.WriteValue(M_Coin.coinList.Count);
        // 코인 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 코인
        foreach (var coin in M_Coin.coinList)
        {
            // 코인 시작
            writer.WriteStartElement("Coin");

            #region 위치
            // 코인 위치 시작
            writer.WriteStartElement("Position");

            #region x
            // 코인 위치 x 시작
            writer.WriteStartElement("x");
            // 코인 위치 x 입력
            writer.WriteValue(coin.transform.position.x);
            // 코인 위치 x 끝
            writer.WriteEndElement();
            #endregion
            #region y
            // 코인 위치 y 시작
            writer.WriteStartElement("y");
            // 코인 위치 y 입력
            writer.WriteValue(coin.transform.position.y);
            // 코인 위치 y 끝
            writer.WriteEndElement();
            #endregion
            #region z
            // 코인 위치 z 시작
            writer.WriteStartElement("z");
            // 코인 위치 z 입력
            writer.WriteValue(coin.transform.position.z);
            // 코인 위치 z 끝
            writer.WriteEndElement();
            #endregion

            // 코인 위치 끝
            writer.WriteEndElement();
            #endregion

            // 코인 끝
            writer.WriteEndElement();
        }
        #endregion

        // 코인 리스트 끝
        writer.WriteEndElement();
        #endregion
        #region 벽 리스트
        // 주석
        writer.WriteComment("벽");
        // 벽 리스트 시작
        writer.WriteStartElement("WallList");

        #region 갯수
        // 벽 갯수 시작
        writer.WriteStartAttribute("Count");
        // 벽 갯수 입력
        writer.WriteValue(M_Wall.wallList.Count);
        // 벽 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 벽
        foreach (var wall in M_Wall.wallList)
        {
            // 벽 시작
            writer.WriteStartElement("Wall");

            #region 위치
            // 벽 위치 시작
            writer.WriteStartElement("Position");

            #region x
            // 벽 위치 x 시작
            writer.WriteStartElement("x");
            // 벽 위치 x 입력
            writer.WriteValue(wall.transform.position.x);
            // 벽 위치 x 끝
            writer.WriteEndElement();
            #endregion
            #region y
            // 벽 위치 y 시작
            writer.WriteStartElement("y");
            // 벽 위치 y 입력
            writer.WriteValue(wall.transform.position.y);
            // 벽 위치 y 끝
            writer.WriteEndElement();
            #endregion
            #region z
            // 벽 위치 z 시작
            writer.WriteStartElement("z");
            // 벽 위치 z 입력
            writer.WriteValue(wall.transform.position.z);
            // 벽 위치 z 끝
            writer.WriteEndElement();
            #endregion

            // 벽 위치 끝
            writer.WriteEndElement();
            #endregion
            #region 색깔
            // 벽 색깔 시작
            writer.WriteStartElement("Color");

            #region r
            // 벽 색깔 r 시작
            writer.WriteStartElement("r");
            // 벽 색깔 r 입력
            writer.WriteValue(wall.tile.color.r);
            // 벽 색깔 r 끝
            writer.WriteEndElement();
            #endregion
            #region g
            // 벽 색깔 g 시작
            writer.WriteStartElement("g");
            // 벽 색깔 g 입력
            writer.WriteValue(wall.tile.color.g);
            // 벽 색깔 g 끝
            writer.WriteEndElement();
            #endregion
            #region b
            // 벽 색깔 b 시작
            writer.WriteStartElement("b");
            // 벽 색깔 b 입력
            writer.WriteValue(wall.tile.color.b);
            // 벽 색깔 b 끝
            writer.WriteEndElement();
            #endregion

            // 벽 색깔 끝
            writer.WriteEndElement();
            #endregion

            // 벽 끝
            writer.WriteEndElement();
        }
        #endregion

        // 벽 리스트 끝
        writer.WriteEndElement();
        #endregion
        #region 안전지역 리스트
        // 주석
        writer.WriteComment("안전지역");
        // 안전지역 리스트 시작
        writer.WriteStartElement("SafetyZoneList");

        #region 갯수
        // 안전지역 갯수 시작
        writer.WriteStartAttribute("Count");
        // 안전지역 갯수 입력
        writer.WriteValue(M_SafetyZone.safetyZoneList.Count);
        // 안전지역 갯수 끝
        writer.WriteEndAttribute();
        #endregion
        #region 안전지역
        foreach (var safetyZone in M_SafetyZone.safetyZoneList)
        {
            // 안전지역 시작
            writer.WriteStartElement("SafetyZone");

            #region 위치
            // 안전지역 위치 시작
            writer.WriteStartElement("Position");

            #region x
            // 안전지역 위치 x 시작
            writer.WriteStartElement("x");
            // 안전지역 위치 x 입력
            writer.WriteValue(safetyZone.transform.position.x);
            // 안전지역 위치 x 끝
            writer.WriteEndElement();
            #endregion
            #region y
            // 안전지역 위치 y 시작
            writer.WriteStartElement("y");
            // 안전지역 위치 y 입력
            writer.WriteValue(safetyZone.transform.position.y);
            // 안전지역 위치 y 끝
            writer.WriteEndElement();
            #endregion
            #region z
            // 안전지역 위치 z 시작
            writer.WriteStartElement("z");
            // 안전지역 위치 z 입력
            writer.WriteValue(safetyZone.transform.position.z);
            // 안전지역 위치 z 끝
            writer.WriteEndElement();
            #endregion

            // 안전지역 위치 끝
            writer.WriteEndElement();
            #endregion
            #region 종료지역 여부
            // 안전지역 종료지역 시작
            writer.WriteStartElement("IsFinishZone");
            // 안전지역 종료지역 입력
            writer.WriteValue(safetyZone.isFinishZone);
            // 안전지역 종료지역 끝
            writer.WriteEndElement();
            #endregion

            // 안전지역 끝
            writer.WriteEndElement();
        }
        #endregion

        // 안전지역 리스트 끝
        writer.WriteEndElement();
        #endregion

        // 루트 끝
        writer.WriteEndElement();

        writer.Close();
        #endregion
    }
    public void LoadStage(string stageName)
    {
        #region Tile
        string tilePath = Path.Combine("Stage", stageName + "_tile");
        var stageData = CSVUtility.Read(tilePath);

        width = stageData[0].Count - 1;
        height = stageData.Count;

        m_Stage = new E_TileType[height, width];

        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                m_Stage[i, j] = (E_TileType)stageData[i][(j + 1).ToString()];
            }
        }
        #endregion
        #region Object
        string objectPath = Path.Combine(Application.dataPath, "Stage", stageName + "_object.xml");

        #endregion
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {

    }
    public void OnPlayExit()
    {

    }
    #endregion
    #region 유니티 콜백 함수
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveStage("TestStage");
            //LoadStage();
        }
    }
    #endregion
}
