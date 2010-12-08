<Query Kind="Statements">
  <Connection>
    <ID>337b631f-296e-4a3b-aa36-09f23a9a9a38</ID>
    <Server>.\SQLEXPRESS</Server>
    <Persist>true</Persist>
    <Database>mc_sz04a</Database>
    <ShowServer>true</ShowServer>
  </Connection>
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

var paging=from p in WPagings
           group p by p.PacketTime.Value.Date.ToString()+" "+p.PacketTime.Value.Hour.ToString() into tt
		   select new {
		   tt.Key,	   
		   寻呼成功次数=tt.Sum(e=>e.Paging_Response),
		   BSC寻呼成功率=(tt.Sum(e=>e.Paging_Response)+0.0)/tt.Sum(e=>e.Paging_Request),
           //BSC寻呼拥塞次数
           BSC一次寻呼次数=tt.Sum(e=>e.Paging_Request),
           BSC二次寻呼次数=tt.Sum(e=>e.Paging_Repeated),
           一次寻呼成功=tt.Where(e=>e.Paging_Repeated==null).Sum(e=>e.Paging_Response),
           二次寻呼成功=tt.Where(e=>e.Paging_Repeated!=null).Sum(e=>e.Paging_Response)
		   };

paging.Dump();
