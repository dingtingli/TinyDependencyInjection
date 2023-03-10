// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;
using System.ComponentModel;

var register = new DependencyRegister();
register.AddTransient<A>();
register.AddTransient<IB, B>();
register.AddSingleton<IC, C>();
//register.AddTransient<IC, C>();

var container = new DependencyContainer(register);
var c = container.GetService<IC>();
var b = container.GetService<IB>();
var a = container.GetService<A>();

c.MethodC();
b.MethodB();
a.MethodA();

public class DependencyContainer
{
    DependencyRegister _container;

    public DependencyContainer(DependencyRegister register)
    {
        _container = register;
    }

    public T GetService<T>()
    {
        return (T)GetService(typeof(T));
    }

    public object GetService(Type type)
    {
        var dependency = _container.GetRegister(type);

        switch (dependency.Lifetime)
        {
            case DependencyLifetimeType.Transient:
                break;
            case DependencyLifetimeType.Singleton:
                var SingletonObject = _container.GetService(type);
                if (SingletonObject == null)
                {
                    break;
                }
                else
                {
                    return SingletonObject;
                }
            default: break;
        }

        var constructor = dependency.ServiceType.GetConstructors().Single();
        var parameters = constructor.GetParameters().ToArray();

        var parameterImplementations = new List<object>();

        if (parameters.Length > 0)
        {
            foreach (var parameter in parameters)
            {
                var parameterImplementation = GetService(parameter.ParameterType);
                parameterImplementations.Add(parameterImplementation);
            }
        }

        object result = null;
        result = Activator.CreateInstance(dependency.ServiceType, parameterImplementations.ToArray());

        switch (dependency.Lifetime)
        {
            case DependencyLifetimeType.Transient:
                break;
            case DependencyLifetimeType.Singleton:
                _container.SetService(type, result);
                break;
            default: break;
        }

        return result;
    }
}

public class DependencyRegister
{
    private ConcurrentDictionary<Type, DependencyType> _registers;

    private ConcurrentDictionary<Type, object?> _services;

    public DependencyRegister()
    {
        _registers = new ConcurrentDictionary<Type, DependencyType>();
        _services = new ConcurrentDictionary<Type, object>();
    }

    public void AddTransient<TFrom, TTo>()
    {
        var dependencyType = new DependencyType(typeof(TTo), DependencyLifetimeType.Transient);
        AddRegister(typeof(TFrom), dependencyType);
    }

    public void AddTransient<TFrom>()
    {
        var dependencyType = new DependencyType(typeof(TFrom), DependencyLifetimeType.Transient);
        AddRegister(typeof(TFrom), dependencyType);
    }

    public void AddSingleton<TFrom, TTo>()
    {
        var dependencyType = new DependencyType(typeof(TTo), DependencyLifetimeType.Singleton);
        AddRegister(typeof(TFrom), dependencyType);
    }

    public void AddSingleton<TFrom>()
    {
        var dependencyType = new DependencyType(typeof(TFrom), DependencyLifetimeType.Singleton);
        AddRegister(typeof(TFrom), dependencyType);
    }

    public void AddRegister(Type TFrom, DependencyType TTo)
    {
        _registers.TryAdd(TFrom, TTo);
    }

    public DependencyType? GetRegister(Type TFrom)
    {
        if (_registers.TryGetValue(TFrom, out var toType))
        {
            return toType;
        }

        return null;
    }

    public object? GetService(Type TFrom)
    {
        if (_services.TryGetValue(TFrom, out var toObject))
        {
            return toObject;
        }

        return null;
    }

    public void SetService(Type type, object value)
    {
        _services.TryAdd(type, value);
    }
}

public enum DependencyLifetimeType
{
    Singleton,
    //Scoped,
    Transient,
}

public class DependencyType
{
    public Type ServiceType { get; set; }

    public DependencyLifetimeType Lifetime { get; set; }

    public DependencyType(Type TTo, DependencyLifetimeType lifetime)
    {
        ServiceType = TTo;
        Lifetime = lifetime;
    }
}

//Test Services
class A
{
    private IB _b;

    public A(IB b)
    {
        _b = b;
    }

    public void MethodA()
    {
        Console.WriteLine("Class A -> Method A / before call Method B");
        _b.MethodB();
    }
}

interface IB
{
    void MethodB();
}

class B : IB
{
    private IC _c;

    public B(IC c)
    {
        _c = c;
    }
    public void MethodB()
    {
        Console.WriteLine("Class B -> Method B / before call Method C");
        _c.MethodC();
    }
}

interface IC
{
    void MethodC();
}

class C : IC
{
    int _random;

    public C()
    {
        _random = new Random().Next();
    }

    public void MethodC()
    {
        Console.WriteLine($"{_random} Class C -> Method C");
    }
}