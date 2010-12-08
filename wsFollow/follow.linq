<Query Kind="Statements">
  <Connection>
    <ID>337b631f-296e-4a3b-aa36-09f23a9a9a38</ID>
    <Server>.\SQLEXPRESS</Server>
    <Persist>true</Persist>
    <Database>sz04a_mc_all</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

//LA_update1s.Select(e=>e.Ip_version_MsgType).Distinct().Dump();


var a=from p in LA_update1s
      where p.Ip_version_MsgType=="DTAP MM.Location Updating Request"
	  select p.Opcdpcsccp;

var b=from p in LA_update1s
      where a.Contains(p.Opcdpcsccp)
	  select p;
	  
	  b.Dump();