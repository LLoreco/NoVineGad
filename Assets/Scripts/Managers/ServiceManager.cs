using System.Collections.Generic;
using UnityEngine;

public class ServiceManager : MonoBehaviour
{
    private static ServiceManager instance;
    public static ServiceManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("ServiceLocator");
                instance = go.AddComponent<ServiceManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }
    private Dictionary<System.Type, object> services = new Dictionary<System.Type, object>();

    public void RegisterService<T>(T service)
    {
        System.Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            Debug.LogWarning($"������ {type.Name} ��� ����������. ��������������...");
        }

        services[type] = service;
        Debug.Log($"������ {type.Name} ���������������");
    }

    public T GetService<T>()
    {
        System.Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }

        Debug.LogError($"������ {type.Name} �� ������!");
        return default(T);
    }

    public bool HasService<T>()
    {
        return services.ContainsKey(typeof(T));
    }
}
