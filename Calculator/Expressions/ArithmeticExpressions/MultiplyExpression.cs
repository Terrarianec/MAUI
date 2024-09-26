namespace Calculator.Expressions.ArithmeticExpressions;

public sealed class MultiplyExpression(Expression operand1, Expression operand2) : ArithmeticExpression("×", operand1, operand2, 1)
{
	public override double Solve() => Operand1.Solve() * Operand2.Solve();
}