using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    protected E_TileType m_Type;
    protected Image m_Image;
    protected Color m_InitColor;

    #region 내부 프로퍼티
    #region 매니져
    protected __EditManager M_Edit => __EditManager.Instance;
    protected __GameManager M_Game => __GameManager.Instance;
    protected PlayerManager M_Player => PlayerManager.Instance;
    protected TileManager M_Tile => TileManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected EnemyRoadManager M_Road => EnemyRoadManager.Instance;
    protected CoinManager M_Coin => CoinManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    protected StageManager M_Stage => StageManager.Instance;
    #endregion

    protected Color WallColor => M_Game.m_WallColor;
    protected Color SafetyZoneColor => M_Game.m_SafetyZoneColor;
    #endregion

    #region 외부 함수
    public void SetType(E_TileType type)
    {
        if (m_Type == type)
            return;

        m_Type = type;

        Vector3 spawnPos = transform.position;
        spawnPos.z = -5f;

        switch (m_Type)
        {
            case E_TileType.None:
                m_Image.color = m_InitColor;
                break;
            case E_TileType.Wall:
                m_Image.color = WallColor;

                // 스폰
                Wall wall = M_Wall.SpawnWall();
                // 위치 설정
                wall.transform.position = spawnPos;
                // 초기화
                wall.__Initialize();
                // 활성화
                wall.gameObject.SetActive(true);
                break;
            case E_TileType.SafetyZone:
                m_Image.color = SafetyZoneColor;

                // 스폰
                SafetyZone safetyZone = M_SafetyZone.SpawnSafetyZone();
                // 위치 설정
                safetyZone.transform.position = spawnPos;
                // 초기화
                safetyZone.__Initialize();
                // 활성화
                safetyZone.gameObject.SetActive(true);
                break;
        }
    }
    #endregion

    public void __Initialize(int width, int height)
    {
        if (null == m_Image)
        {
            m_Image = GetComponent<Image>();
        }

        // 짝수 칸
        if ((width + height) % 2 == 0)
        {
            m_Type = E_TileType.Even;
            m_InitColor = M_Game.m_EvenBGColor;
        }
        // 홀수 칸
        else
        {
            m_Type = E_TileType.Odd;
            m_InitColor = M_Game.m_OddBGColor;
        }
    }
}