using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Queste.Test
{

  public class UnitTest1
  {
    [Fact]
    public void TestMethod1()
    {
      var kvps = new[]
      {
        new KeyValuePair<string, List<DateTime>>("key1", new List<DateTime> { new DateTime(2016,6,1) }),
        new KeyValuePair<string, List<DateTime>>("key2", new List<DateTime> { new DateTime(2016,6,2) }),
        new KeyValuePair<string, List<DateTime>>("key3", new List<DateTime> { new DateTime(2016,6,3) }),
        new KeyValuePair<string, List<DateTime>>("key4", new List<DateTime> { new DateTime(2016,6,1), new DateTime(2016,6,2) }),
      };

      var expression = ExpressionBuilder.Build<KeyValuePair<string, List<DateTime>>>("value=2016-06-01%2B2016-06-02");

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void TestMethod2()
    {
      var kvps = new[]
      {
        new KeyValuePair<string, DateTime>("key1", new DateTime(2016,6,1)),
        new KeyValuePair<string, DateTime>("key2", new DateTime(2016,6,2)),
        new KeyValuePair<string, DateTime>("key3", new DateTime(2016,6,3)),
        new KeyValuePair<string, DateTime>("key4", new DateTime(2016,6,1)),
      };

      var expression = ExpressionBuilder.Build<KeyValuePair<string, DateTime>>("value=2016-06-01%2B2016-06-02");

      kvps.AsQueryable().Where(expression).Should().NotBeNullOrEmpty();
    }
  }
}
