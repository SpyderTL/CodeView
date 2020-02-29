namespace CodeView
{
	public abstract class Change
	{
		public abstract void Apply();
		public abstract void Undo();
	}
}