using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	public sealed class Physics2DManager : Singleton<Physics2DManager>
	{
		private static List<Collider2D> m_ColliderList = new List<Collider2D>();
		private static List<CollisionEventArgs> m_HitColliderList = new List<CollisionEventArgs>();
		private static List<CollisionEventArgs> m_OldHitColliderList = new List<CollisionEventArgs>();
		private static List<CollisionEventArgs> m_TriggerHitColliderList = new List<CollisionEventArgs>();
		private static List<CollisionEventArgs> m_OldTriggerHitColliderList = new List<CollisionEventArgs>();

		#region 외부 프로퍼티
		public static List<Collider2D> colliderList => m_ColliderList;
		#endregion
		#region 내부 함수
		// 피타고라스 정리
		private float PythagoreanSolve(float A, float B)
		{
			return Mathf.Sqrt(A * A + B * B);
		}
		// 충돌 검사
		private void CollisionTest()
		{
			for (int i = 0; i < m_ColliderList.Count; ++i)
			{
				Collider2D A = m_ColliderList[i];

				for (int j = i + 1; j < m_ColliderList.Count; ++j)
				{
					Collider2D B = m_ColliderList[j];

					Collision2D collision = new Collision2D(A, B);

					if (!Physics2D.PreCollisionTest(collision))
						continue;

					if (Physics2D.CollisionTestByType(ref collision))
					{
						if (A.isTrigger || B.isTrigger)
							m_TriggerHitColliderList.Add(new CollisionEventArgs(collision));
						else
							m_HitColliderList.Add(new CollisionEventArgs(collision));

						ResolveCollision(collision);

						Debug.Log("충돌\n" +
							A.transform.parent.name + ": " + A.type + "\n" +
							B.transform.parent.name + ": " + B.type);
					}
				}
			}
		}
		// 충돌 해결(추후 회전 추가)
		private void ResolveCollision(Collision2D collision)
		{
			#region Rigidbody
			Rigidbody2D A = collision.rigidbody;
			Rigidbody2D B = collision.otherRigidbody;

			if (null == A)
			{
				A = new Rigidbody2D();
				A.type = Rigidbody2D.E_BodyType.Static;
			}
			if (null == B)
			{
				B = new Rigidbody2D();
				B.type = Rigidbody2D.E_BodyType.Static;
			}
			#endregion
			#region Calculate
			#region Impulse
			Vector2 rv = B.velocity - A.velocity;

			float velAlongNormal = Vector2.Dot(rv, collision.normal);

			if (velAlongNormal > 0.0000001f)
				return;

			float e = Mathf.Min(collision.collider.bounciness, collision.otherCollider.bounciness);

			float invMassA = 1f / A.mass;
			float invMassB = 1f / B.mass;

			float j = (-(1f + e) * velAlongNormal) / (invMassA + invMassB);

			Vector2 impulse = j * collision.normal;
			#endregion
			#region Friction
			Vector2 tangent = rv - velAlongNormal * collision.normal;
			tangent.Normalize();

			float jt = (-Vector2.Dot(rv, tangent)) / (invMassA + invMassB);

			float frictionA = collision.collider.friction;
			float frictionB = collision.otherCollider.friction;
			float mu = PythagoreanSolve(frictionA, frictionB);

			Vector2 frictionImpulse;

			if (Mathf.Abs(jt) < j * mu)
			{
				frictionImpulse = tangent * jt;
			}
			else
			{
				frictionImpulse = tangent * (-j * mu);
			}
			#endregion
			#region Interpolation
			const float percent = 0.2f;
			const float slop = 0.01f;
			Vector2 correction = Mathf.Max(collision.penetration - slop, 0f) / (invMassA + invMassB) * percent * collision.normal;
			#endregion
			#endregion
			#region Apply
			if (null != A)
			{
				switch (A.type)
				{
					case Rigidbody2D.E_BodyType.Dynamic:
						// Impulse + Friction
						A.velocity -= invMassA * (impulse + frictionImpulse);
						// Interpolation
						A.transform.localPosition -= (Vector3)(invMassA * correction);
						break;
					case Rigidbody2D.E_BodyType.Kinematic:
						// Interpolation
						A.transform.localPosition -= (Vector3)(collision.penetration * correction.normalized);
						break;
				}
			}
			if (null != B)
			{
				switch (B.type)
				{
					case Rigidbody2D.E_BodyType.Dynamic:
						// Impulse + Friction
						B.velocity += invMassB * (impulse + frictionImpulse);
						// Interpolation
						B.transform.localPosition += (Vector3)(invMassB * correction);
						break;
					case Rigidbody2D.E_BodyType.Kinematic:
						// Interpolation
						B.transform.localPosition += (Vector3)(collision.penetration * correction.normalized);
						break;
				}
			}
			#endregion
		}
		#endregion
		#region 유니티 콜백 함수
		void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void FixedUpdate()
		{
			#region 충돌
			m_TriggerHitColliderList.Clear();

			CollisionTest();

			// Enter, Stay
			foreach (var item in m_TriggerHitColliderList)
			{
				// Trigger Enter Event
				if (!m_OldTriggerHitColliderList.Contains(item))
				{
					item.A.onTriggerEnter2D?.Invoke(item.B);
					item.B.onTriggerEnter2D?.Invoke(item.A);
					Debug.Log("TriggerEnter2D");
				}

				// Trigger Stay Event
				item.A.onTriggerStay2D?.Invoke(item.B);
				item.B.onTriggerStay2D?.Invoke(item.A);
			}

			// Exit
			foreach (var item in m_OldTriggerHitColliderList)
			{
				if (!m_TriggerHitColliderList.Contains(item))
				{
					item.A.onTriggerExit2D?.Invoke(item.B);
					item.B.onTriggerExit2D?.Invoke(item.A);
					Debug.Log("TriggerExit2D");
				}
			}

			m_OldTriggerHitColliderList.Clear();
			m_OldTriggerHitColliderList.AddRange(m_TriggerHitColliderList);
			#endregion
		}
		#endregion

		private struct CollisionEventArgs
		{
			public Collider2D A;
			public Collider2D B;

			public CollisionEventArgs(Collision2D collision)
			{
				A = collision.collider;
				B = collision.otherCollider;
			}
		}
	}
}