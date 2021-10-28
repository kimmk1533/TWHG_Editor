using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	[DefaultExecutionOrder(1)]
	public class Rigidbody2D : MonoBehaviour
	{
		// 물리 타입
		[SerializeField]
		protected E_BodyType m_BodyType;

		// 질량
		[SerializeField]
		protected float m_Mass = 1f;
		// 항력 계수
		[SerializeField]
		protected float m_Drag = 0f;
		// 중력 사용 여부
		[SerializeField]
		protected bool m_UseGravity = false;
		// 가해지는 중력
		[SerializeField]
		protected Vector2 m_Gravity = Physics2D.gravity;

		#region 내부 정보
		[Header("Infos")]
		[Space(5)]
		// 속도
		[SerializeField, ReadOnly]
		protected Vector2 m_Velocity;
		// 힘 (작용)
		protected Vector2 m_Force;
		// 저항 (반작용)
		protected Vector2 m_CalculedDrag;
		// 충돌 계산할 콜라이더
		protected Collider2D m_Collider;
		// 충돌 계산시 사용될 레이어 마스크
		protected int m_LayerMask;
		#endregion

		#region 외부 프로퍼티
		public float mass { get => m_Mass; set => m_Mass = value; }
		public float drag { get => m_Drag; set => m_Drag = value; }
		public bool useGravity { get => m_UseGravity; set => m_UseGravity = value; }
		public Vector2 gravity { get => m_Gravity; set => m_Gravity = value; }
		public E_BodyType type { get => m_BodyType; set => m_BodyType = value; }
		public Vector2 velocity { get => m_Velocity; set => m_Velocity = value; }
		public Vector2 force { get => m_Force; set => m_Force = value; }
		public int layerMask { get => m_LayerMask; set => m_LayerMask = value; }
		#endregion
		#region 내부 함수
		protected Vector2 CalculateDrag(Vector2 force)
		{
			if (force.magnitude <= 0f)
				return Vector2.zero;

			// 밀도
			float p = (null == m_Collider) ? 1f : m_Collider.density;
			// 면적
			const float A = 1f;
			// 속도의 속력
			float v = velocity.magnitude;
			// 항력 계수
			float Cd = 0.03134f * m_Drag;
			// 속도의 방향
			Vector2 dir = velocity.normalized;

			// 항력 방정식
			Vector2 Fd = 0.5f * p * v * v * A * Cd * -dir;

			return Fd;
		}

		protected void Gravity()
		{
			if (!m_UseGravity)
				return;

			Vector2 gravity = m_Gravity;

			switch (m_BodyType)
			{
				case E_BodyType.Dynamic:
					gravity *= m_Mass;
					break;
				case E_BodyType.Kinematic:
					break;
			}

			AddForce(gravity);
		}
		protected void Velocity()
		{
			switch (m_BodyType)
			{
				case E_BodyType.Dynamic:
					m_Velocity += m_Force / m_Mass * Time.fixedDeltaTime;
					break;
				case E_BodyType.Kinematic:
					m_Velocity += m_Force;
					break;
			}
		}
		protected void Drag()
		{
			if (m_BodyType == E_BodyType.Kinematic)
				return;

			if (m_Velocity.magnitude > 0)
			{
				m_CalculedDrag = CalculateDrag(m_Velocity);

				if (Mathf.Abs(m_Velocity.x) <= Mathf.Abs(m_CalculedDrag.x))
				{
					m_Velocity.x = 0f;
				}
				else
				{
					m_Velocity.x += m_CalculedDrag.x;
				}

				if (Mathf.Abs(m_Velocity.y) <= Mathf.Abs(m_CalculedDrag.y))
				{
					m_Velocity.y = 0f;
				}
				else
				{
					m_Velocity.y += m_CalculedDrag.y;
				}
			}
			else
			{
				m_CalculedDrag = Vector2.zero;
			}
		}
		protected void Clamp()
		{
			if (Mathf.Abs(m_Velocity.x) < 0.01f)
			{
				m_Velocity.x = 0f;
			}
			if (Mathf.Abs(m_Velocity.y) < 0.01f)
			{
				m_Velocity.y = 0f;
			}
		}
		protected void Move()
		{
			transform.localPosition += (Vector3)m_Velocity * Time.fixedDeltaTime;
		}
		#endregion
		#region 외부 함수
		public void AddForce(Vector2 force)
		{
			m_Force += force;
		}
		#endregion

		#region 유니티 콜백 함수
		void Awake()
		{
			m_Collider = GetComponent<Collider2D>();
			m_Collider.attachedRigidbody = this;
		}
		void FixedUpdate()
		{
			if (m_BodyType == E_BodyType.Static)
				return;

			if (m_BodyType == E_BodyType.Kinematic)
			{
				m_Velocity = Vector2.zero;
			}
			#region 테스트 이동
			//Vector2 force = Vector2.zero;

			//force += Vector2.right * Input.GetAxisRaw("Horizontal");
			//force += Vector2.up * Input.GetAxisRaw("Vertical");

			//AddForce(force);
			#endregion

			// 중력
			Gravity();
			// 속도 계산
			Velocity();
			// 공기저항
			Drag();
			// 충돌
			//Collision();

			// 보정
			Clamp();

			// 이동
			Move();
		}
		void LateUpdate()
		{
			#region 초기화
			m_Force = Vector2.zero;
			#endregion
		}
		#endregion

		public enum E_BodyType
		{
			Dynamic,
			Kinematic,
			Static
		}
	}

	//protected void Collision()
	//{
	//    if (null == m_Collider)
	//        return;

	//    Vector2 center = m_Collider.bounds.center;
	//    Vector2 size = m_Collider.bounds.size;
	//    Vector2 halfSize = size * 0.5f;
	//    Vector2 dir;
	//    if (!m_IsKinematic)
	//    {
	//        if (m_Velocity.x > 0f)
	//            dir.x = 1f;
	//        else if (m_Velocity.x < 0f)
	//            dir.x = -1f;
	//        else
	//            dir.x = 0f;

	//        if (m_Velocity.y > 0f)
	//            dir.y = 1f;
	//        else if (m_Velocity.y < 0f)
	//            dir.y = -1f;
	//        else
	//            dir.y = 0f;
	//    }
	//    else
	//    {
	//        if (m_Force.x > 0f)
	//            dir.x = 1f;
	//        else if (m_Force.x < 0f)
	//            dir.x = -1f;
	//        else
	//            dir.x = 0f;

	//        if (m_Force.y > 0f)
	//            dir.y = 1f;
	//        else if (m_Force.y < 0f)
	//            dir.y = -1f;
	//        else
	//            dir.y = 0f;
	//    }
	//    float distance = (!m_IsKinematic ? m_Velocity.magnitude : m_Force.magnitude) * Time.deltaTime;
	//    Vector2[] vertices = new Vector2[4]
	//    {
	//        center + Vector2.left * halfSize.x + Vector2.up * halfSize.y,
	//        center + Vector2.right * halfSize.x + Vector2.up * halfSize.y,

	//        center + Vector2.left * halfSize.x + Vector2.down * halfSize.y,
	//        center + Vector2.right * halfSize.x + Vector2.down * halfSize.y
	//    };

	//    RaycastHit2D[] hits = new RaycastHit2D[8]
	//    {
	//        Physics2D.Raycast(vertices[0], new Vector2(dir.x, 0f), distance, m_LayerMask),
	//        Physics2D.Raycast(vertices[0], new Vector2(0f, dir.y), distance, m_LayerMask),

	//        Physics2D.Raycast(vertices[1], new Vector2(dir.x, 0f), distance, m_LayerMask),
	//        Physics2D.Raycast(vertices[1], new Vector2(0f, dir.y), distance, m_LayerMask),

	//        Physics2D.Raycast(vertices[2], new Vector2(dir.x, 0f), distance, m_LayerMask),
	//        Physics2D.Raycast(vertices[2], new Vector2(0f, dir.y), distance, m_LayerMask),

	//        Physics2D.Raycast(vertices[3], new Vector2(dir.x, 0f), distance, m_LayerMask),
	//        Physics2D.Raycast(vertices[3], new Vector2(0f, dir.y), distance, m_LayerMask)
	//    };

	//    //if (!m_IsKinematic)
	//    //{
	//    //    for (int i = 0; i < hits.Length; ++i)
	//    //    {
	//    //        if (null != hits[i].transform &&
	//    //            hits[i].transform != m_Collider.transform)
	//    //        {
	//    //            Vector2 hit_distance = hits[i].point - vertices[i / 2];

	//    //            if (i % 2 == 0)
	//    //            {
	//    //                if (Mathf.Abs(hit_distance.x) < Mathf.Abs(m_Velocity.x))
	//    //                {
	//    //                    m_Velocity.x = hit_distance.x;
	//    //                }
	//    //            }
	//    //            else
	//    //            {
	//    //                if (Mathf.Abs(hit_distance.y) < Mathf.Abs(m_Velocity.y))
	//    //                {
	//    //                    m_Velocity.y = hit_distance.y;
	//    //                }
	//    //            }
	//    //        }
	//    //    }
	//    //}
	//    //else
	//    {
	//        for (int i = 0; i < hits.Length; ++i)
	//        {
	//            if (null != hits[i].transform &&
	//                hits[i].transform != m_Collider.transform)
	//            {
	//                Vector2 hit_distance = hits[i].point - vertices[i / 2];

	//                if (i % 2 == 0)
	//                {
	//                    if (Mathf.Abs(hit_distance.x) < Mathf.Abs(m_Force.x))
	//                    {
	//                        m_Force.x = hit_distance.x;
	//                    }
	//                    if (Mathf.Abs(hit_distance.x) < Mathf.Abs(m_Velocity.x))
	//                    {
	//                        m_Velocity.x = hit_distance.x;
	//                    }
	//                }
	//                else
	//                {
	//                    if (Mathf.Abs(hit_distance.y) < Mathf.Abs(m_Force.y))
	//                    {
	//                        m_Force.y = hit_distance.y;
	//                    }
	//                    if (Mathf.Abs(hit_distance.y) < Mathf.Abs(m_Velocity.y))
	//                    {
	//                        m_Velocity.y = hit_distance.y;
	//                    }
	//                }
	//            }
	//        }
	//    }
	//}
}