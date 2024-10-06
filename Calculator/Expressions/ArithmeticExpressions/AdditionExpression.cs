namespace Calculator.Expressions.ArithmeticExpressions;

public sealed class AdditionExpression(Expression operand1, Expression operand2) : ArithmeticExpression("+", operand1, operand2, 0)
{
	public override double Solve() => Operand1.Solve() + Operand2.Solve();
}