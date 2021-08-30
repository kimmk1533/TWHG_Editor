using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-97)]
public abstract class ObjectManager<Pool, Origin> : Singleton<Pool> where Pool : MonoBehaviour where Origin : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 100;

    public Dictionary<string, Origin> m_Origins = null;
    protected Dictionary<string, MemoryPool<Origin>> m_Pools = null;

    protected InGameManager M_Game => InGameManager.Instance;
    protected EditManager M_Edit => EditManager.Instance;
    protected ResourcesManager M_Resources => ResourcesManager.Instance;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public abstract void OnPlayEnter();
    public abstract void OnPlayExit();
    public virtual void __Initialize()
    {
        if (null == m_Origins)
        {
            m_Origins = new Dictionary<string, Origin>();
        }
        if (null == m_Pools)
        {
            m_Pools = new Dictionary<string, MemoryPool<Origin>>();
        }

        //GameObject[] origins = m_Resources.GetPrefabs(ResourcesType);

        //for (int i = 0; i < origins.Length; ++i)
        //{
        //    GameObject parent = new GameObject();
        //    parent.transform.SetParent(this.transform);
        //    parent.name = origins[i].name + "_parent";

        //    m_Pools.Add(new MemoryPool(origins[i], m_PoolSize, parent.transform));
        //}
    }

    public virtual void __Finalize()
    {
        if (m_Pools == null)
            return;

        foreach (var item in m_Pools)
        {
            item.Value?.Dispose();
        }

        m_Pools.Clear();
        m_Pools = null;
    }

    protected virtual bool AddPool(string key, Origin origin, Transform parent)
    {
        if (m_Origins.ContainsKey(key))
            return false;

        m_Origins.Add(key, origin);

        GameObject Parent = new GameObject();
        Parent.name = origin.name;
        Parent.transform.SetParent(parent);
        origin.transform.SetParent(Parent.transform);

        m_Pools.Add(key, new MemoryPool<Origin>(origin, m_PoolSize, Parent.transform));

        origin.name += "_Origin";

        //#if UNITY_EDITOR
        //        m_DebugOrigin.Add(key, origin);
        //#endif
        return true;
    }
    public virtual MemoryPool<Origin> GetPool(string key)
    {
        if (key == null)
            return null;

        if (m_Pools.ContainsKey(key))
            return m_Pools[key];

        return null;
    }
}
