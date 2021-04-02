using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : ObjectManager<CoinManager>
{
    public List<GameObject> Coins;

    protected override void Awake()
    {
        base.Awake();

        ResourcesType = E_ResourcesType.Coin;

        PoolSize = 100;
    }

    public void SpawnCoin(Vector3 pos)
    {
        GameObject Coin = GetPool().Spawn();
        Coin.transform.position = pos;
        Coin.SetActive(true);
        Coins.Add(Coin);
    }
    public void DespawnCoin(GameObject coin)
    {
        Coins.Remove(coin);
        GetPool().DeSpawn(coin);
    }

    public bool IsLeftedCoin()
    {
        for (int i = 0; i < Coins.Count; ++i)
        {
            if (Coins[i].activeSelf)
            {
                return true;
            }
        }

        return false;
    }
    public void RespawnCoin()
    {
        int Count = Coins.Count;

        GameObject Coin;

        for (int i = Count - 1; i >= 0; --i)
        {
            if (!Coins[i].activeSelf)
            {
                Coin = GetPool().Spawn();
                Coin.transform.position = Coins[i].transform.position;
                Coin.SetActive(true);

                GetPool().DeSpawn(Coins[i]);
                Coins.RemoveAt(i);

                Coins.Insert(i, Coin);
            }
        }
    }

    public override void OnPlayEnter()
    {
        
    }
    public override void OnPlayExit()
    {
        RespawnCoin();
    }

    public override void __Initialize()
    {
        Coins = new List<GameObject>();

        base.__Initialize();
    }
    public override void __Finalize()
    {
        ClearCoins();

        base.__Finalize();
    }

    void ClearCoins()
    {
        for (int i = Coins.Count - 1; i >= 0; --i)
        {
            GetPool().DeSpawn(Coins[i]);
        }

        Coins.Clear();
    }
}
