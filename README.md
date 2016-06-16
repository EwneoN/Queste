# Queste  

## Detailed instructions coming soon  
### Basic example:  

```C#
var kvps = new Dictionary<string, DateTime[]>
{
  ["key1"]  = new [] { new DateTime(2016,6,1) },
  ["key2"]  = new [] { new DateTime(2016,6,2) },
  ["key3"]  = new [] { new DateTime(2016,6,3) },
  ["key4"]  = new [] { new DateTime(2016,6,1), new DateTime(2016,6,2) },
};

kvps.Where("?value=2016-06-2");
```