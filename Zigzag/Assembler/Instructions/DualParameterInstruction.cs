public abstract class DualParameterInstruction : Instruction
{
    public Result First { get; private set; }
    public Result Second { get; private set; }

    public DualParameterInstruction(Unit unit, Result first, Result second) : base(unit)
    {
        First = first;
        Second = second;
    }

    public override Result[] GetHandles()
    {
        return new Result[] { Result, First, Second };
    }
}