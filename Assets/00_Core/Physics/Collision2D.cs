using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public class Collision2D
    {
        public Collision2D(Collider2D A, Collider2D B)
        {
            m_Collider = A;
            m_OtherCollider = B;
        }

        private Collider2D m_Collider;
        private Collider2D m_OtherCollider;
        private float m_Penetration;
        private Vector2 m_Normal;

        public Collider2D collider { get => m_Collider; }
        public Collider2D otherCollider { get => m_OtherCollider; }
        public Rigidbody2D rigidbody { get => m_Collider.attachedRigidbody; }
        public Rigidbody2D otherRigidbody { get => m_OtherCollider.attachedRigidbody; }
        public float penetration { get => m_Penetration; set => m_Penetration = value; }
        public Vector2 normal { get => m_Normal; set => m_Normal = value; }
    }
}