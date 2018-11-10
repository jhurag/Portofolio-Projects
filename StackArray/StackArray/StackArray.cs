using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArray
{
    class StackArray
    {
        public bool IsEmpty { get { return count == 0; } }

        public int Count { get { return count; } set { count = value; } }

        private int[] items;

        private int count;

        public StackArray()
        {
            items = new int[5];
            count = 0;
        }


        public void Push(int value)
        {

            if (count == items.Length)
            {

                int[] newArray = new int[items.Length * 2];

                for (int i = 0; i < items.Length; i++)
                    newArray[i] = items[i];

                items = newArray;
            }
            items[count] = value;
            count++;
        }
        public int Pop()
        {
            if (count == 0)
                throw new InvalidOperationException("Stack is empty");

            if (count <= items.Length / 4)
            {
                int[] newArray = new int[items.Length / 2];

                for (int i = 0; i < newArray.Length; i++)
                    newArray[i] = items[i];

                items = newArray;
            }

            count--;
            return items[count];
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            for (int i = count - 1; i >= 0; i--)
            {
                s.AppendFormat(string.Format("{0} ", items[i]));
            }

            return s.ToString();
        }

    }
}
