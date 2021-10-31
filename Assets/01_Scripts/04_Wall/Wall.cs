using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static WallManager;

public class Wall : MonoBehaviour, IClickerableObject, IEraserableTile
{
	protected Tile m_Tile;
	[SerializeField, ReadOnly]
	protected Wall[] m_Walls;

	#region 내부 컴포넌트
	protected LineRenderer[] m_Lines;
	protected MyPhysics.BoxCollider2D m_Collider;
	#endregion
	#region 내부 프로퍼티
	#region 매니져
	protected __EditManager M_Edit => __EditManager.Instance;

	protected WallManager M_Wall => WallManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public Tile tile => m_Tile;
	#endregion
	#region 내부 함수
	private void OnTrigger2DEnter(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
		{
			collider.GetComponent<IEraserableObject>()?.EraseObject();
		}
	}
	#endregion
	#region 외부 함수
	public void __Initialize(Tile tile)
	{
		m_Tile = tile;

		m_Walls = new Wall[(int)E_WallDirection.Max];
		m_Lines = new LineRenderer[(int)E_WallDirection.Max];

		if (null == m_Collider)
		{
			m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
			m_Collider.onTriggerEnter2D += OnTrigger2DEnter;
			m_Collider.center = (Vector2)transform.position + m_Collider.offset;
		}

		Vector2 extends = m_Collider.bounds.extents;

		int layerMask = LayerMask.GetMask("Wall");
		MyPhysics.RaycastHit2D[] hits = new MyPhysics.RaycastHit2D[(int)E_WallDirection.Max];
		hits[0] = MyPhysics.Physics2D.Raycast(transform.position - Vector3.right, Vector2.left, 0.1f, layerMask);
		hits[1] = MyPhysics.Physics2D.Raycast(transform.position + Vector3.right, Vector2.right, 0.1f, layerMask);
		hits[2] = MyPhysics.Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, 0.1f, layerMask);
		hits[3] = MyPhysics.Physics2D.Raycast(transform.position - Vector3.up, Vector2.down, 0.1f, layerMask);

		for (E_WallDirection direction = 0; direction < E_WallDirection.Max; ++direction)
		{
			int i = (int)direction;

			if (null == m_Lines[i])
			{
				m_Lines[i] = transform.Find(direction.ToString()).GetComponent<LineRenderer>();
			}

			if (hits[i])
			{
				m_Walls[i] = hits[i].collider.GetComponent<Wall>();

				if (null != m_Walls[i])
				{
					int index = (int)(direction + 1 - (int)direction % 2 * 2);
					m_Walls[i].m_Walls[index] = this;

					m_Lines[i].gameObject.SetActive(false);
					m_Walls[i].m_Lines[index].gameObject.SetActive(false);
				}
			}
		}
	}
	#endregion
	#region 이벤트 함수
	public void OnPlayModeEnter()
	{

	}
	public void OnPlayModeExit()
	{

	}
	#endregion
	#region 인터페이스 함수
	public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
	{
		if (currentType != E_ObjectType.Wall)
		{
			for (E_WallDirection i = 0; i < E_WallDirection.Max; ++i)
			{
				int index = (int)(i + 1 - (int)i % 2 * 2);

				m_Lines[(int)i].gameObject.SetActive(true);

				if (null != m_Walls[(int)i])
				{
					m_Walls[(int)i].m_Walls[index] = null;
					m_Walls[(int)i].m_Lines[index].gameObject.SetActive(true);
				}
			}

			M_Wall.DespawnWall(this);
		}
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.Wall;
	}
	public GameObject GetGameObject()
	{
		return gameObject;
	}
	public Renderer GetRenderer()
	{
		return null;
	}
	#endregion
}
