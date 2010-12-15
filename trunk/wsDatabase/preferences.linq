<Query Kind="Statements">
  <IncludePredicateBuilder>true</IncludePredicateBuilder>
</Query>

bool flag=false;
bool haveWrite=false;
Hashtable<string> hs=new Hashtable<string>();
StreamReader objReader = new StreamReader(@"C:\Documents and Settings\Administrator\Application Data\Wireshark\preferences"); 
while (!objReader.EndOfStream)
{
	sLine = objReader.ReadLine();
	if (sLine.IndexOf("*") != -1)
		flag=true;
	if(flag==true)
	 if (sLine.IndexOf("***") != -1)
		flag=false;
	
	if(flag)
	{
	  hs.Add(sLine);
	}
	else
	{
	   //replace
	   if(!havewite)
	   {
	    StreamReader sr = new StreamReader(@"G:\wiresharkplugin\wsDatabase\column.format");
		while (!objReader.EndOfStream)
		{
		   srLine=objReader.ReadLine();
		   hs.Add(srLine);
		}
		havewite=true;
	   }
	}
}
StreamWriter objWriter = new StreamReader(mstipLayerlbFilepath.Text);			
foreach(string s in hs)
	objWriter.WriteLine(s);
objWriter .Flush();
objWriter .Close();