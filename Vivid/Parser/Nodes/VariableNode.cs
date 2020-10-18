using System;
using System.Collections.Generic;

public class VariableNode : Node, IType
{
	public Variable Variable { get; set; }

	public VariableNode(Variable variable)
	{
		Variable = variable;
		Variable.References.Add(this);
	}

	public new Type? GetType()
	{
		return Variable.Type;
	}

	public override NodeType GetNodeType()
	{
		return NodeType.VARIABLE;
	}

	public override bool Equals(object? other)
	{
		return other is VariableNode node &&
				base.Equals(other) &&
				EqualityComparer<Variable>.Default.Equals(Variable, node.Variable);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.GetHashCode(), Variable);
	}
}