using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace b2
{
//public bool Equals(Foo x, Foo y)

//{

//       if (x.a == y.b && x.b == y.a || x.a == y.a && x.b == y.b)

//              return true;

//       else

//              return false;

//}

 

//At last, we reached this solution. 

 

    class Foo

    {

        public int a;

        public int b;

        public Foo(int a, int b)

        {

            this.a = a;

            this.b = b;

        }

    }

 

    class FooComparer : IEqualityComparer<Foo>

    {

        public bool Equals(Foo x, Foo y)

        {

            if (x.a == y.b && x.b == y.a ||

                x.a == y.a && x.b == y.b || x.a == y.a)

                return true;

            else

                return false;

        }

 

        public int GetHashCode(Foo obj)

        {
            //return obj.a.GetHashCode() ^ obj.b.GetHashCode();
            //return obj.a.GetHashCode() ^ obj.b.GetHashCode();
            if (Object.ReferenceEquals(obj, null)) return 0; return 1;
        }

    }

 

    class Program

    {

        static void Main(string[] args)

        {

            FooComparer cmp = new FooComparer();

            HashSet<Foo> hashSet = new HashSet<Foo>(cmp);

            Foo foo1 = new Foo(3, 4);

            hashSet.Add(foo1);

 

            Foo foo2 = new Foo(4, 3);

            if (!hashSet.Contains(foo2))

                Console.WriteLine("Foo2(4,3) is not added");

            else

                Console.WriteLine("Foo2(4,3) is added");

            Foo foo3 = new Foo(5, 2);

            //Foo foo3 = new Foo(3, 2); 

            if (!hashSet.Contains(foo3))

                Console.WriteLine("Foo3(5,2) is not added");

            else

                Console.WriteLine("Foo3(5,2) is added"); Console.ReadKey();

        }

    }

 


}
