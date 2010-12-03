public Tuple<int, int> GetDivAndRemainder(int i, int j) 
{ 
    Tuple.Create(i/j, i%j); 
} 
public void CallMethod() 
{ 
    var tuple = GetDivAndRemainder(10,3); 
    Console.WriteLine("{0} and {1}", tuple.item1, tuple.item2); 
} 