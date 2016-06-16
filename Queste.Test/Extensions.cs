using System.Collections.Generic;
using System.Linq;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Queste.Test
{
  public static class Extensions
  {
    public static void BeAnyOf<TAssertions, TSource>(this TAssertions assertions, IEnumerable<TSource> expectations,
                                                     string because = null)
      where TAssertions : ObjectAssertions
    {
      Execute.Assertion
        .ForCondition(expectations.Any(e => Equals((TSource) ((ObjectAssertions) assertions.Subject).Subject, e)))
        .BecauseOf(because)
        .FailWith("Expected {context:string} to be any of {0}{reason}", expectations);
    }
  }
}