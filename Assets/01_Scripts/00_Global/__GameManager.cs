using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __GameManager : Singleton<__GameManager>
{
    [SerializeField, ReadOnly(true)]
    protected int m_width;
    [SerializeField, ReadOnly(true)]
    protected int m_height;

    [Header("TileColors")]
    // 홀수 칸
    [ReadOnly(true)]
    public Color m_OddBGColor;
    // 짝수 칸
    [ReadOnly(true)]
    public Color m_EvenBGColor;
    [ReadOnly(true)]
    public Color m_SafetyZoneColor;
    [ReadOnly(true)]
    public Color m_WallColor;

    #region 내부 프로퍼티
    #region 매니져
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;

    protected StageManager M_Stage => StageManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected EnemyRoadManager M_EnemyRoad => EnemyRoadManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public int width { get => m_width; set => m_width = value; }
    public int height { get => m_height; set => m_height = value; }
    #endregion
    #region 내부 함수
    void __Initialize()
    {
        M_Resources.__Initialize();
        M_Edit.__Initialize();

        M_Stage.__Initialize();
        M_Player.__Initialize();
        M_Enemy.__Initialize();
        M_EnemyRoad.__Initialize();
        M_Coin.__Initialize();
        M_Wall.__Initialize();
        M_SafetyZone.__Initialize();
        M_Tile.__Initialize();
    }
    void __Finalize()
    {
        M_Edit.__Finalize();
        M_Coin.__Finalize();
        M_EnemyRoad.__Finalize();
        M_Enemy.__Finalize();
        M_SafetyZone.__Finalize();
        M_Wall.__Finalize();
        M_Tile.__Finalize();
        M_Player.__Finalize();
    }
    #endregion
    #region 이벤트 함수
    public event Action OnPlayEnter;
    public event Action OnPlayExit;

    public void EnterPlayMode()
    {
        OnPlayEnter?.Invoke();
    }
    public void ExitPlayMode()
    {
        OnPlayExit?.Invoke();
    }
    #endregion
    #region 유니티 콜백 함수
    protected void Awake()
    {
        __Initialize();
    }
    protected void OnApplicationQuit()
    {
        __Finalize();
    }
    #endregion
}