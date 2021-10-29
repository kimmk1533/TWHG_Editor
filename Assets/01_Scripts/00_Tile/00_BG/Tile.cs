using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
	[SerializeField]
	protected E_TileType m_Type;
	[SerializeField]
	protected E_TileIndexType m_IndexType;
	protected Image m_Image;

	protected int m_X;
	protected int m_Y;

	#region 내부 프로퍼티
	#region 매니져
	protected __GameManager M_Game => __GameManager.Instance;
	protected StageManager M_Stage => StageManager.Instance;

	protected TileManager M_Tile => TileManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public int index { get => m_X + (M_Game.width * m_Y); }
	public E_TileType type => m_Type;
	public E_TileIndexType indexType => m_IndexType;
	public Color color { get => m_Image.color; set => m_Image.color = value; }
	#endregion
	#region 외부 함수
	public void __Initialize(int x, int y)
	{
		if (null == m_Image)
		{
			m_Image = GetComponent<Image>();
		}

		m_X = x;
		m_Y = y;
		int index = (x + y) % 2;

		// 짝수 칸
		if (index == 0)
			m_IndexType = E_TileIndexType.Even;
		else
			m_IndexType = E_TileIndexType.Odd;
	}

	public void SetColor(E_TileType type)
	{
		switch (type)
		{
			case E_TileType.None:
				switch (m_IndexType)
				{
					case E_TileIndexType.Odd:
						m_Image.color = M_Tile.oddColor;
						break;
					case E_TileIndexType.Even:
						m_Image.color = M_Tile.evenColor;
						break;
					default:
						Debug.LogError("타일 홀짝 오류");
						break;
				}
				break;
			case E_TileType.Wall:
				m_Image.color = M_Tile.wallColor;
				break;
			case E_TileType.SafetyZone:
				m_Image.color = M_Tile.safetyZoneColor;
				break;
			case E_TileType.GravityZone:
				m_Image.color = M_Tile.gravityZoneColor;
				break;
			case E_TileType.IceZone:
				m_Image.color = M_Tile.iceZoneColor;
				break;
		}
	}
	public bool SetType(E_TileType type)
	{
		if (m_Type == type)
			return false;

		M_Stage.stage[m_Y, m_X] = m_Type = type;

		SetColor(type);

		return true;
	}
	#endregion
}