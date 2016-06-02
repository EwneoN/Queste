using System;
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

      Dictionary<string, PropertyInfo> properties = typeof(T).GetProperties().ToDictionary(p => p.Name.ToLower());

      List <Expression> expressions = parameters.Cast<string>()
        .SelectMany(key => parameters.GetValues(key), (key, value) => new KeyValuePair<string, string>(key.ToLower(), value))
        .Select(kvp =>
        {
          PropertyInfo propertyInfo;

          if (!properties.TryGetValue(kvp.Key, out propertyInfo))
          {
            return null;
          }

          Type propertyType = propertyInfo.PropertyType;

          BinaryExpression or;
          BinaryExpression[] equalsExpressions;
          MemberExpression propertyExp = Expression.Property(param, propertyInfo);
          MethodCallExpression propertyToStringCall = Expression.Call(propertyExp, nameof(ToString), null, null);

          string[] values = kvp.Value.Split('+');

          Type[] propertyInterfaces = propertyType.GetInterfaces();
          Func<Type, bool> isIEnumerableFunc = i => i.IsGenericType &&
                                                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>);

          if (propertyInterfaces.Any(isIEnumerableFunc) && typeof(string) != propertyType)
          {
            Type iEnumerableType = propertyInterfaces.FirstOrDefault(isIEnumerableFunc);
            Type elementType = iEnumerableType?.GetGenericArguments()[0];
            
            if(elementType == null)
            {
              return (Expression)null;
            }

            ParameterExpression elementParam = Expression.Parameter(elementType, "e");
            MethodCallExpression elementToStringCall = Expression.Call(elementParam, nameof(ToString), null, null);

            equalsExpressions = values
              .Select(value => BuildEqualExpression(elementType, value, elementParam, elementToStringCall))
              .ToArray();

            or = Expression.Or(equalsExpressions.First(), equalsExpressions.Last());

            for (int i = equalsExpressions.Length - 2; i > 0; i--)
            {
              or = Expression.Or(equalsExpressions[i], or);
            }

            var func = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));

            return Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new [] { elementType }, propertyExp, 
              Expression.Lambda(func, or, elementParam));
          }

          if (values.Length == 1)
          {
            return BuildEqualExpression(propertyType, kvp.Value, propertyExp, propertyToStringCall);
          }

          equalsExpressions = values
            .Select(value => BuildEqualExpression(propertyType, value, propertyExp, propertyToStringCall))
            .ToArray();

          or = Expression.Or(equalsExpressions.First(), equalsExpressions.Last());

          for (int i = equalsExpressions.Length - 2; i > 0; i--)
          {
            or = Expression.Or(equalsExpressions[i], or);
          }

          return or;
        })
        .ToList();

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

    private static BinaryExpression BuildEqualExpression(Type type, string queryValue,
      MemberExpression propertyExpression, MethodCallExpression toStringExpression)
    {
      Expression valueExpression;
      Expression propertyValueExpression;

      bool typeCanBeConst = type.GetInterface(nameof(IConvertible)) != null &&
                            type.IsValueType || type == typeof(string);

      if (typeCanBeConst)
      {
        object value = typeof(string) != type
          ? Convert.ChangeType(queryValue, type)
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

    private static BinaryExpression BuildEqualExpression(Type type, string queryValue,
      ParameterExpression parameterExpression, MethodCallExpression toStringExpression)
    {
      Expression valueExpression;
      Expression parameterValueExpression;

      bool typeCanBeConst = type.GetInterface(nameof(IConvertible)) != null &&
                            type.IsValueType || type == typeof(string);

      if (typeCanBeConst)
      {
        valueExpression = Expression.Constant(typeof(string) != type
          ? Convert.ChangeType(queryValue, type)
          : queryValue);
        parameterValueExpression = parameterExpression;
      }
      else
      {
        valueExpression = Expression.Constant(queryValue);
        parameterValueExpression = toStringExpression;
      }

      return Expression.Equal(parameterValueExpression, valueExpression);
    }
  }
}