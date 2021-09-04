﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    protected Player m_Player;

    public event Action OnPlayerRespawn;

    #region 내부 프로퍼티
    #region 매니져
    protected __GameManager M_Game => __GameManager.Instance;
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion

    protected Vector3 PlayerPos
    {
        get => m_Player.transform.position;
        set => m_Player.transform.position = value;
    }
    #endregion
    #region 외부 프로퍼티
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
        #region 이벤트 링크
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;
        #endregion

        if (null == m_Player)
        {
            Player Player_Prefab = M_Resources.GetGameObject<Player>("Player", "Player");

            m_Player = Player.Instantiate(Player_Prefab);
            m_Player.transform.SetParent(this.transform);
            m_Player.name = "Player";
            m_Player.__Initialize();
        }
    }
    public void __Finalize()
    {

    }

    public void SpawnPlayer(Vector3 pos)
    {
        PlayerPos = pos;
        m_Player.gameObject.SetActive(true);
    }
    #endregion
    #region 이벤트 함수
    public void OnPlayEnter()
    {
        m_Player.SetSpawnPos(M_SafetyZone.m_StartPoint.GetCenter());
    }
    public void OnPlayExit()
    {

    }
    public void RespawnPlayer()
    {
        OnPlayerRespawn?.Invoke();
    }
    #endregion
}
