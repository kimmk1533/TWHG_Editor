using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public class Physics2DManager : Singleton<Physics2DManager>
    {
        protected static List<Collider2D> m_ColliderList;
        protected List<KeyValuePair<Collider2D, Collider2D>> m_HitColliderList;
        protected event CollisionEventHandler m_OnCollisionEnter;

        #region 외부 프로퍼티
        public static List<Collider2D> colliderList => m_ColliderList;
        public event CollisionEventHandler onCollisionEnter
        {
            add => m_OnCollisionEnter += value;
            remove => m_OnCollisionEnter -= value;
        }
        #endregion
        #region 유니티 콜백 함수
        void Awake()
        {
            m_ColliderList = new List<Collider2D>();
            m_HitColliderList = new List<KeyValuePair<Collider2D, Collider2D>>();
        }

        void FixedUpdate()
        {
            m_HitColliderList.Clear();

            for (int i = 0; i < m_ColliderList.Count; ++i)
            {
                Collider2D collider2D_A = m_ColliderList[i];

                for (int j = i + 1; j < m_ColliderList.Count; ++j)
                {
                    Collider2D collider2D_B = m_ColliderList[j];

                    // 다이나믹 AABB 추가해야함
                    //if (!Physics2D.FirstCheckCollision(collider2D_A, collider2D_B))
                    //    continue;

                    if (Physics2D.TypeCollision(collider2D_A, collider2D_B))
                    {
                        m_HitColliderList.Add(new KeyValuePair<Collider2D, Collider2D>(collider2D_A, collider2D_B));
                        Debug.Log(collider2D_A + " 충돌 " + collider2D_B);
                    }
                }
            }
        }
        #endregion

        public delegate void CollisionEventHandler(CollisionEventArgs args);
        public class CollisionEventArgs : EventArgs
        {
            public Collider2D A { get; set; }
            public Collider2D B { get; set; }
        }
    }
}