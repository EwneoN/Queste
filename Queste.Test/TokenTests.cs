using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Queste.Test
{
  public class TokenTests
  {
    [Theory]
    [InlineData("=", "+", 10)]
    [InlineData("=", "%2B", 10)]
    [InlineData("%3d", "%2B", 10)]
    [InlineData("%3d", "+", 10)]
    [InlineData("=", "+", 100)]
    [InlineData("=", "%2B", 100)]
    [InlineData("%3d", "%2B", 100)]
    [InlineData("%3d", "+", 100)]
    [InlineData("=", "+", 1000)]
    [InlineData("=", "%2B", 1000)]
    [InlineData("%3d", "%2B", 1000)]
    [InlineData("%3d", "+", 1000)]
    [InlineData("=", "+", 10000)]
    [InlineData("=", "%2B", 10000)]
    [InlineData("%3d", "%2B", 10000)]
    [InlineData("%3d", "+", 10000)]
    [InlineData("=", "+", 100000)]
    [InlineData("=", "%2B", 100000)]
    [InlineData("%3d", "%2B", 100000)]
    [InlineData("%3d", "+", 100000)]
    [InlineData("=", "+", 1000000)]
    [InlineData("=", "%2B", 1000000)]
    [InlineData("%3d", "%2B", 1000000)]
    [InlineData("%3d", "+", 1000000)]
    public void TokenFunctionTest(string equalsToken, string plusToken, int collectionSize)
    {
      var kvps = new KeyValuePair<string, string>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, string>($"key{i}", $"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}");
      }

      string value1 = kvps[rand1.Next(0, collectionSize - 1)].Value;
      string value2 = kvps[rand1.Next(0, collectionSize - 1)].Value;

      string queryString = $"value{equalsToken}{value1}{plusToken}{value2}";

      var sw = Stopwatch.StartNew();
      var expression = ExpressionBuilder.BuildFunction<KeyValuePair<string, string>>(queryString);

      kvps.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      kvps.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      kvps.Any(expression).Should().BeTrue();
      kvps.All(expression).Should().BeFalse();
      kvps.Count(expression).Should().Be(kvps.Count(q => q.Value == value1 || q.Value == value2));

      sw.Stop();
    }

    [Theory]
    [InlineData("=", "+", 10)]
    [InlineData("=", "%2B", 10)]
    [InlineData("%3d", "%2B", 10)]
    [InlineData("%3d", "+", 10)]
    [InlineData("=", "+", 100)]
    [InlineData("=", "%2B", 100)]
    [InlineData("%3d", "%2B", 100)]
    [InlineData("%3d", "+", 100)]
    [InlineData("=", "+", 1000)]
    [InlineData("=", "%2B", 1000)]
    [InlineData("%3d", "%2B", 1000)]
    [InlineData("%3d", "+", 1000)]
    [InlineData("=", "+", 10000)]
    [InlineData("=", "%2B", 10000)]
    [InlineData("%3d", "%2B", 10000)]
    [InlineData("%3d", "+", 10000)]
    [InlineData("=", "+", 100000)]
    [InlineData("=", "%2B", 100000)]
    [InlineData("%3d", "%2B", 100000)]
    [InlineData("%3d", "+", 100000)]
    [InlineData("=", "+", 1000000)]
    [InlineData("=", "%2B", 1000000)]
    [InlineData("%3d", "%2B", 1000000)]
    [InlineData("%3d", "+", 1000000)]
    public void TokenExpressionTest(string equalsToken, string plusToken, int collectionSize)
    {
      var kvps = new KeyValuePair<string, string>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, string>($"key{i}", $"{Guid.NewGuid()}{Guid.NewGuid()}{Guid.NewGuid()}");
      }

      string value1 = kvps[rand1.Next(0, collectionSize - 1)].Value;
      string value2 = kvps[rand1.Next(0, collectionSize - 1)].Value;

      string queryString = $"value{equalsToken}{value1}{plusToken}{value2}";

      IQueryable<KeyValuePair<string, string>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();
      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, string>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value == value1 || q.Value == value2));

      sw.Stop();
    }
  }
}
