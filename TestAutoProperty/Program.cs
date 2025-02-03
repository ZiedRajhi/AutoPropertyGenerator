using System;

[AutoProperty]
public partial class Person
{
    public string? Name;
    public int Age;
}

class Program
{
    static void Main()
    {
        Person p = new Person { Name = "Alice", Age = 30 };
        Console.WriteLine($"Nom: {p.Name}, Age: {p.Age}");
    }
}
