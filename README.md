Detailed instructions coming soon

Basic example:

var kvps = new []
{ 
	new KeyValuePair<string, List<DateTime>>("key1", new List<DateTime> { new DateTime(2016,6,1) }),
	new KeyValuePair<string, List<DateTime>>("key2", new List<DateTime> { new DateTime(2016,6,2) }),
	new KeyValuePair<string, List<DateTime>>("key3", new List<DateTime> { new DateTime(2016,6,3) }),
	new KeyValuePair<string, List<DateTime>>("key4", new List<DateTime> { new DateTime(2016,6,1), new DateTime(2016,6,2) }),
};

var expression = ExpressionBuilder.BuildFunction<KeyValuePair<string, List<DateTime>>>("?value=2016-06-2");

kvps.Where(expression).Dump();