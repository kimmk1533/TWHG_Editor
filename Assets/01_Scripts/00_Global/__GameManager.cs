using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __GameManager : Singleton<__GameManager>
{
    [ReadOnly(true)]
    public float m_Width;
    [ReadOnly(true)]
    public float m_Height;

    [HideInInspector]
    public int m_width;
    [HideInInspector]
    public int m_height;

    [Header("TileColors")]
    // 홀수 칸
    [ReadOnly(true)]
    public Color m_OddBG;
    // 짝수 칸
    [ReadOnly(true)]
    public Color m_EvenBG;
    [ReadOnly(true)]
    public Color m_SafetyZone;
    [ReadOnly(true)]
    public Color m_Wall;

    public Vector2 m_StandardPos;

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

    protected void Awake()
    {
        m_width = Mathf.RoundToInt(m_Width);
        m_height = Mathf.RoundToInt(m_Height);

        m_StandardPos = new Vector2(m_Width * 0.5f, m_Height * 0.5f);

        __Initialize();
    }

    private void OnApplicationQuit()
    {
        __Finalize();
    }

    public void EnterPlayMode()
    {
        OnPlayEnter?.Invoke();
    }
    public void ExitPlayMode()
    {
        OnPlayExit?.Invoke();
    }

    //public delegate void OnPlayEvent();
    public event Action OnPlayEnter;
    public event Action OnPlayExit;

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
}
