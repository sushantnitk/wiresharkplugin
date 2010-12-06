using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace pop3hexDecoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //StreamWriter writer = new StreamWriter(appPath + "\\urlLog\\urlCode" + textboxDate + ".txt");
            //StreamReader reader = new StreamReader("E:\\aa.eth", System.Text.Encoding.BigEndianUnicode);
            ////string hexValues = "48 65 6C 6C 6F 20 57 6F 72 6C 64 21";
            //richTextBox2.Text = reader.ReadToEnd();
            string hexValues = richTextBox2.Text;
            string[] hexValuesSplit = hexValues.Trim().Split();
            foreach (String hex in hexValuesSplit)
            {
                if (hex.Length > 1)
                {
                    // Convert the number expressed in base-16 to an integer.
                    int value = Convert.ToInt32(hex, 16);
                    // Get the character corresponding to the integral value.
                    string stringValue = Char.ConvertFromUtf32(value);
                    //string stringValue = Char.ConvertFromUtf32
                    char charValue = (char)value;
                    //Console.WriteLine("hexadecimal value = {0}, int value = {1}, char value = {2} or {3}",
                    //                    hex, value, stringValue, charValue);
                    richTextBox1.Text += charValue;
                }
            }
            /* Output:
                hexadecimal value = 48, int value = 72, char value = H or H
                hexadecimal value = 65, int value = 101, char value = e or e
                hexadecimal value = 6C, int value = 108, char value = l or l
                hexadecimal value = 6C, int value = 108, char value = l or l
                hexadecimal value = 6F, int value = 111, char value = o or o
                hexadecimal value = 20, int value = 32, char value =   or
                hexadecimal value = 57, int value = 87, char value = W or W
                hexadecimal value = 6F, int value = 111, char value = o or o
                hexadecimal value = 72, int value = 114, char value = r or r
                hexadecimal value = 6C, int value = 108, char value = l or l
                hexadecimal value = 64, int value = 100, char value = d or d
                hexadecimal value = 21, int value = 33, char value = ! or !
            */


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //FileStream fs = new FileStream(@"E:\\aa.eth", FileMode.Open, FileAccess.Read);
            //BinaryReader br = new BinaryReader(fs, Encoding.UTF8);
            //byte[] buffer = new byte[1024];
            //MemoryStream ms = new MemoryStream();
            //Encoding encoding = Encoding.UTF8;
            //int i=0;
            //while (i < fs.Length )
            //{

            //    int cLen = ms.Read(buffer, i, buffer.Length);
            //    string msg = encoding.GetString(br.ReadBytes(cLen));
            //    msg.Replace("\n", Environment.NewLine);
            //    richTextBox1.Text += msg;
            //    i += buffer.Length;
            //    //richTextBox1.Text += System.Text.Encoding.ASCII.GetBytes(buffer);

            //    //// 为文件打开一个二进制编写器。
            //    //FileInfo f = new FileInfo("E:\\aa.eth");
            //    //// 以原始字节形式读取数据。
            //    //BinaryReader br = new BinaryReader(f.OpenRead());
            //    //byte[] output = br.ReadBytes(255);
            //    //String outputString = Encoding.UTF8.GetString(output);
            //    //richTextBox1.Text = outputString;
            //    //br.Close();

            //}

            string str;
            //FileStream fs = new FileStream(@"e:\\1.txt", FileMode.Open, FileAccess.Read);
            FileStream fs = new FileStream(@"E:\\aa.eth", FileMode.Open, FileAccess.Read);  
            BinaryReader br=new BinaryReader(fs);
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                str = br.ReadByte();
                richTextBox2.Text += str;
            }  
            br.Close(); 
            fs.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = sbase64.decodingforstring(richTextBox2.Text,System.Text .Encoding .UTF8);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = DecodeBase64(richTextBox2.Text, "ascii");

        }

        /// <summary> 
    /// 将字符串使用base64算法加密 
    /// </summary> 
    /// <param name="code_type">编码类型（编码名称） 
    /// * 代码页 名称 
    /// * 1200 "UTF-16LE"、"utf-16"、"ucs-2"、"unicode"或"ISO-10646-UCS-2" 
    /// * 1201 "UTF-16BE"或"unicodeFFFE" 
    /// * 1252 "windows-1252" 
    /// * 65000 "utf-7"、"csUnicode11UTF7"、"unicode-1-1-utf-7"、"unicode-2-0-utf-7"、"x-unicode-1-1-utf-7"或"x-unicode-2-0-utf-7" 
    /// * 65001 "utf-8"、"unicode-1-1-utf-8"、"unicode-2-0-utf-8"、"x-unicode-1-1-utf-8"或"x-unicode-2-0-utf-8" 
    /// * 20127 "us-ascii"、"us"、"ascii"、"ANSI_X3.4-1968"、"ANSI_X3.4-1986"、"cp367"、"csASCII"、"IBM367"、"iso-ir-6"、"ISO646-US"或"ISO_646.irv:1991" 
    /// * 54936 "GB18030"    
    /// </param> 
    /// <param name="code">待加密的字符串</param> 
    /// <returns>加密后的字符串</returns> 
    public string EncodeBase64(string code_type, string code) 
    { 
        string encode = ""; 
        byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);  //将一组字符编码为一个字节序列. 
        try 
        { 
            encode = Convert.ToBase64String(bytes);  //将8位无符号整数数组的子集转换为其等效的,以64为基的数字编码的字符串形式. 
        } 
        catch 
        { 
            encode = code; 
        } 
        return encode; 
    } 

/// <summary> 
    /// 将字符串使用base64算法解密 
    /// </summary> 
    /// <param name="code_type">编码类型</param> 
    /// <param name="code">已用base64算法加密的字符串</param> 
    /// <returns>解密后的字符串</returns> 
    public string DecodeBase64(string code_type, string code) 
    { 
        string decode = ""; 
        byte[] bytes = Convert.FromBase64String(code);  //将2进制编码转换为8位无符号整数数组. 
        try 
        { 
            decode = Encoding.GetEncoding(code_type).GetString(bytes);  //将指定字节数组中的一个字节序列解码为一个字符串。 
        } 
        catch 
        { 
            decode = code; 
        } 
        return decode; 
    }

//-

//资料引用:http://www.knowsky.com/534207.html
    }
}
