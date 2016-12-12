using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSharpCookbookUnit1
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSort();
            Console.ReadKey();
        }

        public static void TestSort()
        {
            List<Square> listOfSquares = new List<Square>(){
                                            new Square(1,3),
                                            new Square(4,3),
                                            new Square(2,1),
                                            new Square(6,1)};
            // 测试List<String>
            Console.WriteLine("List<String>");
            Console.WriteLine("Original list");
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }
            Console.WriteLine();
            IComparer<Square> heightCompare = new CompareHeight();
            listOfSquares.Sort(heightCompare);
            Console.WriteLine("Sorted list using IComparer<Square>=heightCompare");
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }
            Console.WriteLine();
            Console.WriteLine("Sorted list using IComparable<Square>");
            listOfSquares.Sort();
            foreach (Square square in listOfSquares)
            {
                Console.WriteLine(square.ToString());
            }
            // 测试SortedList
            var sortedListOfSquares = new SortedList<int, Square>(){
                                            { 1, new Square(1,3)},
                                            { 2, new Square(3,3)},
                                            { 0, new Square(2,1)},
                                            { 3, new Square(6,1)}};
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("SortedList<Square>");
            foreach (KeyValuePair<int, Square> kvp in sortedListOfSquares)
            {
                Console.WriteLine(kvp.Key + ":" + kvp.Value);
            }
        }
    }
}
