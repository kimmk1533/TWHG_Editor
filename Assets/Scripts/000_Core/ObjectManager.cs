using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectManager<T> : Singleton<T> where T : MonoBehaviour
{
    [ReadOnly(true)]
    public E_ResourcesType ResourcesType;
    [ReadOnly(true)]
    public int PoolSize = 1000;
    protected List<MemoryPool> Pools;

    protected __GameManager m_Game = null;
    protected ResourcesManager m_Resources = null;

    protected virtual void Awake()
    {
        m_Game = __GameManager.Instance;
        m_Resources = ResourcesManager.Instance;
    }

    public abstract void OnPlayEnter();
    public abstract void OnPlayExit();
    public virtual void __Initialize()
    {
        Pools = new List<MemoryPool>();

        GameObject[] origins = m_Resources.GetPrefabs(ResourcesType);

        for (int i = 0; i < origins.Length; ++i)
        {
            GameObject parent = new GameObject();
            parent.transform.SetParent(this.transform);
            parent.name = origins[i].name + "_parent";

            Pools.Add(new MemoryPool(origins[i], PoolSize, parent.transform));
        }
    }

    public virtual void __Finalize()
    {
        for (int i = Pools.Count - 1; i >= 0; --i)
        {
            Pools[i].Dispose();
        }
        Pools.Clear();
        Pools = null;
    }

    public virtual MemoryPool GetPool(int index = 0)
    {
        return Pools[index];
    }
}
