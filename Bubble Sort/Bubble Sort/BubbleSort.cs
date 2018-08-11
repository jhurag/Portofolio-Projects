using System;

namespace Bubble_Sort
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] array = GenerateArray(10);

            Console.WriteLine("Unsorted array : ");
            foreach (int x in array)
                Console.Write(x + " ");
            Console.WriteLine();

            int[] sortedArray = SortArray(array);
            Console.WriteLine("Sorted array : ");
            foreach (int x in sortedArray)
                Console.Write(x + " ");
            Console.WriteLine();


        }

        private static int[] SortArray(int[] array)
        {
            int leftNum;
            int rightNum;
            int numbersLeftToSort = 0;

            for(int j = array.Length; j > 0; j--)
            {
                for(int i = array.Length - 1; i > numbersLeftToSort; i--)
                {
                    rightNum = array[i];
                    leftNum = array[i - 1];

                    if (rightNum < leftNum)
                    {
                        int temp = array[i - 1];
                        array[i - 1] = array[i];
                        array[i] = temp;
                        numbersLeftToSort++;
                    }
                    else
                    {
                        numbersLeftToSort--;
                        if (numbersLeftToSort < 0)
                            numbersLeftToSort = 0;

                    }
                }

            }
            return array;

           }

        #region RepeatableCode
        // Approach : 
        // Fill array with numbers 1-10.
        // Shuffle them.
        // That way i get random number sequence.
        private static int[] GenerateArray(int lenght)
        {
            int[] array = new int[lenght];

            for (int i = 1; i <= lenght; i++)
                array[i - 1] = i;

            return ShuffleArray(array);
        }
        // Using Fisher-Yates algorithm 
        // Which is faster than just randomly shuffling array
        private static int[] ShuffleArray(int[] array)
        {
            Random random = new Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = random.Next(n--);
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
            return array;
        }
#endregion
    }
}
