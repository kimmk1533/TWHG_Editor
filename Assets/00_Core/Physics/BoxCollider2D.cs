using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPhysics
{
    public sealed class BoxCollider2D : Collider2D
    {
        [Space(10)]
        [SerializeField]
        private Vector2 m_Size = Vector2.one;

        #region 외부 프로퍼티
        public Vector2 size
        {
            get => m_Size;
            set { m_Bounds.size = m_Size = value; }
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
        public override Vector2 this[int index]
        {
            get
            {
                Vector2 min = -m_Bounds.extents;
                Vector2 max = m_Bounds.extents;

                float theta = (null != m_AttachedRigidbody) ? -m_AttachedRigidbody.rotation * Mathf.Deg2Rad :
                        ((null != this) ? -transform.eulerAngles.z * Mathf.Deg2Rad : 0f);
                float cos = Mathf.Cos(theta);
                float sin = Mathf.Sin(theta);

                Vector2 rotVec = new Vector2();
                Matrix4x4 rotMat = new Matrix4x4();
                rotMat.m00 = cos; rotMat.m01 = sin;
                rotMat.m10 = -sin; rotMat.m11 = cos;

                switch (index)
                {
                    case 0:
                        rotVec.x = min.x;
                        rotVec.y = max.y;
                        break;
                    case 1:
                        rotVec.x = max.x;
                        rotVec.y = max.y;
                        break;
                    case 2:
                        rotVec.x = max.x;
                        rotVec.y = min.y;
                        break;
                    case 3:
                        rotVec.x = min.x;
                        rotVec.y = min.y;
                        break;
                }

                Vector2 center = (Vector2)m_Bounds.center + m_Offset;
                Vector2 result = rotMat * rotVec;

                return center + result;
            }
        }
		#endregion
		#region 외부 함수
		public override bool OverlapPoint(Vector2 point)
        {
            float theta = transform.eulerAngles.z * Mathf.Deg2Rad;
            float cos = Mathf.Cos(theta);
            float sin = Mathf.Sin(theta);

            Matrix4x4 rotMat = new Matrix4x4();
            rotMat.m00 = cos; rotMat.m01 = -sin;
            rotMat.m10 = sin; rotMat.m11 = cos;

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; ++i)
            {
                Vector2 vertex = rotMat * this[i];

                if (vertex.x < min.x)
                    min.x = vertex.x;
                if (vertex.y < min.y)
                    min.y = vertex.y;

                if (vertex.x > max.x)
                    max.x = vertex.x;
                if (vertex.y > max.y)
                    max.y = vertex.y;
            }

            Vector2 rotPoint = rotMat * point;

            return min.x <= rotPoint.x && rotPoint.x <= max.x &&
                min.y <= rotPoint.y && rotPoint.y <= max.y;
        }
		public override Bounds GetBoundingBox()
        {
            Bounds bounds = new Bounds();
            
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; ++i)
            {
                Vector2 vertex = this[i];

                if (vertex.x < min.x)
                    min.x = vertex.x;
                if (vertex.y < min.y)
                    min.y = vertex.y;

                if (vertex.x > max.x)
                    max.x = vertex.x;
                if (vertex.y > max.y)
                    max.y = vertex.y;
            }

            bounds.SetMinMax(min, max);

            return bounds;
        }
        #endregion
        #region 유니티 콜백 함수
        private void Awake()
        {
            m_ColliderType = E_ColliderType.Box;
            Vector2 center = (Vector2)transform.localPosition + m_Offset;
            m_Bounds = new Bounds(center, m_Size);
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            Vector2 size = m_Size * transform.lossyScale;
            m_Bounds.size = size;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            Vector2 size = m_Size * transform.lossyScale;
            m_Bounds.size = size;
        }
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Color color = Gizmos.color;

            #region BoundingBox
            Gizmos.color = Physics2D.boundingBoxColor;

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < 4; ++i)
            {
                Vector2 vertex = this[i];

                if (vertex.x < min.x)
                    min.x = vertex.x;
                if (vertex.y < min.y)
                    min.y = vertex.y;

                if (vertex.x > max.x)
                    max.x = vertex.x;
                if (vertex.y > max.y)
                    max.y = vertex.y;
            }

            Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y));
            Gizmos.DrawLine(new Vector2(max.x, max.y), new Vector2(max.x, min.y));
            Gizmos.DrawLine(new Vector2(max.x, min.y), new Vector2(min.x, min.y));
            Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y));
            #endregion
            #region Collider
            Gizmos.color = Physics2D.colliderColor;

            Vector2[] vertexs = new Vector2[4]
            {
                this[0], this[1], this[2], this[3]
            };

            for (int i = 0; i < 4; ++i)
            {
                Vector2 from = vertexs[i];
                Vector2 to = vertexs[(i + 1) % 4];

                Gizmos.DrawLine(from, to);
            }
            #endregion

            Gizmos.color = color;
        }
        #endregion
    }
}