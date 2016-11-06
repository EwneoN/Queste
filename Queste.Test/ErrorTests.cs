using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Queste.Test
{
  public class ErrorTests
  {
    [Theory]
    [InlineData("")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("      ")]
    [InlineData("      \t")]
    [InlineData("      \n")]
    [InlineData("      \r\n")]
    [InlineData(null)]
    public void ExpressionShouldReturnNull1(string queryString)
    {
      ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime>>(queryString).Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("\r\n")]
    [InlineData("      ")]
    [InlineData("      \t")]
    [InlineData("      \n")]
    [InlineData("      \r\n")]
    [InlineData(null)]
    public void FunctionShouldReturnNull1(string queryString)
    {
      ExpressionBuilder.BuildFunction<KeyValuePair<string, DateTime>>(queryString).Should().BeNull();
    }

    [Theory]
    [InlineData("name=something")]
    [InlineData("=")]
    [InlineData("=")]
    public void ExpressionShouldNotReturnNull2(string queryString)
    {
      ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime>>(queryString).Should().NotBeNull();
    }

    [Theory]
    [InlineData("name=something")]
    [InlineData("=")]
    [InlineData("=")]
    public void FunctionShouldNotReturnNull2(string queryString)
    {
      ExpressionBuilder.BuildFunction<KeyValuePair<string, DateTime>>(queryString).Should().NotBeNull();
    }
  }
}
