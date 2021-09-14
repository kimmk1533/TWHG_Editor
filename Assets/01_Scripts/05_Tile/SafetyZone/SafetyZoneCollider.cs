using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneCollider : MonoBehaviour, IEraserable, IObjectType
{
    protected SafetyZone m_SafetyZone;
    protected BoxCollider2D m_Collider;

    #region 내부 프로퍼티
    protected __EditManager M_Edit => __EditManager.Instance;

    protected PlayerManager M_Player => PlayerManager.Instance;
    protected SafetyZoneManager M_SafetyZone => SafetyZoneManager.Instance;
    #endregion
    #region 외부 프로퍼티
    public BoxCollider2D Collider => m_Collider;
    public bool isFinishZone => m_SafetyZone.isFinishZone;
    #endregion
    #region 외부 함수
    public void __Initialize(SafetyZone safetyZone)
    {
        m_SafetyZone = safetyZone;

        if (null == m_Collider)
        {
            m_Collider = GetComponent<BoxCollider2D>();
        }
    }

    public void Erase()
    {
        M_SafetyZone.DespawnSafetyZone(m_SafetyZone);
    }
    public GameObject GetGameObject()
    {
        return gameObject;
    }
    public E_ObjectType GetObjectType()
    {
        return E_ObjectType.SafetyZone;
    }
    #endregion
    #region 유니티 콜백 함수
    #endregion

    #region 기존 함수
    /*public Vector2 GetCenter()
    {
        Vector2[] vertexs = m_Polygon.points; // (BoxCollider2D -> PolygonCollider)
        return GetCenter(vertexs);
    }*/
    /*Vector2 GetCenter(Vector2[] vertexs)
    {
        float sum;
        float Area = 0f;
        Vector2 result = new Vector2();

        for (int i = 0; i < vertexs.Length; ++i)
        {
            int index = (i + 1) % vertexs.Length;
            sum = (vertexs[i].x * vertexs[index].y) - (vertexs[index].x * vertexs[i].y);
            Area += sum;
            result += new Vector2((vertexs[i].x + vertexs[index].x) * sum, (vertexs[i].y + vertexs[index].y) * sum);
        }

        Area *= 0.5f;
        Area *= 6f;

        sum = (1f / Area);

        return result * sum;
    }*/
    #endregion
}
