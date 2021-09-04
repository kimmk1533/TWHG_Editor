﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinManager : ObjectManager<CoinManager, Coin>
{
    // 현재 게임에 남아있는 코인 갯수
    protected int m_CoinCount = 0;
    List<Coin> m_CoinList;

    #region 내부 프로퍼티
    #region 매니져
    protected PlayerManager M_Player => PlayerManager.Instance;
    #endregion
    #endregion
    #region 외부 프로퍼티
    public int CoinCount { get => m_CoinCount; set => m_CoinCount = value; }
    public bool IsLeftedCoin => m_CoinCount > 0;
    #endregion
    #region 내부 함수
    void ClearCoins()
    {
        // 모든 코인 디스폰
        for (int i = m_CoinList.Count - 1; i >= 0; --i)
        {
            GetPool("Coin").DeSpawn(m_CoinList[i]);
        }

        // 관리 리스트 초기화
        m_CoinList.Clear();
    }
    #endregion
    #region 외부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        #region 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        M_Player.OnPlayerRespawn += RespawnCoin;
        #endregion

        // 코인 풀 원본
        Coin coin = M_Resources.GetGameObject<Coin>("Coin", "Coin");
        // 코인 풀 생성
        AddPool("Coin", coin, transform);

        // 관리 리스트 초기화
        if (null == m_CoinList)
        {
            m_CoinList = new List<Coin>();
        }
    }
    public override void __Finalize()
    {
        // 모든 코인 삭제
        ClearCoins();

        base.__Finalize();
    }

    // 코인 스폰 함수
    public Coin SpawnCoin()
    {
        // 스폰
        Coin coin = GetPool("Coin").Spawn();
        // 초기화
        coin.__Initialize();
        // 활성화
        coin.gameObject.SetActive(true);
        // 관리 리스트에 추가
        m_CoinList.Add(coin);

        return coin;
    }
    // 코인 디스폰 함수
    public void DespawnCoin(Coin coin)
    {
        coin.__Finalize();
        // 관리 리스트에서 제거
        m_CoinList.Remove(coin);
        // 디스폰
        GetPool("Coin").DeSpawn(coin);
    }
    #endregion
    #region 이벤트 함수
    public override void OnPlayEnter()
    {
        m_CoinCount = m_CoinList.Count;
    }
    public override void OnPlayExit()
    {
        RespawnCoin();
        m_CoinCount = m_CoinList.Count;
    }
    public void RespawnCoin()
    {
        foreach (var item in m_CoinList)
        {
            if (!item.gameObject.activeSelf)
            {
                item.gameObject.SetActive(true);
            }
        }
    }
    #endregion
    #region 유니티 콜백 함수
    //protected override void Awake()
    //{
    //    base.Awake();
    //}
    #endregion
}
