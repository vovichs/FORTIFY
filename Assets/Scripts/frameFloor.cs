public class frameFloor : ConditionalCheck
{
	public PlacePart[] placePartArr;

	public override void RunCheck()
	{
		int num = 0;
		for (int i = 0; i < block.sockets.Length; i++)
		{
			socket socket = block.sockets[i];
			if (socket.male)
			{
				placePartArr[num].conditionBlocked = floorMidCheck(socket);
				num++;
			}
		}
	}

	private bool floorMidCheck(socket sock)
	{
		if (sock.CheckSocketOccupied())
		{
			foreach (socket connection in sock.connections)
			{
				if ((bool)connection && connection.owner.wall && connection.name == "floor mid")
				{
					return true;
				}
			}
		}
		return false;
	}
}
