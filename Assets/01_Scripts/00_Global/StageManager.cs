using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    protected E_TileType[,] m_Stage;

    [SerializeField, ReadOnly(true)]
    protected List<GameObject> m_ManagerList;
    protected List<ISaveHandler> m_SaveList;
    protected List<ILoadHandler> m_LoadList;

    #region 내부 프로퍼티
    #region 매니저
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;

    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;
    #endregion

    protected int width { get => M_Game.width; set => M_Game.width = value; }
    protected int height { get => M_Game.height; set => M_Game.height = value; }
    #endregion
    #region 외부 프로퍼티
    public E_TileType[,] stage { get => m_Stage; }
    #endregion
    #region 내부 함수
    protected void ResetStage()
    {
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                m_Stage[y, x] = E_TileType.None;
            }
        }
    }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        m_Stage = new E_TileType[height, width];

        ResetStage();

        if (null == m_SaveList)
        {
            m_SaveList = new List<ISaveHandler>();
        }
        if (null == m_LoadList)
        {
            m_LoadList = new List<ILoadHandler>();
        }

        foreach (var item in m_ManagerList)
        {
            m_SaveList.Add(item.GetComponent<ISaveHandler>());
            m_LoadList.Add(item.GetComponent<ILoadHandler>());
        }
    }
    public void __Finalize()
    {
        ResetStage();
    }

    public void SaveStage(string stageName)
    {
        string path = Path.Combine(Application.dataPath, "Stage");
        string file = stageName;
        if (!file.EndsWith(".xml"))
        {
            file += ".xml";
        }
        string filepath = Path.Combine(path, file);

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        XmlWriter writer = XmlWriter.Create(filepath, settings);

        try
        {
            // 루트 시작
            writer.WriteStartElement("root");

            //// 주석
            //writer.WriteComment("버젼");
            //// 버젼 시작
            //writer.WriteStartElement("Game Version");
            //// 버젼 입력
            //writer.WriteValue(m_Version);
            //// 버젼 끝
            //writer.WriteEndElement();

            foreach (var item in m_SaveList)
            {
                item?.Save(writer);
            }

            // 루트 끝
            writer.WriteEndElement();

            M_FloatingText.SpawnFloatingText("저장 완료");
        }
        catch (Exception e)
        {
            M_FloatingText.SpawnFloatingText("저장 실패");
            M_FloatingText.SpawnFloatingText(e.Message);
        }
        finally
        {
            writer.Close();
            writer.Dispose();
        }
    }
    public void LoadStage(string stageName)
    {
        string path = Path.Combine(Application.dataPath, "Stage", stageName + ".xml");

        XmlReader reader = XmlReader.Create(path);

        try
        {
            foreach (var item in m_LoadList)
            {
                item?.Load(reader);
            }

            M_FloatingText.SpawnFloatingText("불러오기 완료");
        }
        catch (Exception e)
        {
            M_FloatingText.SpawnFloatingText("불러오기 실패");
            M_FloatingText.SpawnFloatingText(e.Message);
        }
        finally
        {
            reader.Close();
            reader.Dispose();
        }
    }
    #endregion
    #region 유니티 콜백 함수
    private void Update()
    {
        if (M_Edit.isEditMode)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                SaveStage("TestStage");
                Debug.Log("Save Complete");
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                LoadStage("TestStage");
                Debug.Log("Load Complete");
            }
        }
    }
    #endregion
}
