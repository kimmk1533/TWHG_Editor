using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneCollider : MonoBehaviour
{
    public PolygonCollider2D m_Polygon;
    public List<Vector2> m_Indices;

    public void __Initialize()
    {
        if (null == m_Indices)
        {
            m_Indices = new List<Vector2>();
        }

        if (null == m_Polygon)
        {
            m_Polygon = GetComponent<PolygonCollider2D>();
        }
    }

    public Vector2 GetCenter()
    {
        Vector2[] vertexs = m_Polygon.points;
        return GetCenter(vertexs);
    }
    Vector2 GetCenter(Vector2[] vertexs)
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
    }

    //Vector2 GetRandomPos()
    //{
    //    m_Polygon.
    //}
}
