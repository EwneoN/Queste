using System.Collections.Generic;
using System.Linq;
using static Queste.ExpressionBuilder;

namespace Queste
{
  public static class QuerableExtensions
  {
    public static bool Any<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.Any(BuildExpression<TSource>(queryString));
    }

    public static bool All<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.All(BuildExpression<TSource>(queryString));
    }

    public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.Where(BuildExpression<TSource>(queryString));
    }

    public static TSource First<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.First(BuildExpression<TSource>(queryString));
    }

    public static TSource FirstOrDefault<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.FirstOrDefault(BuildExpression<TSource>(queryString));
    }

    public static int Count<TSource>(this IQueryable<TSource> queryable, string queryString)
    {
      return queryable.Count(BuildExpression<TSource>(queryString));
    }
  }
}
