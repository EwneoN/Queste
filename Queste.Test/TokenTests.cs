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
        kvps[i] = new KeyValuePair<string, string>($"key{i}", Guid.NewGuid().ToString() + Guid.NewGuid() + Guid.NewGuid());
      }

      string queryString = $"value{equalsToken}{kvps[rand1.Next(0, collectionSize - 1)].Value}{plusToken}" +
                           $"{kvps[rand1.Next(0, collectionSize - 1)].Value}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.BuildFunction<KeyValuePair<string, string>>(queryString);

      kvps.Where(expression).Should().NotBeNullOrEmpty();

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
        kvps[i] = new KeyValuePair<string, string>($"key{i}", Guid.NewGuid().ToString() + Guid.NewGuid() + Guid.NewGuid());
      }

      string queryString = $"value{equalsToken}{kvps[rand1.Next(0, collectionSize - 1)].Value}{plusToken}" +
                           $"{kvps[rand1.Next(0, collectionSize - 1)].Value}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, string>>(queryString);

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();

      sw.Stop();
    }
  }
}
