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

```
var path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "\\radius.dictionary";
var dictionary = new RadiusDictionary(path);
var authenticationServer = new RadiusServer(new IPEndPoint(IPAddress.Any, 1812), dictionary, RadiusServerType.Authentication);                
var packetHandler = new TestPacketHandler();
authenticationServer.AddPacketHandler(IPAddress.Parse("127.0.0.1"), "secret", packetHandler);
authenticationServer.Start();
```  

The packet handler should implement IPacketHandler
