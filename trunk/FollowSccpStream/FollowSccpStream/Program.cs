﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Data.Linq;

namespace FollowSccpStream
{
    class Program
    {
        static void Main(string[] args)
        {
            FollowStream fs = new FollowStream();
            var msgmax = common.mydb.LA_update.Select(e => e.FileNum).Count();
            Console.WriteLine(msgmax);
            var msg = common.mydb.LA_update;
            for (int i = 0; i < msgmax; i++)
                fs.FollowSccpStream(msg.Where(e => e.PacketNum == i).FirstOrDefault());

        }
    }
}
 