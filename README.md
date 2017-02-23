# Queste  

### Example 1  

```C#
var kvps = new Dictionary<string, DateTime[]>
{
    ["key1"]  = new [] { new DateTime(2016,6,1) },
    ["key2"]  = new [] { new DateTime(2016,6,2) },
    ["key3"]  = new [] { new DateTime(2016,6,3) },
    ["key4"]  = new [] { new DateTime(2016,6,1), new DateTime(2016,6,2) },
};

var getAllWithDate = kvps.Where("?value=2016-06-2");
var fourthKvp = kvps.First("?key=key4");
```

### Example 2

```C#
public class Shape
{
	public string Name { get; set; }
	public int Length { get; set; }
	public int Width { get; set; }

	public Shape(int length, int width, string name = null)
	{
		Length = length;
		Width = width;
		Name = name;
	}
}

public static class Program
{
	public static void Main(string[] args)
	{
		var shapes = new[]
		{
			new Shape(10, 5),
			new Shape(20, 10),
			new Shape(40, 20),
			new Shape(20, 10, "Oblong"),
			new Shape(20, 5, "Rhombus"),
			new Shape(80, 20, "Rhombus"),
			new Shape(40, 20, "Squircle") //square circle ;)
		};

		var narrowShapes = shapes.Where("?width=5+10");
		var longShapes = shapes.Where("?length=80+40");
		var rhombi = shapes.Where("?name=Rhombus");
		var squircle = shapes.FirstOrDefault("?name=squircle");

		//uncomment if running in linqpad
		//narrowShapes.Dump("narrowShapes");
		//longShapes.Dump("longShapes");
		//rhombi.Dump("rhombi");
		//squircle.Dump("squircle");
	}
}
```
### Example 3

```C#
//this example uses EF 6

[Table(nameof(Animal))]
public class Animal
{
  [Key]
  public Guid Id { get; set; }
	public string Name { get; set; }
	public int Weight { get; set; }
	public int Age { get; set; }

	public Animal(int weight, int age, string name = null)
	{
		Weight = weight;
		Age = age;
		Name = name;
	}
}

public AnimalDbContext: DbContext
{
  public DbSet<Animal> Animals { get; set; }
}

public static class Program
{
	public static void Main(string[] args)
	{
		var animals = new[]
		{
			new Animal(10, 15, "Rufus"),
			new Animal(20, 10, "Jerry"),
			new Animal(40, 20, "Fido"),
			new Animal(20, 10, "Nessy"),
			new Animal(80, 5, "Freki"),
			new Animal(80, 5, "Geri"),
			new Animal(100, 5, "Russ")
		};

		var smallShapes = animals.Where("?weight=10+20");
		var oldAnimals = animals.Where("?age=20+15");
		var nessy = animals.FirstOrDefault("?name=nessy");
		var wolves = animals.Where("?name=freki+geri+russ");

		//uncomment if running in linqpad
		//smallShapes.Dump("smallShapes");
		//oldAnimals.Dump("oldAnimals");
		//nessy.Dump("nessy");
		//wolves.Dump("wolves");
	}
}
```