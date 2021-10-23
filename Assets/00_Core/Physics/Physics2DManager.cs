using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public sealed class Physics2DManager : Singleton<Physics2DManager>
    {
        private static List<Collider2D> m_ColliderList = new List<Collider2D>();
        private static List<Collision2D> m_HitColliderList = new List<Collision2D>();

        #region 외부 프로퍼티
        public static List<Collider2D> colliderList => m_ColliderList;
        #endregion
        #region 내부 함수
        private float PythagoreanSolve(float A, float B)
        {
            return Mathf.Sqrt(A * A + B * B);
        }
        private float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        private Vector2 Cross(Vector2 a, float s)
        {
            return new Vector2(s * a.y, -s * a.x);
        }
        private Vector2 Cross(float s, Vector2 a)
        {
            return new Vector2(-s * a.y, s * a.x);
        }
        #endregion
        #region 유니티 콜백 함수
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void FixedUpdate()
        {
            m_HitColliderList.Clear();

            #region 충돌 체크
            for (int i = 0; i < m_ColliderList.Count; ++i)
            {
                Collider2D collider2D_A = m_ColliderList[i];

                for (int j = i + 1; j < m_ColliderList.Count; ++j)
                {
                    Collider2D collider2D_B = m_ColliderList[j];

                    Collision2D collision = new Collision2D(collider2D_A, collider2D_B);

                    if (!Physics2D.First_Collision_Check(collision))
                        continue;

                    if (Physics2D.Type_Check_Collision(ref collision))
                    {
                        //if (collision.collider.isTrigger ||
                        //    collision.otherCollider.isTrigger)
                        //    continue;

                        m_HitColliderList.Add(collision);
                        Debug.Log("충돌\n" +
                            collider2D_A.transform.parent.name + ": " + collider2D_A.type + "\n" +
                            collider2D_B.transform.parent.name + ": " + collider2D_B.type);
                    }
                }
            }
            #endregion

            #region 충돌 처리
            foreach (var item in m_HitColliderList)
            {
                Rigidbody2D rigidbody_A = item.rigidbody;
                Rigidbody2D rigidbody_B = item.otherRigidbody;

                if (null == rigidbody_A)
                {
                    rigidbody_A = new Rigidbody2D();
                }
                if (null == rigidbody_B)
                {
                    rigidbody_B = new Rigidbody2D();
                }

                Vector2 rv = rigidbody_B.velocity - rigidbody_A.velocity;

                float velAlongNormal = Vector2.Dot(rv, item.normal);

                if (velAlongNormal > 0.0000001f)
                    return;

                float e = Mathf.Min(item.collider.bounciness, item.otherCollider.bounciness);

                float invMassA = 1f / rigidbody_A.mass;
                float invMassB = 1f / rigidbody_B.mass;

                float j = -(1f + e) * velAlongNormal;
                j /= invMassA + invMassB;

                Vector2 impulse = j * item.normal;
                if (!rigidbody_A.isKinematic)
                    rigidbody_A.velocity -= invMassA * impulse;
                if (!rigidbody_B.isKinematic)
                    rigidbody_B.velocity += invMassB * impulse;

                Vector2 tangent = rv - Vector2.Dot(rv, item.normal) * item.normal;
                tangent.Normalize();

                float jt = (-Vector2.Dot(rv, tangent)) / (invMassA + invMassB);

                float frictionA = item.collider.friction;
                float frictionB = item.otherCollider.friction;
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

                if (!rigidbody_A.isKinematic)
                    rigidbody_A.velocity -= invMassA * frictionImpulse;
                if (!rigidbody_B.isKinematic)
                    rigidbody_B.velocity += invMassB * frictionImpulse;

                const float percent = 1f;
                const float slop = 0.01f;
                Vector2 correction = Mathf.Max(item.penetration - slop, 0f) / (invMassA + invMassB) * percent * item.normal;

                if (null != rigidbody_A && !rigidbody_A.isKinematic)
                    rigidbody_A.transform.localPosition -= invMassA * (Vector3)correction;
                if (null != rigidbody_B && !rigidbody_B.isKinematic)
                    rigidbody_B.transform.localPosition += invMassB * (Vector3)correction;
            }
            #endregion
        }
        #endregion
    }
}