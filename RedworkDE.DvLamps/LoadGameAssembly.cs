namespace RedworkDE.DvLamps
{
	public class LoadGameAssembly
	{
		[AutoLoad]
		public static void Init()
		{
			_ = new LoadAssembly();
		}
	}
}