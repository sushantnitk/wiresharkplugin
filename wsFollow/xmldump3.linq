<Query Kind="Statements">
  <Connection>
    <ID>337b631f-296e-4a3b-aa36-09f23a9a9a38</ID>
    <Server>.\SQLEXPRESS</Server>
    <Persist>true</Persist>
    <Database>mc_sz04a</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Reference>&lt;RuntimeDirectory&gt;System.XML.dll</Reference>
  <Namespace>System.Xml</Namespace>
  <Namespace>System.Xml.Xsl</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var xmlTree = XDocument.Load(@"G:\wiresharkplugin\wsFollow\mFlow.xml");
xmlTree.Dump();
XDocument newTree = new XDocument();
"------".Dump();
using (XmlWriter writer = newTree.CreateWriter()) {
	XslCompiledTransform xslt = new XslCompiledTransform();
	xslt.Load(@"G:\wiresharkplugin\wsFollow\SQLgenerationtemplates.xsl");
	xslt.Transform(xmlTree.CreateReader(), writer);}
Console.WriteLine(newTree);
