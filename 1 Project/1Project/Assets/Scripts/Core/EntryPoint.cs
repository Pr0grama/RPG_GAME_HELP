using System;
using System.Collections.Generic;
using UnityEngine;


public interface IService
{
    void Initialize();
    void Cleanup();
}

public abstract class EntryPoint : MonoBehaviour
{
    protected ServiceLocator services;

    public virtual void Initialize(ServiceLocator serviceLocator)
    {
        services = serviceLocator;
    }

    public abstract void Run();

    public virtual void Cleanup() { }
}

public class ServiceLocator
{
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public void Register<T>(T service) where T : class
    {
        services[typeof(T)] = service;
    }

    public T Get<T>() where T : class
    {
        if (services.TryGetValue(typeof(T), out object service))
            return service as T;
        return null;
    }

    public void Unregister<T>() where T : class
    {
        services.Remove(typeof(T));
    }
}