using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    protected Player m_Player;

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
    public bool isSafe { get => m_Player.isSafe; set => m_Player.isSafe = value; }
    #endregion
    #region 외부 함수
    public void __Initialize()
    {
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

    public Player SpawnPlayer()
    {
        m_Player.gameObject.SetActive(true);
        return m_Player;
    }
    #endregion
    #region 이벤트 함수
    public event Action OnPlayerRespawn;

    public void RespawnPlayer()
    {
        OnPlayerRespawn?.Invoke();
    }
    #endregion
}
