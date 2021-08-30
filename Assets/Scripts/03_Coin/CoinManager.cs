using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinManager : ObjectManager<CoinManager, Coin>
{
    public List<Coin> m_CoinList;
    public int m_CoinCount = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    public void SpawnCoin(Vector3 pos)
    {
        Coin Coin = GetPool("Coin").Spawn();
        Coin.transform.position = pos;
        Coin.gameObject.SetActive(true);
        m_CoinList.Add(Coin);
    }
    public void DespawnCoin(Coin coin)
    {
        m_CoinList.Remove(coin);
        GetPool("Coin").DeSpawn(coin);
    }

    public bool IsLeftedCoin()
    {
        return m_CoinCount > 0;
    }
    public void RespawnCoin()
    {
        int Count = m_CoinList.Count;

        //Coin coin;

        //for (int i = Count - 1; i >= 0; --i)
        //{
        //    if (!m_CoinList[i].activeSelf)
        //    {
        //        coin = GetPool().Spawn();
        //        coin.transform.position = m_CoinList[i].transform.position;
        //        coin.SetActive(true);

        //        GetPool().DeSpawn(m_CoinList[i]);
        //        m_CoinList.RemoveAt(i);

        //        m_CoinList.Insert(i, coin);
        //    }
        //}
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
        base.__Initialize();

        if (null == m_CoinList)
        {
            m_CoinList = new List<Coin>();
        }
    }
    public override void __Finalize()
    {
        ClearCoins();

        base.__Finalize();
    }

    void ClearCoins()
    {
        for (int i = m_CoinList.Count - 1; i >= 0; --i)
        {
            GetPool("Coin").DeSpawn(m_CoinList[i]);
        }

        m_CoinList.Clear();
    }
}
