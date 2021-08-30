using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public Player m_Player;

    public Vector2 m_InitPos;

    ResourcesManager M_Resources;
    SafetyZoneManager M_SafetyZone;

    private void Awake()
    {
        M_Resources = ResourcesManager.Instance;
        M_SafetyZone = SafetyZoneManager.Instance;
    }

    public void OnPlayEnter()
    {
        m_Player.gameObject.SetActive(true);

        m_InitPos = M_SafetyZone.m_StartPoint.GetCenter();

        m_Player.transform.position = m_InitPos;
    }
    public void OnPlayExit()
    {
        m_Player.transform.position = m_InitPos;
    }

    public void __Initialize()
    {
        Player Player_Prefab = M_Resources.GetGameObject<Player>("Player", "Player");

        m_Player = Player.Instantiate(Player_Prefab);
        m_Player.transform.parent = this.transform;
        m_Player.gameObject.SetActive(false);
    }
    public void __Finalize()
    {

    }
}
