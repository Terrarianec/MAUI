using System.Reflection;
using System.Text.RegularExpressions;

namespace Calculator.Expressions;

public abstract class ArithmeticExpression(string @operator, Expression operand1, Expression operand2, int priority = 0) : Expression
{
	protected Expression Operand2 = operand2;
	protected Expression Operand1 = operand1;

	private struct Operation(string @operator, int priority, Type arithmeticExpression)
	{
		public readonly string Operator => @operator;
		public readonly int Priority => priority;
		public readonly Type ArithmeticExpression => arithmeticExpression;
	}
	private static string _arithmeticExpressionPatternTemplate = @$"(?<v1>{ValuePattern})\s*(?<operator>(?:<template>))\s*(?<v2>{ValuePattern})";
	private static List<IGrouping<int, Operation>> _operationGroups = [];
	private static List<IGrouping<int, Operation>> OperationGroups
	{
		get
		{
			if (_operationGroups.Count == 0)
			{
				var operations = new List<Operation>();
				var arithmeticExpressionTypes = Assembly.GetExecutingAssembly()
														.GetTypes()
														.Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(ArithmeticExpression)));

				foreach (var type in arithmeticExpressionTypes)
				{
					var instance = (ArithmeticExpression)Activator.CreateInstance(type, new ValueExpression(), new ValueExpression())!;
					operations.Add(new Operation(instance.Operator, instance.Priority, type));
				}

				_operationGroups = [.. operations.GroupBy(operation => operation.Priority).OrderByDescending(group => group.Key)];
			}

			return _operationGroups;
		}
	}

	private static List<string> HighPriorityOperators
	{
		get
		{
			var minusPriority = OperationGroups.Where(group => group.Any(operation => operation.Operator == "-")).First().Key;

			return OperationGroups.Where(group => group.Key >= minusPriority).SelectMany(group => group.Select(operation => operation.Operator)).ToList();
		}
	}

	public string Operator => @operator;
	public int Priority => priority;

	public override string Beautify()
	{
		var result = string.Empty;

		if (Operand1 is ArithmeticExpression operand1 && operand1.Priority < Priority)
			result += $"({operand1})";
		else
			result += $"{Operand1}";

		result += $" {@operator} ";

		if (Operand2 is ArithmeticExpression operand2 && operand2.Priority < Priority)
			result += $"({operand2})";
		else
			result += $"{Operand2}";

		return result;
	}

	public static bool Replace(ref string expression, List<Expression> placeholders)
	{
		var pattern = $@"(?<v>{ValuePattern})-(?<expression>{_arithmeticExpressionPatternTemplate.Replace("<template>", string.Join("|", HighPriorityOperators.Select(Regex.Escape)))})";
		var minusAndOtherOperationRegex = new Regex(pattern, RegexOptions.Compiled);

		if (minusAndOtherOperationRegex.IsMatch(expression))
		{
			expression = minusAndOtherOperationRegex.Replace(expression, r =>
			{
				var v = r.Groups["v"].Value;
				var expression = r.Groups["expression"].Value;

				placeholders.Add(Parse(expression));

				return $"{v}-{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
			});

			return true;
		}

		var replaced = false;

		foreach (var group in OperationGroups)
		{
			var regex = new Regex(_arithmeticExpressionPatternTemplate.Replace("<template>", string.Join("|", group.Select(operation => Regex.Escape(operation.Operator)))));

			replaced = regex.IsMatch(expression);
			if (!replaced)
				continue;

			expression = regex.Replace(expression, r =>
			{
				var @operator = r.Groups["operator"].Value;
				var v1 = r.Groups["v1"].Value;
				var v2 = r.Groups["v2"].Value;

				placeholders.Add((ArithmeticExpression)Activator.CreateInstance(
					group.First(operation => operation.Operator == @operator).ArithmeticExpression,
					GetExpression(v1, placeholders),
					GetExpression(v2, placeholders))!
				);

				return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
			});

			break;
		}

		return replaced;
	}
}