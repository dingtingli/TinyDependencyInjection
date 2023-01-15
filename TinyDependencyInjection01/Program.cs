// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;

var container = new DependencyRegister();
container.AddRegister<A>();
container.AddRegister<IB,B>();

var resolver = new DependencyContainer(container);
var b = resolver.GetService<IB>();
var a = resolver.GetService<A>();

b.MethodB();
//a.MethodA();//error

public class DependencyContainer
{
    DependencyRegister _container;

    public DependencyContainer(DependencyRegister container)
    {
        _container = container;
    } 
    
    public T GetService<T>()
    {
        var type = _container.GetRegister(typeof(T));
        return (T)Activator.CreateInstance(type);
    }
}

public class DependencyRegister
{
    private ConcurrentDictionary<Type, Type> _registers;

    public DependencyRegister()
    {
        _registers = new ConcurrentDictionary<Type, Type>();
    }

    public void AddRegister<TFrom, TTo>()
    {
        AddRegister(typeof(TFrom), typeof(TTo));
    }

    public void AddRegister<TFrom>()
    {
        AddRegister(typeof(TFrom), typeof(TFrom));
    }

    public void AddRegister(Type TFrom, Type TTo)
    {
        _registers.TryAdd(TFrom, TTo);
    }

    public Type? GetRegister(Type TFrom)
    {
        if(_registers.TryGetValue(TFrom, out var toType))
        {
            return toType;
        }

        return null;
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
        Console.WriteLine("Class A -> Method A before call Method B");
        _b.MethodB();
    }
}

interface IB
{
    void MethodB();
}

class B : IB
{
    public void MethodB()
    {
        Console.WriteLine("Class B -> Method B");
    }
}