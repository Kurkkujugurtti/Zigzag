using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class FunctionImplementation : Context
{
	public Function Metadata { get; set; }

	public new List<Variable> Variables => base.Variables.Values.Concat(Parameters).ToList();
	
	public List<Variable> Parameters { get; private set; } = new List<Variable>();
	public List<Type> ParameterTypes => Parameters.Select(p => p.Type).ToList();
	
	public List<Variable> Locals => base.Variables.Values.Where(v => v.Category == VariableCategory.LOCAL)
										.Concat(Subcontexts.SelectMany(c => c.Variables.Values.Where(v => v.Category == VariableCategory.LOCAL))).ToList();
	public int LocalMemorySize => Variables.Where(v => v.Category == VariableCategory.LOCAL).Select(v => v.Type.Size).Sum() +
									Subcontexts.Sum(c => c.Variables.Values.Where(v => v.Category == VariableCategory.LOCAL).Select(v => v.Type.Size).Sum());
	
	public Node? Node { get; set; }

	public List<Node> References { get; private set; } = new List<Node>();

	public Type ReturnType { get; set; }

	public bool IsInline => References.Count == 1 && false;
	
	/// <summary>
	/// Optionally links this function to some context
	/// </summary>
	/// <param name="context">Context to link into</param>
	public FunctionImplementation(Context context = null)
	{
		if (context != null)
		{
			Link(context);
		}
	}

	/// <summary>
	/// Sets the function parameters
	/// </summary>
	/// <param name="parameters">Parameters packed with name and type</param>
	public void SetParameters(List<Parameter> parameters)
	{
		foreach (var properties in parameters)
		{
			var parameter = new Variable(properties.Type, VariableCategory.PARAMETER, properties.Name, AccessModifier.PUBLIC);
			parameter.Context = this;

			Parameters.Add(parameter);
		}
	}

	/// <summary>
	/// Implements the function using the given blueprint
	/// </summary>
	/// <param name="blueprint">Tokens from which to implement the function</param>
	public void Implement(List<Token> blueprint)
	{
		Node = new ImplementationNode(this);
		Parser.Parse(Node, this, blueprint, 0, 20);
	}

	/// <summary>
	/// Returns the header of the function.
	/// Examples:
	/// Name(Type, Type, ...) [-> Result]
	/// f(number, number) -> number
	/// g(A, B) -> C
	/// h() -> A
	/// i()
	/// </summary>
	/// <returns>Header of the function</returns>
	public string GetHeader()
	{
		var header = Metadata.Name + '(';

		foreach (var type in ParameterTypes)
		{
			header += $"{type.Name}, ";
		}

		if (ParameterTypes.Count > 0)
		{
			header = header.Substring(0, header.Length - 2);
		}

		if (ReturnType != null)
		{
			header += $") -> {ReturnType.Name}";
		}
		else
		{
			header += ')';
		}

		return header;
	}

	public override bool IsLocalVariableDeclared(string name)
	{
		return Parameters.Any(p => p.Name == name) || base.IsLocalVariableDeclared(name);
	}

	public override bool IsVariableDeclared(string name)
	{
		return Parameters.Any(p => p.Name == name) || base.IsVariableDeclared(name);
	}

	public override Variable GetVariable(string name)
	{
		if (Parameters.Any(p => p.Name == name))
		{
			return Parameters.Find(p => p.Name == name);
		}

		return base.GetVariable(name);
	}
}
