using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IClickerableObject, IEraserableObject
{
	[SerializeField]
	protected E_EnemyType m_Type;
	[SerializeField]
	protected float m_Speed;

	protected Vector3 m_InitPos;

	#region Linear
	[SerializeField, ReadOnly]
	protected List<EnemyGizmo> m_WayPointList;

	protected int m_WayPointCount;
	protected bool m_Revert;
	#endregion
	#region Circular
	[SerializeField, ReadOnly]
	protected EnemyGizmo m_Center;

	protected float m_Degree;
	#endregion

	#region 내부 컴포넌트
	protected SpriteRenderer m_Renderer;
	protected MyPhysics.CircleCollider2D m_Collider;
	#endregion
	#region 내부 프로퍼티
	#region 매니져
	protected __EditManager M_Edit => __EditManager.Instance;

	protected EnemyManager M_Enemy => EnemyManager.Instance;
	protected EnemyGizmoManager M_EnemyGizmo => EnemyGizmoManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public E_EnemyType type { get => m_Type; set => m_Type = value; }
	public float speed { get => m_Speed; set => m_Speed = value; }
	public List<EnemyGizmo> wayPointList { get => m_WayPointList; set => m_WayPointList = value; }
	public EnemyGizmo center { get => m_Center; set => m_Center = value; }
	#endregion
	#region 내부 함수
	protected void Move()
	{
		switch (m_Type)
		{
			case E_EnemyType.Linear:
				LinearMove();
				break;
			case E_EnemyType.LinearRepeat:
				LinearRepeatMove();
				break;
			case E_EnemyType.Circular:
				CircularMove();
				break;
		}
	}
	protected void LinearMove()
	{
		transform.position = Vector3.MoveTowards(
			transform.position,
			m_WayPointList[m_WayPointCount].transform.position,
			m_Speed * Time.deltaTime);

		if (CloseTarget(m_WayPointList[m_WayPointCount], 0.05f))
		{
			if (0 <= m_WayPointCount && m_WayPointCount <= m_WayPointList.Count - 1)
				if (!m_Revert)
				{
					if (m_WayPointCount < m_WayPointList.Count - 1)
					{
						++m_WayPointCount;
					}
					else
					{
						m_Revert = !m_Revert;
					}
				}
				else
				{
					if (m_WayPointCount > 0)
					{
						--m_WayPointCount;
					}
					else
					{
						m_Revert = !m_Revert;
					}
				}
		}
	}
	protected void LinearRepeatMove()
	{
		transform.position = Vector3.MoveTowards(
			transform.position,
			m_WayPointList[m_WayPointCount].transform.position,
			m_Speed * Time.deltaTime);

		if (CloseTarget(m_WayPointList[m_WayPointCount], 0.05f))
		{
			++m_WayPointCount;
			if (m_WayPointCount >= m_WayPointList.Count)
			{
				m_WayPointCount = 0;
			}
		}
	}
	protected void CircularMove()
	{
		m_Degree += m_Speed;

		float radius = Vector2.Distance(transform.position, m_Center.transform.position);
		float radian = m_Degree * Mathf.Deg2Rad;

		Vector3 newPos = new Vector3();
		newPos.x = radius * Mathf.Cos(radian);
		newPos.y = radius * Mathf.Sin(radian);
		transform.position = m_Center.transform.position + newPos;
	}

	protected bool CloseTarget(Vector3 targetPos, float distance)
	{
		return Vector3.Distance(targetPos, transform.position) <= distance;
	}
	protected bool CloseTarget(EnemyGizmo waypoint, float distance)
	{
		return CloseTarget(waypoint.transform.position, distance);
	}
	#endregion
	#region 외부 함수
	public void __Initialize()
	{
		if (null == m_Renderer)
		{
			m_Renderer = transform.Find("Renderer").GetComponent<SpriteRenderer>();
		}
		if (null == m_Collider)
		{
			m_Collider = GetComponent<MyPhysics.CircleCollider2D>();
		}

		if (null == m_WayPointList)
		{
			m_WayPointList = new List<EnemyGizmo>();
		}

		if (null == m_Center)
		{
			m_Center = M_EnemyGizmo.SpawnGizmo();
			m_Center.transform.position = Vector3.zero;
			m_Center.gameObject.SetActive(false);
		}
	}

	public void AddWayPoint()
	{
		AddWayPoint(Vector3.zero);
	}
	public void AddWayPoint(Vector3 pos)
	{
		EnemyGizmo wayPoint = M_EnemyGizmo.SpawnGizmo();
		wayPoint.transform.position = pos;

		wayPoint.index = m_WayPointList.Count;
		wayPoint.text.text = (wayPoint.index + 1).ToString();
		m_WayPointList.Add(wayPoint);
	}
	public void RemoveWayPoint(int index)
	{
		if (index < 0 || index >= m_WayPointList.Count)
			return;

		EnemyGizmo wayPoint = m_WayPointList[index];
		M_EnemyGizmo.DespawnGizmo(wayPoint);
		m_WayPointList.RemoveAt(index);

		for (int i = index; i < m_WayPointList.Count; ++i)
		{
			m_WayPointList[i].index = i;
			m_WayPointList[i].text.text = (i + 1).ToString();
		}
	}
	#endregion
	#region 이벤트 함수
	public void OnPlayModeEnter()
	{
		m_InitPos = transform.position;

		switch (m_Type)
		{
			case E_EnemyType.Linear:
			case E_EnemyType.LinearRepeat:
				m_WayPointCount = 0;
				m_Revert = false;
				break;
			case E_EnemyType.Circular:
				Vector3 v = transform.position - m_Center.transform.position;
				m_Degree = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
				break;
		}
	}
	public void OnPlayModeExit()
	{
		transform.position = m_InitPos;
	}
	#endregion
	#region 인터페이스 함수
	public void EraseObject()
	{
		for (int i = 0; i < wayPointList.Count; ++i)
		{
			M_EnemyGizmo.DespawnGizmo(wayPointList[i]);
		}

		M_Enemy.DespawnEnemy(this);
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.Enemy;
	}
	public GameObject GetGameObject()
	{
		return gameObject;
	}
	public Renderer GetRenderer()
	{
		return m_Renderer;
	}
	#endregion
	#region 유니티 콜백 함수
	void Update()
	{
		if (M_Edit.isPlayMode)
		{
			Move();
		}
	}
	#endregion
}
