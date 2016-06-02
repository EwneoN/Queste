using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Queste.Test
{

  public class UnitTest1
  {
    [Theory]
    [InlineData(10,10)]
    [InlineData(100, 10)]
    [InlineData(1000, 100)]
    [InlineData(10000, 100)]
    public void TestMethod1(int collectionSize, int maxSubCollectionSize)
    {
      var kvps = new KeyValuePair<string, List<DateTime>>[collectionSize];

      int day = 1;
      int month = 1;
      int year = 2016;

      for (int i = 0; i < collectionSize; i++)
      {
        string key = $"key{i}";

        KeyValuePair<string, List<DateTime>> kvp = new KeyValuePair<string, List<DateTime>>(key, new List<DateTime>());

        for (int j = 0; j < maxSubCollectionSize; j++)
        {
          kvp.Value.Add(new DateTime(year, month, day++));

          if (day > 28)
          {
            day = 1;
            month++;
          }

          if (month <= 12)
          {
            continue;
          }

          month = 1;
          year++;
        }

        kvps[i] = kvp;
      }

      Random rand = new Random();

      string queryString = $"value={kvps[rand.Next(0, collectionSize - 1)].Value[0]}%2B" +
                           $"{kvps[rand.Next(0, collectionSize - 1)].Value[0]}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.Build<KeyValuePair<string, List<DateTime>>>(queryString);

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();

      sw.Stop();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void TestMethod2(int collectionSize)
    {
      var kvps = new KeyValuePair<string, DateTime>[collectionSize];

      int day = 1;
      int month = 1;
      int year = 2016;

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, DateTime>($"key{i}", new DateTime(year, month, day++));

        if(day > 28)
        {
          day = 1;
          month++;
        }

        if (month <= 12)
        {
          continue;
        }

        month = 1;
        year++;
      }

      Random rand = new Random();

      string queryString = $"value={kvps[rand.Next(0, collectionSize - 1)].Value}%2B" +
                           $"{kvps[rand.Next(0, collectionSize - 1)].Value}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.Build<KeyValuePair<string, DateTime>>(queryString);

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();

      sw.Stop();
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void TestMethod3(int collectionSize)
    {
      var kvps = new KeyValuePair<string, int>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        Random rand2 = new Random(rand1.Next(0, int.MaxValue));

        kvps[i] = new KeyValuePair<string, int>($"key{i}", rand2.Next(1, collectionSize));
      }

      string queryString = $"value={kvps[rand1.Next(0, collectionSize - 1)].Value}%2B" +
                           $"{kvps[rand1.Next(0, collectionSize - 1)].Value}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.Build<KeyValuePair<string, int>>(queryString);

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();

      sw.Stop();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void TestMethod4(int collectionSize)
    {
      var kvps = new KeyValuePair<string, string>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, string>($"key{i}", Guid.NewGuid().ToString() + Guid.NewGuid() + Guid.NewGuid());
      }

      string queryString = $"value={kvps[rand1.Next(0, collectionSize - 1)].Value}%2B" +
                           $"{kvps[rand1.Next(0, collectionSize - 1)].Value}";

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.Build<KeyValuePair<string, string>>(queryString);

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();

      sw.Stop();
    }
  }
}
