using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-98)]
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField]
    protected bool flag;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                Singleton<T>[] objs = FindObjectsOfType<Singleton<T>>();

                GameObject obj = objs.Where(item => item.flag == true).FirstOrDefault()?.gameObject; //GameObject.Find(typeof(T).Name);
                if (obj == null)
                {
                    if (objs.Length > 0)
                        return objs[0].GetComponent<T>();
                    obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
                else
                {
                    instance = obj.GetComponent<T>();
                }
            }
            return instance;
        }
    }

    [ContextMenu("다른 켜져있는 싱글톤 모두 끄기")]
    private void SetFlagAllOff()
    {
        Singleton<T>[] objs = FindObjectsOfType<Singleton<T>>();
        Singleton<T>[] flags = objs.Where(item => item.flag == true).ToArray();
        foreach (var item in flags)
        {
            if (this != item)
            {
                item.flag = false;
            }
        }
    }
    private void OnValidate()
    {
        if (!flag)
            return;

        SetFlagAllOff();
    }
}

public class SingletonBasic<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
}