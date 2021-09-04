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

    protected Color WallColor => M_Game.m_Wall;
    protected Color SafetyZoneColor => M_Game.m_SafetyZone;
    #endregion

    #region 외부 함수
    public void SetInitColor(Color color)
    {
        m_InitColor = color;
    }
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
                safetyZone.transform.position = spawnPos;
                safetyZone.__Initialize();
                safetyZone.gameObject.SetActive(true);
                break;
        }
    }
    #endregion

    static Vector2 m_Index;
    static Vector2 m_Pos;
    public static PointerEventData m_CurrentEventData;

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
            m_InitColor = M_Game.m_EvenBG;
        }
        // 홀수 칸
        else
        {
            m_Type = E_TileType.Odd;
            m_InitColor = M_Game.m_OddBG;
        }
    }

    private void Update()
    {
        if (M_Edit.isEdit)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (null != m_CurrentEventData)
                {
                    CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
                }
            }
        }
    }

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    if (M_Edit.isEdit)
    //    {
    //        m_CurrentEventData = null;
    //    }
    //}
    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    if (M_Edit.isEdit)
    //    {
    //        m_CurrentEventData = eventData;

    //        CalcIndexandPos(eventData, out m_Index, out m_Pos);

    //        UpdateTile_Click();
    //    }
    //}
    //public void OnDrag(PointerEventData eventData)
    //{
    //    if (M_Edit.isEdit)
    //    {
    //        if (M_Edit.m_IsDraw)
    //        {
    //            m_CurrentEventData = eventData;

    //            UpdateTile_Drag();
    //        }
    //    }
    //}

    //void UpdateTile_Click()
    //{
    //    E_ObjectType type = M_Edit.CurrentSelectedType;

    //    if (M_Edit.m_IsErase)
    //    {
    //        type = E_ObjectType.Erase;
    //    }

    //    switch (type)
    //    {
    //        case E_ObjectType.Player:
    //            PlayerProcess();
    //            break;
    //        case E_ObjectType.Enemy:
    //            EnemyProcess();
    //            break;
    //        case E_ObjectType.Coin:
    //            CoinProcess();
    //            break;
    //        case E_ObjectType.SafetyZone:
    //            SafetyZoneProcess();
    //            break;
    //        case E_ObjectType.Wall:
    //            WallProcess();
    //            break;
    //        case E_ObjectType.Erase:
    //            EraseProcess();
    //            break;
    //    }
    //}
    //void UpdateTile_Drag()
    //{
    //    E_ObjectType type = M_Edit.CurrentSelectedType;

    //    if (M_Edit.m_IsErase)
    //    {
    //        type = E_ObjectType.Erase;
    //    }

    //    switch (type)
    //    {
    //        case E_ObjectType.SafetyZone:
    //            SafetyZoneProcess();
    //            break;
    //        case E_ObjectType.Wall:
    //            WallProcess();
    //            break;
    //        case E_ObjectType.Erase:
    //            EraseProcess();
    //            break;
    //    }
    //}

    void CalcIndexandPos(PointerEventData eventData, out Vector2 index, out Vector2 pos)
    {
        index = pos = eventData.position * 0.01f;

        //index.x = (int)(index.x * 0.01f);
        //index.y = (int)(index.y * 0.01f);

        if (index.x >= M_Game.m_width)
        {
            Debug.Log("클릭_X좌표: " + index.x + " 오류");
            index.x = M_Game.m_width - 1;
        }
        if (index.y >= M_Game.m_height)
        {
            Debug.Log("클릭_Y좌표: " + index.y + " 오류");
            index.y = M_Game.m_height - 1;
        }

        //pos.x -= M_Game.m_Width * 0.5f;
        //pos.y -= M_Game.m_Height * 0.5f;
    }
    //    void PlayerProcess()
    //    {
    //        if (null == M_SafetyZone.m_StartPoint)
    //        {
    //            M_Player.m_Player.transform.position = m_Pos;
    //            M_Player.m_InitPos = m_Pos;
    //        }
    //        else
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //            M_Player.m_InitPos = M_SafetyZone.m_StartPoint.GetCenter();
    //        }

    //        M_Player.m_Player.gameObject.SetActive(true);

    //        M_Edit.CurrentSelectedType = E_ObjectType.None;
    //        M_Edit.CurrentSelectedText.text = "Selected:" + "\n" + "None";
    //        M_Edit.CurrentSelectedImage.color = Color.white * 0f;
    //    }
    //    void EnemyProcess()
    //    {
    //        if (!M_Edit.AddPoint)
    //        {
    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear ||
    //                M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
    //            {
    //                if (M_Road.m_WayPoints.Count < 1)
    //                    return;
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                if (M_Road.m_WayPoints.Count < 1)
    //                    return;
    //            }

    //            M_Road.m_WayPoints.Insert(0, m_Pos);

    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear)
    //            {
    //                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), false);
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Linear_Repeat)
    //            {
    //                M_Road.CreateLinearRoad(M_Road.m_WayPoints.ToArray(), float.Parse(M_Edit.input_EnemySpeed.text), true);
    //            }
    //            else if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                M_Road.CreateCircularRoad(M_Road.m_WayPoints[0], M_Road.m_WayPoints[1], float.Parse(M_Edit.input_EnemySpeed.text));
    //            }

    //            M_Road.m_WayPoints.Clear();
    //        }
    //        else
    //        {
    //            if (M_Road.m_CurrentSelectedType == E_EnemyType.Circular)
    //            {
    //                M_Road.m_WayPoints.Insert(0, m_Pos);
    //                M_Edit.PointButtonPressed();
    //            }
    //            else
    //            {
    //                M_Road.m_WayPoints.Add(m_Pos);
    //            }
    //        }
    //    }
    //    void CoinProcess()
    //    {
    //        M_Coin.SpawnCoin(m_Pos);
    //    }
    //    void SafetyZoneProcess()
    //    {
    //        M_SafetyZone.ClearSafetyZone();

    //        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
    //        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

    //        m_Image.color = M_Game.m_SafetyZone;
    //        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.SafetyZone;

    //        M_SafetyZone.CreateSafetyZone();

    //        if (M_SafetyZone.m_StartPoint != null)
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //        }

    //        M_Wall.ClearWall();
    //        M_Wall.CreateWall();
    //    }
    //    void WallProcess()
    //    {
    //        M_Wall.ClearWall();

    //        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);
    //        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

    //        m_Image.color = M_Game.m_Wall;
    //        if (m_Index.x < 0 || m_Index.x >= M_Stage.m_Stage.GetLength(0))
    //            Debug.Log("인덱스 x : " + m_Index.x);
    //        if (m_Index.y < 0 || m_Index.y >= M_Stage.m_Stage.GetLength(1))
    //            Debug.Log("인덱스 y : " + m_Index.y);
    //        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.Wall;

    //        M_Wall.CreateWall();

    //        M_SafetyZone.ClearSafetyZone();
    //        M_SafetyZone.CreateSafetyZone();

    //        if (M_SafetyZone.m_StartPoint == null)
    //        {
    //            M_Player.m_Player.transform.position = M_Player.m_InitPos;
    //        }
    //        else
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //        }
    //    }
    //    void EraseProcess()
    //    {
    //        CalcIndexandPos(m_CurrentEventData, out m_Index, out m_Pos);

    //        RaycastHit2D[] hits = Physics2D.RaycastAll(m_Pos, Vector2.zero, 0f);
    //#if UNITY_EDITOR
    //        Debug.DrawRay(m_Pos, Vector2.zero, Color.red, 1f);
    //#endif

    //        foreach (var item in hits)
    //        {
    //            item.transform.GetComponent<IEraserable>()?.Erase();
    //            //if (item.transform.CompareTag("Player"))
    //            //{
    //            //    M_Player.m_Player.gameObject.SetActive(false);
    //            //}
    //            //else if (item.transform.CompareTag("Enemy"))
    //            //{
    //            //    M_Road.RemoveRoad(item.transform.parent.gameObject);
    //            //}
    //            //else if (item.transform.CompareTag("Coin"))
    //            //{
    //            //    M_Coin.DespawnCoin(item.transform.GetComponent<CoinCollider>().m_Coin);
    //            //}
    //        }

    //        m_Image = M_Tile.m_Tiles[M_Game.m_width * (int)m_Index.y + (int)m_Index.x].GetComponent<Image>();

    //        if (((int)m_Index.x + (int)m_Index.y) % 2 == 0)
    //        {
    //            m_Image.color = M_Game.m_EvenBG;
    //        }
    //        else
    //        {
    //            m_Image.color = M_Game.m_OddBG;
    //        }
    //        M_Stage.m_Stage[(int)m_Index.y, (int)m_Index.x] = E_TileType.None;

    //        M_Wall.ClearWall();
    //        M_Wall.CreateWall();
    //        M_SafetyZone.ClearSafetyZone();
    //        M_SafetyZone.CreateSafetyZone();

    //        if (M_SafetyZone.m_StartPoint == null)
    //        {
    //            M_Player.m_Player.transform.position = M_Player.m_InitPos;
    //        }
    //        else
    //        {
    //            M_Player.m_Player.transform.position = M_SafetyZone.m_StartPoint.GetCenter();
    //        }
    //    }
}