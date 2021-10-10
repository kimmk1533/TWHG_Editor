using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public sealed class Physics2DManager : Singleton<Physics2DManager>
    {
        private static List<Collider2D> m_ColliderList;
        private List<KeyValuePair<Collider2D, Collider2D>> m_HitColliderList;
        private event CollisionEventHandler m_OnCollisionEnter;

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
            DontDestroyOnLoad(gameObject);

            m_ColliderList = new List<Collider2D>();
            m_HitColliderList = new List<KeyValuePair<Collider2D, Collider2D>>();

            for (E_ObjectType i = E_ObjectType.Player; i < E_ObjectType.Max; ++i)
            {
                Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer(i.ToString()), -1);
            }
            Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("Player"), LayerMask.GetMask("Enemy", "Coin", "Wall", "SafetyZone", "GravityZone", "IceZone"));
            Physics2D.SetLayerCollisionMask(LayerMask.NameToLayer("Wall"), LayerMask.GetMask("Enemy", "Coin"));
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

                    if (!Physics2D.FirstCheckCollision(collider2D_A, collider2D_B))
                        continue;

                    if (Physics2D.TypeCollision(collider2D_A, collider2D_B))
                    {
                        m_HitColliderList.Add(new KeyValuePair<Collider2D, Collider2D>(collider2D_A, collider2D_B));
                        Debug.Log(collider2D_A.name + " 충돌 " + collider2D_B.name);
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