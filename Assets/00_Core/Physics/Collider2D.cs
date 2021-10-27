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
		[SerializeField, ReadOnly]
		protected float m_Bounciness;
		// 밀도
		protected float m_Density;
		[SerializeField, ReadOnly]
		protected Bounds m_Bounds;

		#region 외부 프로퍼티
		public E_ColliderType type { get => m_ColliderType; }
		public float bounciness { get => m_Bounciness; set => m_Bounciness = value; }
		public float friction { get => m_Friction; set => m_Friction = value; }
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

				float theta = -transform.eulerAngles.z * Mathf.Deg2Rad;
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
			float rot = transform.eulerAngles.z + 90f;

			result.x += m_Bounds.extents.y * Mathf.Cos(Mathf.Deg2Rad * rot);
			result.y += m_Bounds.extents.y * Mathf.Sin(Mathf.Deg2Rad * rot);

			return result;
		}
		public virtual Vector2 GetRightVector()
		{
			Vector2 result = new Vector2();
			float rot = transform.eulerAngles.z;

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