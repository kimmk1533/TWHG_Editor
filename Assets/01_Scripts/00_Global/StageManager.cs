using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    [SerializeField]
    protected string m_StageName;
    protected E_TileType[,] m_Stage;

    #region 내부 컴포넌트
    protected int width { get => M_Game.width; set => M_Game.width = value; }
    protected int height { get => M_Game.height; set => M_Game.height = value; }
    #endregion
    #region 내부 프로퍼티
    protected __GameManager M_Game => __GameManager.Instance;
    #endregion
    #region 외부 프로퍼티
    public string stageName { get => m_StageName; set => m_StageName = value; }
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

    [ContextMenu("test")]
    public void SaveStage()
    {
        #region Tile
        string tilePath = "Stage/" + m_StageName + "_tile";

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
        string objectPath = "Stage/" + m_StageName + "_object";
        #endregion
    }
    public void LoadStage()
    {
        #region Tile
        string tilePath = "Stage/" + m_StageName + "_tile";
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
        string objectPath = "Stage/" + m_StageName + "_object";
        // var objectData = CSVReader.Read(objectPath);
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
            SaveStage();
            //LoadStage();
        }
    }
    #endregion
}
