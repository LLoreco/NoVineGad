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
            Debug.LogWarning($"Сервис {type.Name} уже существует. Перезаписываем...");
        }

        services[type] = service;
        Debug.Log($"Сервис {type.Name} зарегистрирован");
    }

    public T GetService<T>()
    {
        System.Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return (T)service;
        }

        Debug.LogError($"Сервис {type.Name} не найден!");
        return default(T);
    }

    public bool HasService<T>()
    {
        return services.ContainsKey(typeof(T));
    }
}
