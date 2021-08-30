using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum E_ResourcesType
//{
//    Player,
//    Enemy,
//    Coin,
//    Wall,
//    Tile,
//    UI,

//    Max
//}

public enum E_TileType
{
    Empty = 0,
    Wall = 1,
    SafetyZone = 2,
}
public class StageManager : Singleton<StageManager>
{
    public E_TileType[,] m_Stage;

    void ResetStage()
    {
        for (int i = 0; i < m_Stage.GetLength(0); ++i)
        {
            for (int j = 0; j < m_Stage.GetLength(1); ++j)
            {
                m_Stage[i, j] = E_TileType.Empty;
            }
        }
    }

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        //if (null == m_Stage)
        //{
        //    m_Stage = new E_TileType[M_InGame.m_height, M_InGame.m_width];
        //}
    }

    void Update()
    {
        
    }
    #endregion
}
