using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    Image image;

    bool IsDraw;
    bool IsErase;

    __EditManager m_Edit;
    __GameManager m_Game;

    ResourcesManager m_Resources;
    PlayerManager m_Player;
    TileManager m_Tile;
    WallManager m_Wall;
    RoadManager m_Road;
    CoinManager m_Coin;
    SafetyZoneManager m_SafetyZone;

    static Vector2 index, pos;
    public static PointerEventData currentEventData;

    private void Awake()
    {
        m_Edit = __EditManager.Instance;
        m_Game = __GameManager.Instance;

        m_Resources = ResourcesManager.Instance;
        m_Player = PlayerManager.Instance;
        m_Tile = TileManager.Instance;
        m_Wall = WallManager.Instance;
        m_Road = RoadManager.Instance;
        m_Coin = CoinManager.Instance;
        m_SafetyZone = SafetyZoneManager.Instance;

        image = null;

        IsDraw = false;
        IsErase = false;
    }

    public void __Initialize()
    {

    }

    private void Update()
    {
        if (m_Edit.isEdit)
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsDraw = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                IsDraw = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                IsDraw = true;
                IsErase = true;
            }
            if (Input.GetMouseButtonUp(1))
            {
                IsDraw = false;
                IsErase = false;
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (currentEventData != null)
                {
                    CalcIndexandPos(currentEventData, out index, out pos);
                }
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (m_Edit.isEdit)
        {
            currentEventData = null;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (m_Edit.isEdit)
        {
            currentEventData = eventData;

            CalcIndexandPos(eventData, out index, out pos);

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                IsErase = true;
            }

            UpdateTile_Click();
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (m_Edit.isEdit)
        {
            if (IsDraw)
            {
                currentEventData = eventData;

                UpdateTile_Drag();
            }
        }
    }

    void UpdateTile_Click()
    {
        E_SelectedType type = m_Edit.currentSelectedType;

        if (IsErase)
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
        E_SelectedType type = m_Edit.currentSelectedType;

        if (IsErase)
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
        index = pos = eventData.position / m_Game.PixelUnit;

        index.x = (int)index.x;
        index.y = (int)index.y;

        if (index.x >= m_Resources.Stage.GetLength(1))
        {
            Debug.Log("클릭_X좌표 오류");
            index.x = m_Resources.Stage.GetLength(1) - 1;
        }
        if (index.y >= m_Resources.Stage.GetLength(0))
        {
            Debug.Log("클릭_Y좌표 오류");
            index.y = m_Resources.Stage.GetLength(0) - 1;
        }

        pos.x -= m_Game.Width * 0.5f;
        pos.y -= m_Game.Height * 0.5f;
    }
    void PlayerProcess()
    {
        if (m_SafetyZone.StartPoint == null)
        {
            m_Player.Player.transform.position = pos;
            m_Player.InitPos = pos;
        }
        else
        {
            m_Player.Player.transform.position = m_SafetyZone.StartPoint.GetCenter();
            m_Player.InitPos = m_SafetyZone.StartPoint.GetCenter();
        }
        m_Player.Player.SetActive(true);

        m_Edit.currentSelectedType = E_SelectedType.None;
        m_Edit.currentSelectedText.text = "Selected:" + "\n" + "None";
        m_Edit.currentSelectedImage.color = Color.white * 0f;
    }
    void EnemyProcess()
    {
        if (!m_Edit.AddPoint)
        {
            if (m_Road.currentSelectedType == E_EnemyType.Linear ||
                m_Road.currentSelectedType == E_EnemyType.Linear_Repeat)
            {
                if (m_Road.WayPoints.Count < 1)
                    return;
            }
            else if (m_Road.currentSelectedType == E_EnemyType.Circular)
            {
                if (m_Road.WayPoints.Count < 1)
                    return;
            }

            m_Road.WayPoints.Insert(0, pos);

            if (m_Road.currentSelectedType == E_EnemyType.Linear)
            {
                m_Road.CreateLinearRoad(m_Road.WayPoints.ToArray(), float.Parse(m_Edit.input_EnemySpeed.text), false);
            }
            else if (m_Road.currentSelectedType == E_EnemyType.Linear_Repeat)
            {
                m_Road.CreateLinearRoad(m_Road.WayPoints.ToArray(), float.Parse(m_Edit.input_EnemySpeed.text), true);
            }
            else if (m_Road.currentSelectedType == E_EnemyType.Circular)
            {
                m_Road.CreateCircularRoad(m_Road.WayPoints[0], m_Road.WayPoints[1], float.Parse(m_Edit.input_EnemySpeed.text));
            }

            m_Road.WayPoints.Clear();
        }
        else
        {
            if (m_Road.currentSelectedType == E_EnemyType.Circular)
            {
                m_Road.WayPoints.Insert(0, pos);
                m_Edit.PointButtonPressed();
            }
            else
            {
                m_Road.WayPoints.Add(pos);
            }
        }
    }
    void CoinProcess()
    {
        m_Coin.SpawnCoin(pos);
    }
    void SafetyZoneProcess()
    {
        m_SafetyZone.ClearSafetyZone();

        CalcIndexandPos(currentEventData, out index, out pos);
        image = m_Tile.Tiles[m_Game.width * (int)index.y + (int)index.x].GetComponent<Image>();

        image.color = m_Game.SafetyZone;
        m_Resources.Stage[(int)index.y, (int)index.x] = E_StageType.SafetyZone;

        m_SafetyZone.CreateSafetyZone();

        if (m_SafetyZone.StartPoint != null)
        {
            m_Player.Player.transform.position = m_SafetyZone.StartPoint.GetCenter();
        }

        m_Wall.ClearWall();
        m_Wall.CreateWall();
    }
    void WallProcess()
    {
        m_Wall.ClearWall();

        CalcIndexandPos(currentEventData, out index, out pos);
        image = m_Tile.Tiles[m_Game.width * (int)index.y + (int)index.x].GetComponent<Image>();

        image.color = m_Game.Wall;
        m_Resources.Stage[(int)index.y, (int)index.x] = E_StageType.Wall;

        m_Wall.CreateWall();

        m_SafetyZone.ClearSafetyZone();
        m_SafetyZone.CreateSafetyZone();

        if (m_SafetyZone.StartPoint == null)
        {
            m_Player.Player.transform.position = m_Player.InitPos;
        }
        else
        {
            m_Player.Player.transform.position = m_SafetyZone.StartPoint.GetCenter();
        }
    }
    void EraseProcess()
    {
        CalcIndexandPos(currentEventData, out index, out pos);

        RaycastHit2D[] hits = Physics2D.RaycastAll(pos, Vector2.zero, 0f);
        Debug.DrawRay(pos, Vector2.zero, Color.red, 1f);

        foreach (var item in hits)
        {
            if (item.transform.CompareTag("Player"))
            {
                m_Player.Player.SetActive(false);
            }
            else if (item.transform.CompareTag("Enemy"))
            {
                m_Road.RemoveRoad(item.transform.parent.gameObject);
            }
            else if (item.transform.CompareTag("Coin"))
            {
                m_Coin.DespawnCoin(item.transform.parent.gameObject);
            }
        }

        image = m_Tile.Tiles[m_Game.width * (int)index.y + (int)index.x].GetComponent<Image>();

        if (((int)index.x + (int)index.y) % 2 == 0)
        {
            image.color = m_Game.EvenBG;
        }
        else
        {
            image.color = m_Game.OddBG;
        }
        m_Resources.Stage[(int)index.y, (int)index.x] = E_StageType.Empty;

        m_Wall.ClearWall();
        m_Wall.CreateWall();
        m_SafetyZone.ClearSafetyZone();
        m_SafetyZone.CreateSafetyZone();

        if (m_SafetyZone.StartPoint == null)
        {
            m_Player.Player.transform.position = m_Player.InitPos;
        }
        else
        {
            m_Player.Player.transform.position = m_SafetyZone.StartPoint.GetCenter();
        }
    }
}