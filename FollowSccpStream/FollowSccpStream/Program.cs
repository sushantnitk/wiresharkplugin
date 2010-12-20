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
            var message = common.mydb.LA_update.Select(e => e.ip_version_MsgType).Distinct().ToList();
            FollowStream flowstream = new FollowStream(message);
            var msgmax = common.mydb.LA_update.Select(e => e.FileNum).Count();
            Console.WriteLine(msgmax);
            var msg = common.mydb.LA_update;
            for (int i = 0; i < msgmax; i++)
                flowstream.FollowSccpStream(msg.Where(e => e.PacketNum == i).FirstOrDefault());
            //保存
            flowstream.flowstat.Save();

        }
    }
}
 