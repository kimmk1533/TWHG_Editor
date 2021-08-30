using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : ObjectManager<TileManager, Tile>
{
    RectTransform m_RectTransform;

    Image m_Image;
    GameObject m_Tile_parent;
    public List<Tile> m_Tiles;

    protected StageManager M_Stage => StageManager.Instance;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        m_PoolSize = M_Game.m_width * M_Game.m_height;
    }

    public override void OnPlayEnter()
    {
        ClearTiles();

        CreateTiles();
    }
    public override void OnPlayExit()
    {
        ClearTiles();

        CreateTiles();
    }

    public override void __Initialize()
    {
        m_Tiles = new List<Tile>();

        m_PoolSize = M_Game.m_width * M_Game.m_height;

        base.__Initialize();

        m_Tile_parent = new GameObject();
        m_Tile_parent.transform.SetParent(M_Edit.Canvas_BG.transform);
        m_RectTransform = m_Tile_parent.AddComponent<RectTransform>();
        m_Tile_parent.layer = LayerMask.NameToLayer("UI");
        m_Tile_parent.name = "BackGrounds";

        m_RectTransform.anchorMin = new Vector2(0, 0);
        m_RectTransform.anchorMax = new Vector2(0, 0);
        m_Tile_parent.transform.localScale = Vector3.one;
        m_RectTransform.sizeDelta = new Vector2(M_Game.m_PixelUnit, M_Game.m_PixelUnit);
        m_RectTransform.anchoredPosition3D = CalcPosition(m_RectTransform.pivot, 0, 0);

        CreateTiles();

        foreach (var item in m_Tiles)
        {
            item.GetComponent<Tile>().__Initialize();
        }
    }
    public override void __Finalize()
    {
        ClearTiles();

        base.__Finalize();
    }

    void CreateTiles()
    {
        for (int i = 0; i < M_Game.m_height; ++i)
        {
            for (int j = 0; j < M_Game.m_width; ++j)
            {
                CreateTile(j, i);
            }
        }
    }

    void CreateTile(int width, int height)
    {
        Tile temp = GetPool("Tile").Spawn();
        temp.transform.SetParent(m_Tile_parent.transform);
        temp.transform.localScale = Vector3.one;
        m_RectTransform = temp.GetComponent<RectTransform>();
        m_RectTransform.anchoredPosition3D = CalcPosition(m_RectTransform.pivot, width, height);

        m_Image = temp.GetComponent<Image>();

        switch (M_Stage.m_Stage[height, width])
        {
            case E_TileType.Empty:
                if ((width + height) % 2 == 0)
                {
                    m_Image.color = M_Game.m_EvenBG;
                }
                else
                {
                    m_Image.color = M_Game.m_OddBG;
                }
                break;
            case E_TileType.Wall:
                m_Image.color = M_Game.m_Wall;
                break;
            case E_TileType.SafetyZone:
                m_Image.color = M_Game.m_SafetyZone;
                break;
            default:
                Debug.LogError("스테이지 값 오류");
                break;
        }

        temp.gameObject.SetActive(true);
        m_Tiles.Add(temp);
    }
    Vector2 CalcPosition(Vector2 pivot, int width, int height)
    {
        Vector2 temp = new Vector2();
        temp.x = (width + pivot.x) * M_Game.m_PixelUnit;
        temp.y = (height + pivot.y) * M_Game.m_PixelUnit;
        return temp;
    }
    void ClearTiles()
    {
        for (int i = m_Tiles.Count - 1; i >= 0; --i)
        {
            GetPool("Tile").DeSpawn(m_Tiles[i]);
        }

        m_Tiles.Clear();
    }
}
