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
            FollowStream fs = new FollowStream(common.mydb.LA_update);
        }
    }
}
 