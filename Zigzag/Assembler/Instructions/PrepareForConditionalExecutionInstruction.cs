using System;
using System.Linq;

public class BranchInstruction : Instruction
{
   public Node[] Branches { get; private set; }
   
   public BranchInstruction(Unit unit, Node[] branches) : base(unit)
   {
      Branches = branches;
      Description = "Prepares variables for branching";
   }

   public override void OnBuild() {}

   public override Result? GetDestinationDependency()
   {
      throw new InvalidOperationException("Tried to redirect Branch-Instruction");
   }

   public override InstructionType GetInstructionType()
   {
      return InstructionType.BRANCH;
   }

   public override Result[] GetResultReferences()
   {
      return new Result[] { Result };
   }
}