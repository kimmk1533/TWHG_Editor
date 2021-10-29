using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
	public static class Physics2D
	{
		// 중력
		private static Vector2 m_Gravity = new Vector2(0f, -9.81f);
		private static Color m_ColliderColor = new Color(145f / 255f, 244f / 255f, 139f / 255f, 192f / 255f);
		private static Color m_BoundingBoxColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

		#region 외부 프로퍼티
		public static Vector2 gravity { get => m_Gravity; set => m_Gravity = value; }
		public static Color colliderColor { get => m_ColliderColor; set => m_ColliderColor = value; }
		public static Color boundingBoxColor { get => m_BoundingBoxColor; set => m_BoundingBoxColor = value; }
		#endregion
		#region 내부 함수
		#region Collision Test
		// Collider Layer
		private static bool GetIgnoreLayerCollision(int layer, int layerMask)
		{
			return (layerMask & (1 << layer)) == 0;
		}

		// Collision https://tt91.tistory.com/57
		// 출처: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
		// AABB (Axis Aligned Bounding Box)
		// AABB vs AABB
		private static bool CollisionTest_AABB_AABB(Collision2D collision)
		{
			Collider2D A = collision.collider;
			Collider2D B = collision.otherCollider;

			Bounds bounds_A = A.GetBoundingBox();
			Bounds bounds_B = B.GetBoundingBox();

			Vector2 extents = bounds_A.extents + bounds_B.extents;

			Vector2 pos_A = (Vector2)A.bounds.center + A.offset;
			Vector2 pos_B = (Vector2)B.bounds.center + B.offset;

			Vector2 distance = pos_B - pos_A;

			float x_overlap = extents.x - Mathf.Abs(distance.x);
			float y_overlap = extents.y - Mathf.Abs(distance.y);

			if (x_overlap < 0 || y_overlap < 0)
				return false;

			return true;
		}
		// OBB (Oriented Bounding Box)
		// OBB vs OBB
		private static bool CollisionTest_OBB_OBB(ref Collision2D collision)
		{
			// 출처: https://justicehui.github.io/other-algorithm/2018/06/23/OBB/
			// 출처: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
			Collider2D A = collision.collider;
			Collider2D B = collision.otherCollider;

			Vector2 pos_A = (Vector2)A.bounds.center + A.offset;
			Vector2 pos_B = (Vector2)B.bounds.center + B.offset;

			Vector2 distance = pos_B - pos_A;

			Vector2[] axises = new Vector2[4]
			{
				A.GetUpVector(),
				B.GetUpVector(),
				A.GetRightVector(),
				B.GetRightVector(),
			};

			for (int i = 0; i < 4; ++i)
			{
				float sum = 0f;
				Vector2 unit = axises[i].normalized;

				for (int j = 0; j < 4; ++j)
				{
					sum += Mathf.Abs(Vector2.Dot(axises[j], unit));
				}

				// 분리축 검사
				if (Mathf.Abs(Vector2.Dot(distance, unit)) >= sum)
					return false;
			}

			Bounds bounds_A = A.GetBoundingBox();
			Bounds bounds_B = B.GetBoundingBox();

			Vector2 extents = bounds_A.extents + bounds_B.extents;

			float x_overlap = extents.x - Mathf.Abs(distance.x);
			float y_overlap = extents.y - Mathf.Abs(distance.y);

			if (x_overlap < y_overlap)
			{
				if (distance.x < 0f)
					collision.normal = Vector2.left;
				else
					collision.normal = Vector2.right;

				collision.penetration = x_overlap;
			}
			else
			{
				if (distance.y < 0f)
					collision.normal = Vector2.down;
				else
					collision.normal = Vector2.up;

				collision.penetration = y_overlap;
			}

			return true;
		}
		// Circle vs Circle
		private static bool CollisionTest_Circle_Circle(ref Collision2D collision)
		{
			CircleCollider2D A = collision.collider as CircleCollider2D;
			CircleCollider2D B = collision.otherCollider as CircleCollider2D;

			Vector2 pos_A = (Vector2)A.bounds.center + A.offset;
			Vector2 pos_B = (Vector2)B.bounds.center + B.offset;

			Vector2 distance = pos_B - pos_A;

			float radius = A.radius + B.radius;
			float squardRadius = radius * radius;

			if (squardRadius <= distance.sqrMagnitude)
				return false;

			float d = distance.magnitude;

			// 원(중심)이 한 점에 겹치는 경우
			if (d == 0)
			{
				collision.penetration = A.radius;
				collision.normal = Vector2.up;
				return true;
			}
			// 원(중심)이 서로 다른 점에 있는 경우
			else
			{
				collision.penetration = squardRadius - d;
				collision.normal = distance.normalized;
				return true;
			}
		}
		// OBB vs Circle
		private static bool CollisionTest_OBB_Circle(ref Collision2D collision)
		{
			// 출처: https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection
			Collider2D A = collision.collider;
			CircleCollider2D B = collision.otherCollider as CircleCollider2D;

			float theta = (null != A.attachedRigidbody) ? A.attachedRigidbody.rotation * Mathf.Deg2Rad :
						((null != A) ? A.transform.eulerAngles.z * Mathf.Deg2Rad : 0f);

			Matrix4x4 rotMat = Matrix4x4.identity;
			if (theta != 0f)
			{
				float sin = Mathf.Sin(theta);
				float cos = Mathf.Cos(theta);
				rotMat.m00 = cos; rotMat.m01 = sin;
				rotMat.m10 = -sin; rotMat.m11 = cos;
			}

			Vector2[] vertices = new Vector2[4];
			for (int i = 0; i < 4; ++i)
			{
				vertices[i] = rotMat * A[i];
			}

			Vector2 pos_A = (Vector2)A.bounds.center + A.offset;
			Vector2 pos_B = (Vector2)B.bounds.center + B.offset;

			Vector2 distance = pos_B - pos_A;

			distance = rotMat * distance;

			Vector2 extents = (vertices[1] - vertices[3]) * 0.5f;

			Vector2 closest = new Vector2();
			closest.x = Mathf.Clamp(distance.x, -extents.x, extents.x);
			closest.y = Mathf.Clamp(distance.y, -extents.y, extents.y);

			bool inside = false;

			if (distance == closest)
			{
				inside = true;

				if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
				{
					if (closest.x > 0f)
						closest.x = extents.x;
					else
						closest.x = -extents.x;
				}
				else
				{
					if (closest.y > 0f)
						closest.y = extents.y;
					else
						closest.y = -extents.y;
				}
			}

			rotMat.m01 = -rotMat.m01;
			rotMat.m10 = -rotMat.m10;
			distance = rotMat * distance;
			closest = rotMat * closest;

			Vector2 d_Closest = distance - closest;
			float d_Squared_Closest = d_Closest.sqrMagnitude;

			if (d_Squared_Closest >= B.radius * B.radius && !inside)
				return false;

			float distance_Closest = d_Closest.magnitude;

			if (inside)
			{
				collision.normal = -distance;
			}
			else
			{
				collision.normal = distance;
			}
			collision.penetration = B.radius - distance_Closest;

			return true;
		}
		#endregion
		#endregion
		#region 외부 함수
		#region Collision Test
		// 사전 검사 (바운딩 박스의 AABB 사용)
		public static bool PreCollisionTest(Collision2D collision)
		{
			Collider2D A = collision.collider;
			Collider2D B = collision.otherCollider;

			if (UnityEngine.Physics2D.GetIgnoreLayerCollision(A.gameObject.layer, B.gameObject.layer))
				return false;

			if (null == A.attachedRigidbody &&
				null == B.attachedRigidbody)
				return false;

			return CollisionTest_AABB_AABB(collision);
		}
		// 타입별 충돌 검사
		public static bool CollisionTestByType(ref Collision2D collision)
		{
			Collider2D A = collision.collider;
			Collider2D B = collision.otherCollider;

			if (A.type == Collider2D.E_ColliderType.Box &&
				B.type == Collider2D.E_ColliderType.Box)
			{
				return CollisionTest_OBB_OBB(ref collision);
			}
			else if (A.type == Collider2D.E_ColliderType.Circle &&
				B.type == Collider2D.E_ColliderType.Circle)
			{
				return CollisionTest_Circle_Circle(ref collision);
			}
			else if ((A.type == Collider2D.E_ColliderType.Box && B.type == Collider2D.E_ColliderType.Circle) ||
				(A.type == Collider2D.E_ColliderType.Circle && B.type == Collider2D.E_ColliderType.Box))
			{
				if (A.type == Collider2D.E_ColliderType.Circle &&
					B.type == Collider2D.E_ColliderType.Box)
				{
					collision = new Collision2D(B, A);
				}

				return CollisionTest_OBB_Circle(ref collision);
			}

			return false;
		}

		//UnityEngine.Physics2D
		#region Raycast
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction)
		{
			return Raycast(origin, direction, float.MaxValue);
		}
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance)
		{
			Vector2 dir_normal = direction.normalized;
			Vector2 C = origin;
			Vector2 D = origin + dir_normal * distance;

			float minR = float.MaxValue;
			Collider2D collider = null;
			foreach (var item in Physics2DManager.colliderList)
			{
				float Ax = item.bounds.min.x, Bx = item.bounds.max.x;
				float Ay = item.bounds.min.y, By = item.bounds.max.y;

				float r_up = (Bx - Ax) * (C.y - Ay) - (C.x - Ax) * (By - Ay);
				float r_down = (D.x - C.x) * (By - Ay) - (Bx - Ax) * (D.y - C.y);
				float r = r_up / r_down;

				if (minR > r)
				{
					minR = r;
					collider = item;
				}
			}

			RaycastHit2D hit_result = new RaycastHit2D(collider);
			hit_result.point = minR * (D - C) + C;

			return hit_result;
		}
		public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask)
		{


			RaycastHit2D hit2D = new RaycastHit2D();
			//hit2D.
			return hit2D;
		}
		#endregion
		#region OverlapBox
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle)
		{
			GameObject tempObj = new GameObject("temp");
			BoxCollider2D collider = tempObj.AddComponent<BoxCollider2D>();
			collider.attachedRigidbody = tempObj.AddComponent<Rigidbody2D>();
			collider.attachedRigidbody.position = point;
			collider.attachedRigidbody.rotation = angle;
			collider.center = point;
			collider.size = size;

			foreach (var item in Physics2DManager.colliderList)
			{
				Collision2D collision = new Collision2D(item, collider);

				if (CollisionTestByType(ref collision))
				{
					GameObject.DestroyImmediate(tempObj);

					return item;
				}
			}

			GameObject.DestroyImmediate(tempObj);

			return null;
		}
		public static Collider2D OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			GameObject tempObj = new GameObject("temp");
			BoxCollider2D collider = tempObj.AddComponent<BoxCollider2D>();
			collider.attachedRigidbody = tempObj.AddComponent<Rigidbody2D>();
			collider.attachedRigidbody.position = point;
			collider.attachedRigidbody.rotation = angle;
			collider.center = point;
			collider.size = size;

			foreach (var item in Physics2DManager.colliderList)
			{
				if (GetIgnoreLayerCollision(item.gameObject.layer, layerMask))
					continue;

				Collision2D collision = new Collision2D(item, collider);

				if (CollisionTestByType(ref collision))
				{
					GameObject.DestroyImmediate(tempObj);

					return item;
				}
			}
			
			GameObject.DestroyImmediate(tempObj);

			return null;
		}
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle)
		{
			List<Collider2D> colliders = new List<Collider2D>();

			GameObject tempObj = new GameObject("temp");
			BoxCollider2D collider = tempObj.AddComponent<BoxCollider2D>();
			collider.attachedRigidbody = tempObj.AddComponent<Rigidbody2D>();
			collider.attachedRigidbody.position = point;
			collider.attachedRigidbody.rotation = angle;
			collider.center = point;
			collider.size = size;

			foreach (var item in Physics2DManager.colliderList)
			{
				Collision2D collision = new Collision2D(item, collider);

				if (CollisionTestByType(ref collision))
				{
					colliders.Add(item);
				}
			}

			GameObject.DestroyImmediate(tempObj);

			return colliders.ToArray();
		}
		public static Collider2D[] OverlapBoxAll(Vector2 point, Vector2 size, float angle, int layerMask)
		{
			List<Collider2D> colliders = new List<Collider2D>();

			GameObject tempObj = new GameObject("temp");
			BoxCollider2D collider = tempObj.AddComponent<BoxCollider2D>();
			collider.attachedRigidbody = tempObj.AddComponent<Rigidbody2D>();
			collider.attachedRigidbody.position = point;
			collider.attachedRigidbody.rotation = angle;
			collider.center = point;
			collider.size = size;

			foreach (var item in Physics2DManager.colliderList)
			{
				if (GetIgnoreLayerCollision(item.gameObject.layer, layerMask))
					continue;

				Collision2D collision = new Collision2D(item, collider);

				if (CollisionTestByType(ref collision))
				{
					colliders.Add(item);
				}
			}

			GameObject.DestroyImmediate(tempObj);

			return colliders.ToArray();
		}
		#endregion
		#region OverlapPoint
		public static Collider2D OverlapPoint(Vector2 point)
		{
			foreach (var item in Physics2DManager.colliderList)
			{
				if (!item.OverlapPoint(point))
					continue;

				return item;
			}

			return null;
		}
		public static Collider2D OverlapPoint(Vector2 point, int layerMask)
		{
			foreach (var item in Physics2DManager.colliderList)
			{
				if (GetIgnoreLayerCollision(item.gameObject.layer, layerMask))
					continue;

				if (!item.OverlapPoint(point))
					continue;

				return item;
			}

			return null;
		}
		public static Collider2D[] OverlapPointAll(Vector2 point)
		{
			List<Collider2D> colliders = new List<Collider2D>();

			foreach (var item in Physics2DManager.colliderList)
			{
				if (!item.OverlapPoint(point))
					continue;

				colliders.Add(item);
			}

			return colliders.ToArray();
		}
		public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
		{
			List<Collider2D> colliders = new List<Collider2D>();

			foreach (var item in Physics2DManager.colliderList)
			{
				if (GetIgnoreLayerCollision(item.gameObject.layer, layerMask))
					continue;

				if (!item.OverlapPoint(point))
					continue;

				colliders.Add(item);
			}

			return colliders.ToArray();
		}
		#endregion
		#endregion
		#endregion
	}
}