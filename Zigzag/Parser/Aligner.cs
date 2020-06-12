using System.Collections.Generic;
using System.Linq;

public class Aligner
{
	private const int MEMBER_FUNCTION_PARAMETER_OFFSET = 2;
	private const int GLOBAL_FUNCTION_PARAMETER_OFFSET = 1;
	
	/// <summary>
	/// Aligns all variable and parameters recursively in the context
	/// </summary>
	/// <param name="context">Context to scan through</param>
	public static void Align(Context context)
	{
		// Align types and subtypes
		foreach (var type in context.Types.Values)
		{
			Aligner.Align(type);
		}

		// Align function variables in memory
		foreach (var implementation in context.GetImplementedFunctions())
		{
			// Align function parameters using global function offset
			Aligner.Align(implementation, GLOBAL_FUNCTION_PARAMETER_OFFSET);
		}
	}

	/// <summary>
	/// Aligns the local memory used by a function
	/// </summary>
	public static void AlignLocalMemory(IEnumerable<Variable> variables, List<TemporaryMemoryHandle> temporary_handles, int top)
	{
		var position = -top;

		foreach (var variable in variables)
		{
			position -= variable.Type!.ReferenceSize;
			variable.Alignment = position;
		}

		while (temporary_handles.Count > 0)
		{
			var first = temporary_handles.First();
			var identifier = first.Identifier;

			position -= first.Size.Bytes;

			var copies = temporary_handles.Where(t => t.Identifier.Equals(identifier)).ToList();

			copies.ForEach(c => c.Offset = position);
			copies.ForEach(c => temporary_handles.Remove(c));
		}

		foreach (var temporary_handle in temporary_handles)
		{
			temporary_handle.Offset = position;
		}
	}

	/// <summary>
	/// Aligns member variables, function and subtypes
	/// </summary>
	/// <param name="type">Type to scan through</param>
	private static void Align(Type type)
	{
		var position = 0;

		// Member variables:
		foreach (var variable in type.Variables.Values)
		{
			if (variable.IsUsed)
			{
				variable.Alignment = position;
				position += variable.Type!.ReferenceSize;
			}
		}

		// Member functions:
		foreach (var implementation in type.GetImplementedFunctions())
		{
			Aligner.Align(implementation, MEMBER_FUNCTION_PARAMETER_OFFSET);
		}

		// Constructors:
		foreach (var constructor in type.GetConstructors().Overloads)
		{
			foreach (var implementation in constructor.Implementations)
			{
				Aligner.Align(implementation, GLOBAL_FUNCTION_PARAMETER_OFFSET);
			}
		}

		// Destructors:
		foreach (var destructor in type.GetDestructors().Overloads)
		{
			foreach (var implementation in destructor.Implementations)
			{
				Aligner.Align(implementation, MEMBER_FUNCTION_PARAMETER_OFFSET);
			}
		}

		// Align subtypes
		foreach (var subtype in type.Types.Values)
		{
			Aligner.Align(subtype);
		}
	}

	/// <summary>
	/// Aligns function variables
	/// </summary>
	/// <param name="function">Function to scan through</param>
	/// <param name="offset">Base offset to apply to all variables</param>
	private static void Align(FunctionImplementation function, int offset)
	{
		var position = offset * Parser.Size.Bytes;

		// Align the this pointer if it exists
		if (function.Variables.TryGetValue(Function.THIS_POINTER_IDENTIFIER, out Variable? this_pointer))
		{
			this_pointer.Alignment = position - Parser.Size.Bytes;
		}

		// Parameters:
		foreach (var variable in function.Parameters)
		{
			if (variable.Category == VariableCategory.PARAMETER)
			{
				variable.Alignment = position;
				position += variable.Type!.ReferenceSize;
			}
		}

		position = 0;

		// Local variables:
		/*foreach (var variable in function.Locals)
		{
			if (variable.Category == VariableCategory.LOCAL)
			{
				variable.Alignment = position;
				position += variable.Type!.ReferenceSize;
			}
		}*/
	}
}