using System.Reflection;

namespace Calculator.Expressions.FunctionExpressions;

public sealed class TwoArgumentsFunctionExpression : FunctionExpression
{
	private readonly Expression _param1;
	private readonly Expression _param2;
	private static readonly Dictionary<string, TwoArgumentsFunction> _functions = [];
	private static Dictionary<string, TwoArgumentsFunction> Functions
	{
		get
		{
			if (_functions.Count == 0)
			{
				var functions = typeof(TwoArgumentsFunctionExpression).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
					.Where(m => !m.IsSpecialName);

				foreach (var fn in functions)
					_functions.Add(fn.Name.ToLower(), fn.CreateDelegate<TwoArgumentsFunction>());
			}

			return _functions;
		}
	}

	public delegate double TwoArgumentsFunction(double a, double b);

	public Expression Param1 => _param1;
	public Expression Param2 => _param2;

	public TwoArgumentsFunctionExpression(string fn, Expression param1, Expression param2) : base(fn, [param1, param2])
	{
		if (!Functions.ContainsKey(fn))
			throw new InvalidOperationException(fn);

		_param1 = param1;
		_param2 = param2;
	}

	public override double Solve() => Functions[FunctionName](Param1.Solve(), Param2.Solve());

	private static double Log(double a, double b) => Math.Log(a, b);
	private static double Root(double a, double b) => Math.Pow(a, 1 / b);
}