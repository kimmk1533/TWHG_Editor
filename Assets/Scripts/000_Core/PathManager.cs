using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : Singleton<PathManager>
{
    public readonly string Stage = "000_Stage/";

    public readonly string Player = "001_Player/";
    public readonly string Enemy = "002_Enemy/";
    public readonly string Coin = "003_Coin/";
    public readonly string Wall = "004_Wall/";
    public readonly string Tile = "005_Tile/";
    public readonly string UI = "006_UI/";

    [HideInInspector]
    public List<string> Paths;

    private void Awake()
    {

    }

    void AddPath(string path)
    {
        if (path == null)
            Debug.LogError(path + "경로 없음");
        Paths.Add(path);
    }

    public void OnPlayEnter()
    {

    }
    public void OnPlayExit()
    {

    }

    public void __Initialize()
    {
        Paths = new List<string>();

        AddPath(Player);
        AddPath(Enemy);
        AddPath(Coin);
        AddPath(Wall);
        AddPath(Tile);
        AddPath(UI);
    }
    public void __Finalize()
    {

    }
}
