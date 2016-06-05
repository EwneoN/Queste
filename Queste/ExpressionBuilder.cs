﻿using System;
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
    #region Public Methods

    /// <summary>
    /// Takes URL encoded query string and creates a dynamic Linq expression.
    /// </summary>
    /// <typeparam name="TSource">The type of object we will be building the expression against.</typeparam>
    /// <param name="queryString">A URL encoded query string</param>
    /// <returns>A basic linq expression for use in linq queries like Where, First, FirstOrDefault, Any, or All.</returns>
    public static Expression<Func<TSource, bool>> BuildExpression<TSource>(string queryString)
    {
      if (string.IsNullOrWhiteSpace(queryString))
      {
        return null;
      }

      queryString = queryString.Replace("%3d", "=");
      queryString = queryString.Replace("+", "%2B");

      NameValueCollection queryStringPairs = HttpUtility.ParseQueryString(queryString);
      ParameterExpression parameter = Expression.Parameter(typeof(TSource), "p");

      //we get all the properties on TSource as we will try to match query string keys to property names
      Dictionary<string, PropertyInfo> properties = typeof(TSource).GetProperties().ToDictionary(p => p.Name.ToLower());

      List<Expression> expressions = queryStringPairs.Cast<string>()
        .SelectMany(key => queryStringPairs.GetValues(key), (key, value) => new KeyValuePair<string, string>(key.ToLower(), value))
        .Select(kvp =>
        {
          PropertyInfo propertyInfo;

          //see if this key matches to a property on TSource
          if (!properties.TryGetValue(kvp.Key, out propertyInfo))
          {
            return null;
          }

          //we split the query string value on + as this character is used to delimit a collection of items
          string[] values = kvp.Value.Split('+');
          Type propertyType = propertyInfo.PropertyType;

          BinaryExpression or;
          BinaryExpression[] equalsArray;
          MemberExpression property = Expression.Property(parameter, propertyInfo);
          MethodCallExpression propertyToString = Expression.Call(property, nameof(ToString), null, null);

          Type[] propertyInterfaces = propertyType.GetInterfaces();
          Func<Type, bool> isIEnumerableFunc = i => i.IsGenericType &&
                                                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>);

          // see if this propety is of type IEnumerable<> and if it is we will loop through its values 
          // for comparison with query string value items
          if (propertyInterfaces.Any(isIEnumerableFunc) && typeof(string) != propertyType)
          {
            Type iEnumerableType = propertyInterfaces.FirstOrDefault(isIEnumerableFunc);
            Type itemType = iEnumerableType?.GetGenericArguments()[0];

            //if for whatever reason we have failed to get an element type just return null
            if (itemType == null)
            {
              return (Expression)null;
            }

            //create a parameter expression for value items
            //also create a ToString call expression for this param expression
            ParameterExpression item = Expression.Parameter(itemType, "e");
            MethodCallExpression itemToString = Expression.Call(item, nameof(ToString), null, null);

            //get our func type ready using the value item type
            var func = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

            if (values.Length == 1)
            {
              // .Any(e => e == values[0]) 
              // .Any(e => e.ToString() == values[0])
              return Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new[] { itemType }, property,
                Expression.Lambda(func, BuildEqualExpression(itemType, values[0], item, itemToString), item));
            }

            // e == values[0]
            // e.ToString() == values[0]
            equalsArray = values
              .Select(value => BuildEqualExpression(itemType, value, item, itemToString))
              .ToArray();

            // (e == values[0] || e == value[1])
            // (e.ToString() == values[0] || e.ToString() == values[1])
            or = Expression.Or(equalsArray.First(), equalsArray.Last());

            for (int i = equalsArray.Length - 2; i > 0; i--)
            {
              // (e == values[0] || e == values[1] || e == values[2])
              // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
              or = Expression.Or(equalsArray[i], or);
            }

            // .Any(e == values[0] || e == values[1] || e == values[2])
            // .Any(e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
            return Expression.Call(typeof(Enumerable), nameof(Enumerable.Any), new[] { itemType }, property,
              Expression.Lambda(func, or, item));
          }

          if (values.Length == 1)
          {
            // e == values[0]
            // e.ToString() == values[0]
            return BuildEqualExpression(propertyType, kvp.Value, property, propertyToString);
          }

          // e == values[0]
          // e.ToString() == values[0]
          equalsArray = values
            .Select(value => BuildEqualExpression(propertyType, value, property, propertyToString))
            .ToArray();

          // (e == values[0] || e == value[1])
          // (e.ToString() == values[0] || e.ToString() == values[1])
          or = Expression.Or(equalsArray.First(), equalsArray.Last());

          for (int i = equalsArray.Length - 2; i > 0; i--)
          {
            // (e == values[0] || e == values[1] || e == values[2])
            // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
            or = Expression.Or(equalsArray[i], or);
          }

          // (e == values[0] || e == values[1] || e == values[2])
          // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
          return or;
        })
        .Where(e => e != null)
        .ToList();

      switch (expressions.Count)
      {
        case 1:
          return Expression.Lambda<Func<TSource, bool>>(expressions[0], parameter);

        case 2:
          Expression and = Expression.And(expressions[0], expressions[1]);
          return Expression.Lambda<Func<TSource, bool>>(and, parameter);

        default:
          Expression ands = Expression.And(expressions.First(), expressions.Last());

          for (int i = expressions.Count - 2; i > 0; i--)
          {
            ands = Expression.And(expressions[i], ands);
          }

          return Expression.Lambda<Func<TSource, bool>>(ands, parameter);
      }
    }

    /// <summary>
    /// Takes URL encoded query string and creates a dynamic Linq expression and compiles it into a function.
    /// </summary>
    /// <typeparam name="TSource">The type of object we will be building the function against.</typeparam>
    /// <param name="queryString">A URL encoded query string</param>
    /// <returns>A basic function for use in linq queries like Where, First, FirstOrDefault, Any, or All.</returns>
    public static Func<TSource, bool> BuildFunction<TSource>(string queryString)
    {
      return BuildExpression<TSource>(queryString).Compile();
    }

    #endregion

    #region Private Methods

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

    #endregion
  }
}