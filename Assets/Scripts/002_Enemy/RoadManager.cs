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

public class RoadManager : Singleton<RoadManager>
{
    public E_EnemyType currentSelectedType;

    int PoolSize = 100;

    LinearRoad linearRoad;
    CircularRoad circularRoad;

    public List<Vector2> WayPoints;
    List<GameObject> linearRoads;
    List<GameObject> circularRoads;

    MemoryPool LinearRoadPool;
    MemoryPool CircularRoadPool;
    MemoryPool EnemyPool;

    public void OnPlayEnter()
    {
        for (int i = 0; i < linearRoads.Count; ++i)
        {
            linearRoads[i].GetComponent<LinearRoad>().OnPlayEnter();
        }
        for (int i = 0; i < circularRoads.Count; ++i)
        {
            circularRoads[i].GetComponent<CircularRoad>().OnPlayEnter();
        }
    }
    public void OnPlayExit()
    {
        for (int i = 0; i < linearRoads.Count; ++i)
        {
            linearRoads[i].GetComponent<LinearRoad>().OnPlayExit();
        }
        for (int i = 0; i < circularRoads.Count; ++i)
        {
            circularRoads[i].GetComponent<CircularRoad>().OnPlayExit();
        }
    }

    public void __Initialize()
    {
        linearRoads = new List<GameObject>();
        circularRoads = new List<GameObject>();

        GameObject origin, parent;

        origin = new GameObject();
        origin.AddComponent<LinearRoad>();
        origin.name = "LinearRoad";

        parent = new GameObject();
        parent.transform.SetParent(this.transform);
        parent.name = "LinearRoad_parent";

        LinearRoadPool = new MemoryPool(origin, PoolSize, parent.transform);

        Destroy(origin);

        origin = new GameObject();
        origin.AddComponent<CircularRoad>();
        origin.name = "CircularRoad";

        parent = new GameObject();
        parent.transform.SetParent(this.transform);
        parent.name = "CircularRoad_parent";

        CircularRoadPool = new MemoryPool(origin, PoolSize, parent.transform);

        Destroy(origin);

        WayPoints = new List<Vector2>();

        EnemyPool = EnemyManager.Instance.GetPool();
    }
    public void __Finalize()
    {
        ClearRoads();
    }

    void ClearRoads()
    {
        for (int i = linearRoads.Count - 1; i >= 0; --i)
        {
            linearRoads[i].GetComponent<LinearRoad>().Despawn();
            LinearRoadPool.DeSpawn(linearRoads[i]);
        }
        for (int i = circularRoads.Count - 1; i >= 0; --i)
        {
            circularRoads[i].GetComponent<CircularRoad>().Despawn();
            CircularRoadPool.DeSpawn(circularRoads[i]);
        }

        linearRoads.Clear();
        circularRoads.Clear();
    }

    public void CreateLinearRoad(Vector2[] waypoints, float speed, bool repeat = false)
    {
        GameObject enemy = EnemyPool.Spawn();
        enemy.transform.position = waypoints[0];
        enemy.SetActive(true);

        GameObject temproad = LinearRoadPool.Spawn();
        temproad.SetActive(true);

        linearRoad = temproad.GetComponent<LinearRoad>();

        linearRoad.InitPos = waypoints[0];
        linearRoad.Enemy = enemy;
        linearRoad.Speed = speed;
        for (int i = 0; i < waypoints.Length; ++i)
        {
            linearRoad.AddWayPoint(waypoints[i]);
        }
        linearRoad.Repeat = repeat;

        linearRoads.Add(temproad);
    }
    public void CreateCircularRoad(Vector2 startpos, Vector2 center, float speed)
    {
        GameObject enemy = EnemyPool.Spawn();
        enemy.transform.position = startpos;
        enemy.SetActive(true);

        GameObject temproad = CircularRoadPool.Spawn();
        temproad.SetActive(true);

        circularRoad = temproad.GetComponent<CircularRoad>();

        circularRoad.InitPos = startpos;
        circularRoad.Enemy = enemy;
        circularRoad.Speed = speed;
        circularRoad.Center = center;

        circularRoads.Add(temproad);
    }
    public void RemoveRoad(GameObject enemy)
    {
        for (int i = 0; i < linearRoads.Count; ++i)
        {
            linearRoad = linearRoads[i].GetComponent<LinearRoad>();
            if (linearRoad.Enemy == enemy)
            {
                linearRoad.Despawn();
                LinearRoadPool.DeSpawn(linearRoads[i]);
                linearRoads.RemoveAt(i);
            }
        }
        for (int i = 0; i < circularRoads.Count; ++i)
        {
            circularRoad = circularRoads[i].GetComponent<CircularRoad>();
            if (circularRoad.Enemy == enemy)
            {
                circularRoad.Despawn();
                CircularRoadPool.DeSpawn(circularRoads[i]);
                circularRoads.RemoveAt(i);
            }
        }
    }
}
