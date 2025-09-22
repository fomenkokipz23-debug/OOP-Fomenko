using System;

class Figure
{
    private string name;

    public string Name 
    { 
        get => name; 
        set => name = value; 
    }

    public double Area 
    {
        get 
        {
            return name.Length * 3.14;
        }
    }

    public Figure(string name)
    {
        this.name = name;
        Console.WriteLine($"Constructor: Figure '{name}' created.");
    }

    ~Figure()
    {
        Console.WriteLine($"Destructor: Figure '{name}' destroyed.");
    }

    public string GetFigure()
    {
        return $"Figure: {name}, Area: {Area}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        Figure f1 = new Figure("Circle");
        Figure f2 = new Figure("Square");

        Console.WriteLine(f1.GetFigure());
        Console.WriteLine(f2.GetFigure());

        Figure f3 = new Figure("Triangle");
        Console.WriteLine(f3.GetFigure());
    }
}
