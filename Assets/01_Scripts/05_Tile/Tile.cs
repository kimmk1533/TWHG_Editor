using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    protected E_TileIndexType m_IndexType;
    [SerializeField]
    protected E_TileType m_Type;
    protected Image m_Image;

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected WallManager M_Wall => WallManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion

    protected Color OddColor => M_Game.m_OddBGColor;
    protected Color EvenColor => M_Game.m_EvenBGColor;
    protected Color WallColor => M_Game.m_WallColor;
    protected Color SafetyZoneColor => M_Game.m_SafetyZoneColor;
    #endregion
    #region 외부 프로퍼티
    public E_TileIndexType indexType => m_IndexType;
    public E_TileType type => m_Type;
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
                switch (m_IndexType)
                {
                    case E_TileIndexType.Odd:
                        m_Image.color = OddColor;
                        break;
                    case E_TileIndexType.Even:
                        m_Image.color = EvenColor;
                        break;
                    default:
                        Debug.LogError("타일 홀짝 오류");
                        break;
                }
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

    public void __Initialize(int x, int y)
    {
        if (null == m_Image)
        {
            m_Image = GetComponent<Image>();
        }

        int index = (x + y) % 2;

        // 짝수 칸
        if (index == 0)
            m_IndexType = E_TileIndexType.Even;
        else
            m_IndexType = E_TileIndexType.Odd;
    }
}