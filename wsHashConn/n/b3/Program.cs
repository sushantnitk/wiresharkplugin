using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace b3
{
    class Program
    {
        static void Main(string[] args)
        {
            var rows = new string[][] 
                    { 
                        new string[] {"John", "Apple", "Orange", "Banana"}, 
                        new string[] {"John", "Apple", "Orange", "Banana"}, 
                        new string[] {"John", "Apple", "Lemon", "Banana"}, 
                        new string[] {"John", "Apple", "Orange", "Grape"}, 
                        new string[] {"Sam", "Apple", "Orange", "Banana"}, 
                        new string[] {"Sam", "Apple", "Orange", "Banana"}, 
                    };

            var results = (from f in rows
                           where f.Contains("Sam")
                           select f).Distinct(new ArrayComparer());

            //可以完成近似相同
            //??????


            //下述不行，需要把对象放到Hash表中，相同对象，在对象中实现数组表现？
            //预计可以完成A接口消息的串联，Gb接口的消息串联？

            //Abis接口消息串联，预计需要回调，只解决 channel act即可 ？

            //A接口串联，即比较3个元素相同即可，opc,dpc,sccp？

            //通过M-trix，写串联算法？

            //？？？

            //狄斯卡拉算法，洪水泛滥算法确实还行，是否考虑之？ 


            HashSet<string[]> FlowKey = new HashSet<string[]>();
            foreach (var row in rows)
            {
                if (!FlowKey.Contains(row))
                    FlowKey.Add(row);

            }

            Console.WriteLine(FlowKey.Count());
            Console.ReadKey();
        }
    }


    public class ArrayComparer : IEqualityComparer<string[]>
    {

        public bool Equals(string[] x, string[] y)
        {
            if (x.Length != y.Length)
                return false;

            var left = x.OrderBy(s => s).ToArray();
            var right = y.OrderBy(s => s).ToArray();

            for (int index = 0; index < x.Length; index++)
            {
                if (left[index] == right[index])
                {
                    continue;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(string[] obj)
        {
            int hash = 23;
            foreach (var element in obj.OrderBy(s => s))
            {
                hash = hash * 37 + element.GetHashCode();
            }

            return hash;
        }
    }


}
