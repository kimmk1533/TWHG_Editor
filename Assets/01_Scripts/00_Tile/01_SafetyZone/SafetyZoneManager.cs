using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class SafetyZoneManager : ObjectManager<SafetyZoneManager, SafetyZone>, ISaveHandler, ILoadHandler
{
	protected int m_SafetyZoneIndex;

	[SerializeField, ReadOnly]
	protected List<SafetyZone> m_SafetyZoneList;
	[SerializeField, ReadOnly]
	protected List<SafetyZone> m_FinishZoneList;

	#region 내부 프로퍼티
	#region 매니져
	protected TileManager M_Tile => TileManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public int safetyZoneIndex { get => m_SafetyZoneIndex; }
	public int finishZoneCount { get => m_FinishZoneList.Count; }
	#endregion
	#region 내부 함수
	protected void ClearSafetyZone()
	{
		for (int i = 0; i < m_SafetyZoneList.Count; i++)
		{
			GetPool("SafetyZone").DeSpawn(m_SafetyZoneList[i]);
		}
		m_SafetyZoneList.Clear();
		m_SafetyZoneIndex = 0;
	}
	#endregion
	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		#region 이벤트 링크
		M_Game.onPlayModeEnter += OnPlayModeEnter;
		M_Game.onPlayModeExit += OnPlayModeExit;
		#endregion

		m_SafetyZoneIndex = 0;

		// 풀 사이즈 설정
		m_PoolSize = M_Game.width * M_Game.height;

		// 안전구역 풀 원본
		SafetyZone safetyZone = M_Resources.GetGameObject<SafetyZone>("Tile", "SafetyZone");
		// 안전구역 풀 생성
		AddPool("SafetyZone", safetyZone, transform);

		// 관리 리스트 초기화
		if (null == m_SafetyZoneList)
		{
			m_SafetyZoneList = new List<SafetyZone>();
		}
		if (null == m_FinishZoneList)
		{
			m_FinishZoneList = new List<SafetyZone>();
		}
	}
	public override void __Finalize()
	{
		base.__Finalize();

		//ClearSafetyZone();
	}

	public SafetyZone SpawnSafetyZone()
	{
		SafetyZone safetyZone = GetPool("SafetyZone").Spawn();
		m_SafetyZoneList.Add(safetyZone);
		++m_SafetyZoneIndex;
		M_Edit.AddSafetyZoneOption(m_SafetyZoneIndex);
		return safetyZone;
	}
	public void DespawnSafetyZone(SafetyZone safetyZone)
	{
		M_Edit.RemoveSafetyZoneOption(safetyZone.safetyZoneCount);
		--m_SafetyZoneIndex;
		m_SafetyZoneList.Remove(safetyZone);
		if (safetyZone.isFinishZone)
		{
			m_FinishZoneList.Remove(safetyZone);
		}
		GetPool("SafetyZone").DeSpawn(safetyZone);

		for (int i = 0; i < m_SafetyZoneList.Count; ++i)
		{
			m_SafetyZoneList[i].UpdateText(safetyZone.safetyZoneCount);
		}
	}

	public void ToggleFinishZone(int index)
	{
		if (index < 0 || index >= m_SafetyZoneList.Count)
			return;

		m_SafetyZoneList[index].isFinishZone = !m_SafetyZoneList[index].isFinishZone;

		if (m_SafetyZoneList[index].isFinishZone)
		{
			m_FinishZoneList.Add(m_SafetyZoneList[index]);
		}
		else
		{
			m_FinishZoneList.Remove(m_SafetyZoneList[index]);
		}
	}
	#endregion
	#region 이벤트 함수
	private void OnPlayModeEnter()
	{
		foreach (var item in m_SafetyZoneList)
		{
			item.OnPlayModeEnter();
		}
	}
	private void OnPlayModeExit()
	{
		foreach (var item in m_SafetyZoneList)
		{
			item.OnPlayModeExit();
		}
	}
	#endregion
	#region 인터페이스 함수
	public void SaveData(XmlWriter writer)
	{
		// 주석
		writer.WriteComment("안전구역");
		// 안전구역 리스트 시작
		writer.WriteStartElement("SafetyZoneList");

		#region 갯수
		// 안전구역 갯수 시작
		writer.WriteStartAttribute("Count");
		// 안전구역 갯수 입력
		writer.WriteValue(m_SafetyZoneList.Count);
		// 안전구역 갯수 끝
		writer.WriteEndAttribute();
		#endregion
		#region 안전구역
		foreach (var safetyZone in m_SafetyZoneList)
		{
			// 안전구역 시작
			writer.WriteStartElement("SafetyZone");

			#region 인덱스
			// 안전구역 인덱스 시작
			writer.WriteStartElement("Index");
			// 안전구역 인덱스 입력
			writer.WriteValue(safetyZone.tile.index);
			// 안전구역 인덱스 끝
			writer.WriteEndElement();
			#endregion
			#region 종료구역 여부
			// 안전구역 종료구역 시작
			writer.WriteStartElement("IsFinishZone");
			// 안전구역 종료구역 입력
			writer.WriteValue(safetyZone.isFinishZone);
			// 안전구역 종료구역 끝
			writer.WriteEndElement();
			#endregion

			// 안전구역 끝
			writer.WriteEndElement();
		}
		#endregion

		// 안전구역 리스트 끝
		writer.WriteEndElement();
	}
	public void LoadData(XmlReader reader)
	{
		if (reader.LoadToElement("SafetyZoneList"))
		{
			int count;
			string count_str = reader.GetAttribute("Count");
			if (!int.TryParse(count_str, out count))
			{
				count = 0;
			}

			for (int i = 0; i < count; ++i)
			{
				if (reader.LoadToElement("SafetyZone"))
				{
					int index = 0;

					if (reader.LoadToElement("Index"))
					{
						reader.ReadStartElement("Index");
						if (!int.TryParse(reader.Value, out index))
						{
							index = -1;
						}
						if (index < 0 || index >= M_Tile.tileList.Count)
						{
							Debug.LogError("SafetyZone 불러오기 중 타일 인덱스 오류");
							continue;
						}
					}

					Tile tile = M_Tile.tileList[index];
					tile.SetType(E_TileType.SafetyZone);

					Vector3 spawnPoint = (Vector2)tile.transform.position;

					bool isFinishZone = false;
					if (reader.LoadToElement("IsFinishZone"))
					{
						reader.ReadStartElement("IsFinishZone");
						if (!bool.TryParse(reader.Value, out isFinishZone))
						{
							isFinishZone = false;
						}
					}

					// 스폰
					SafetyZone safetyZone = SpawnSafetyZone();
					// 위치 설정
					safetyZone.transform.position = spawnPoint;
					// 초기화
					safetyZone.__Initialize(tile);

					M_Edit.safetyZoneFinishZone.items[i].toggle.isOn =
					M_Edit.safetyZoneFinishZone.options[i].isOn =
					safetyZone.isFinishZone = isFinishZone;

					if (isFinishZone)
					{
						m_FinishZoneList.Add(safetyZone);
					}

					// 활성화
					safetyZone.gameObject.SetActive(true);
				}
			}
		}
	}
	#endregion
}