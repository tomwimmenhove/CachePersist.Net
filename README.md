# CachePersist.Net
CachePersist.Net is a .NET library that allows you to persistently store data. It contains persistent dictionaries and a caching system.
The persistent dictionaries are IDictionary implementations for persistent storare of key,value pairs.
With the disk-caching system you can use (string) keys load and store any serializable object to and from disk. It lets you choose different serialization methods when storing values and automatically picks the correct deserializer when loading back from disk.

The [Example/Program.cs](https://github.com/tomwimmenhove/CachePersist.Net/blob/main/Example/Program.cs) file contains some simple examples on how to use the library.

## Installation

Through the package manager:
```
Install-Package CachePersist.Net
```

Or using .NET CLI:
```
dotnet add package CachePersist.Net
```

You can find the package on Nuget using this link: https://www.nuget.org/packages/CachePersist.Net/
