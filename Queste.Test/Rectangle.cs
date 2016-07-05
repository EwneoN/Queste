namespace Queste.Test
{
  public class Rectangle : Shape
  {
    private readonly decimal _Length;
    private readonly decimal _Width;

    public decimal Length => _Length;

    public decimal Width => _Width;

    public Rectangle(Colour colour, decimal length, decimal width) : base(colour)
    {
      _Length = length;
      _Width = width;
    }

    protected override decimal CalculateArea()
    {
      return _Length * _Width;
    }

    protected override decimal CalculatePerimeter()
    {
      return _Width * 2 + _Length  * 2;
    }
  }
}
