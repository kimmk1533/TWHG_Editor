using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    Image m_Image;

    bool m_IsDraw;
    bool m_IsErase;

    EditManager M_Edit;
    InGameManager M_Game;
    ResourcesManager M_Resources;
    PlayerManager M_Player;
    TileManager M_Tile;
    WallManager M_Wall;
    EnemyRoadManager M_Road;
    CoinManager M_Coin;
    SafetyZoneManager M_SafetyZone;
    protected StageManager M_Stage => StageManager.Instance;

    static Vector2 m_Index, m_Pos;
    public static PointerEventData m_CurrentEventData;

    private void Awake()
    {
        M_Edit = EditManager.Instance;
        M_Game = InGameManager.Instance;

        M_Resources = ResourcesManager.Instance;
        M_Player = PlayerManager.Instance;
        M_Tile = TileManager.Instance;
        M_Wall = WallManager.Instance;
        M_Road = EnemyRoadManager.Instance;
        M_Coin = CoinManager.Instance;
        M_SafetyZone = SafetyZoneManager.Instance;

        m_Image = null;

        m_IsDraw = false;
        m_IsErase = false;
    }

    public void __Initialize()
    {

    }

    private void Update()
    {
        if (M_Edit.isEdit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_IsDraw = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                m_IsDraw = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                m_IsDraw = true;
                m_IsErase = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                m_IsDraw = false;
                m_IsErase = false;
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (m_CurrentEventData != null)
                {
                    CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (M_Edit.isEdit)
        {
            m_CurrentEventData = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (M_Edit.isEdit)
        {
            m_CurrentEventData = eventData;

            CalcIndexandPos(eventData, out m_Index, out m_Pos);

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                m_IsErase = true;
            }

            UpdateTile_Click();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (M_Edit.isEdit)
        {
            if (m_IsDraw)
            {
                m_CurrentEventData = eventData;

                UpdateTile_Drag();
            }
        }
    }

    void UpdateTile_Click()
    {
        E_SelectedType type = M_Edit.currentSelectedType;

        if (m_IsErase)
        {
            type = E_SelectedType.Erase;
        }

        switch (type)
        {
            case E_SelectedType.Player:
                PlayerProcess();
                break;
            case E_SelectedType.Enemy:
                EnemyProcess();
                break;
            case E_SelectedType.Coin:
                CoinProcess();
                break;
            case E_SelectedType.SafetyZone:
                SafetyZoneProcess();
                break;
            case E_SelectedType.Wall:
                WallProcess();
                break;
            case E_SelectedType.Erase:
                EraseProcess();
                break;
        }
    }
    void UpdateTile_Drag()
    {
        E_SelectedType type = M_Edit.currentSelectedType;

        if (m_IsErase)
        {
            type = E_SelectedType.Erase;
        }

        switch (type)
        {
            case E_SelectedType.SafetyZone:
                SafetyZoneProcess();
                break;
            case E_SelectedType.Wall:
                WallProcess();
                break;
            case E_SelectedType.Erase:
                EraseProcess();
                break;
        }
    }

    void CalcIndexandPos(PointerEventData eventData, out Vector2 index, out Vector2 pos)
    {
        index = pos = eventData.position / M_Game.m_PixelUnit;

        index.x = (int)index.x;
        index.y = (int)index.y;

        if (index.x >= M_Stage.m_Stage.GetLength(1))
        {
            Debug.Log("클릭_X좌표 오류");
            index.x = M_Stage.m_Stage.GetLength(1) - 1;
        }
        if (index.y >= M_Stage.m_Stage.GetLength(0))
        {
            Debug.Log("클릭_Y좌표 오류");
            index.y = M_Stage.m_Stage.GetLength(0) - 1;
        }

        pos.x -= M_Game.m_Width * 0.5f;
        pos.y -= M_Game.m_Height * 0.5f;
    }
    void PlayerProcess()
    {
        if (M_SafetyZone.m_StartPoint == null)
        {
            M_Player.m_Player.transform.position = m_Pos;
            M_Player.m_InitPos = m_Pos;
        }
        else
        {
            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
            M_Player.m_InitPos = M_SafetyZone.m_StartPoint.GetCenter();
        }
        M_Player.m_Player.gameObject.SetActive(true);

        M_Edit.currentSelectedType = E_SelectedType.None;
        M_Edit.currentSelectedText.text = "Selected:" + "\n" + "None";
        M_Edit.currentSelectedImage.color = Color.white * 0f;
    }
    void EnemyProcess()
    {
        if (!M_Edit.AddPoint)
        {
            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear ||
                M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
            {
                if (M_Road.m_WayPoints.Count < 1)
                    return;
            }
            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
            {
                if (M_Road.m_WayPoints.Count < 1)
                    return;
            }

            M_Road.m_WayPoints.Insert(0, m_Pos);

            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear)
            {
                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), false);
            }
            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
            {
                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), true);
            }
            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
            {
                M_Road.CreateCircularRoad(M_Road.m_WayPoints[0], M_Road.m_WayPoints[1], float.Parse(M_Edit.input_EnemySpeed.text));
            }

            M_Road.m_WayPoints.Clear();
        }
        else
        {
            if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
            {
                M_Road.m_WayPoints.Insert(0, m_Pos);
                M_Edit.PointButtonPressed();
            }
            else
            {
                M_Road.m_WayPoints.Add(m_Pos);
            }
        }
    }
    void CoinProcess()
    {
        M_Coin.SpawnCoin(m_Pos);
    }
    void SafetyZoneProcess()
    {
        M_SafetyZone.ClearSafetyZone();

        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

        m_Image.color = M_Game.m_SafetyZone;
        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.SafetyZone;

        M_SafetyZone.CreateSafetyZone();

        if (M_SafetyZone.m_StartPoint != null)
        {
            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
        }

        M_Wall.ClearWall();
        M_Wall.CreateWall();
    }
    void WallProcess()
    {
        M_Wall.ClearWall();

        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

        m_Image.color = M_Game.m_Wall;
        if (m_Index.x < 0 || m_Index.x >= M_Stage.m_Stage.GetLength(0))
            Debug.Log("인덱스 x : " + m_Index.x);
        if (m_Index.y < 0 || m_Index.y >= M_Stage.m_Stage.GetLength(1))
            Debug.Log("인덱스 y : " + m_Index.y);
        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.Wall;

        M_Wall.CreateWall();

        M_SafetyZone.ClearSafetyZone();
        M_SafetyZone.CreateSafetyZone();

        if (M_SafetyZone.m_StartPoint == null)
        {
            M_Player.m_Player.transform.position = M_Player.m_InitPos;
        }
        else
        {
            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
        }
    }
    void EraseProcess()
    {
        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);

        RaycastHit2D[] hits = Physics2D.RaycastAll(m_Pos, Vector2.zero, 0f);
#if UNITY_EDITOR
        Debug.DrawRay(m_Pos, Vector2.zero, Color.red, 1f);
#endif

        foreach (var item in hits)
        {
            if (item.transform.CompareTag("Player"))
            {
                M_Player.m_Player.gameObject.SetActive(false);
            }
            else if (item.transform.CompareTag("Enemy"))
            {
                M_Road.RemoveRoad(item.transform.parent.gameObject);
            }
            else if (item.transform.CompareTag("Coin"))
            {
                M_Coin.DespawnCoin(item.transform.GetComponent<CoinCollider>().m_Coin);
            }
        }

        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

        if (((int)m_Index.x + (int)m_Index.y) % 2 == 0)
        {
            m_Image.color = M_Game.m_EvenBG;
        }
        else
        {
            m_Image.color = M_Game.m_OddBG;
        }
        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.Empty;

        M_Wall.ClearWall();
        M_Wall.CreateWall();
        M_SafetyZone.ClearSafetyZone();
        M_SafetyZone.CreateSafetyZone();

        if (M_SafetyZone.m_StartPoint == null)
        {
            M_Player.m_Player.transform.position = M_Player.m_InitPos;
        }
        else
        {
            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
        }
    }
}