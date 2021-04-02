using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __GameManager : Singleton<__GameManager>
{
    public readonly float PixelUnit = 80f;

    [ReadOnly(true)]
    public float Width;
    [ReadOnly(true)]
    public float Height;

    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;

    [Header("TileColors")]
    [ReadOnly(true)]
    public Color OddBG;
    [ReadOnly(true)]
    public Color EvenBG;
    [ReadOnly(true)]
    public Color SafetyZone;
    //[ReadOnly(true)]
    public Color Wall;

    public Vector2 StandardPos;

    // Managers
    __EditManager m_Edit;

    PathManager m_Path;
    ResourcesManager m_Resources;
    PlayerManager m_Player;
    TileManager m_Tile;
    WallManager m_Wall;
    SafetyZoneManager m_SafetyZone;
    EnemyManager m_Enemy;
    RoadManager m_EnemyRoad;
    CoinManager m_Coin;
    //

    protected void Awake()
    {
        width = Mathf.RoundToInt(Width);
        height = Mathf.RoundToInt(Height);

        StandardPos = new Vector2(Width * 0.5f, Height * 0.5f);

        m_Edit = __EditManager.Instance;

        m_Path = PathManager.Instance;
        m_Resources = ResourcesManager.Instance;
        m_Tile = TileManager.Instance;
        m_Wall = WallManager.Instance;
        m_SafetyZone = SafetyZoneManager.Instance;
        m_Enemy = EnemyManager.Instance;
        m_EnemyRoad = RoadManager.Instance;
        m_Coin = CoinManager.Instance;
        m_Player = PlayerManager.Instance;

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

    public delegate void OnPlayEvent();
    public event OnPlayEvent OnPlayEnter;
    public event OnPlayEvent OnPlayExit;

    void __Initialize()
    {
        m_Edit.__Initialize();
        m_Path.__Initialize();
        m_Resources.__Initialize();

        m_Player.__Initialize();
        m_Tile.__Initialize();
        m_Wall.__Initialize();
        m_SafetyZone.__Initialize();
        m_Enemy.__Initialize();
        m_EnemyRoad.__Initialize();
        m_Coin.__Initialize();

        OnPlayEnter += m_Edit.OnPlayEnter;

        OnPlayEnter += m_Path.OnPlayEnter;
        OnPlayEnter += m_Resources.OnPlayEnter;
        OnPlayEnter += m_Tile.OnPlayEnter;
        OnPlayEnter += m_Wall.OnPlayEnter;
        OnPlayEnter += m_SafetyZone.OnPlayEnter;
        OnPlayEnter += m_Enemy.OnPlayEnter;
        OnPlayEnter += m_EnemyRoad.OnPlayEnter;
        OnPlayEnter += m_Coin.OnPlayEnter;
        OnPlayEnter += m_Player.OnPlayEnter;

        OnPlayExit += m_Edit.OnPlayExit;

        OnPlayExit += m_Path.OnPlayExit;
        OnPlayExit += m_Resources.OnPlayExit;
        OnPlayExit += m_Tile.OnPlayExit;
        OnPlayExit += m_Wall.OnPlayExit;
        OnPlayExit += m_SafetyZone.OnPlayExit;
        OnPlayExit += m_Enemy.OnPlayExit;
        OnPlayExit += m_EnemyRoad.OnPlayExit;
        OnPlayExit += m_Coin.OnPlayExit;
        OnPlayExit += m_Player.OnPlayExit;
    }
    void __Finalize()
    {
        m_Edit.__Finalize();
        m_Coin.__Finalize();
        m_EnemyRoad.__Finalize();
        m_Enemy.__Finalize();
        m_SafetyZone.__Finalize();
        m_Wall.__Finalize();
        m_Tile.__Finalize();
        m_Player.__Finalize();
        m_Resources.__Finalize();
        m_Path.__Finalize();
    }
}
