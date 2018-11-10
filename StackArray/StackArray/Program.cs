using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArray
{
    class Program
    {
        static void Main(string[] args)
        {
            //Stack using Arrays
            StackArray stackArray = new StackArray();
            stackArray.Push(3);
            stackArray.Push(4);
            stackArray.Push(5);

            Console.WriteLine(stackArray.ToString()); // 5 4 3

            stackArray.Pop();

            Console.WriteLine(stackArray.ToString()); // 4 3

            //Stack using Lists

            StackList stackList = new StackList();
            stackList.Push(1);
            stackList.Push(2);
            stackList.Push(3);

            Console.WriteLine(stackList.ToString()); //  1 -> 2 -> 3 -> *

            stackList.Pop();

            Console.WriteLine(stackList.ToString()); //  2 -> 3 -> *
        }

    }
}
