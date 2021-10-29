using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IClickerableObject, IEraserableObject
{
	[SerializeField]
	protected float m_Speed;
	[SerializeField, ReadOnly]
	protected Vector3 m_InitPos;
	[SerializeField]
	protected Vector3 m_SpawnPos;
	[SerializeField]
	protected bool m_IsSafe;
	[SerializeField]
	protected bool m_IsDie;

	#region 내부 컴포넌트
	protected SpriteRenderer m_Renderer;
	protected PlayerAnimator m_Animator;
	protected MyPhysics.BoxCollider2D m_Collider;
	protected MyPhysics.Rigidbody2D m_RigidBody;
	#endregion
	#region 내부 프로퍼티
	#region 매니져
	protected __GameManager M_Game => __GameManager.Instance;
	protected __EditManager M_Edit => __EditManager.Instance;
	protected StageManager M_Stage => StageManager.Instance;
	protected FloatingTextManager M_FloatingText => FloatingTextManager.Instance;

	protected PlayerManager M_Player => PlayerManager.Instance;
	protected CoinManager M_Coin => CoinManager.Instance;
	#endregion

	protected bool canMove => !M_Edit.isEditMode && !m_IsDie;
	#endregion
	#region 내부 함수
	private void Move()
	{
		if (Input.anyKey)
		{
			float halfSize = (transform.lossyScale.x + transform.lossyScale.y) * 0.5f * 0.5f;
			float rayAdjust = halfSize - 0.05f;

			float xDir = Input.GetAxisRaw("Horizontal");
			float yDir = Input.GetAxisRaw("Vertical");

			Vector2 xVec = new Vector2(xDir, 0f);
			Vector2 yVec = new Vector2(0f, yDir);

			int layer = 1 << LayerMask.NameToLayer("Wall");

			Vector2 pos = transform.position;

			RaycastHit2D[] raycastHits = new RaycastHit2D[4];

			float xMove = m_Speed * Time.deltaTime;
			float yMove = m_Speed * Time.deltaTime;

			raycastHits[0] = Physics2D.Raycast(pos + yVec * halfSize + (Vector2.right * rayAdjust), yVec, yMove, layer);
			raycastHits[1] = Physics2D.Raycast(pos + yVec * halfSize + (Vector2.left * rayAdjust), yVec, yMove, layer);
			raycastHits[2] = Physics2D.Raycast(pos + xVec * halfSize + (Vector2.up * rayAdjust), xVec, xMove, layer);
			raycastHits[3] = Physics2D.Raycast(pos + xVec * halfSize + (Vector2.down * rayAdjust), xVec, xMove, layer);

			// 상, 하
			if (raycastHits[0].transform != null)
			{
				if (raycastHits[0].distance <= yMove)
				{
					yMove = raycastHits[0].distance;
				}
			}
			if (raycastHits[1].transform != null)
			{
				if (raycastHits[1].distance <= yMove)
				{
					yMove = raycastHits[1].distance;
				}
			}
			if (raycastHits[2].transform != null)
			{
				if (raycastHits[2].distance <= xMove)
				{
					xMove = raycastHits[2].distance;
				}
			}
			if (raycastHits[3].transform != null)
			{
				if (raycastHits[3].distance <= xMove)
				{
					xMove = raycastHits[3].distance;
				}
			}

			Vector2 temp = new Vector2(xDir * xMove, yDir * yMove);

			transform.Translate(temp);
		}
	}
	private void ClampPos()
	{
		// 뷰포트 기준 좌측 하단의 좌표를 월드좌표로 변환 (최솟값)
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		// 뷰포트 기준 우측 상단의 좌표를 월드좌표로 변환 (최댓값)
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		// 플레이어 월드좌표
		Vector2 PlayerPos = transform.position;

		// 플레이어의 월드좌표를 자신의 사이즈까지 고려하여 화면에 맞게 변환 
		PlayerPos.x = Mathf.Clamp(PlayerPos.x, min.x + 0.5f, max.x - 0.5f);
		PlayerPos.y = Mathf.Clamp(PlayerPos.y, min.y + 0.5f, max.y - 0.5f);

		// 제한한 위치로 이동
		transform.position = PlayerPos;
	}
	private void Death()
	{
		if (m_IsDie)
			return;

		m_IsDie = true;
		m_Animator.Death();
	}

	#region CollisionEnter
	private void OnCollision2DEnter(MyPhysics.Collider2D collider)
	{
		switch (collider.tag)
		{
			case "Enemy":
				CollisionEnterEnemy(collider);
				break;
			case "SafetyZone":
				CollisionEnterSafetyZone(collider);
				break;
			case "GravityZone":
				CollisionEnterGravityZone(collider);
				break;
			case "IceZone":
				CollisionEnterIceZone(collider);
				break;
		}
	}

	private void CollisionEnterEnemy(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
			return;

		if (!m_IsSafe)
		{
			Death();
		}
	}
	private void CollisionEnterSafetyZone(MyPhysics.Collider2D collider)
	{
		m_IsSafe = true;
		m_SpawnPos = collider.transform.position;

		if (M_Edit.isPlayMode)
		{
			bool isFinishZone = collider.GetComponent<SafetyZone>().isFinishZone;

			if (isFinishZone && !M_Coin.IsLeftCoin)
			{
				// 승리
				M_Stage.canSave = true;
				M_FloatingText.SpawnFloatingText("클리어!", new Vector2(0.5f, 0.3f));
				M_Game.ExitPlayMode(0.5f);
			}
		}
	}
	private void CollisionEnterGravityZone(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
			return;

		m_RigidBody.useGravity = true;
		m_RigidBody.gravity = collider.GetComponent<GravityZone>().gravity;
	}
	private void CollisionEnterIceZone(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
			return;

		m_RigidBody.type = MyPhysics.Rigidbody2D.E_BodyType.Dynamic;
		m_RigidBody.drag = collider.GetComponent<IceZone>().friction;
		//if (m_RigidBody.velocity.magnitude == 0f)
		//{
		//    m_RigidBody.velocity += m_Player.rigidBody2D.force;
		//}
	}
	#endregion
	#region CollisionExit
	private void OnCollision2DExit(MyPhysics.Collider2D collider)
	{
		switch (collider.tag)
		{
			case "SafetyZone":
				CollisionExitSafetyZone(collider);
				break;
			case "GravityZone":
				CollisionExitGravityZone(collider);
				break;
			case "IceZone":
				CollisionExitIceZone(collider);
				break;
		}
	}

	private void CollisionExitSafetyZone(MyPhysics.Collider2D collider)
	{
		Vector2 point = m_Collider.center;
		Vector2 size = m_Collider.size;
		float rot = transform.eulerAngles.z;
		int layerMask = LayerMask.GetMask("SafetyZone");
		MyPhysics.Collider2D[] colliders = MyPhysics.Physics2D.OverlapBoxAll(point, size, rot, layerMask);
		if (colliders.Length <= 0)
		{
			m_IsSafe = false;
		}
	}
	private void CollisionExitGravityZone(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
			return;

		Vector2 point = m_Collider.center;
		Vector2 size = m_Collider.size;
		float rot = transform.eulerAngles.z;
		int layerMask = LayerMask.GetMask("GravityZone");
		MyPhysics.Collider2D[] colliders = MyPhysics.Physics2D.OverlapBoxAll(point, size, rot, layerMask);
		if (colliders.Length > 0)
		{
			m_RigidBody.gravity = colliders[0].GetComponent<GravityZone>().gravity;
		}
		else
		{
			m_RigidBody.useGravity = false;
			m_RigidBody.gravity = MyPhysics.Physics2D.gravity;
		}
	}
	private void CollisionExitIceZone(MyPhysics.Collider2D collider)
	{
		if (M_Edit.isEditMode)
			return;

		Vector2 point = m_Collider.center;
		Vector2 size = m_Collider.size;
		float rot = transform.eulerAngles.z;
		int layerMask = LayerMask.GetMask("IceZone");
		MyPhysics.Collider2D[] colliders = MyPhysics.Physics2D.OverlapBoxAll(point, size, rot, layerMask);
		if (colliders.Length > 0)
		{
			m_RigidBody.drag = collider.GetComponent<IceZone>().friction;
		}
		else
		{
			m_RigidBody.type = MyPhysics.Rigidbody2D.E_BodyType.Kinematic;
			m_RigidBody.drag = 1f;
		}
	}
	#endregion
	#endregion
	#region 외부 함수
	public void __Initialize()
	{
		#region 이벤트 링크
		M_Player.onPlayerRespawn += Respawn;
		#endregion

		#region 내부 컴포넌트
		if (null == m_Renderer)
		{
			m_Renderer = GetComponentInChildren<SpriteRenderer>();
		}
		if (null == m_Animator)
		{
			m_Animator = GetComponentInChildren<PlayerAnimator>();
			m_Animator.__Initialize(this);
		}
		if (null == m_Collider)
		{
			m_Collider = GetComponent<MyPhysics.BoxCollider2D>();
			// 마찰 설정
			m_Collider.friction = 0f;
			// 탄성 설정
			m_Collider.bounciness = 0f;
			m_Collider.onCollisionEnter2D += OnCollision2DEnter;
			m_Collider.onCollisionExit2D += OnCollision2DExit;
		}
		if (null == m_RigidBody)
		{
			m_RigidBody = GetComponent<MyPhysics.Rigidbody2D>();
			m_RigidBody.layerMask = LayerMask.GetMask("Wall");
		}
		#endregion

		m_IsDie = false;

		gameObject.SetActive(false);
	}
	#endregion
	#region 이벤트 함수
	public void OnPlayModeEnter()
	{
		m_InitPos = transform.position;

		m_Animator.OnPlayModeEnter();

		int layerMask = LayerMask.GetMask("Enemy", "Coin", "GravityZone");
		MyPhysics.Collider2D[] colliders = MyPhysics.Physics2D.OverlapBoxAll(m_Collider.center, m_Collider.size, 0f, layerMask);
		foreach (var item in colliders)
		{
			OnCollision2DEnter(item);
		}
	}
	public void OnPlayModeExit()
	{
		m_Animator.OnPlayModeExit();

		transform.position = m_InitPos;

		m_RigidBody.useGravity = false;
		m_RigidBody.gravity = MyPhysics.Physics2D.gravity;
		m_RigidBody.type = MyPhysics.Rigidbody2D.E_BodyType.Kinematic;
		m_RigidBody.drag = 0f;
		m_RigidBody.velocity = Vector3.zero;
		m_RigidBody.force = Vector3.zero;
	}
	private void Respawn()
	{
		if (M_Edit.isEditMode)
		{
			transform.position = m_InitPos;
		}
		else
		{
			transform.position = m_SpawnPos;
		}

		gameObject.SetActive(true);
		m_IsDie = false;
	}
	#endregion
	#region 인터페이스 함수
	public void EraseObject()
	{
		gameObject.SetActive(false);
	}
	public E_ObjectType GetObjectType()
	{
		return E_ObjectType.Player;
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
	protected void FixedUpdate()
	{
		if (canMove)
		{
			Vector2 force = Vector2.zero;

			force += Vector2.right * Input.GetAxisRaw("Horizontal") * m_Speed;
			force += Vector2.up * Input.GetAxisRaw("Vertical") * m_Speed;

			m_RigidBody.AddForce(force);
		}
	}
	/*protected void LateUpdate()
	{
		ClampPos();
	}*/
	#endregion
}
