using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZone : MonoBehaviour, IClickerableObject, IEraserableTile
{
	protected Tile m_Tile;
	protected int m_SafetyZoneCount;
	[SerializeField, ReadOnly]
	protected bool m_IsFinishZone;

	#region 내부 컴포넌트
	protected SafetyZoneAnimator m_Animator;
	protected MyPhysics.BoxCollider2D m_Collider;
	#endregion
	#region 내부 프로퍼티
	#region 매니저
	protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
	#endregion
	#endregion
	#region 외부 프로퍼티
	public Tile tile => m_Tile;

	public int safetyZoneCount => m_SafetyZoneCount;
	public bool isFinishZone
	{
		get => m_IsFinishZone;
		set => m_IsFinishZone = value;
	}
	#endregion
	#region 외부 함수
	public void __Initialize(Tile tile)
	{
		m_Tile = tile;

		m_SafetyZoneCount = M_SafetyZone.safetyZoneIndex;

		if (null == m_Animator)
		{
			m_Animator = transform.Find("Renderer").GetComponent<SafetyZoneAnimator>();
			m_Animator.__Initialize(this);
		}
		if (null == m_Collider)
		{
			m_Collider = transform.GetComponent<MyPhysics.BoxCollider2D>();
		}
	}

	public void UpdateText(int standard)
	{
		if (m_SafetyZoneCount < standard)
			return;

		--m_SafetyZoneCount;
		m_Animator.text.text = m_SafetyZoneCount.ToString();
	}
	#endregion
	#region 이벤트 함수
	public void OnPlayModeEnter()
	{
		m_Animator.OnPlayModeEnter();
	}
	public void OnPlayModeExit()
	{
		m_Animator.OnPlayModeExit();
	}
	#endregion
	#region 인터페이스 함수
	public void EraseTile(E_ObjectType currentType = E_ObjectType.None)
	{
		if (currentType != E_ObjectType.SafetyZone)
		{
			M_SafetyZone.DespawnSafetyZone(this);
		}
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.SafetyZone;
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

	#region 기존 함수
	/*public Vector2 GetCenter()
	{
		Vector2[] vertexs = m_Polygon.points; // (BoxCollider2D -> PolygonCollider)
		return GetCenter(vertexs);
	}*/
	/*Vector2 GetCenter(Vector2[] vertexs)
	{
		float sum;
		float Area = 0f;
		Vector2 result = new Vector2();

		for (int i = 0; i < vertexs.Length; ++i)
		{
			int index = (i + 1) % vertexs.Length;
			sum = (vertexs[i].x * vertexs[index].y) - (vertexs[index].x * vertexs[i].y);
			Area += sum;
			result += new Vector2((vertexs[i].x + vertexs[index].x) * sum, (vertexs[i].y + vertexs[index].y) * sum);
		}

		Area *= 0.5f;
		Area *= 6f;

		sum = (1f / Area);

		return result * sum;
	}*/
	#endregion
}
