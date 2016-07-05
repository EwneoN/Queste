using System;

namespace Queste.Test
{
  public class Circle : Shape
  {
    private readonly decimal _Diameter;

    public decimal Diameter => _Diameter;

    public Circle(Colour colour, decimal diameter) : base(colour)
    {
      _Diameter = diameter;
    }

    protected override decimal CalculateArea()
    {
      return (decimal) (Math.PI*Math.Pow((double) _Diameter/2, 2));
    }

    protected override decimal CalculatePerimeter()
    {
      return (decimal) (Math.PI*(double) _Diameter);
    }
  }
}