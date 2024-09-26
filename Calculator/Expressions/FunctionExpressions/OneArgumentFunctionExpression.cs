using System.Reflection;

namespace Calculator.Expressions.FunctionExpressions;

public sealed class OneArgumentFunctionExpression : FunctionExpression
{
	private readonly Expression _param;
	private static readonly Dictionary<string, OneArgumentFunction> _functions = [];
	private static Dictionary<string, OneArgumentFunction> Functions
	{
		get
		{
			if (_functions.Count == 0)
			{
				var functions = typeof(OneArgumentFunctionExpression).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
					.Where(m => !m.IsSpecialName);

				foreach (var fn in functions)
					_functions.Add(fn.Name.ToLower(), fn.CreateDelegate<OneArgumentFunction>());
			}

			return _functions;
		}
	}

	public delegate double OneArgumentFunction(double v);

	public Expression Param => _param;

	public OneArgumentFunctionExpression(string fn, Expression param) : base(fn, [param])
	{
		if (!Functions.ContainsKey(fn))
			throw new InvalidOperationException(fn);

		_param = param;
	}

	public override double Solve() => Functions[FunctionName](Param.Solve());

	private static double Ln(double v) => Math.Log(v);
	private static double Lg(double v) => Math.Log10(v);
	private static double Sqrt(double v) => Math.Sqrt(v);
	private static double Cos(double v) => Math.Cos(v);
	private static double Sin(double v) => Math.Sin(v);
	private static double Tg(double v) => Math.Tan(v);
	private static double Ctg(double v) => 1 / Math.Tan(v);
	private static double Fact(double v)
	{
		if (v < 0 || Math.Round(v) != v)
			throw new ArgumentOutOfRangeException(nameof(v), "Факториал вычисляется только для неотрицательных целых чисел");

		if (v == 0)
			return 1;

		var n = 1L;

		for (; v > 0; v--)
			n *= (int)v;

		return n;
	}
}