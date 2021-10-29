using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	public abstract class Collider2D : MonoBehaviour
	{
		protected E_ColliderType m_ColliderType;
		[SerializeField]
		protected bool m_IsTrigger;
		[SerializeField]
		protected Vector2 m_Offset;
		[SerializeField, ReadOnly]
		protected Rigidbody2D m_AttachedRigidbody;
		// 마찰
		[SerializeField, ReadOnly]
		protected float m_Friction = 0.4f;
		// 탄성
		[SerializeField, ReadOnly]
		protected float m_Bounciness = 0f;
		// 밀도
		protected float m_Density = 1f;
		[SerializeField, ReadOnly]
		protected Bounds m_Bounds;

		public delegate void CollisionEventHandler(Collider2D otherCollider);
		protected CollisionEventHandler m_OnCollisionEnter2D;
		protected CollisionEventHandler m_OnCollisionStay2D;
		protected CollisionEventHandler m_OnCollisionExit2D;
		protected CollisionEventHandler m_OnTriggerEnter2D;
		protected CollisionEventHandler m_OnTriggerStay2D;
		protected CollisionEventHandler m_OnTriggerExit2D;

		#region 외부 프로퍼티
		public E_ColliderType type { get => m_ColliderType; }
		// 탄성
		public float bounciness { get => m_Bounciness; set => m_Bounciness = value; }
		// 마찰
		public float friction { get => m_Friction; set => m_Friction = value; }
		// 밀도
		public float density { get => m_Density; set => m_Density = value; }
		public bool isTrigger { get => m_IsTrigger; set => m_IsTrigger = value; }
		public Vector2 offset { get => m_Offset; set => m_Offset = value; }
		public Rigidbody2D attachedRigidbody { get => m_AttachedRigidbody; set => m_AttachedRigidbody = value; }
		public Bounds bounds { get => m_Bounds; }
		public virtual Vector2 this[int index]
		{
			get
			{
				Vector2 min = -m_Bounds.extents;
				Vector2 max = m_Bounds.extents;

				float theta = (null != m_AttachedRigidbody) ? m_AttachedRigidbody.rotation * Mathf.Deg2Rad :
						((null != this) ? transform.eulerAngles.z * Mathf.Deg2Rad : 0f);
				float cos = Mathf.Cos(theta);
				float sin = Mathf.Sin(theta);

				Vector2 rotVec = new Vector2();
				Matrix4x4 rotMat = new Matrix4x4();
				rotMat.m00 = cos; rotMat.m01 = sin;
				rotMat.m10 = -sin; rotMat.m11 = cos;

				switch (index)
				{
					case 0:
						rotVec.x = min.x;
						rotVec.y = max.y;
						break;
					case 1:
						rotVec.x = max.x;
						rotVec.y = max.y;
						break;
					case 2:
						rotVec.x = max.x;
						rotVec.y = min.y;
						break;
					case 3:
						rotVec.x = min.x;
						rotVec.y = min.y;
						break;
				}

				Vector2 center = m_Bounds.center;
				Vector2 result = rotMat * rotVec;

				return center + result;
			}
		}

		public CollisionEventHandler onCollisionEnter2D { get => m_OnCollisionEnter2D; set => m_OnCollisionEnter2D = value; }
		public CollisionEventHandler onCollisionStay2D { get => m_OnCollisionStay2D; set => m_OnCollisionStay2D = value; }
		public CollisionEventHandler onCollisionExit2D { get => m_OnCollisionExit2D; set => m_OnCollisionExit2D = value; }
		public CollisionEventHandler onTriggerEnter2D { get => m_OnTriggerEnter2D; set => m_OnTriggerEnter2D = value; }
		public CollisionEventHandler onTriggerStay2D { get => m_OnTriggerStay2D; set => m_OnTriggerStay2D = value; }
		public CollisionEventHandler onTriggerExit2D { get => m_OnTriggerExit2D; set => m_OnTriggerExit2D = value; }
		#endregion
		#region 외부 함수
		public virtual bool OverlapPoint(Vector2 point)
		{
			return m_Bounds.min.x <= point.x && point.x <= m_Bounds.max.x &&
				m_Bounds.min.y <= point.y && point.y <= m_Bounds.max.y;
		}
		public abstract Bounds GetBoundingBox();
		public virtual Vector2 GetUpVector()
		{
			Vector2 result = new Vector2();//transform.position;

			float rot = ((null != m_AttachedRigidbody) ? m_AttachedRigidbody.rotation :
					((null != this) ? transform.eulerAngles.z : 0f)) + 90f;

			result.x += m_Bounds.extents.y * Mathf.Cos(Mathf.Deg2Rad * rot);
			result.y += m_Bounds.extents.y * Mathf.Sin(Mathf.Deg2Rad * rot);

			return result;
		}
		public virtual Vector2 GetRightVector()
		{
			Vector2 result = new Vector2();
			float rot = (null != m_AttachedRigidbody) ? m_AttachedRigidbody.rotation :
					((null != this) ? transform.eulerAngles.z : 0f);

			result.x += m_Bounds.extents.x * Mathf.Cos(Mathf.Deg2Rad * rot);
			result.y += m_Bounds.extents.x * Mathf.Sin(Mathf.Deg2Rad * rot);

			return result;
		}
		#endregion
		#region 유니티 콜백 함수
		protected virtual void OnEnable()
		{
			Physics2DManager.colliderList.Add(this);

			m_Bounds.center = (Vector2)transform.position + m_Offset;
		}
		protected virtual void OnDisable()
		{
			Physics2DManager.colliderList.Remove(this);
		}
		protected virtual void FixedUpdate()
		{
			m_Bounds.center = (Vector2)transform.position + m_Offset;
		}
		protected virtual void OnDrawGizmosSelected()
		{
			FixedUpdate();
		}
		#endregion

		public enum E_ColliderType
		{
			Box,
			Circle,
		}
	}
}