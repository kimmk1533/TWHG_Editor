using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	public sealed class Physics2DManager : Singleton<Physics2DManager>
	{
		private static List<Collider2D> m_ColliderList = new List<Collider2D>();

		#region 외부 프로퍼티
		public static List<Collider2D> colliderList => m_ColliderList;
		#endregion
		#region 내부 함수
		private float PythagoreanSolve(float A, float B)
		{
			return Mathf.Sqrt(A * A + B * B);
		}
		// 추후 회전 추가
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
						//if (A.isTrigger || B.isTrigger)
						//	m_TriggerHitColliderList.Add(collision);
						//else
						//	m_HitColliderList.Add(collision);

						ResolveCollision(collision);

						Debug.Log("충돌\n" +
							A.transform.parent.name + ": " + A.type + "\n" +
							B.transform.parent.name + ": " + B.type);
					}
				}
			}
			#endregion
		}
		#endregion
	}
}