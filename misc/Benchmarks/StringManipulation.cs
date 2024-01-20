using BenchmarkDotNet.Attributes;

namespace Benchmarks;

[MemoryDiagnoser]
public class StringManipulation
{
    private const string Input = "12,34";

    [Benchmark]
    public int Substring()
    {
        var commaIndex = Input.IndexOf(',');
        var firstNumber = int.Parse(Input[..commaIndex]);
        var secondNumber = int.Parse(Input[(commaIndex + 1)..]);

        return firstNumber + secondNumber;
    }

    [Benchmark]
    public int Slice()
    {
        var inputSpan = Input.AsSpan();
        var commaIndex = Input.IndexOf(',');
        var firstNumber = int.Parse(inputSpan[..commaIndex]);
        var secondNumber = int.Parse(inputSpan[(commaIndex + 1)..]);

        return firstNumber + secondNumber;
    }
}
