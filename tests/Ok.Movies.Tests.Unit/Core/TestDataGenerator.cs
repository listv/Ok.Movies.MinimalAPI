namespace Ok.Movies.Tests.Unit.Core;

public class TestDataGenerator
{
    public static IEnumerable<object?[]> GetEmptyStrings()
    {
        yield return new object?[] { null };
        yield return new object[] { "" };
        yield return new object[] { " " };
        yield return new object[] { "\n" };
        yield return new object[] { "\t" };
        yield return new object[] { "\r" };
        yield return new object[] { "\r\n" };
    }
}
