<Query Kind="Statements">
  <Connection>
    <ID>337b631f-296e-4a3b-aa36-09f23a9a9a38</ID>
    <Server>.\SQLEXPRESS</Server>
    <Database>mc_sz04a</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var xml=XElement.Load(@"G:\wiresharkplugin\wsFollow\defineflow.xml");
//xml.Dump();
xml.Descendants ("Message").Count();
foreach(var a in xml.Elements())
{
a.Attribute ("Name").Value.Dump();
foreach(var b in a.Elements())
{
b.Attribute ("Name").Value.Dump();
foreach(var c in b.Elements())
{
c.Attribute ("Name").Value.Dump();
}
}
  }
  
  var data = XElement.Parse (@"
<data>
	<customer id='1' name='Mary' credit='100' />
	<customer id='2' name='John' credit='150' />
	<customer id='3' name='Anne' />
</data>");
	
IEnumerable<string> query =
	from cust in data.Elements()
	where (int?) cust.Attribute ("credit") > 100
	select cust.Attribute ("name").Value;
	
query.Dump();
  