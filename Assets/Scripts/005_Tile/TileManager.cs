using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : ObjectManager<TileManager>
{
    RectTransform rc;

    Image image;
    GameObject Tile_parent;
    public List<GameObject> Tiles;

    __EditManager m_Edit;

    protected override void Awake()
    {
        base.Awake();

        ResourcesType = E_ResourcesType.Tile;

        m_Edit = __EditManager.Instance;
    }

    private void Start()
    {
        PoolSize = m_Game.width * m_Game.height;
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
        Tiles = new List<GameObject>();

        PoolSize = m_Game.width * m_Game.height;

        base.__Initialize();

        Tile_parent = new GameObject();
        Tile_parent.transform.SetParent(m_Edit.Canvas_BG.transform);
        rc = Tile_parent.AddComponent<RectTransform>();
        Tile_parent.layer = LayerMask.NameToLayer("UI");
        Tile_parent.name = "BackGrounds";

        rc.anchorMin = new Vector2(0, 0);
        rc.anchorMax = new Vector2(0, 0);
        Tile_parent.transform.localScale = Vector3.one;
        rc.sizeDelta = new Vector2(m_Game.PixelUnit, m_Game.PixelUnit);
        rc.anchoredPosition3D = CalcPosition(rc.pivot, 0, 0);

        CreateTiles();

        foreach (var item in Tiles)
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
        for (int i = 0; i < m_Game.height; ++i)
        {
            for (int j = 0; j < m_Game.width; ++j)
            {
                CreateTile(j, i);
            }
        }
    }

    void CreateTile(int width, int height)
    {
        GameObject temp = GetPool().Spawn();
        temp.transform.SetParent(Tile_parent.transform);
        temp.transform.localScale = Vector3.one;
        rc = temp.GetComponent<RectTransform>();
        rc.anchoredPosition3D = CalcPosition(rc.pivot, width, height);

        image = temp.GetComponent<Image>();

        switch (m_Resources.Stage[height, width])
        {
            case E_StageType.Empty:
                if ((width + height) % 2 == 0)
                {
                    image.color = m_Game.EvenBG;
                }
                else
                {
                    image.color = m_Game.OddBG;
                }
                break;
            case E_StageType.Wall:
                image.color = m_Game.Wall;
                break;
            case E_StageType.SafetyZone:
                image.color = m_Game.SafetyZone;
                break;
            default:
                Debug.LogError("스테이지 값 오류");
                break;
        }

        temp.SetActive(true);
        Tiles.Add(temp);
    }
    Vector2 CalcPosition(Vector2 pivot, int width, int height)
    {
        Vector2 temp = new Vector2();
        temp.x = (width + pivot.x) * m_Game.PixelUnit;
        temp.y = (height + pivot.y) * m_Game.PixelUnit;
        return temp;
    }
    void ClearTiles()
    {
        for (int i = Tiles.Count - 1; i >= 0; --i)
        {
            GetPool().DeSpawn(Tiles[i]);
        }

        Tiles.Clear();
    }
}
