namespace Calculator;

internal class Operator<T>(char @char, Func<T, T, T> action) where T : struct
{
	public char Char { get; } = @char;
	public T Execute(T a, T b) => action.Invoke(a, b);
}
