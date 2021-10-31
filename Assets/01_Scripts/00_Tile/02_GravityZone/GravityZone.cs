using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityZone : MonoBehaviour, IClickerableObject, IEraserableTile
{
	protected Tile m_Tile;

	[SerializeField, ReadOnly]
	protected Vector2 m_Gravity;

	#region 내부 컴포넌트
	protected MyPhysics.BoxCollider2D m_Collider;
	#endregion
	#region 내부 프로퍼티
	#region 매니저
	protected GravityZoneManager M_GravityZone => GravityZoneManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public Tile tile { get => m_Tile; }
	public Vector2 gravity { get => m_Gravity; set => m_Gravity = value; }
	#endregion
	#region 외부 함수
	public void __Initialize(Tile tile)
	{
		m_Tile = tile;

		if (null == m_Collider)
		{
			m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
			m_Collider.center = (Vector2)transform.position + m_Collider.offset;
		}
	}
	#endregion
	#region 인터페이스 함수
	public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
	{
		if (currentType != E_ObjectType.GravityZone)
		{
			M_GravityZone.DespawnGravityZone(this);
		}
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.GravityZone;
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
