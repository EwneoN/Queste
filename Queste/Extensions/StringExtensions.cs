using System.Linq.Expressions;
using static Queste.ExpressionBuilder;

namespace Queste
{
  public static class StringExtensions
  {
    public static Expression ToExpression<TSource>(this string queryString)
    {
      return BuildExpression<TSource>(queryString);
    }

    public static Expression ToFunction<TSource>(this string queryString)
    {
      return BuildExpression<TSource>(queryString);
    }
  }
}
