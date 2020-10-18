﻿using System;
using System.Collections.Generic;
using System.Linq;

public class FunctionList
{
	public List<Function> Overloads { get; } = new List<Function>();

	public void Add(Function function)
	{
		var count = function.Parameters.Count;
		var conflicts = Overloads.Where(i => i.Parameters.Count == count);

		foreach (var conflict in conflicts)
		{
			var pass = false;

			for (var i = 0; i < count; i++)
			{
				var x = function.Parameters[i].Type;
				var y = conflict.Parameters[i].Type;

				if (x == null || y == null || x == y)
				{
					continue;
				}

				pass = true;
				break;
			}

			if (!pass)
			{
				throw new InvalidOperationException("Function overload can be confused with another");
			}
		}

		Overloads.Add(function);
	}

	public override bool Equals(object? other)
	{
		return other is FunctionList list &&
			   EqualityComparer<List<Function>>.Default.Equals(Overloads, list.Overloads);
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(Overloads);
	}

	private static int GetCastCount(Function candidate, List<Type> parameters)
	{
		var casts = 0;

		for (var i = 0; i < parameters.Count; i++)
		{
			if (candidate.Parameters[i].Type == null || candidate.Parameters[i].Type!.Equals(parameters[i]))
			{
				continue;
			}

			casts++;
		}

		return casts;
	}

	public FunctionImplementation? GetImplementation(List<Type> parameters, Type[] template_arguments)
	{
		if (template_arguments.Any())
		{
			var candidates = Overloads.FindAll(i => i is TemplateFunction function && function.TemplateArgumentNames.Count == template_arguments.Length && function.Passes(parameters, template_arguments)).Cast<TemplateFunction>().ToList();

			if (candidates.Count <= 1)
			{
				return candidates.FirstOrDefault()?.Get(parameters, template_arguments);
			}

			return candidates.OrderBy(i => GetCastCount(i, parameters)).First().Get(parameters, template_arguments);
		}
		else
		{
			var candidates = Overloads.FindAll(o => o.Passes(parameters));

			if (candidates.Count <= 1)
			{
				return candidates.FirstOrDefault()?.Get(parameters);
			}

			return candidates.OrderBy(i => GetCastCount(i, parameters)).First().Get(parameters);
		}
	}

	public FunctionImplementation? GetImplementation(List<Type> parameters)
	{
		return GetImplementation(parameters, Array.Empty<Type>());
	}
}
