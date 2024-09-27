using Calculator.Expressions.ArithmeticExpressions;
using Calculator.Expressions.FunctionExpressions;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator.Expressions;

public abstract class Expression
{
    public abstract double Solve();
    public abstract string Beautify();
    public override string ToString() => Beautify();

    private const char PLACEHOLDER_SYMBOL = 'p';
    private static readonly string decimalRegexStr = @"(((?:-)?\d+(?:\.\d+)?(?:E(?:-)?\d+)?)|((?:-)?∞)|(NaN))";
    private static readonly string expressionPlaceholderRegexStr = @$"({PLACEHOLDER_SYMBOL}\d+)";
    private static readonly string valueRegexStr = $"({decimalRegexStr}|e|π|{expressionPlaceholderRegexStr})";
    private static readonly string arithmeticExpressionRegexTemplate = @$"(?<v1>{valueRegexStr})\s*(?<operator>(?:<template>))\s*(?<v2>{valueRegexStr})";
    private static readonly Regex oneArgumentFunctionRegex = new(@$"(?<fn>(?:[a-z]+))\(\s*(?<v>{valueRegexStr})\s*\)", RegexOptions.Compiled);
    private static readonly Regex twoArgumentFunctionRegex = new(@$"(?<fn>(?:[a-z]+))\(\s*(?<v1>{valueRegexStr})\s*,\s*(?<v2>{valueRegexStr})\s*\)", RegexOptions.Compiled);
    private static readonly Regex absRegex = new(@$"\|\s*(?<v>{valueRegexStr})\s*\|", RegexOptions.Compiled);
    private static readonly Regex powerRegex = new(arithmeticExpressionRegexTemplate.Replace("<template>", "\\^"), RegexOptions.Compiled);
    private static readonly Regex multiplyOrDivisionRegex = new(arithmeticExpressionRegexTemplate.Replace("<template>", @"\×|\÷"), RegexOptions.Compiled);
    private static readonly Regex additionOrSubtractionRegex = new(arithmeticExpressionRegexTemplate.Replace("<template>", @"\+|\-"), RegexOptions.Compiled);
    private static readonly Regex expressionInBracketsRegex = new(@"(?<![a-z]+)\((?<expression>[^)(]+)\)", RegexOptions.Compiled);
    private static readonly Regex extraBracketsRegex = new(@"^\((?<expression>[^)()]+)\)$", RegexOptions.Compiled);
    private static readonly Regex placeholderRegex = new($"^{expressionPlaceholderRegexStr}$", RegexOptions.Compiled);

    public static Expression Parse(string expression) => Parse(expression, []);
    private static Expression Parse(string expression, List<Expression> placeholders)
    {
        string previousResult = string.Empty;

        Expression getExpression(string v)
        {
            return placeholderRegex.IsMatch(v)
                ? placeholders.ElementAtOrDefault(int.Parse(v[PLACEHOLDER_SYMBOL.ToString().Length..])) 
                    ?? throw new ArgumentException($"'{expression}' is invalid expression")
                : Parse(v);
        }

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

                        placeholders.Add(new AbsoluteValueExpression(getExpression(v)));

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

                    placeholders.Add(new OneArgumentFunctionExpression(fn, getExpression(v)));

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

                    placeholders.Add(new TwoArgumentsFunctionExpression(fn, getExpression(v1), getExpression(v2)));

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

            if (powerRegex.IsMatch(expression))
            {
                expression = powerRegex.Replace(expression, r =>
                {
                    var @operator = r.Groups["operator"].Value;
                    var v1 = r.Groups["v1"].Value;
                    var v2 = r.Groups["v2"].Value;

                    placeholders.Add(new PowerExpression(getExpression(v1), getExpression(v2)));

                    return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
                });

                continue;
            }

            if (multiplyOrDivisionRegex.IsMatch(expression))
            {
                expression = multiplyOrDivisionRegex.Replace(expression, r =>
                {
                    var @operator = r.Groups["operator"].Value;
                    var v1 = r.Groups["v1"].Value;
                    var v2 = r.Groups["v2"].Value;

                    placeholders.Add(
                        @operator == "×"
                            ? new MultiplyExpression(getExpression(v1), getExpression(v2))
                            : new DivisionExpression(getExpression(v1), getExpression(v2))
                    );

                    return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
                });

                continue;
            }

            if (additionOrSubtractionRegex.IsMatch(expression))
            {
                expression = additionOrSubtractionRegex.Replace(expression, r =>
                {
                    var @operator = r.Groups["operator"].Value;
                    var v1 = r.Groups["v1"].Value;
                    var v2 = r.Groups["v2"].Value;

                    placeholders.Add(
                        @operator == "+"
                            ? new AdditionExpression(getExpression(v1), getExpression(v2))
                            : new SubtractionExpression(getExpression(v1), getExpression(v2))
                    );

                    return $"{PLACEHOLDER_SYMBOL}{placeholders.Count - 1}";
                });

                continue;
            }

            if (expression == "e" || expression == "π" || new Regex($"^{decimalRegexStr}$", RegexOptions.Compiled).IsMatch(expression))
            {
                expression = new Regex(valueRegexStr, RegexOptions.Compiled).Replace(expression, r =>
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

            throw new ArgumentException($"'{expression}' is invalid expression");
        } while (previousResult != expression);

        return getExpression(expression);
    }
}