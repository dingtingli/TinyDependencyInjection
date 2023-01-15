// See https://aka.ms/new-console-template for more information
using System.Collections.Concurrent;

var container = new DependencyRegister();
container.AddRegister<A>();
container.AddRegister<IB, B>();
container.AddRegister<IC, C>();

var resolver = new DependencyContainer(container);
//var b = resolver.GetService<IB>();
var a = resolver.GetService<A>();


//b.MethodB();
a.MethodA();

public class DependencyContainer
{
    DependencyRegister _container;

    public DependencyContainer(DependencyRegister container)
    {
        _container = container;
    }

    public T GetService<T>()
    {
        return (T)GetService(typeof(T));       
    }

    public object GetService(Type type)
    {
        var dependency = _container.GetRegister(type);

        var constructor = dependency.GetConstructors().Single();
        var parameters = constructor.GetParameters().ToArray();

        if (parameters.Length > 0)
        {
            var parameterImplementations = new List<object>();

            foreach (var parameter in parameters)
            {
                var parameterImplementation = GetService(parameter.ParameterType);
                parameterImplementations.Add(parameterImplementation);
            }

            return Activator.CreateInstance(dependency, parameterImplementations.ToArray());
        }

        return Activator.CreateInstance(dependency);
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
        if (_registers.TryGetValue(TFrom, out var toType))
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
    private IC _c;

    public B(IC c)
    {
        _c = c;
    }
    public void MethodB()
    {
        Console.WriteLine("Class B -> Method B  before call Method C");
        _c.MethodC();
    }
}

interface IC
{
    void MethodC();
}

class C : IC
{
    public void MethodC()
    {
        Console.WriteLine("Class C -> Method C");
    }
}