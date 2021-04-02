using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ResourcesType
{
    Player,
    Enemy,
    Coin,
    Wall,
    Tile,
    UI,

    Max
}

public enum E_StageType
{
    Empty = 0,
    Wall = 1,
    SafetyZone = 2,
}

public class ResourcesManager : Singleton<ResourcesManager>
{
    public E_StageType[,] Stage;
    Dictionary<E_ResourcesType, GameObject[]> Prefabs = null;
    Dictionary<E_ResourcesType, Sprite[]> Sprites = null;

    __GameManager m_Game;

    protected void Awake()
    {
        m_Game = __GameManager.Instance;
    }

    public void OnPlayEnter()
    {
        //Stage = LoadStage();
    }
    public void OnPlayExit()
    {
        //ResetStage();
    }

    public void __Initialize()
    {
        Prefabs = new Dictionary<E_ResourcesType, GameObject[]>();
        Sprites = new Dictionary<E_ResourcesType, Sprite[]>();
        Stage = new E_StageType[m_Game.height, m_Game.width];

        Prefabs = LoadAll<GameObject>();
        Sprites = LoadAll<Sprite>();
        ResetStage();
    }
    public void __Finalize()
    {

    }

    public GameObject[] GetPrefabs(E_ResourcesType type)
    {
        return Prefabs[type];
    }
    public Sprite[] GetSprites(E_ResourcesType type)
    {
        return Sprites[type];
    }

    void ResetStage()
    {
        for (int i = 0; i < Stage.GetLength(0); ++i)
        {
            for (int j = 0; j < Stage.GetLength(1); ++j)
            {
                Stage[i, j] = E_StageType.Empty;
            }
        }
    }
    E_StageType[,] LoadStage()
    {
        E_StageType[,] result = new E_StageType[m_Game.height, m_Game.width];

        TextAsset[] texts = Resources.LoadAll<TextAsset>(PathManager.Instance.Stage);

        for (int i = 0; i < texts.Length; ++i)
        {
            if (texts[i].name == "Stage_Info")
            {
                string[] text = texts[i].text.Split('\n');

                for (int j = 0; j < m_Game.height; ++j)
                {
                    for (int k = 0; k < m_Game.width; ++k)
                    {
                        result[(int)m_Game.height - 1 - j, k] = (E_StageType)int.Parse(text[j].Split(' ')[k]);
                    }
                }
            }
        }
        return result;
    }
    Dictionary<E_ResourcesType, T[]> LoadAll<T>() where T : Object
    {
        Dictionary<E_ResourcesType, T[]> result = new Dictionary<E_ResourcesType, T[]>();

        for (E_ResourcesType i = 0; i < E_ResourcesType.Max; ++i)
        {
            result[i] = Resources.LoadAll<T>(PathManager.Instance.Paths[(int)i]);
        }

        return result;
    }
}