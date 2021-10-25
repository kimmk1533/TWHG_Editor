using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public static class Physics2D
    {
        private static Vector2 m_Gravity = new Vector2(0f, -9.81f);
        private static Color m_ColliderColor = new Color(145f / 255f, 244f / 255f, 139f / 255f, 192f / 255f);
        private static Color m_BoundingBoxColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

        public static Vector2 gravity { get => m_Gravity; set => m_Gravity = value; }
        public static Color colliderColor { get => m_ColliderColor; set => m_ColliderColor = value; }
        public static Color boundingBoxColor { get => m_BoundingBoxColor; set => m_BoundingBoxColor = value; }

        #region 내부 함수
        private static bool CheckLayerMask(int layer, int layerMask)
        {
            return (layerMask & (1 << layer)) == 0;
        }
        #endregion
        #region 외부 함수
        #region Collision Detection
        // Collision https://tt91.tistory.com/57
        // 출처: https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
        // 사전 검사 (추후 다이나믹 AABB 추가)
        public static bool First_Collision_Check(Collision2D collision)
        {
            Collider2D A = collision.collider;
            Collider2D B = collision.otherCollider;

            if (UnityEngine.Physics2D.GetIgnoreLayerCollision(A.gameObject.layer, B.gameObject.layer))
                return false;

            if (null == A.attachedRigidbody &&
                null == B.attachedRigidbody)
                return false;

            return AABB_Collision(collision);
        }
        // AABB (Axis Aligned Bounding Box)
        private static bool AABB_Collision(Collision2D collision)
        {
            Collider2D A = collision.collider;
            Collider2D B = collision.otherCollider;

            Bounds bounds_A = A.GetBoundingBox();
            Bounds bounds_B = B.GetBoundingBox();

            Vector2 extents = bounds_A.extents + bounds_B.extents;
            Vector2 distance = B.transform.position - A.transform.position;

            float x_overlap = extents.x - Mathf.Abs(distance.x);
            float y_overlap = extents.y - Mathf.Abs(distance.y);

            if (x_overlap < 0 || y_overlap < 0)
                return false;

            return true;
        }

        // OBB (Oriented Bounding Box)
        // OBB vs OBB
        private static bool OBB_Collision(ref Collision2D collision)
        {
            Collider2D A = collision.collider;
            Collider2D B = collision.otherCollider;

            Vector2 distance = B.transform.position - A.transform.position;

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

                // 분리되어 있거나 접하는 경우
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

            {
                //Collider2D A = collision.collider;
                //Collider2D B = collision.otherCollider;

                //// 기준이 될 각 변
                //Vector2[] edges = new Vector2[4];
                //// 기준이 될 벡터
                //Vector2[] normals = new Vector2[4];
                //// 기준이 될 축
                //Vector2[] axises = new Vector2[4];

                //// 각 변 설정
                //edges[0] = A[2] - A[3];
                //edges[1] = A[1] - A[2];
                //edges[2] = B[2] - B[3];
                //edges[3] = B[1] - B[2];

                //// 기준이 될 축 설정
                //for (int i = 0; i < 4; ++i)
                //{
                //    // 수직 벡터
                //    normals[i] = new Vector2(edges[i].y, -edges[i].x);

                //    // 축으로 설정
                //    axises[i] = normals[i].normalized;
                //}

                //float min_A, max_A;
                //float min_B, max_B;

                //// OBB 체크
                //for (int i = 0; i < 4; ++i)
                //{
                //    min_A = float.MaxValue;
                //    max_A = float.MinValue;

                //    min_B = float.MaxValue;
                //    max_B = float.MinValue;

                //    // SAT (Separating Axis Theorem)
                //    // 분리축 검사
                //    for (int j = 0; j < 4; ++j)
                //    {
                //        float projection_A = Vector2.Dot(A[j], axises[i]);
                //        min_A = Mathf.Min(min_A, projection_A);
                //        max_A = Mathf.Max(max_A, projection_A);

                //        float projection_B = Vector2.Dot(B[j], axises[i]);
                //        min_B = Mathf.Min(min_B, projection_B);
                //        max_B = Mathf.Max(max_B, projection_B);
                //    }

                //    // 분리축이 존재할 경우
                //    if (max_A < min_B || min_A > max_B)
                //        return false; // 충돌 안 함
                //}

                //// 분리축이 존재하지 않는 경우
                //return true; // 충돌
            }
        }
        // Circle vs Circle
        private static bool Circle_Collision(ref Collision2D collision)
        {
            CircleCollider2D A = collision.collider as CircleCollider2D;
            CircleCollider2D B = collision.otherCollider as CircleCollider2D;

            Vector2 distance = B.center - A.center;
            float radius = A.radius + B.radius;
            float squardRadius = radius * radius;

            if (squardRadius <= distance.sqrMagnitude)
                return false;

            float d = distance.magnitude;

            // 두 원이 겹치지 않은 경우
            if (d != 0)
            {
                collision.penetration = squardRadius - d;
                collision.normal = distance.normalized;
                return true;
            }
            // 두 원이 겹친 경우
            else
            {
                collision.penetration = A.radius;
                collision.normal = Vector2.up;
                return true;
            }
        }
        // OBB vs Circle
        private static bool OBB_vs_Circle_Collision(ref Collision2D collision)
        {
            // 출처: https://stackoverflow.com/questions/401847/circle-rectangle-collision-detection-intersection
            Collider2D A = collision.collider;
            CircleCollider2D B = collision.otherCollider as CircleCollider2D;

            // 회전한 각도 구해서
            float theta = A.transform.eulerAngles.z * Mathf.Deg2Rad;

            // 반대 방향으로 회전시켜주는 (회전한게 없어지는) 행렬 생성
            Matrix4x4 rotMat = Matrix4x4.identity;
            if (theta != 0f)
            {
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                rotMat.m00 = cos; rotMat.m01 = sin;
                rotMat.m10 = -sin; rotMat.m11 = cos;
            }

            // 충돌 검사하는 점들은 모두 회전 행렬을 곱하여 축 정렬
            Vector2[] vertices = new Vector2[4];
            for (int i = 0; i < 4; ++i)
            {
                vertices[i] = rotMat * A[i];
            }

            Vector2 distance = B.transform.position - A.transform.position;
            distance = rotMat * distance;

            Vector2 extents = (vertices[1] - vertices[3]) * 0.5f;

            // 원에서 사각형에 가장 가까운 점 검색
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

            // 원과 가장 가까운 점과의 거리 계산
            Vector2 d_Closest = distance - closest;
            // 루트는 무거우므로 제곱으로 비교
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

            // 원의 반지름의 제곱보다 작은 경우 충돌
            return true;

            // 실패한 방법들
            #region https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
            // 성공한 방법과 똑같은 방법이지만, 구현 당시 원리를 이해 못하였었고,
            // 회전에 대해 생각하지 않아 제대로 충돌 검사가 일어나지 않음.
            //Vector2 extents = (A[1] - A[3]) * 0.5f;

            //Vector2 n = B.center - (Vector2)A.bounds.center;
            //Vector2 closest = n;// A.bounds.ClosestPoint(B.center);
            //closest.x = Mathf.Clamp(closest.x, -extents.x, extents.x);
            //closest.y = Mathf.Clamp(closest.y, -extents.y, extents.y);

            //if (n == closest)
            //{
            //    closest = A.bounds.ClosestPoint(B.center);
            //}

            //Vector2 normal = n - closest;
            //float d = normal.sqrMagnitude;
            //float r = B.radius;

            //if (d > r * r)
            //    return false;

            //return true;
            #endregion
            #region 원과 직선의 교점으로 판단
            //return intersectCircle(B, A[0], A[1]) ||
            //    intersectCircle(B, A[1], A[2]) ||
            //    intersectCircle(B, A[2], A[3]) ||
            //    intersectCircle(B, A[3], A[0]);
            //// 직선의 방정식 => m = (y2 - y1)/(x2 - x1)
            //// y - y1 = m * x - x1
            //// => y = m * x - x1 + y1
            //// 원의 방정식 => (x - B.center.x)^2 + (y - B.center.y)^2 = B.radius^2
            //// 연립 방정식 =>
            //// (x - B.center.x)^2 + (m * x - x1 + y1 - B.center.y)^2 - B.radius^2 = 0
            //// (1 + m^2) * x^2 + (-2 * B.center.x - 2 * m * (-x1 + y1 - B.center.y)) * x + B.center.x^2 + (-x1 + y1 - B.center.y)^2 - B.radius^2 = 0
            //// a = (1 + m^2)
            //// b = (-2 * B.center.x - 2 * m * (-x1 + y1 - B.center.y))
            //// c = B.center.x^2 + (-x1 + y1 - B.center.y)^2 - B.radius^2
            //// c = (B.center.x^2) + (x1^2) - 2 * x1 * y1

            //bool intersectCircle(CircleCollider2D circle, Vector2 start, Vector2 end)
            //{
            //    if (end.x - start.x == 0)
            //        return false;

            //    float m = (end.y - start.y) / (end.x - start.x);

            //    float a = 1 + m * m;
            //    float b = (-2 * circle.center.x) - (2 * m * (-start.x + start.y - circle.center.y));
            //    float c = (B.center.x * B.center.x) + ((-start.x + start.y - B.center.y) * (-start.x + start.y - B.center.y)) - (B.radius * B.radius);

            //    return DiscriminantFormula(a, b, c);
            //}
            //bool DiscriminantFormula(float a, float b, float c)
            //{
            //    return b * b - 4 * a * c >= 0;
            //}
            #endregion
            #region 내 생각
            // 원의 중심과 사각형의 중심을 이은 벡터와의 교점을 구하여 교점끼리의 거리를 이용하여 계산
            // 해보려 했으나 감지가 잘못되는 상황이 너무 많았음.
            //Vector2 center_Box = A.bounds.center;
            //Vector2 center_Circle = B.center;

            //Vector2 minus = center_Box - center_Circle;
            //Vector2 normal = minus.normalized;

            //float radius_Circle = B.radius;
            //Vector2 closest_Circle = center_Circle + normal * radius_Circle;
            //Vector2 closest_Box = new Vector2();

            //for (int i = 0; i < 4; ++i)
            //{
            //    Vector2? edge = GetIntersectionOnBox(A[i], A[(i + 1) % 4], center_Circle, center_Box);

            //    if (edge.HasValue)
            //    {
            //        if (null != closest_Box &&
            //            Vector2.Distance(closest_Box, center_Circle) > Vector2.Distance(edge.Value, center_Circle))
            //        {
            //            closest_Box = edge.Value;
            //        }
            //    }
            //}

            //if (null == closest_Box)
            //    return true;

            //float radius_Box = (center_Box - closest_Box).magnitude;

            //float a = (center_Circle - closest_Box).magnitude;
            //float b = (center_Box - closest_Circle).magnitude;

            //if (a <= radius_Circle ||
            //    b <= radius_Box ||
            //    B.OverlapPoint(A[0]) ||
            //    B.OverlapPoint(A[1]) ||
            //    B.OverlapPoint(A[2]) ||
            //    B.OverlapPoint(A[3]))
            //    return true;

            //return false;

            //Vector2? GetIntersectionOnBox(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
            //{
            //    float t;
            //    float s;
            //    float under = (b2.y - b1.y) * (a2.x - a1.x) - (b2.x - b1.x) * (a2.y - a1.y);
            //    if (under == 0)
            //        return null;

            //    float _t = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
            //    float _s = (a2.x - a1.x) * (a1.y - b1.y) - (a2.y - a1.y) * (a1.x - b1.x);

            //    t = _t / under;
            //    s = _s / under;

            //    if (t < 0f || t > 1f || s < 0f || s > 1f)
            //        return null;
            //    if (_t == 0f && _s == 0f)
            //        return null;

            //    float x = a1.x + t * (a2.x - a1.x);
            //    float y = a1.y + t * (a2.y - a1.y);

            //    return new Vector2(x, y);
            //}
            #endregion
        }

        // 타입별 충돌 체크
        public static bool Type_Check_Collision(ref Collision2D collision)
        {
            Collider2D A = collision.collider;
            Collider2D B = collision.otherCollider;

            if (A.type == Collider2D.E_ColliderType.Box &&
                B.type == Collider2D.E_ColliderType.Box)
            {
                return OBB_Collision(ref collision);
            }
            else if (A.type == Collider2D.E_ColliderType.Circle &&
                B.type == Collider2D.E_ColliderType.Circle)
            {
                return Circle_Collision(ref collision);
            }
            else if ((A.type == Collider2D.E_ColliderType.Box && B.type == Collider2D.E_ColliderType.Circle) ||
                (A.type == Collider2D.E_ColliderType.Circle && B.type == Collider2D.E_ColliderType.Box))
            {
                if (A.type == Collider2D.E_ColliderType.Circle &&
                    B.type == Collider2D.E_ColliderType.Box)
                {
                    collision = new Collision2D(B, A);
                }

                return OBB_vs_Circle_Collision(ref collision);
            }

            return false;
        }
        #endregion
        //UnityEngine.Physics2D
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
            Vector2 dir_normal = direction.normalized;
            Vector2 C = origin;
            Vector2 D = origin + dir_normal * distance;
            float Cx = C.x, Dx = D.x;
            float Cy = C.y, Dy = D.y;

            float minR = float.MaxValue;
            Collider2D collider = null;
            foreach (var item in Physics2DManager.colliderList)
            {
                if (CheckLayerMask(item.gameObject.layer, layerMask))
                    continue;

                float Ax = item.bounds.min.x, Bx = item.bounds.max.x;
                float Ay = item.bounds.min.y, By = item.bounds.max.y;

                float r_up = (Bx - Ax) * (Cy - Ay) - (Cx - Ax) * (By - Ay);
                float r_down = (Dx - Cx) * (By - Ay) - (Bx - Ax) * (Dy - Cy);
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
        public static Collider2D OverlapPoint(Vector2 point)
        {
            foreach (var item in Physics2DManager.colliderList)
            {
                if (!item.OverlapPoint(point))
                {
                    continue;
                }

                return item;
            }

            return null;
        }
        public static Collider2D OverlapPoint(Vector2 point, int layerMask)
        {
            foreach (var item in Physics2DManager.colliderList)
            {
                if (CheckLayerMask(item.gameObject.layer, layerMask) ||
                    !item.OverlapPoint(point))
                {
                    continue;
                }

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
                {
                    continue;
                }

                colliders.Add(item);
            }

            if (colliders.Count <= 0)
                return null;

            return colliders.ToArray();
        }
        public static Collider2D[] OverlapPointAll(Vector2 point, int layerMask)
        {
            List<Collider2D> colliders = new List<Collider2D>();

            foreach (var item in Physics2DManager.colliderList)
            {
                if (CheckLayerMask(item.gameObject.layer, layerMask) ||
                    !item.OverlapPoint(point))
                {
                    continue;
                }

                colliders.Add(item);
            }

            return colliders.ToArray();
        }
        #endregion
    }
}