using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public E_TileType[,] m_Stage;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected __GameManager M_Game => __GameManager.Instance;
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    void ResetStage()
    {
        for (int i = 0; i < M_Game.height; ++i)
        {
            for (int j = 0; j < M_Game.width; ++j)
            {
                m_Stage[i, j] = E_TileType.None;
            }
        }
    }
    #endregion

    #region 외부 함수
    public void __Initialize()
    {
        m_Stage = new E_TileType[M_Game.height, M_Game.width];

        ResetStage();
    }
    public void __Finalize()
    {

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
    void Awake()
    {
        //if (null == m_Stage)
        //{
        //    m_Stage = new E_TileType[M_Game.m_height, M_Game.m_width];
        //}
    }

    void Update()
    {
        
    }
    #endregion
}
