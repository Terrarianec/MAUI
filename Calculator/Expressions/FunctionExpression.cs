namespace Calculator.Expressions;

public abstract class FunctionExpression(string fn, params Expression[] parameters) : Expression
{
	public string FunctionName => fn;
	protected Expression[] Parameters = parameters;

	public override string Beautify() => $"{FunctionName}({string.Join(", ", Parameters.Select(p => p.Beautify()))})";
}