using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectManager<Pool, Origin> : Singleton<Pool> where Pool : MonoBehaviour where Origin : MonoBehaviour
{
    [ReadOnly(true)]
    protected int m_PoolSize = 100;

    protected Dictionary<string, Origin> m_Origins = null;
    protected Dictionary<string, MemoryPool<Origin>> m_Pools = null;

    #region 내부 프로퍼티
    #region 매니저
    protected __GameManager M_Game => __GameManager.Instance;
    protected __EditManager M_Edit => __EditManager.Instance;
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    #endregion
    #endregion
    #region 내부 함수
    protected virtual bool AddPool(string key, Origin origin, Transform parent)
    {
        if (m_Origins.ContainsKey(key))
            return false;

        Origin origin_clone = Object.Instantiate<Origin>(origin);
        origin_clone.name = origin_clone.name.Split('(')[0];
        origin_clone.gameObject.SetActive(false);
        m_Origins.Add(key, origin_clone);

        GameObject Parent = new GameObject();
        Parent.name = origin_clone.name + " Pool";
        Parent.transform.SetParent(parent);
        origin_clone.transform.SetParent(Parent.transform);

        m_Pools.Add(key, new MemoryPool<Origin>(origin_clone, m_PoolSize, Parent.transform));

        origin_clone.name += " Origin";

        //#if UNITY_EDITOR
        //        m_DebugOrigin.Add(key, origin);
        //#endif
        return true;
    }
    protected virtual MemoryPool<Origin> GetPool(string key)
    {
        if (key == null)
            return null;

        if (m_Pools.ContainsKey(key))
            return m_Pools[key];

        return null;
    }
    #endregion
    #region 외부 함수
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
        //if (m_Pools == null)
        //    return;

        //foreach (var item in m_Pools)
        //{
        //    item.Value?.Dispose();
        //}

        //m_Pools.Clear();
        //m_Pools = null;
    }
    #endregion
}