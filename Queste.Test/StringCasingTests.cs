using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Queste.Test
{
  public class StringCasingTests
  {
    [Theory]
    [InlineData("KEY", "KEY", "KEY", "KEY")]
    [InlineData("key", "KEY", "KEY", "KEY")]
    [InlineData("key", "key", "KEY", "KEY")]
    [InlineData("key", "key", "key", "KEY")]
    [InlineData("key", "key", "key", "key")]
    [InlineData("KEY", "key", "key", "key")]
    [InlineData("KEY", "KEY", "key", "key")]
    [InlineData("KEY", "KEY", "KEY", "key")]
    [InlineData("VALUE", "VALUE", "VALUE", "VALUE")]
    [InlineData("value", "VALUE", "VALUE", "VALUE")]
    [InlineData("value", "value", "value", "VALUE")]
    [InlineData("value", "value", "value", "value")]
    [InlineData("VALUE", "value", "value", "value")]
    [InlineData("VALUE", "VALUE", "value", "value")]
    [InlineData("VALUE", "VALUE", "VALUE", "value")]
    public void Test1(string searchKey, string searchValue, string key, string value)
    {
      Dictionary<string, string> strings = new Dictionary<string, string>
      {
        [key] = value
      };

      Action action = () =>
      {
        strings.Where($"?{searchKey}={searchValue}").ToArray().Should().NotBeNullOrEmpty();
      };

      action.ShouldNotThrow();
    }
  }
}
