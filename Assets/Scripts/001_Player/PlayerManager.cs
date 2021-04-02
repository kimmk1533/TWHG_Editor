using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public GameObject Player;

    public Vector2 InitPos;

    ResourcesManager m_Resources;
    SafetyZoneManager m_SafetyZone;

    private void Awake()
    {
        m_Resources = ResourcesManager.Instance;
        m_SafetyZone = SafetyZoneManager.Instance;
    }

    public void OnPlayEnter()
    {
        Player.SetActive(true);

        InitPos = m_SafetyZone.StartPoint.GetCenter();

        Player.transform.position = InitPos;
    }
    public void OnPlayExit()
    {
        Player.transform.position = InitPos;
    }

    public void __Initialize()
    {
        GameObject[] Player_Prefabs = m_Resources.GetPrefabs(E_ResourcesType.Player);
        foreach (var item in Player_Prefabs)
        {
            if (item.name == "Player")
            {
                Player = GameObject.Instantiate(item);
                Player.transform.parent = this.transform;
                Player.SetActive(false);
            }
        }
    }
    public void __Finalize()
    {

    }
}
