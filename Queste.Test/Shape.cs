using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queste
{
  public enum Colour
  {
    Red,
    Green,
    Blue
  }

  public abstract class Shape
  {
    private readonly Colour _Colour;

    public decimal Area => CalculateArea();

    public decimal Perimeter => CalculatePerimeter();

    public Colour Colour => _Colour;

    protected Shape(Colour colour)
    {
      _Colour = colour;
    }

    protected abstract decimal CalculateArea();

    protected abstract decimal CalculatePerimeter();
  }
}
