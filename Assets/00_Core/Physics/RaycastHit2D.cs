using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public struct RaycastHit2D
    {
        private Vector2 m_Centroid;
        private Vector2 m_Point;
        private Vector2 m_Normal;
        private float m_Distance;
        private Collider2D m_Collider;

        public RaycastHit2D(Collider2D collider)
        {
            m_Centroid = new Vector2();
            m_Point = new Vector2();
            m_Normal = new Vector2();
            m_Distance = 0f;

            m_Collider = collider;
        }

        #region 외부 프로퍼티
        public Vector2 centroid { get => m_Centroid; set => m_Centroid = value; }
        public Vector2 point { get => m_Point; set => m_Point = value; }
        public Vector2 normal { get => m_Normal; set => m_Normal = value; }
        public float distance { get => m_Distance; set => m_Distance = value; }
        public Collider2D collider { get => m_Collider; }
        public Rigidbody2D rigidbody { get => m_Collider?.attachedRigidbody; }
        public Transform transform { get => m_Collider?.transform; }
        #endregion
    }
}