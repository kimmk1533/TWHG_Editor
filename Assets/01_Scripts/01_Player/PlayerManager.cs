using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>, ISaveHandler, ILoadHandler
{
	protected Player m_Player;

	private event Action m_OnPlayerRespawn;

	#region 내부 프로퍼티
	#region 매니져
	protected __GameManager M_Game => __GameManager.Instance;
	protected ResourcesManager M_Resources => ResourcesManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public Vector3 playerPos
	{
		get => m_Player.transform.position;
		set => m_Player.transform.position = value;
	}
	public bool playerActive
	{
		get => m_Player.gameObject.activeSelf;
		set => m_Player.gameObject.SetActive(value);
	}
	public event Action onPlayerRespawn
	{
		add
		{
			lock (this)
			{
				m_OnPlayerRespawn += value;
			}
		}
		remove
		{
			lock (this)
			{
				m_OnPlayerRespawn -= value;
			}
		}
	}
	#endregion
	#region 외부 함수
	public void __Initialize()
	{
		#region 이벤트 링크
		M_Game.onPlayModeEnter += OnPlayModeEnter;
		M_Game.onPlayModeExit += OnPlayModeExit;
		#endregion

		if (null == m_Player)
		{
			Player Player_Prefab = M_Resources.GetGameObject<Player>("Player", "Player");

			m_Player = Player.Instantiate(Player_Prefab, transform);
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

	public void Save(XmlWriter writer)
	{
		// 주석
		writer.WriteComment("플레이어");
		// 플레이어 시작
		writer.WriteStartElement("Player");

		#region 위치
		// 플레이어 위치 시작
		writer.WriteStartElement("Position");
		// 플레이어 위치 입력
		writer.WriteVector(playerPos);
		// 플레이어 위치 끝
		writer.WriteEndElement();
		#endregion
		#region 활성화
		// 플레이어 활성화 시작
		writer.WriteStartElement("Active");
		// 플레이어 활성화 입력
		writer.WriteValue(playerActive);
		// 플레이어 활성화 끝
		writer.WriteEndElement();
		#endregion

		// 플레이어 끝
		writer.WriteEndElement();
	}
	public void Load(XmlReader reader)
	{
		if (reader.LoadToElement("Player"))
		{
			if (reader.LoadToElement("Position"))
			{
				playerPos = reader.ReadVector("Position");
			}
			if (reader.LoadToElement("Active"))
			{
				bool active;

				reader.ReadStartElement("Active");
				if (!bool.TryParse(reader.Value, out active))
				{
					active = false;
				}

				playerActive = active;
			}
		}
	}
	#endregion
	#region 이벤트 함수
	private void OnPlayModeEnter()
	{
		m_Player.OnPlayModeEnter();
	}
	private void OnPlayModeExit()
	{
		m_Player.OnPlayModeExit();
	}

	public void RespawnPlayer()
	{
		m_OnPlayerRespawn?.Invoke();
	}
	#endregion
}
