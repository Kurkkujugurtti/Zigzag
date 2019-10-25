public class OperatorToken : Token
{
	public Operator Operator { get; private set; }

	public OperatorToken(string identifier) : base(TokenType.OPERATOR)
	{
		Operator = Operators.Get(identifier);
	}

	public OperatorToken(Operator @operator) : base(TokenType.OPERATOR)
	{
		Operator = @operator;
	}
}
