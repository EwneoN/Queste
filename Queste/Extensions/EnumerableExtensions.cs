using System.Collections.Generic;
using System.Linq;
using static Queste.ExpressionBuilder;

namespace Queste
{
  public static class EnumerableExtensions
  {
    public static bool Any<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.Any(BuildFunction<TSource>(queryString));
    }

    public static bool All<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.All(BuildFunction<TSource>(queryString));
    }

    public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.Where(BuildFunction<TSource>(queryString));
    }

    public static TSource First<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.First(BuildFunction<TSource>(queryString));
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.FirstOrDefault(BuildFunction<TSource>(queryString));
    }

    public static int Count<TSource>(this IEnumerable<TSource> enumerable, string queryString)
    {
      return enumerable.Count(BuildFunction<TSource>(queryString));
    }
  }
}
