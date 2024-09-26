public class AddedPart : UndoPart
{
	public BuilderPart bp;

	public int instId;

	public AddedPart(BuilderPart Bp)
	{
		bp = Bp;
		instId = Bp.instId;
	}
}
