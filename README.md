# RadiusServer library for .Net.   
Includes Core functionality for parsing and assembling packets, a server and client
Pluggable packet handlers for different remote IPs. 
Conditionally compliant with RFCs  
https://tools.ietf.org/html/rfc2865  
https://tools.ietf.org/html/rfc2866  
https://tools.ietf.org/html/rfc5997  
  

  
[![Build status](https://ci.appveyor.com/api/projects/status/dbc6ua1ypa9eas3p?svg=true)](https://ci.appveyor.com/project/vforteli/radiusserver)

# RadiusServer usage  
See https://github.com/vforteli/RadiusServerService/tree/Base for an example implementation  

# RadiusClient usage    
```
var dictionary = new RadiusDictionary(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\radius.dictionary");
var client = new RadiusClient(new IPEndPoint(IPAddress.Any, 1824), dictionary);

var packet = new RadiusPacket(PacketCode.AccessRequest, 0, "xyzzy5461");
packet.AddAttribute("User-Name", "nemo");
packet.AddAttribute("User-Password", "arctangent");
packet.AddAttribute("NAS-IP-Address", IPAddress.Parse("192.168.1.16"));
packet.AddAttribute("NAS-Port", 3);

var responsePacket = await client.SendPacketAsync(packet, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1812), TimeSpan.FromSeconds(11));
if (responsePacket.Code == PacketCode.AccessAccept)
{
  // Hooray          
}
```

Multiple requests and responses can be made asynchronously on the same local port as long as the identifier and remote host:port remain unique
