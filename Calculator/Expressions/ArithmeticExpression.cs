namespace Calculator.Expressions;

public abstract class ArithmeticExpression(string @operator, Expression operand1, Expression operand2, int priority = 0) : Expression
{
	protected Expression Operand2 = operand2;
	protected Expression Operand1 = operand1;
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
}