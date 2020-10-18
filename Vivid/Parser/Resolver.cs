using System;
using System.Collections.Generic;
using System.Linq;

public static class Resolver
{
	/// <summary>
	/// Tries to resolve the specified type if it's unresolved
	/// </summary>
	public static Type? Resolve(Context context, Type type)
	{
		return type is UnresolvedType unresolved ? unresolved.TryResolveType(context) : null;
	}

	/// <summary>
	/// Tries to resolve the given node tree
	/// </summary>
	public static void Resolve(Context context, Node node)
	{
		var resolved = ResolveTree(context, node);

		if (resolved != null)
		{
			node.Replace(resolved);
		}
	}

	/// <summary>
	/// Tries to resolve the problems in the given context
	/// </summary>
	public static void ResolveContext(Context context)
	{
		ResolveVariables(context);

		foreach (var type in context.Types.Values)
		{
			ResolveVariables(type);
			ResolveContext(type);

			if (type.Initialization != null)
			{
				Resolve(type, type.Initialization);
			}
		}

		// Resolve parameter types
		foreach (var function in context.Functions.Values.SelectMany(f => f.Overloads))
		{
			foreach (var parameter in function.Parameters)
			{
				if (parameter.Type != null && parameter.Type.IsUnresolved)
				{
					var type = parameter.Type.To<UnresolvedType>().TryResolveType(context);

					if (!Equals(type, Types.UNKNOWN))
					{
						parameter.Type = type;
					}
				}
			}
		}

		var implementations = context.GetFunctionImplementations().ToList();

		foreach (var implementation in implementations)
		{
			ResolveVariables(implementation);

			// Check if the implementation has a return type and if it's unresolved
			if (implementation.ReturnType?.IsUnresolved ?? false)
			{
				var type = implementation.ReturnType!.To<UnresolvedType>().TryResolveType(implementation);

				if (type != Types.UNKNOWN)
				{
					implementation.ReturnType = type;
				}
			}

			if (implementation.Node != null)
			{
				if (!implementation.IsConstructor && !implementation.Metadata!.IsImported && implementation.Node.Find(i => i.Is(NodeType.RETURN)) == null)
				{
					implementation.ReturnType = Types.UNIT;
				}

				ResolveTree(implementation, implementation.Node!);
			}

			// Resolve short functions
			ResolveContext(implementation);
		}
	}

	/// <summary>
	/// Tries to resolve the problems in the node tree
	/// </summary>
	private static Node? ResolveTree(Context context, Node node)
	{
		if (node is IResolvable resolvable)
		{
			try
			{
				return resolvable.Resolve(context) ?? node;
			}
			catch
			{
				Console.WriteLine("Warning: Resolvable threw an exception");
			}

			return null;
		}

		var iterator = node.First;

		while (iterator != null)
		{
			var resolved = ResolveTree(context, iterator);

			if (resolved != null)
			{
				iterator.Replace(resolved);
			}

			iterator = iterator.Next;
		}

		return node;
	}

	/// <summary>
	/// Returns the number type that should be the outcome when using the two given numbers together
	/// </summary>
	private static Type GetSharedNumber(Number a, Number b)
	{
		return a.Bits > b.Bits ? a : b;
	}

	/// <summary>
	/// Returns the shared type between the types
	/// </summary>
	/// <returns>Success: Shared type between the types, Failure: null</returns>
	public static Type? GetSharedType(Type? expected, Type? actual)
	{
		if (Equals(expected, actual))
		{
			return expected;
		}

		if (expected == Types.UNKNOWN || actual == Types.UNKNOWN)
		{
			return Types.UNKNOWN;
		}

		if (expected is Number x && actual is Number y)
		{
			if (expected is Decimal || actual is Decimal)
			{
				return Types.DECIMAL;
			}

			return GetSharedNumber(x, y);
		}

		return actual.IsSuperTypeDeclared(expected) ? expected : Types.UNKNOWN;
	}

	/// <summary>
	/// Returns the shared type between the types
	/// </summary>
	/// <param name="types">Type list to go through</param>
	/// <returns>Success: Shared type between the types, Failure: null</returns>
	private static Type? GetSharedType(IReadOnlyList<Type> types)
	{
		switch (types.Count)
		{
			case 0:
				return Types.UNKNOWN;
			case 1:
				return types[0];
		}

		var current = types[0];

		for (var i = 1; i < types.Count; i++)
		{
			if (current == null)
			{
				break;
			}

			current = GetSharedType(current, types[i]);
		}

		return current;
	}

	/// <summary>
	/// Returns the types of the child nodes of the given node
	/// </summary>
	/// <returns>Success: Types of the child nodes, Failure: null</returns>
	public static List<Type>? GetTypes(Node node)
	{
		var types = new List<Type>();
		var iterator = node.First;

		while (iterator != null)
		{
			if (iterator is IType x)
			{
				var type = x.GetType();

				if (type == Types.UNKNOWN || type.IsUnresolved)
				{
					// This operation must be aborted since type list cannot contain unresolved types
					return null;
				}

				types.Add(type);
			}
			else
			{
				// This operation must be aborted since type list cannot contain unresolved types
				return null;
			}

			iterator = iterator.Next;
		}

		return types;
	}

	/// <summary>
	/// Tries to get the assign type from the given assign operation
	///</summary>
	private static Type? TryGetTypeFromAssignOperation(Node assign)
	{
		var operation = assign.To<OperatorNode>();

		// Try to resolve type via contextable right side of the assign operator
		if (operation.Operator == Operators.ASSIGN &&
			operation.Right is IType x)
		{
			return x.GetType();
		}

		return Types.UNKNOWN;
	}

	/// <summary>
	/// Tries to resolve the given variable by going through its references
	/// </summary>	
	private static void Resolve(Variable variable)
	{
		var types = new List<Type>();

		// Try resolving the type of the variable from its references
		foreach (var reference in variable.References)
		{
			var parent = reference.Parent;

			if (parent == null) continue;

			if (parent.GetNodeType() == NodeType.OPERATOR) // Locals
			{
				// Reference must be the destination in assign operation in order to resolve the type
				if (parent.First != reference)
				{
					continue;
				}

				var type = TryGetTypeFromAssignOperation(parent);

				if (type != Types.UNKNOWN)
				{
					types.Add(type);
				}
			}
			else if (parent.GetNodeType() == NodeType.LINK) // Members
			{
				// Reference must be the destination in assign operation in order to resolve the type
				if (parent.Last != reference)
				{
					continue;
				}

				parent = parent.Parent;

				if (parent == null || !parent.Is(NodeType.OPERATOR))
				{
					continue;
				}

				var type = TryGetTypeFromAssignOperation(parent!);

				if (type != Types.UNKNOWN)
				{
					types.Add(type);
				}
			}
		}

		// Get the shared type between the references
		var shared = GetSharedType(types);

		if (shared != Types.UNKNOWN)
		{
			// Now the type is resolved
			variable.Type = shared;
		}
	}

	/// <summary>
	/// Tries to resolve all the variables in the given context
	/// </summary>
	private static void ResolveVariables(Context context)
	{
		foreach (var variable in context.Variables.Values.Where(v => Equals(v.Type, Types.UNKNOWN) || v.Type.IsUnresolved))
		{
			Resolve(variable);
		}

		foreach (var subcontext in context.Subcontexts)
		{
			ResolveVariables(subcontext);
		}
	}
}