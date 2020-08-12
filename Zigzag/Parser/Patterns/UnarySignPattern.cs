
using System.Collections.Generic;

public class UnarySignPattern : Pattern
{
	public const int PRIORITY = 18;

	public const int OPERATOR = 0;
	public const int SIGN = 1;
	public const int OBJECT = 2;

	/// Example:
	/// a = -x
	public UnarySignPattern() : base
	(
		TokenType.OPERATOR | TokenType.KEYWORD | TokenType.OPTIONAL,
		TokenType.OPERATOR,
		TokenType.OBJECT
	) {}

	public override bool Passes(Context context, PatternState state, List<Token> tokens)
	{
		var sign = tokens[SIGN].To<OperatorToken>().Operator;
		return (sign == Operators.ADD || sign == Operators.SUBTRACT) && (tokens[OPERATOR].Type != TokenType.NONE || tokens[SIGN].IsFirst);
	}

	public override Node? Build(Context context, List<Token> tokens)
	{
		var target = Singleton.Parse(context, tokens[OBJECT]);
		var sign = tokens[SIGN].To<OperatorToken>().Operator;

		if (target is NumberNode number)
		{
			if (sign == Operators.SUBTRACT)
			{
				number.Negate();
			}

			return number;
		}

		if (sign == Operators.SUBTRACT)
		{
			return new NegateNode(target);
		}

		return target;
	}

	public override int GetPriority(List<Token> tokens)
	{
		return PRIORITY;
	}

	public override int GetStart()
	{
		return SIGN;
	}
}