using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pop3hexDecoder
{
  
  ///     
  ///   有关base64编码算法的相关操作   
  ///by   自由奔腾（wgscd）   
  ///     
  public   class   sbase64   
  {   
  public   sbase64()   
  {   
  //   
  //   todo:   在此处添加构造函数逻辑   
  //   
  }   
  //--------------------------------------------------------------------------------   
  ///     
  ///   将字符串使用base64算法加密   
  ///     
  ///   待加密的字符串   
  ///   system.text.encoding   对象，如创建中文编码集对象：system.text.encoding.getencoding(54936)   
  ///   加码后的文本字符串   
  public   static   string   encodingforstring(string   sourcestring,System.Text.Encoding    ens)   
  {
      return Convert.ToBase64String(ens.GetBytes(sourcestring));  
  }   
    
    
    
    
  ///     
  ///   将字符串使用base64算法加密   
  ///     
  ///   待加密的字符串   
  ///   加码后的文本字符串   
  public   static   string   encodingforstring(string   sourcestring)   
  {
      return encodingforstring(sourcestring, System.Text.Encoding.GetEncoding (54936));   
  }   
    
    
    
    
  ///     
  ///   从base64编码的字符串中还原字符串，支持中文   
  ///     
  ///   base64加密后的字符串   
  ///   system.text.encoding   对象，如创建中文编码集对象：system.text.encoding.getencoding(54936)   
  ///   还原后的文本字符串   
  public static string decodingforstring(string base64string, System.Text.Encoding ens)   
  {   
  /**   
  *   ***********************************************************   
  *     
  *   从base64string中取得的字节值为字符的机内码（ansi字符编码）   
  *   一般的，用机内码转换为汉字是公式：   
  *   (char)(第一字节的二进制值*256   第二字节值)   
  *   而在c#中的char或string由于采用了unicode编码，就不能按照上面的公式计算了   
  *   ansi的字节编和unicode编码不兼容   
  *   故利用.net类库提供的编码类实现从ansi编码到unicode代码的转换   
  *     
  *   getencoding   方法依赖于基础平台支持大部分代码页。但是，对于下列情况提供系统支持：默认编码，即在执行此方法的计算机的区域设置中指定的编码；little-endian   unicode   (utf-16le)；big-endian   unicode   (utf-16be)；windows   操作系统   (windows-1252)；utf-7；utf-8；ascii   以及   gb18030（简体中文）。   
  *   
  *指定下表中列出的其中一个名称以获取具有对应代码页的系统支持的编码。   
  *   
  *   代码页   名称     
  *   1200   ＆#8220;utf-16le＆#8221;、＆#8220;utf-16＆#8221;、＆#8220;ucs-2＆#8221;、＆#8220;unicode＆#8221;或＆#8220;iso-10646-ucs-2＆#8221;     
  *   1201   ＆#8220;utf-16be＆#8221;或＆#8220;unicodefffe＆#8221;     
  *   1252   ＆#8220;windows-1252＆#8221;     
  *   65000   ＆#8220;utf-7＆#8221;、＆#8220;csunicode11utf7＆#8221;、＆#8220;unicode-1-1-utf-7＆#8221;、＆#8220;unicode-2-0-utf-7＆#8221;、＆#8220;x-unicode-1-1-utf-7＆#8221;或＆#8220;x-unicode-2-0-utf-7＆#8221;     
  *   65001   ＆#8220;utf-8＆#8221;、＆#8220;unicode-1-1-utf-8＆#8221;、＆#8220;unicode-2-0-utf-8＆#8221;、＆#8220;x-unicode-1-1-utf-8＆#8221;或＆#8220;x-unicode-2-0-utf-8＆#8221;     
  *   20127   ＆#8220;us-ascii＆#8221;、＆#8220;us＆#8221;、＆#8220;ascii＆#8221;、＆#8220;ansi_x3.4-1968＆#8221;、＆#8220;ansi_x3.4-1986＆#8221;、＆#8220;cp367＆#8221;、＆#8220;csascii＆#8221;、＆#8220;ibm367＆#8221;、＆#8220;iso-ir-6＆#8221;、＆#8220;iso646-us＆#8221;或＆#8220;iso_646.irv:1991＆#8221;     
  *   54936   ＆#8220;gb18030＆#8221;     
  *   
  *   某些平台可能不支持特定的代码页。例如，windows   98   的美国版本可能不支持日语   shift-jis   代码页（代码页   932）。这种情况下，getencoding   方法将在执行下面的   c#   代码时引发   notsupportedexception：   
  *   
  *   encoding   enc   =   encoding.getencoding(shift-jis);     
  *   
  *   **************************************************************/   
  //从base64string中得到原始字符   
      return ens.GetString(Convert.FromBase64String(base64string));  
  }   
    
    
    
    
  ///     
  ///   从base64编码的字符串中还原字符串，支持中文   
  ///     
  ///   base64加密后的字符串   
  ///   还原后的文本字符串   
  public   static   string   decodingforstring(string   base64string)   
  {
      return decodingforstring(base64string, System.Text.Encoding.GetEncoding(54936));  
  }   
    
    
    
    
  //--------------------------------------------------------------------------------------   
    
    
    
  ///     
  ///   对任意类型的文件进行base64加码   
  ///     
  ///   文件的路径和文件名   
  ///   对文件进行base64编码后的字符串   
  public   static   string   encodingforfile(string   filename)   
  {
      System.IO.FileStream fs = System.IO.File.OpenRead(filename);
      System.IO.BinaryReader br = new System.IO.BinaryReader(fs);   
    
    
    
  /*system.byte[]   b=new   system.byte[fs.length];   
  fs.read(b,0,convert.toint32(fs.length));*/




      string base64string = Convert.ToBase64String(br.ReadBytes((int)fs.Length));




      br.Close();
      fs.Close();  
  return   base64string;   
  }   
    
    
    
  ///     
  ///   把经过base64编码的字符串保存为文件   
  ///     
  ///   经base64加码后的字符串   
  ///   保存文件的路径和文件名   
  ///   保存文件是否成功   
  public   static   bool   savedecodingtofile(string   base64string,string   filename)   
  {
      System.IO.FileStream fs = new System.IO.FileStream(filename, System.IO.FileMode.Create);
      System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
      bw.Write(Convert.FromBase64String(base64string));   
  bw.Close();   
  fs.Close();   
  return   true;   
  }   
    
    
    
    
  //-------------------------------------------------------------------------------   
    
    
    
  ///     
  ///   从网络地址一取得文件并转化为base64编码   
  ///     
  ///   文件的url地址,一个绝对的url地址   
  ///   system.net.webclient   对象   
  ///     
  public static string encodingfilefromurl(string url, System.Net.WebClient objwebclient)   
  {
      return Convert.ToBase64String(objwebclient.DownloadData(url));
  }   
        //return   convert.tobase64string(objwebclient.downloaddata(url));   
    
    
    
  ///     
  ///   从网络地址一取得文件并转化为base64编码   
  ///     
  ///   文件的url地址,一个绝对的url地址   
  ///   将文件转化后的base64字符串   
  public   static   string   encodingfilefromurl(string   url)   
  {   
  //system.net.webclient   mywebclient   =   new   system.net.webclient();   
      return encodingfilefromurl(url, new System.Net.WebClient());   
  }   
  }   
  }
