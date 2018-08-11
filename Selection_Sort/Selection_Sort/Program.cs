using System;

namespace Selection_Sort
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
            

            int[] sortedArray = SelectionSort(array);
            Console.WriteLine("Sorted array : ");
            foreach (int x in sortedArray)
                Console.Write(x + " ");
            Console.WriteLine();


        }
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
        private static int[] ShuffleArray( int[] array)
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

        private static int[] SelectionSort(int[] array)
        {
            for(int i = 0; i < array.Length ; i++)
            {
                int indexOfMin = i; 
                for (int j = i + 1; j < array.Length; j++)
                {
                    if(array[indexOfMin] > array[j])
                        indexOfMin = j;
                }
               
                //Swaping places of two numbers.
                if(array[i] != array[indexOfMin])
                {
                    int temp = array[i];
                    array[i] = array[indexOfMin];
                    array[indexOfMin] = temp;
                }

            }
            return array;
        }
    }
}