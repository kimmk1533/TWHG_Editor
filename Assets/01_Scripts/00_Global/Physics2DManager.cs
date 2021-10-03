using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public class Physics2DManager : Singleton<Physics2DManager>
    {
        protected List<Collider2D> m_MyColliderList;
        protected List<KeyValuePair<Collider2D, Collider2D>> m_MyColliderHitList;

        #region 외부 프로퍼티
        public List<Collider2D> myColliderList => m_MyColliderList; // { get => new List<MyCollider2D>(m_MyColliderList); }
        #endregion
        #region 유니티 콜백 함수
        void Awake()
        {
            m_MyColliderList = new List<Collider2D>();
            m_MyColliderHitList = new List<KeyValuePair<Collider2D, Collider2D>>();
        }

        void FixedUpdate()
        {
            m_MyColliderHitList.Clear();

            for (int i = 0; i < m_MyColliderList.Count; ++i)
            {
                Collider2D collider2D_A = m_MyColliderList[i];

                for (int j = i + 1; j < m_MyColliderList.Count; ++j)
                {
                    Collider2D collider2D_B = m_MyColliderList[j];

                    // 다이나믹 AABB 추가해야함
                    //if (!Physics2D.FirstCheckCollision(collider2D_A, collider2D_B))
                    //    continue;

                    if (Physics2D.TypeCollision(collider2D_A, collider2D_B))
                    {
                        m_MyColliderHitList.Add(new KeyValuePair<Collider2D, Collider2D>(collider2D_A, collider2D_B));
                        Debug.Log("충돌");
                    }
                }
            }
        }
        #endregion
    }
}