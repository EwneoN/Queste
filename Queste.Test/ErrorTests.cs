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
    public void ExpressionShouldNotReturnNull1(string queryString)
    {
      ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime>>(queryString).Should().NotBeNull();
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
    public void FunctionShouldNotReturnNull1(string queryString)
    {
      ExpressionBuilder.BuildFunction<KeyValuePair<string, DateTime>>(queryString).Should().NotBeNull();
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

    [Theory]
    [InlineData("")]
    [InlineData("name=something")]
    [InlineData("name=something&")]
    [InlineData("=")]
    [InlineData("=")]
    public void ExpressionShouldNotThrow(string queryString)
    {
      ((Action)(() => ExpressionBuilder.BuildExpression<KeyValuePair<string, DateTime>>(queryString))).ShouldNotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("name=something")]
    [InlineData("name=something&")]
    [InlineData("=")]
    [InlineData("=")]
    public void FunctionShouldNotThrow(string queryString)
    {
      ((Action)(() => ExpressionBuilder.BuildFunction<KeyValuePair<string, DateTime>>(queryString))).ShouldNotThrow();
    }
  }
}
