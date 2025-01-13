namespace Navigation
{
	public class AccessManager
	{
		public bool HasAccess { get; private set; } = false;

		public void Accept()
		{
			HasAccess = true;
		}

		public void Decline()
		{
			HasAccess = false;
		}
	}
}
