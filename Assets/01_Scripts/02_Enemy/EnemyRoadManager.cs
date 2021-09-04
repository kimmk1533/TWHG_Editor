using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EnemyType
{
    Linear,
    Linear_Repeat,
    Circular,

    Max
}

public class EnemyRoadManager : Singleton<EnemyRoadManager>
{
    public E_EnemyType m_CurrentSelectedType;

    int m_PoolSize = 100;

    LinearRoad m_LinearRoad;
    CircularRoad m_CircularRoad;

    public List<Vector2> m_WayPoints;
    List<GameObject> m_LinearRoads;
    List<GameObject> m_CircularRoads;

    MemoryPool m_LinearRoadPool;
    MemoryPool m_CircularRoadPool;
    MemoryPool m_EnemyPool;

    protected __GameManager M_Game => __GameManager.Instance;

    public void OnPlayEnter()
    {
        for (int i = 0; i < m_LinearRoads.Count; ++i)
        {
            m_LinearRoads[i].GetComponent<LinearRoad>().OnPlayEnter();
        }
        for (int i = 0; i < m_CircularRoads.Count; ++i)
        {
            m_CircularRoads[i].GetComponent<CircularRoad>().OnPlayEnter();
        }
    }
    public void OnPlayExit()
    {
        for (int i = 0; i < m_LinearRoads.Count; ++i)
        {
            m_LinearRoads[i].GetComponent<LinearRoad>().OnPlayExit();
        }
        for (int i = 0; i < m_CircularRoads.Count; ++i)
        {
            m_CircularRoads[i].GetComponent<CircularRoad>().OnPlayExit();
        }
    }

    public void __Initialize()
    {
        M_Game.OnPlayEnter += OnPlayEnter;
        M_Game.OnPlayExit += OnPlayExit;

        m_LinearRoads = new List<GameObject>();
        m_CircularRoads = new List<GameObject>();

        GameObject origin, parent;

        origin = new GameObject();
        origin.AddComponent<LinearRoad>();
        origin.name = "LinearRoad";

        parent = new GameObject();
        parent.transform.SetParent(this.transform);
        parent.name = "LinearRoad_parent";

        m_LinearRoadPool = new MemoryPool(origin, m_PoolSize, parent.transform);

        Destroy(origin);

        origin = new GameObject();
        origin.AddComponent<CircularRoad>();
        origin.name = "CircularRoad";

        parent = new GameObject();
        parent.transform.SetParent(this.transform);
        parent.name = "CircularRoad_parent";

        m_CircularRoadPool = new MemoryPool(origin, m_PoolSize, parent.transform);

        Destroy(origin);

        m_WayPoints = new List<Vector2>();

        //m_EnemyPool = EnemyManager.Instance.GetPool("Enemy");
    }
    public void __Finalize()
    {
        ClearRoads();
    }

    void ClearRoads()
    {
        for (int i = m_LinearRoads.Count - 1; i >= 0; --i)
        {
            m_LinearRoads[i].GetComponent<LinearRoad>().Despawn();
            m_LinearRoadPool.DeSpawn(m_LinearRoads[i]);
        }
        for (int i = m_CircularRoads.Count - 1; i >= 0; --i)
        {
            m_CircularRoads[i].GetComponent<CircularRoad>().Despawn();
            m_CircularRoadPool.DeSpawn(m_CircularRoads[i]);
        }

        m_LinearRoads.Clear();
        m_CircularRoads.Clear();
    }

    public void CreateLinearRoad(Vector2[] waypoints, float speed, bool repeat = false)
    {
        GameObject enemy = m_EnemyPool.Spawn();
        enemy.transform.position = waypoints[0];
        enemy.SetActive(true);

        GameObject temproad = m_LinearRoadPool.Spawn();
        temproad.SetActive(true);

        m_LinearRoad = temproad.GetComponent<LinearRoad>();

        m_LinearRoad.m_InitPos = waypoints[0];
        //m_LinearRoad.m_Enemy = enemy;
        m_LinearRoad.m_Speed = speed;
        for (int i = 0; i < waypoints.Length; ++i)
        {
            m_LinearRoad.AddWayPoint(waypoints[i]);
        }
        m_LinearRoad.m_Repeat = repeat;

        m_LinearRoads.Add(temproad);
    }
    public void CreateCircularRoad(Vector2 startpos, Vector2 center, float speed)
    {
        GameObject enemy = m_EnemyPool.Spawn();
        enemy.transform.position = startpos;
        enemy.SetActive(true);

        GameObject temproad = m_CircularRoadPool.Spawn();
        temproad.SetActive(true);

        m_CircularRoad = temproad.GetComponent<CircularRoad>();

        m_CircularRoad.m_InitPos = startpos;
        //m_CircularRoad.m_Enemy = enemy;
        m_CircularRoad.m_Speed = speed;
        m_CircularRoad.m_Center = center;

        m_CircularRoads.Add(temproad);
    }
    public void RemoveRoad(GameObject enemy)
    {
        for (int i = 0; i < m_LinearRoads.Count; ++i)
        {
            m_LinearRoad = m_LinearRoads[i].GetComponent<LinearRoad>();
            if (m_LinearRoad.m_Enemy == enemy)
            {
                m_LinearRoad.Despawn();
                m_LinearRoadPool.DeSpawn(m_LinearRoads[i]);
                m_LinearRoads.RemoveAt(i);
            }
        }
        for (int i = 0; i < m_CircularRoads.Count; ++i)
        {
            m_CircularRoad = m_CircularRoads[i].GetComponent<CircularRoad>();
            if (m_CircularRoad.m_Enemy == enemy)
            {
                m_CircularRoad.Despawn();
                m_CircularRoadPool.DeSpawn(m_CircularRoads[i]);
                m_CircularRoads.RemoveAt(i);
            }
        }
    }
}
