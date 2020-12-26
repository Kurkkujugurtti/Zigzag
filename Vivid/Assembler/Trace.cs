using System.Collections.Generic;
using System.Linq;

public static class Trace
{
	public static List<Directive> GetDirectives(Unit unit, Result result)
	{
		var directives = new List<Directive>();
		var calls = new List<CallInstruction>();
		var returns = new List<ReturnInstruction>();

		var start = result.Lifetime.Start;
		var end = result.Lifetime.End;

		end = end == -1 ? unit.Instructions.Count - 1 : end;
		start = start == -1 ? unit.Instructions.Count - 1 : start;

		for (var i = start; i <= end; i++)
		{
			if (!unit.Instructions[i].Is(InstructionType.CALL))
			{
				continue;
			}

			calls.Add(unit.Instructions[i].To<CallInstruction>());
		}

		// There can be calls which intersect with the lifetime of the value but it does not mean the value is used after them
		if (calls.Any(i => i.Position < end))
		{
			directives.Add(new NonVolatilityDirective());
		}

		for (var i = start; i <= end; i++)
		{
			if (!unit.Instructions[i].Is(InstructionType.RETURN))
			{
				continue;
			}
			
			var instruction = unit.Instructions[i].To<ReturnInstruction>();

			if (!instruction.Object?.Equals(result) ?? true)
			{
				continue;
			}

			directives.Add(new SpecificRegisterDirective(instruction.ReturnRegister));
			break;
		}

		var avoid = new List<Register>();

		if (Assembler.IsX64)
		{
			var divisions = new List<DivisionInstruction>();

			for (var i = start; i <= end; i++)
			{
				if (!unit.Instructions[i].Is(InstructionType.DIVISION))
				{
					continue;
				}

				var division = unit.Instructions[i].To<DivisionInstruction>();

				if (division.First.Equals(result))
				{
					directives.Add(new SpecificRegisterDirective(unit.GetNumeratorRegister()));
				}
				else
				{
					avoid.Add(unit.GetNumeratorRegister());
				}

				avoid.Add(unit.GetRemainderRegister());
			}
		}

		foreach (var call in calls)
		{
			foreach (var iterator in call.ParameterInstructions)
			{
				if (!iterator.Is(InstructionType.MOVE))
				{
					continue;
				}

				var instruction = iterator.To<MoveInstruction>();

				if (!instruction.Second.Equals(result) || !instruction.First.IsAnyRegister)
				{
					continue;
				}

				directives.Add(new SpecificRegisterDirective(instruction.First.Value.To<RegisterHandle>().Register));
			}
		}

		var registers = calls.SelectMany(i => i.ParameterInstructions).Where(i => i.Is(InstructionType.MOVE)).Cast<MoveInstruction>()
			.Select(i => i.First).Where(i => i.IsAnyRegister).Select(i => i.Value.To<RegisterHandle>().Register).Distinct();

		directives.Add(new AvoidRegistersDirective(avoid.Concat(registers).ToArray()));

		return directives;
	}


	/// <summary>
	/// Returns whether the specified result lives through at least one call
	/// </summary>
	public static bool IsUsedAfterCall(Unit unit, Result result)
	{
		var start = result.Lifetime.Start;
		var end = result.Lifetime.End;

		end = end == -1 ? unit.Instructions.Count : end;
		start = start == -1 ? unit.Instructions.Count : start;

		for (var i = start + 1; i < end; i++)
		{
			if (unit.Instructions[i].Is(InstructionType.CALL))
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Returns whether the specified result stays constant during the lifetime of the specified parent
	/// </summary>
	public static bool IsLoadingRequired(Unit unit, Result result)
	{
		if (IsUsedAfterCall(unit, result))
		{
			return true;
		}

		var start = result.Lifetime.Start;
		var end = result.Lifetime.End;

		end = end == -1 ? unit.Instructions.Count : end;
		start = start == -1 ? unit.Instructions.Count : start;

		return unit.Instructions.GetRange(start, end - start).Any(i => 
			i.Is(InstructionType.GET_VARIABLE) && i.To<GetVariableInstruction>().Mode == AccessMode.WRITE && !i.To<GetVariableInstruction>().Result.Equals(result) ||
			i.Is(InstructionType.GET_OBJECT_POINTER) && i.To<GetObjectPointerInstruction>().Mode == AccessMode.WRITE && !i.To<GetObjectPointerInstruction>().Result.Equals(result) ||
			i.Is(InstructionType.GET_MEMORY_ADDRESS) && i.To<GetMemoryAddressInstruction>().Mode == AccessMode.WRITE && !i.To<GetMemoryAddressInstruction>().Result.Equals(result)
		);
	}
}