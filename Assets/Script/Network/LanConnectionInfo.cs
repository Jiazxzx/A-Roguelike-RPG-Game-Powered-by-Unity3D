public struct LanConnectionInfo
{
    public string ipAddress;
    public int port;

    public LanConnectionInfo(string fromAddress, string data)
    {
        ipAddress = fromAddress.Substring(fromAddress.LastIndexOf(":") + 1, fromAddress.Length - (fromAddress.LastIndexOf(":") + 1));
        string portText = data;
        port = 7777;
        int.TryParse(portText, out port);
    }

}
