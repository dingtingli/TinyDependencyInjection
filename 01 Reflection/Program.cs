
//Type a = typeof(A);

//foreach (var item in a.GetMembers())
//{
//    Console.WriteLine($"{item.Name} : {item.MemberType}");
//}

//print:
//get_A2: Method
//set_A2 : Method
//MenthodA : Method
//GetType : Method
//ToString : Method
//Equals : Method
//GetHashCode : Method
//.ctor : Constructor
//.ctor : Constructor
//A2 : Property
//A1 : Field

//Type b = Type.GetType("A");

//foreach (var item in b.GetMembers())
//{
//    Console.WriteLine($"{item.Name} : {item.MemberType}");
//}

//pint:
//get_A2: Method
//set_A2 : Method
//MenthodA : Method
//GetType : Method
//ToString : Method
//Equals : Method
//GetHashCode : Method
//.ctor : Constructor
//.ctor : Constructor
//A2 : Property
//A1 : Field

using System.Text.Encodings.Web;

Type a = typeof(A);
object test = Activator.CreateInstance(a);

Console.WriteLine(((A)test).A1);
//pint：
//a1

public class A
{
    public string A1;
    public string A2 { get; set; }

    public A()
    {
        A1 = "a1";
    }

    public A(string a2)
    {
        A2 = a2;
    }

    public void MenthodA()
    {
        Console.WriteLine(A2);
    }
}