using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class __GameManager : Singleton<__GameManager>
{
    [SerializeField, ReadOnly(true)]
    protected int m_width;
    [SerializeField, ReadOnly(true)]
    protected int m_height;

    [Header("TileColors")]
    // 홀수 칸
    [SerializeField, ReadOnly(true)]
    protected Color m_OddColor;
    // 짝수 칸
    [SerializeField, ReadOnly(true)]
    protected Color m_EvenColor;
    [SerializeField, ReadOnly(true)]
    protected Color m_WallColor;
    [SerializeField, ReadOnly(true)]
    protected Color m_SafetyZoneColor;

    #region 내부 프로퍼티
    #region 매니져
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;

    protected StageManager M_Stage => StageManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public int width { get => m_width; set => m_width = value; }
    public int height { get => m_height; set => m_height = value; }
    public bool isPlayMode => !M_Edit.isEditMode;

    public Color oddColor { get => m_OddColor; set => m_OddColor = value; }
    public Color evenColor { get => m_EvenColor; set => m_EvenColor = value; }
    public Color wallColor { get => m_WallColor; set => m_WallColor = value; }
    public Color safetyZoneColor { get => m_SafetyZoneColor; set => m_SafetyZoneColor = value; }
    #endregion
    #region 내부 함수
    void __Initialize()
    {
        M_Resources.__Initialize();
        M_Edit.__Initialize();

        M_Stage.__Initialize();
        M_Player.__Initialize();
        M_Enemy.__Initialize();
        M_EnemyGizmo.__Initialize();
        M_Coin.__Initialize();
        M_Wall.__Initialize();
        M_SafetyZone.__Initialize();
        M_Tile.__Initialize();
    }
    void __Finalize()
    {
        M_Edit.__Finalize();
        M_Coin.__Finalize();
        M_Enemy.__Finalize();
        M_EnemyGizmo.__Finalize();
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