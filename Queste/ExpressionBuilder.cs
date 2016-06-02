using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace Queste
{
  public static class ExpressionBuilder
  {
    public static Expression<Func<T, bool>> Build<T>(string queryString)
    {
      NameValueCollection parameters = HttpUtility.ParseQueryString(queryString);

      ParameterExpression param = Expression.Parameter(typeof(T), "p");

      List<Expression> expressions = parameters.Cast<string>()
        .SelectMany(key => parameters.GetValues(key), (key, value) => new KeyValuePair<string, string>(key, value))
        .Select(kvp =>
        {
          PropertyInfo propertyInfo = typeof(T).GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance |
                                                                     BindingFlags.Static | BindingFlags.IgnoreCase);

          if (propertyInfo == null)
          {
            return null;
          }

          Type propertyType = propertyInfo.PropertyType;

          BinaryExpression or;
          MemberExpression propertyExp = Expression.Property(param, propertyInfo);
          ConstantExpression stringTypeExp = Expression.Constant(typeof(string));
          ConstantExpression objectTypeExp = Expression.Constant(typeof(object));
          ConstantExpression propertyTypeExp = Expression.Constant(propertyType);
          MemberExpression isValueTypeTest;
          BinaryExpression isStringTest;
          BinaryExpression stringOrValueTest;
          MethodCallExpression propertyToStringCall = Expression.Call(propertyExp, "ToString", null, null);

          string[] values = kvp.Value.Split('+');
          ConstantExpression valuesExp = Expression.Constant(values);

          if (typeof(IEnumerable).IsAssignableFrom(propertyType) &&
              typeof(string) != propertyType)
          {
            isValueTypeTest = Expression.Property(propertyTypeExp, "IsValueType");
            isStringTest = Expression.Equal(propertyTypeExp, stringTypeExp);
            stringOrValueTest = Expression.Or(isStringTest, isValueTypeTest);

            var elementType = propertyType.IsArray
              ? propertyType.GetElementType()
              : propertyType.GetGenericArguments()[0];
            var elementTypeExp = Expression.Constant(elementType);

            var itemParam = Expression.Parameter(typeof(string), "i");

            var ifStringTestReturn = Expression.Label(typeof(object));

            var changeTypeCall = Expression.Call(typeof(Convert), "ChangeType", null, itemParam, elementTypeExp);
            var returnItemParam = Expression.Return(ifStringTestReturn, itemParam);
            var returnElementType = Expression.Return(ifStringTestReturn, changeTypeCall);

            var ifStringTest = Expression.IfThenElse(isStringTest, returnItemParam, returnElementType);

            var block1 = Expression.Block(ifStringTest, Expression.Label(ifStringTestReturn,
              Expression.Constant(default(object))));

            var ifStringOrValueTestReturn = Expression.Label(typeof(object));

            var returnIfStringOrValue = Expression.Return(ifStringOrValueTestReturn, block1);

            var ifStringOrValueTest = Expression.IfThenElse(stringOrValueTest, returnIfStringOrValue, returnItemParam);

            Expression block2 = Expression.Block(ifStringOrValueTest, Expression.Label(ifStringOrValueTestReturn,
              Expression.Constant(default(object))));

            Type selectValueFuncType = typeof(Func<,>).MakeGenericType(typeof(string), typeof(object));

            var selectValuesCall = Expression.Call(typeof(Enumerable), "Select",
              new[] { typeof(string), typeof(object) },
              valuesExp, Expression.Lambda(selectValueFuncType, block2, itemParam));

            var elementParam = Expression.Parameter(typeof(string), "e");

            Type selectElementFuncType = typeof(Func<,>).MakeGenericType(elementType, typeof(object));
            MethodCallExpression elementToStringCall = Expression.Call(elementType, "ToString", null, null);
            MethodCallExpression selectElementCall = Expression.Call(typeof(Enumerable), "Select",
              new[] { elementType, typeof(string) }, propertyExp, Expression.Lambda(selectElementFuncType,
                elementToStringCall, itemParam));

            var containsCall = Expression.Call(typeof(Enumerable), "Contains", null, selectElementCall, elementParam);

            MethodCallExpression selectContainsCall = Expression.Call(typeof(Enumerable), "Select",
              new[] { elementType, typeof(string) }, propertyExp, Expression.Lambda(selectElementFuncType,
                elementToStringCall, itemParam));

            List<MethodCallExpression> collectionExpression = values
              .ToList();

            if (collectionExpression.Count == 1)
            {
              return (Expression) collectionExpression[0];
            }

            or = Expression.Or(collectionExpression.First(), collectionExpression.Last());

            for (int i = collectionExpression.Count - 2; i > 0; i--)
            {
              or = Expression.Or(collectionExpression[i], or);
            }

            return or;
          }

          if (values.Length == 1)
          {
            return BuildEqualExpression(propertyType, kvp.Value, propertyExp, propertyToStringCall);
          }

          List<BinaryExpression> equalsExpressions = values
            .Select(value => BuildEqualExpression(propertyType, value, propertyExp, propertyToStringCall))
            .ToList();

          or = Expression.Or(equalsExpressions.First(), equalsExpressions.Last());

          for (int i = equalsExpressions.Count - 2; i > 0; i--)
          {
            or = Expression.Or(equalsExpressions[i], or);
          }

          return or;
        }).ToList();

      switch (expressions.Count)
      {
        case 1:
          return Expression.Lambda<Func<T, bool>>(expressions[0], param);

        case 2:
          Expression and = Expression.And(expressions[0], expressions[1]);
          return Expression.Lambda<Func<T, bool>>(and, param);

        default:
          Expression ands = Expression.And(expressions.First(), expressions.Last());

          for (int i = expressions.Count - 2; i > 0; i--)
          {
            ands = Expression.And(expressions[i], ands);
          }

          return Expression.Lambda<Func<T, bool>>(ands, param);
      }
    }

    private static BinaryExpression BuildEqualExpression(Type propertyType, string queryValue,
      MemberExpression propertyExpression, MethodCallExpression toStringExpression)
    {
      Expression valueExpression;
      Expression propertyValueExpression;

      if (propertyType.IsValueType ||
          typeof(string) == propertyType)
      {
        object value = typeof(string) != propertyType
          ? Convert.ChangeType(queryValue, propertyType)
          : queryValue;

        valueExpression = Expression.Constant(value);
        propertyValueExpression = propertyExpression;
      }
      else
      {
        valueExpression = Expression.Constant(queryValue);
        propertyValueExpression = toStringExpression;
      }

      return Expression.Equal(propertyValueExpression, valueExpression);
    }
  }
}