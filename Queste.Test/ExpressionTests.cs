using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Queste.Test
{

  public class ExpressionTests
  {
    [Theory]
    [InlineData(10, 10)]
    [InlineData(100, 10)]
    [InlineData(1000, 100)]
    [InlineData(10000, 100)]
    public void DateTimeArrayTest(int collectionSize, int maxSubCollectionSize)
    {
      var kvps = new KeyValuePair<string, DateTime[]>[collectionSize];

      int day = 1;
      int month = 1;
      int year = 2016;

      for (int i = 0; i < collectionSize; i++)
      {
        string key = $"key{i}";

        KeyValuePair<string, DateTime[]> kvp =
          new KeyValuePair<string, DateTime[]>(key, new DateTime[maxSubCollectionSize]);

        for (int j = 0; j < maxSubCollectionSize; j++)
        {
          kvp.Value[j] = new DateTime(year, month, day++);

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

      DateTime value1 = kvps[rand.Next(0, collectionSize - 1)].Value[0];
      DateTime value2 = kvps[rand.Next(0, collectionSize - 1)].Value[0];

      string queryString = $"value={value1}%2B{value2}";

      IQueryable<KeyValuePair<string, DateTime[]>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime[]>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value.Contains(value1) || kvp.Value.Contains(value2)));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value.Contains(value1) || kvp.Value.Contains(value2)));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value.Contains(value1) || q.Value.Contains(value2)));

      sw.Stop();
    }

    [Theory]
    [InlineData(10,10)]
    [InlineData(100, 10)]
    [InlineData(1000, 100)]
    [InlineData(10000, 100)]
    public void DateTimeListTest(int collectionSize, int maxSubCollectionSize)
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

      DateTime value1 = kvps[rand.Next(0, collectionSize - 1)].Value[0];
      DateTime value2 = kvps[rand.Next(0, collectionSize - 1)].Value[0];

      string queryString = $"value={value1}%2B{value2}";

      IQueryable<KeyValuePair<string, List<DateTime>>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();

      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, List<DateTime>>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value.Contains(value1) || kvp.Value.Contains(value2)));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value.Contains(value1) || kvp.Value.Contains(value2)));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value.Contains(value1) || q.Value.Contains(value2)));

      sw.Stop();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void DateTimeTest(int collectionSize)
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

      DateTime value1 = kvps[rand.Next(0, collectionSize - 1)].Value;
      DateTime value2 = kvps[rand.Next(0, collectionSize - 1)].Value;

      string queryString = $"value={value1}%2B{value2}";

      IQueryable<KeyValuePair<string, DateTime>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();
      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value == value1 || q.Value == value2));

      sw.Stop();
    }
    
    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void IntTest(int collectionSize)
    {
      var kvps = new KeyValuePair<string, int>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        Random rand2 = new Random(rand1.Next(0, int.MaxValue));

        kvps[i] = new KeyValuePair<string, int>($"key{i}", rand2.Next(1, collectionSize));
      }

      int value1 = kvps[rand1.Next(0, collectionSize - 1)].Value;
      int value2 = kvps[rand1.Next(0, collectionSize - 1)].Value;

      string queryString = $"value={value1}%2B{value2}";

      IQueryable<KeyValuePair<string, int>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();
      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, int>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value == value1 || q.Value == value2));

      sw.Stop();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void GuidTest(int collectionSize)
    {
      var kvps = new KeyValuePair<string, Guid>[collectionSize];

      Random rand = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, Guid>($"key{i}", Guid.NewGuid());
      }

      Guid value1 = kvps[rand.Next(0, collectionSize - 1)].Value;
      Guid value2 = kvps[rand.Next(0, collectionSize - 1)].Value;

      string queryString = $"value={value1}%2B{value2}";

      IQueryable<KeyValuePair<string, Guid>> queryable = kvps.AsQueryable();

      var sw = Stopwatch.StartNew();
      var expression = ExpressionBuilder.BuildExpression<KeyValuePair<string, Guid>>(queryString);

      queryable.FirstOrDefault(expression).Should().NotBeNull().And
        .Should().BeAnyOf(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Where(expression).Should().NotBeNullOrEmpty().And
        .Contain(kvps.Where(kvp => kvp.Value == value1 || kvp.Value == value2));

      queryable.Any(expression).Should().BeTrue();
      queryable.All(expression).Should().BeFalse();
      queryable.Count(expression).Should().Be(queryable.Count(q => q.Value == value1 || q.Value == value2));

      sw.Stop();
    }

    [Theory]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    [InlineData(100000)]
    [InlineData(1000000)]
    public void StringTest(int collectionSize)
    {
      var kvps = new KeyValuePair<string, string>[collectionSize];

      Random rand1 = new Random();

      for (int i = 0; i < collectionSize; i++)
      {
        kvps[i] = new KeyValuePair<string, string>($"key{i}", Guid.NewGuid().ToString() + Guid.NewGuid() + Guid.NewGuid());
      }

      string value1 = kvps[rand1.Next(0, collectionSize - 1)].Value;
      string value2 = kvps[rand1.Next(0, collectionSize - 1)].Value;

      string queryString = $"value={value1}%2B{value2}";

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
