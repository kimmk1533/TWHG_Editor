using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IEraserableObject, IClickedObject
{
	#region 내부 컴포넌트
	protected SpriteRenderer m_Renderer;
	protected MyPhysics.CircleCollider2D m_Collider;
	#endregion
	#region 내부 프로퍼티
	#region 매니져
	protected __EditManager M_Edit => __EditManager.Instance;

	protected CoinManager M_Coin => CoinManager.Instance;
	#endregion
	#endregion
	#region 내부 함수
	private void OnTrigger2DEnter(MyPhysics.Collider2D collider)
	{
		if (!M_Edit.isEditMode)
		{
			if (collider.CompareTag("Player"))
			{
				gameObject.SetActive(false);

				--M_Coin.CoinCount;
			}
		}
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
			m_Collider.onTriggerEnter2D += OnTrigger2DEnter;
		}
	}
	public void __Finalize()
	{

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
	public void EraseObject()
	{
		M_Coin.DespawnCoin(this);
	}
	public SpriteRenderer GetSpriteRenderer()
	{
		return m_Renderer;
	}
	public GameObject GetGameObject()
	{
		return gameObject;
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.Coin;
	}
	#endregion
}
