<Query Kind="Statements">
  <Connection>
    <ID>337b631f-296e-4a3b-aa36-09f23a9a9a38</ID>
    <Server>.\SQLEXPRESS</Server>
    <Persist>true</Persist>
    <Database>sz04a_mc_all</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <Namespace>System</Namespace>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var message=LA_update1s.Select(e=>e.Ip_version_MsgType).Distinct();
Dictionary<string, int> myDic = new Dictionary<string, int>();
foreach(string m in message)
myDic.Add(m,0);
myDic.Dump();

List<string> startmessage=new List<string>();
startmessage.Add("DTAP MM.Location Updating Request");
startmessage.Add("DTAP MM.CM Service Request");
startmessage.Add("DTAP RR.Paging Response");
startmessage.Add("BSSMAP.Handover Request");


foreach(var start in startmessage){
Dictionary<string, int> newDic = new Dictionary<string, int>();
foreach (KeyValuePair<string, int> pair in myDic)
	newDic.Add(pair.Key, 0);

var a=from p in LA_update1s
	  where p.Ip_version_MsgType==start
	  select p.Opcdpcsccp;

foreach(var b in a)
{
    foreach (KeyValuePair<string, int> kvp in myDic)
	{
      var c=LA_update1s.Where(e=>e.Opcdpcsccp==b).Where(e=>e.Ip_version_MsgType==kvp.Key).FirstOrDefault();
      if(c !=null)
	      newDic[kvp.Key]=newDic[kvp.Key]+1;
	} 
}


newDic.OrderByDescending(e=>e.Value).Dump();
}