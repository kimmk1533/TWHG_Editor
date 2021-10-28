using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileManager : ObjectManager<TileManager, Tile>
{
	// 생성한 타일(BG) 리스트
	protected List<Tile> m_TileList;

	// 타일(BG) 부모
	[SerializeField, ReadOnly(true)]
	protected GameObject m_TileParent;
	// 정렬 컴포넌트
	protected GridLayoutGroup m_GridLayoutGroup;

	#region 타일 컬러
	[Header("TileColors")]
	[SerializeField, ReadOnly(true)]
	protected Color m_OddColor; // 홀수 칸
	[SerializeField, ReadOnly(true)]
	protected Color m_EvenColor; // 짝수 칸
	[SerializeField, ReadOnly(true)]
	protected Color m_WallColor;
	[SerializeField, ReadOnly(true)]
	protected Color m_SafetyZoneColor;
	[SerializeField, ReadOnly(true)]
	protected Color m_GravityZoneColor;
	[SerializeField, ReadOnly(true)]
	protected Color m_IceZoneColor;
	#endregion

	#region 내부 프로퍼티
	#region 매니져
	protected StageManager M_Stage => StageManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public List<Tile> tileList { get => m_TileList; }

	#region 타일 컬러
	public Color oddColor { get => m_OddColor; set => m_OddColor = value; }
	public Color evenColor { get => m_EvenColor; set => m_EvenColor = value; }
	public Color wallColor { get => m_WallColor; set => m_WallColor = value; }
	public Color safetyZoneColor { get => m_SafetyZoneColor; set => m_SafetyZoneColor = value; }
	public Color gravityZoneColor { get => m_GravityZoneColor; set => m_GravityZoneColor = value; }
	public Color iceZoneColor { get => m_IceZoneColor; set => m_IceZoneColor = value; }
	#endregion
	#endregion
	#region 내부 함수
	// 타일(BG) 전체 생성 함수
	void CreateTiles()
	{
		for (int y = 0; y < M_Game.height; ++y)
		{
			for (int x = 0; x < M_Game.width; ++x)
			{
				CreateTile(x, y);
			}
		}
	}
	// 타일(BG) 하나 생성 함수
	void CreateTile(int x, int y)
	{
		// 타일(BG) 스폰
		Tile tile = GetPool("Tile").Spawn();

		// 타일(BG) 설정
		tile.__Initialize(x, y);
		tile.transform.SetParent(m_TileParent.transform);
		tile.transform.localScale = Vector3.one;
		tile.transform.localPosition = Vector3.zero;
		tile.SetColor(M_Stage.stage[y, x]);
		tile.gameObject.SetActive(true);

		// 관리 리스트에 추가
		m_TileList.Add(tile);
	}
	// 타일(BG) 전체 삭제 함수
	void ClearTiles()
	{
		for (int i = m_TileList.Count - 1; i >= 0; --i)
		{
			GetPool("Tile").DeSpawn(m_TileList[i]);
		}

		m_TileList.Clear();
	}
	#endregion
	#region 외부 함수
	public override void __Initialize()
	{
		base.__Initialize();

		// 이벤트 링크
		M_Game.onPlayModeEnter += OnEnterPlayMode;
		M_Game.onPlayModeExit += OnExitPlayMode;

		// 관리 리스트 초기화
		if (null == m_TileList)
		{
			m_TileList = new List<Tile>();
		}

		// 정렬 설정
		if (null == m_GridLayoutGroup)
		{
			m_GridLayoutGroup = m_TileParent.GetComponent<GridLayoutGroup>();
		}
		if (m_GridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
			m_GridLayoutGroup.constraintCount = M_Game.width;
		else if (m_GridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
			m_GridLayoutGroup.constraintCount = M_Game.height;

		// 풀 사이즈 설정
		m_PoolSize = M_Game.width * M_Game.height;

		// 타일(BG) 풀 원본
		Tile tile = M_Resources.GetGameObject<Tile>("Tile", "Tile");
		// 타일(BG) 풀 생성
		AddPool("Tile", tile, transform);

		// 타일(BG) 생성
		CreateTiles();
	}
	public override void __Finalize()
	{
		// 타일(BG) 삭제
		ClearTiles();

		base.__Finalize();
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