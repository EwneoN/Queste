using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queste.Test
{
  public class ShapeCollection
  {
    private Shape[] _Shapes;

    public Shape[] Shapes
    {
      get { return _Shapes; }
      set { _Shapes = value; }
    }

    public ShapeCollection(IEnumerable<Shape> shapes)
    {
      _Shapes = shapes?.ToArray() ?? new Shape[0];
    }
  }
}
