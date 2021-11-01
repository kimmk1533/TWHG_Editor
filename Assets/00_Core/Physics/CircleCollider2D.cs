using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public sealed class CircleCollider2D : Collider2D
    {
        [Space(10)]
        [SerializeField]
        private float m_Radius = 0.5f;

        #region 내부 프로퍼티
        private float diameter => m_Radius * 2f;
        private float scaledRadius => m_Radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
        private float scaledDiameter => scaledRadius * 2f;
        #endregion
        #region 외부 프로퍼티
        public float radius
        {
            get => m_Radius;
            set
            {
                m_Radius = value;
                m_Bounds.size = Vector2.one * scaledDiameter;
            }
        }
        public Vector2 center
        {
            get => m_Bounds.center;
            set
            {
                m_Bounds.center = value;

                if (null != this)
                {
                    transform.position = value - m_Offset;
                }
            }
        }
        public override Vector2 this[int angle]
        {
            get
            {
                float rot = (null != m_AttachedRigidbody) ? m_AttachedRigidbody.rotation :
                        ((null != this) ? transform.eulerAngles.z : 0f);
                float x = m_Bounds.center.x + Mathf.Cos(Mathf.Deg2Rad * (angle + rot)) * scaledRadius;
                float y = m_Bounds.center.y + Mathf.Sin(Mathf.Deg2Rad * (angle + rot)) * scaledRadius;
                Vector2 max = new Vector2(x, y);

                return max;
            }
        }
        #endregion
        #region 외부 함수
        public override bool OverlapPoint(Vector2 point)
        {
            Vector2 Distance = (Vector2)m_Bounds.center - point;

            return m_Radius * m_Radius > Distance.sqrMagnitude;
        }
        public override Bounds GetBoundingBox()
        {
            return m_Bounds;
        }
        #endregion
        #region 유니티 콜백 함수
        private void Awake()
        {
            m_ColliderType = E_ColliderType.Circle;
            Vector2 center = (Vector2)transform.localPosition + m_Offset;
            Vector2 size = Vector2.one * scaledDiameter;
            m_Bounds = new Bounds(center, size);
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            Vector2 size = Vector2.one * scaledDiameter;
            m_Bounds.size = size;
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Color color = Gizmos.color;

            #region BoundingBox
            Gizmos.color = Physics2D.boundingBoxColor;

            Vector2 min = m_Bounds.min;
            Vector2 max = m_Bounds.max;

            Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y));
            Gizmos.DrawLine(new Vector2(max.x, max.y), new Vector2(max.x, min.y));
            Gizmos.DrawLine(new Vector2(max.x, min.y), new Vector2(min.x, min.y));
            Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y));
            #endregion
            #region Collider
            Gizmos.color = Physics2D.colliderColor;

            for (int angle = 0; angle < 360; ++angle)
            {
                float from_x = m_Bounds.center.x + Mathf.Cos(Mathf.Deg2Rad * (angle + transform.eulerAngles.z)) * scaledRadius;
                float from_y = m_Bounds.center.y + Mathf.Sin(Mathf.Deg2Rad * (angle + transform.eulerAngles.z)) * scaledRadius;
                Vector2 from = new Vector2(from_x, from_y);

                float to_x = m_Bounds.center.x + Mathf.Cos(Mathf.Deg2Rad * (angle + 1 + transform.eulerAngles.z)) * scaledRadius;
                float to_y = m_Bounds.center.y + Mathf.Sin(Mathf.Deg2Rad * (angle + 1 + transform.eulerAngles.z)) * scaledRadius;
                Vector2 to = new Vector2(to_x, to_y);

                Gizmos.DrawLine(from, to);
            }
            Gizmos.DrawLine(m_Bounds.center, (Vector2)m_Bounds.center + GetRightVector().normalized * scaledRadius);
            #endregion

            Gizmos.color = color;
        }
        #endregion
    }
}