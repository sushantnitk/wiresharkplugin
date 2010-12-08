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
xml.Attribute("Name").Value.Dump();
foreach(var a in xml.Elements().Elements())
{
   a.Attribute ("Name").Value.Dump();
}
  
 