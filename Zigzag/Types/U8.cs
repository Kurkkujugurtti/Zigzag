public class U8 : Number
{
	private const int BYTES = 1;

	public U8() : base(Format.UINT8, 8, true, "u8") { }

	public override int GetReferenceSize()
	{
		return BYTES;
	}

	public override int GetContentSize()
	{
		return BYTES;
	}
}