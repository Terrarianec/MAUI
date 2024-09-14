namespace Calculator;

internal class Operator(char @char, Func<int, int, int> action)
{
	public char Char { get; } = @char;
	public int Execute(int a, int b) => action.Invoke(a, b);
}
