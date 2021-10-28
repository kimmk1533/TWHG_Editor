using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class IceZoneManager : ObjectManager<IceZoneManager, IceZone>, ISaveHandler, ILoadHandler
{
	[SerializeField]
	protected List<IceZone> m_IceZoneList;

	#region 내부 컴포넌트
	#region 매니저
	protected TileManager M_Tile => TileManager.Instance;
	#endregion
	#endregion
	#region 내부 프로퍼티
	#endregion
	#region 외부 프로퍼티
	#endregion
	#region 내부 함수
	#endregion
	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		M_Game.onPlayModeEnter += OnEnterPlayMode;
		M_Game.onPlayModeExit += OnExitPlayMode;

		// 풀 사이즈 설정
		m_PoolSize = M_Game.width * M_Game.height;

		// 얼음구역 풀 원본
		IceZone iceZone = M_Resources.GetGameObject<IceZone>("Tile", "IceZone");
		// 얼음구역 풀 생성
		AddPool("IceZone", iceZone, transform);

		// 관리 리스트 초기화
		if (null == m_IceZoneList)
		{
			m_IceZoneList = new List<IceZone>();
		}
	}
	public override void __Finalize()
	{
		base.__Finalize();

	}

	public IceZone SpawnIceZone()
	{
		IceZone iceZone = GetPool("IceZone").Spawn();
		m_IceZoneList.Add(iceZone);
		return iceZone;
	}
	public void DespawnIceZone(IceZone iceZone)
	{
		m_IceZoneList.Remove(iceZone);
		GetPool("IceZone").DeSpawn(iceZone);
	}

	public void Save(XmlWriter writer)
	{
		// 주석
		writer.WriteComment("얼음구역");
		// 얼음구역 리스트 시작
		writer.WriteStartElement("IceZoneList");

		#region 갯수
		// 얼음구역 갯수 시작
		writer.WriteStartAttribute("Count");
		// 얼음구역 갯수 입력
		writer.WriteValue(m_IceZoneList.Count);
		// 얼음구역 갯수 끝
		writer.WriteEndAttribute();
		#endregion
		#region 얼음구역
		foreach (var iceZone in m_IceZoneList)
		{
			// 얼음구역 시작
			writer.WriteStartElement("IceZone");

			#region 인덱스
			// 얼음구역 인덱스 시작
			writer.WriteStartElement("Index");
			// 얼음구역 인덱스 입력
			writer.WriteValue(iceZone.tile.index);
			// 얼음구역 인덱스 끝
			writer.WriteEndElement();
			#endregion
			#region 저항
			// 얼음구역 저항 시작
			writer.WriteStartElement("Drag");
			// 얼음구역 저항 입력
			writer.WriteValue(iceZone.drag);
			// 얼음구역 저항 끝
			writer.WriteEndElement();
			#endregion

			// 얼음구역 끝
			writer.WriteEndElement();
		}
		#endregion

		// 얼음구역 리스트 끝
		writer.WriteEndElement();
	}
	public void Load(XmlReader reader)
	{
		if (reader.LoadToElement("IceZoneList"))
		{
			int count;
			string count_str = reader.GetAttribute("Count");
			if (!int.TryParse(count_str, out count))
			{
				count = 0;
			}

			for (int i = 0; i < count; ++i)
			{
				if (reader.LoadToElement("IceZone"))
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
							Debug.LogError("IceZone 불러오기 중 타일 인덱스 오류");
							continue;
						}
					}

					Tile tile = M_Tile.tileList[index];
					tile.SetType(E_TileType.IceZone);

					Vector3 spawnPoint = (Vector2)tile.transform.position;

					float drag = 0f;
					if (reader.LoadToElement("Drag"))
					{
						reader.ReadStartElement("Drag");
						if (!float.TryParse(reader.Value, out drag))
						{
							drag = 0f;
						}
					}

					// 스폰
					IceZone iceZone = SpawnIceZone();
					// 위치 설정
					iceZone.transform.position = spawnPoint;
					// 초기화
					iceZone.__Initialize(tile);
					// 저항 설정
					iceZone.drag = drag;
					// 활성화
					iceZone.gameObject.SetActive(true);
				}
			}
		}
	}
	#endregion
	#region 이벤트 함수
	public void OnEnterPlayMode()
	{

	}
	public void OnExitPlayMode()
	{

	}
	#endregion
}
