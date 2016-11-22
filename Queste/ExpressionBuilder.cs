using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using static System.Linq.Expressions.Expression;

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
        return p => false;
      }

      queryString = queryString.Replace("%3d", "=");
      queryString = queryString.Replace("+", "%2B");

      NameValueCollection queryStringPairs = HttpUtility.ParseQueryString(queryString);
      ParameterExpression parameter = Parameter(typeof(TSource), "p");

      //we get all the properties on TSource as we will try to match query string keys to property names
      Dictionary<string, PropertyInfo> properties = typeof(TSource).GetProperties()
        .ToDictionary(p => p.Name.ToLower());

      Expression expression = null;

      queryStringPairs.Cast<string>()
        .SelectMany(key => queryStringPairs.GetValues(key),
          (key, value) => new KeyValuePair<string, string>(key.ToLower(), value))
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
          MemberExpression property = Property(parameter, propertyInfo);
          MethodCallExpression propertyToString = Call(property, nameof(ToString), null, null);

          Type[] propertyInterfaces = propertyType.GetInterfaces();
          Func<Type, bool> isIEnumerableFunc = i => i.IsGenericType &&
                                                    i.GetGenericTypeDefinition() == typeof(IEnumerable<>);

          // see if this propety is of type IEnumerable<> and if it is we will loop through its values 
          // for comparison with query string value items
          if (propertyInterfaces.Any(isIEnumerableFunc) &&
              typeof(string) != propertyType)
          {
            Type iEnumerableType = propertyInterfaces.FirstOrDefault(isIEnumerableFunc);
            Type itemType = iEnumerableType?.GetGenericArguments()[0];

            //if for whatever reason we have failed to get an element type just return null
            if (itemType == null)
            {
              return (Expression) null;
            }

            //create a parameter expression for value items
            //also create a ToString call expression for this param expression
            ParameterExpression item = Parameter(itemType, "e");
            MethodCallExpression itemToString = Call(item, nameof(ToString), null, null);

            var propertyNotNullExpression = NotEqual(property, Constant(null));

            //get our func type ready using the value item type
            var func = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

            if (values.Length == 1)
            {
              BinaryExpression equals = BuildEqualExpression(itemType, values[0], item, itemToString);

              // .Any(e => e == values[0]) 
              // .Any(e => e.ToString() == values[0])
              return AndAlso(propertyNotNullExpression, Call(typeof(Enumerable), nameof(Enumerable.Any), 
                new[] { itemType }, property, Lambda(func, equals, item)));
            }

            // e == values[0]
            // e.ToString() == values[0]
            equalsArray = values
              .Select(value => BuildEqualExpression(itemType, value, item, itemToString))
              .ToArray();

            // (e == values[0] || e == value[1])
            // (e.ToString() == values[0] || e.ToString() == values[1])
            or = Or(equalsArray.First(), equalsArray.Last());

            for (int i = equalsArray.Length - 2; i > 0; i--)
            {
              // (e == values[0] || e == values[1] || e == values[2])
              // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
              or = Or(equalsArray[i], or);
            }

            // .Any(e == values[0] || e == values[1] || e == values[2])
            // .Any(e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
            return AndAlso(propertyNotNullExpression, Call(typeof(Enumerable), nameof(Enumerable.Any), 
              new[] { itemType }, property, Lambda(func, or, item)));
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
          or = Or(equalsArray.First(), equalsArray.Last());

          for (int i = equalsArray.Length - 2; i > 0; i--)
          {
            // (e == values[0] || e == values[1] || e == values[2])
            // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
            or = Or(equalsArray[i], or);
          }

          // (e == values[0] || e == values[1] || e == values[2])
          // (e.ToString() == values[0] || e.ToString() == values[1] || e.ToString() == values[2])
          return or;
        })
        .ForEach(e =>
        {
          if (e == null)
          {
            return;
          }

          if (expression == null)
          {
            expression = e;
            return;
          }

          expression = And(e, expression);
        });

      return expression != null
        ? Lambda<Func<TSource, bool>>(expression, parameter)
        : p => false;
    }

    /// <summary>
    /// Takes URL encoded query string and creates a dynamic Linq expression and compiles it into a function.
    /// </summary>
    /// <typeparam name="TSource">The type of object we will be building the function against.</typeparam>
    /// <param name="queryString">A URL encoded query string</param>
    /// <returns>A basic function for use in linq queries like Where, First, FirstOrDefault, Any, or All.</returns>
    public static Func<TSource, bool> BuildFunction<TSource>(string queryString)
    {
      return BuildExpression<TSource>(queryString)?.Compile();
    }

    #endregion

    #region Private Methods

    private static BinaryExpression BuildEqualExpression(Type type, string queryValue,
      Expression parameterExpression, MethodCallExpression toStringExpression)
    {
      Expression valueExpression;

      bool typeCanBeConst = type.GetInterface(nameof(IConvertible)) != null &&
                            type.IsValueType || type == typeof(string);

      if (typeCanBeConst)
      {
        valueExpression = Constant(typeof(string) != type
          ? System.Convert.ChangeType(queryValue, type)
          : queryValue);

        return Equal(parameterExpression, valueExpression);
      }

      valueExpression = Constant(queryValue);

      if (type.IsValueType)
      {
        return Equal(toStringExpression, valueExpression);
      }

      Expression nullExpression = Constant(null);

      return AndAlso(NotEqual(parameterExpression, nullExpression),
        Equal(toStringExpression, valueExpression));
    }

    #endregion
  }
}