using Calculator.Expressions.FunctionExpressions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator.Expressions;

public abstract class Expression
{
	public abstract double Solve();
	public abstract string Beautify();
	public override string ToString() => Beautify();

	protected const char PLACEHOLDER_SYMBOL = 'p';
	private static readonly string decimalRegexPattern = @"(((?:-)?\d+(?:\.\d+)?(?:E(?:-)?\d+)?)|((?:-)?∞)|(NaN))";
	private static readonly string expressionPlaceholderRegexPattern = @$"({PLACEHOLDER_SYMBOL}\d+)";
	protected static readonly string ValuePattern = $"({decimalRegexPattern}|e|π|{expressionPlaceholderRegexPattern})";
	private static readonly Regex oneArgumentFunctionRegex = new(@$"(?<fn>(?:[a-z]+))\(\s*(?<v>{ValuePattern})\s*\)", RegexOptions.Compiled);
	private static readonly Regex twoArgumentFunctionRegex = new(@$"(?<fn>(?:[a-z]+))\(\s*(?<v1>{ValuePattern})\s*,\s*(?<v2>{ValuePattern})\s*\)", RegexOptions.Compiled);
	private static readonly Regex absRegex = new(@$"\|\s*(?<v>{ValuePattern})\s*\|", RegexOptions.Compiled);
	private static readonly Regex expressionInBracketsRegex = new(@"(?<![a-z]+)\((?<expression>.+)\)", RegexOptions.Compiled);
	private static readonly Regex extraBracketsRegex = new(@"^\((?<expression>.+)\)$", RegexOptions.Compiled);
	private static readonly Regex placeholderRegex = new($"^{expressionPlaceholderRegexPattern}$", RegexOptions.Compiled);
	private static readonly Regex negativePlaceholderRegex = new(@$"^-(?<placeholder>{expressionPlaceholderRegexPattern})$", RegexOptions.Compiled);


	public static Expression Parse(string expression) => Parse(expression, []);
	private static Expression Parse(string expression, List<Expression> placeholders)
	{
		string previousResult = string.Empty;

		do
		{
			previousResult = expression;

			if (placeholderRegex.IsMatch(expression))
				break;

			if (absRegex.IsMatch(expression))
			{
				expression = absRegex.Replace(expression, r =>
					{
						var v = r.Groups["v"].Value;

						placeholders.Add(new AbsoluteValueExpression(GetExpression(v, placeholders)));

						return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
					});

				continue;
			}

			if (oneArgumentFunctionRegex.IsMatch(expression))
			{
				expression = oneArgumentFunctionRegex.Replace(expression, r =>
				{
					var fn = r.Groups["fn"].Value;
					var v = r.Groups["v"].Value;

					placeholders.Add(new OneArgumentFunctionExpression(fn, GetExpression(v, placeholders)));

					return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
				});

				continue;
			}

			if (twoArgumentFunctionRegex.IsMatch(expression))
			{
				expression = twoArgumentFunctionRegex.Replace(expression, r =>
				{
					var fn = r.Groups["fn"].Value;
					var v1 = r.Groups["v1"].Value;
					var v2 = r.Groups["v2"].Value;

					placeholders.Add(new TwoArgumentsFunctionExpression(fn, GetExpression(v1, placeholders), GetExpression(v2, placeholders)));

					return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
				});

				continue;
			}

			if (extraBracketsRegex.IsMatch(expression))
			{
				expression = extraBracketsRegex.Replace(expression, r =>
				{
					var expression = r.Groups["expression"].Value;

					return expression;
				});

				continue;
			}

			if (expressionInBracketsRegex.IsMatch(expression))
			{
				expression = expressionInBracketsRegex.Replace(expression, r =>
				{
					var expression = r.Groups["expression"].Value;

					placeholders.Add(Parse(expression));

					return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
				});

				continue;
			}

			if (ArithmeticExpression.Replace(ref expression, placeholders))
				continue;

			if (expression == "e" || expression == "π" || new Regex($"^{decimalRegexPattern}$", RegexOptions.Compiled).IsMatch(expression))
			{
				expression = new Regex(ValuePattern, RegexOptions.Compiled).Replace(expression, r =>
				{
					var value = r.Value;

					switch (value)
					{
						case "e":
							placeholders.Add(new ValueExpression(Math.E));
							break;

						case "π":
							placeholders.Add(new ValueExpression(Math.PI));
							break;

						case "∞":
							placeholders.Add(new ValueExpression(double.PositiveInfinity));
							break;

						case "-∞":
							placeholders.Add(new ValueExpression(double.NegativeInfinity));
							break;

						default:
							{
								if (!double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var d))
									throw new InvalidDataException(value);

								placeholders.Add(new ValueExpression(d));
							}
							break;
					}

					return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
				});

				continue;
			}

			if (negativePlaceholderRegex.IsMatch(expression))
			{
				expression = negativePlaceholderRegex.Replace(expression, r =>
				{
					var placeholder = r.Groups["placeholder"].Value;

					placeholders.Add(new NegativeExpression(GetExpression(placeholder, placeholders)));

					return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
				});

				continue;
			}

			throw new ArgumentException($"'{expression}' is invalid expression");
		} while (previousResult != expression);

		return GetExpression(expression, placeholders);
	}

	public static Expression GetExpression(string value, List<Expression> placeholders)
	{
		return placeholderRegex.IsMatch(value)
			? placeholders.ElementAtOrDefault(int.Parse(value[PLACEHOLDER_SYMBOL.ToString().Length..]))
				?? throw new ArgumentException($"Invalid expression")
			: Parse(value);
	}
}