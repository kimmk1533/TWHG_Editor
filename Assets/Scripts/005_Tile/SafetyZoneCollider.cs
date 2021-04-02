using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafetyZoneCollider : MonoBehaviour
{
    public PolygonCollider2D Polygon;
    public List<Vector2> indices;

    __GameManager m_Game;

    private void Awake()
    {
        m_Game = __GameManager.Instance;
    }

    public void __Initialize()
    {
        indices = new List<Vector2>();

        Polygon = GetComponent<PolygonCollider2D>();
        Polygon.isTrigger = true;
    }

    public Vector2 GetCenter()
    {
        Vector2[] vertexs = Polygon.points;
        return GetCenter(vertexs);
    }
    Vector2 GetCenter(Vector2[] vertexs)
    {
        float sum = 0f;
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
}
