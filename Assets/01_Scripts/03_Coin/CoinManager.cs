using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class CoinManager : ObjectManager<CoinManager, Coin>, ISaveHandler, ILoadHandler
{
	// 관리 리스트
	protected List<Coin> m_CoinList;
	// 현재 게임에 남아있는 코인 갯수
	[SerializeField, ReadOnly]
	protected int m_CoinCount = 0;

	#region 내부 프로퍼티
	#region 매니져
	protected PlayerManager M_Player => PlayerManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public int CoinCount { get => m_CoinCount; set => m_CoinCount = value; }
	public bool IsLeftCoin => m_CoinCount > 0;
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
		M_Game.onPlayModeEnter += OnPlayModeEnter;
		M_Game.onPlayModeExit += OnPlayModeExit;

		M_Player.onPlayerRespawn += RespawnCoin;
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
	private void OnPlayModeEnter()
	{
		m_CoinCount = m_CoinList.Count;

		foreach (var item in m_CoinList)
		{
			item.OnPlayModeEnter();
		}
	}
	private void OnPlayModeExit()
	{
		foreach (var item in m_CoinList)
		{
			item.OnPlayModeExit();
		}

		RespawnCoin();
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

		m_CoinCount = m_CoinList.Count;
	}
	#endregion
	#region 인터페이스 함수
	public void Save(XmlWriter writer)
	{
		// 주석
		writer.WriteComment("코인");
		// 코인 리스트 시작
		writer.WriteStartElement("CoinList");

		#region 갯수
		// 코인 갯수 시작
		writer.WriteStartAttribute("Count");
		// 코인 갯수 입력
		writer.WriteValue(m_CoinList.Count);
		// 코인 갯수 끝
		writer.WriteEndAttribute();
		#endregion
		#region 코인
		foreach (var coin in m_CoinList)
		{
			// 코인 시작
			writer.WriteStartElement("Coin");

			#region 위치
			// 코인 위치 시작
			writer.WriteStartElement("Position");
			// 코인 위치 입력
			writer.WriteVector(coin.transform.position);
			// 코인 위치 끝
			writer.WriteEndElement();
			#endregion

			// 코인 끝
			writer.WriteEndElement();
		}
		#endregion

		// 코인 리스트 끝
		writer.WriteEndElement();
	}
	public void Load(XmlReader reader)
	{
		if (reader.LoadToElement("CoinList"))
		{
			int count;
			string count_str = reader.GetAttribute("Count");
			if (!int.TryParse(count_str, out count))
			{
				count = 0;
			}

			for (int i = 0; i < count; ++i)
			{
				Coin coin = SpawnCoin();

				if (reader.LoadToElement("Coin"))
				{
					if (reader.LoadToElement("Position"))
					{
						coin.transform.position = reader.ReadVector("Position");
					}
				}
			}
		}
	}
	#endregion
}
