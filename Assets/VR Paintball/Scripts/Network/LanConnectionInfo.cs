public struct LanConnectionInfo
{
	public string ipAddress;
	public int port;
	public string name;

	public LanConnectionInfo(string fromAddress, string data)
	{
		ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1));

		string[] fields = data.Split(':');
		
		string portText = fields[2];
		port = 7777;
		int.TryParse(portText, out port);

		name = fields[3];
	}
}