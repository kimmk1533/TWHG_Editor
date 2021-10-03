using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public class Physics2D
    {
        protected static float m_Gravity = -9.81f;
        protected static Color m_ColliderColor = new Color(145f / 255f, 244f / 255f, 139f / 255f, 192f / 255f);
        protected static Color m_BoundingBoxColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 255f / 255f);

        public static float gravity { get => m_Gravity; set => m_Gravity = value; }
        public static Color colliderColor { get => m_ColliderColor; set => m_ColliderColor = value; }
        public static Color boundingBoxColor { get => m_BoundingBoxColor; set => m_BoundingBoxColor = value; }

        // Collision https://tt91.tistory.com/57
        // ���� �˻� (���� ���̳��� AABB�� ����)
        public static bool FirstCheckCollision(Collider2D A, Collider2D B)
        {
            Vector2 CenterDistance = B.bounds.center - A.bounds.center;

            Vector2 A_min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 A_max = new Vector2(float.MinValue, float.MinValue);

            Vector2 B_min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 B_max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; ++i)
            {
                if (A[i].x < A_min.x)
                    A_min.x = A[i].x;
                if (A[i].y < A_min.y)
                    A_min.y = A[i].y;

                if (A[i].x > A_max.x)
                    A_max.x = A[i].x;
                if (A[i].y > A_max.y)
                    A_max.y = A[i].y;

                if (B[i].x < B_min.x)
                    B_min.x = B[i].x;
                if (B[i].y < B_min.y)
                    B_min.y = B[i].y;

                if (B[i].x > B_max.x)
                    B_max.x = B[i].x;
                if (B[i].y > B_max.y)
                    B_max.y = B[i].y;
            }

            Vector2 A_Extents = (A_max - A_min) * 0.5f;
            Vector2 B_Extents = (B_max - B_min) * 0.5f;
            Vector2 Distance = A_Extents + B_Extents;

            if (Mathf.Abs(CenterDistance.x) <= Mathf.Abs(Distance.x) &&
                Mathf.Abs(CenterDistance.y) <= Mathf.Abs(Distance.y))
            {

                return true;
            }

            return false;
        }

        // AABB (Axis Aligned Bounding Box)
        public static bool AABBCollision(Collider2D A, Collider2D B)
        {
            if (A.bounds.max.x < B.bounds.min.x || A.bounds.min.x > B.bounds.max.x) return false;
            if (A.bounds.max.y < B.bounds.min.y || A.bounds.min.y > B.bounds.max.y) return false;

            return true;
        }
        // OBB (Oriented Bounding Box)
        public static bool OBBCollision(Collider2D A, Collider2D B)
        {
            // ������ �� �� ��
            Vector2[] edges = new Vector2[4];
            // ������ �� ������ ����
            float[] m = new float[4];
            // ������ �� ����
            Vector2[] normals = new Vector2[4];
            // ������ �� ��
            Vector2[] axises = new Vector2[4];

            // �� �� ����
            edges[0] = A[2] - A[3];
            edges[1] = A[1] - A[2];
            edges[2] = B[2] - B[3];
            edges[3] = B[1] - B[2];

            // ������ �� �� ����
            for (int i = 0; i < 4; ++i)
            {
                // ������ ������ ���� ���
                if (edges[i].x == 0)
                {
                    normals[i] = Vector2.right;
                }
                else if (edges[i].y == 0)
                {
                    normals[i] = Vector2.up;
                }
                else
                {
                    m[i] = -1f / (edges[i].y / edges[i].x);

                    // ������ ���� ����
                    normals[i] = new Vector2(1f, m[i]);
                }

                // ������ ����
                axises[i] = normals[i].normalized;
            }

            float min_A, max_A;
            float min_B, max_B;

            // OBB üũ
            for (int i = 0; i < 4; ++i)
            {
                min_A = float.MaxValue;
                max_A = float.MinValue;

                min_B = float.MaxValue;
                max_B = float.MinValue;

                // SAT (Separating Axis Theorem)
                // �и��� �˻�
                for (int j = 0; j < 4; ++j)
                {
                    float projection_A = Vector2.Dot(A[j], axises[i]);
                    min_A = Mathf.Min(min_A, projection_A);
                    max_A = Mathf.Max(max_A, projection_A);

                    float projection_B = Vector2.Dot(B[j], axises[i]);
                    min_B = Mathf.Min(min_B, projection_B);
                    max_B = Mathf.Max(max_B, projection_B);
                }

                // �и����� ������ ���
                if (max_A < min_B || min_A > max_B)
                    return false; // �浹 �� ��
            }

            // �и����� �������� �ʴ� ���
            return true; // �浹
        }

        // BoundingBox vs BoundingBox
        public static bool BoundingBoxVSBoundingBoxCollision(Collider2D A, Collider2D B)
        {
            return false;
        }
        // Circle vs Circle
        public static bool CircleVSCircleCollision(CircleCollider2D A, CircleCollider2D B)
        {
            Vector2 AtoB = B.center - A.center;
            float radius = A.radius + B.radius;

            return radius * radius > AtoB.sqrMagnitude;
        }
        // BoundingBox vs Circle
        public static bool BoundingBoxVSCircleCollision(Collider2D A, CircleCollider2D B)
        {
            // ȸ���� ���� ���ؼ�
            float theta = A.transform.eulerAngles.z * Mathf.Deg2Rad;

            // �ݴ� �������� ȸ�������ִ� (ȸ���Ѱ� ��������) ��� ����
            Matrix4x4 rotMat = Matrix4x4.identity;
            if (theta != 0f)
            {
                float sin = Mathf.Sin(theta);
                float cos = Mathf.Cos(theta);
                rotMat.m00 = cos; rotMat.m01 = sin;
                rotMat.m10 = -sin; rotMat.m11 = cos;
            }

            // �浹 �˻��ϴ� ������ ��� ȸ�� ����� ���Ͽ� �� ���Ľ�Ŵ
            Vector2 circleCenter = rotMat * B.center;
            Vector2[] vertices = new Vector2[4];
            for (int i = 0; i < 4; ++i)
            {
                vertices[i] = rotMat * A[i];
            }

            // ������ �簢���� ���� ����� ���� ã��
            Vector2 closest = new Vector2();
            closest.x = Mathf.Clamp(circleCenter.x, vertices[0].x, vertices[1].x);
            closest.y = Mathf.Clamp(circleCenter.y, vertices[2].y, vertices[0].y);

            // ���� ���� ����� ������ �Ÿ� ���
            Vector2 distance = circleCenter - closest;
            // ��Ʈ�� ���ſ�Ƿ� �����Ͽ� ��
            float distanceSquared = Vector2.Dot(distance, distance);
            // ���� �������� �������� ���� ��� �浹 (=�� ������ ���ϴ� ������ �浹)
            return distanceSquared < B.radius * B.radius;

            // ������ �����
            #region https://gamedevelopment.tutsplus.com/tutorials/how-to-create-a-custom-2d-physics-engine-the-basics-and-impulse-resolution--gamedev-6331
            // ������ ����� �Ȱ��� ���������, ���� ��� ������ ���� ���Ͽ�����,
            // ȸ���� ���� �������� �ʾ� ����� �浹 �˻簡 �Ͼ�� ����.
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
            #region ���� ������ �������� �Ǵ�
            //return intersectCircle(B, A[0], A[1]) ||
            //    intersectCircle(B, A[1], A[2]) ||
            //    intersectCircle(B, A[2], A[3]) ||
            //    intersectCircle(B, A[3], A[0]);
            //// ������ ������ => m = (y2 - y1)/(x2 - x1)
            //// y - y1 = m * x - x1
            //// => y = m * x - x1 + y1
            //// ���� ������ => (x - B.center.x)^2 + (y - B.center.y)^2 = B.radius^2
            //// ���� ������ =>
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
            #region �� ����
            // ���� �߽ɰ� �簢���� �߽��� ���� ���Ϳ��� ������ ���Ͽ� ���������� �Ÿ��� �̿��Ͽ� ���
            // �غ��� ������ ������ �߸��Ǵ� ��Ȳ�� �ʹ� ������.
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
        public static bool BoundingBoxVSCircleCollision(Collider2D A, Collider2D B)
        {
            if (A.type == Collider2D.E_ColliderType.Box && B.type == Collider2D.E_ColliderType.Circle)
                return BoundingBoxVSCircleCollision(A, B as CircleCollider2D);
            else if (A.type == Collider2D.E_ColliderType.Circle && B.type == Collider2D.E_ColliderType.Box)
                return BoundingBoxVSCircleCollision(B, A as CircleCollider2D);

            Debug.LogError("�浹 Ÿ�� �߸� ����");
            return false;
        }

        public static bool TypeCollision(Collider2D A, Collider2D B)
        {
            if (A.type == Collider2D.E_ColliderType.Box &&
                B.type == Collider2D.E_ColliderType.Box)
            {
                return OBBCollision(A, B);
            }
            else if (A.type == Collider2D.E_ColliderType.Circle &&
                B.type == Collider2D.E_ColliderType.Circle)
            {
                return CircleVSCircleCollision(A as CircleCollider2D, B as CircleCollider2D);
            }
            else if ((A.type == Collider2D.E_ColliderType.Box && B.type == Collider2D.E_ColliderType.Circle) ||
                (A.type == Collider2D.E_ColliderType.Circle && B.type == Collider2D.E_ColliderType.Box))
            {
                return BoundingBoxVSCircleCollision(A, B);
            }

            return OBBCollision(A, B);
        }
    }
}