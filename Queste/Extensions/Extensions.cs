using System;
using System.Collections.Generic;

namespace Queste
{
  public static class Extensions
  {
    public static void ForEach<T>(this IEnumerable<T> iEnumerable, Action<T> action)
    {
      foreach (T item in iEnumerable)
      {
        action(item);
      }
    }
  }
}