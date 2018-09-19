using System;

namespace BarcodeGenerator
{
    class BarcodeGeneratorProgram
    {
        private static readonly object syncLock = new object();

        private static int[] possibleBarCode;
        private static string prevNumber = "";
        static void Main(string[] args)
        {
            string barcode = "";

            possibleBarCode = new int[13];

            GenerateBarcode(possibleBarCode);

            barcode = ArrayToString(possibleBarCode);

            prevNumber = barcode;

            while (!CheckBarcode(possibleBarCode))
            {
                prevNumber = barcode;

                Console.WriteLine("Number {0} is not valid barcode number.", barcode);

                GenerateBarcode(possibleBarCode);
                barcode = ArrayToString(possibleBarCode);

                while (prevNumber == barcode)
                {
                    if (prevNumber != barcode)
                        break;
                    GenerateBarcode(possibleBarCode);
                    barcode = ArrayToString(possibleBarCode);
                }

            }
            Console.WriteLine("Number {0} is valid barcode number. Ending program.", barcode);
        }

        private static string ArrayToString(int[] array)
        {
            string barcode = "";
            for (int i = 0; i < 13; i++)
                barcode = barcode + array[i].ToString();
            return barcode;
        }

        private static bool CheckBarcode(int[] possibleBarCode)
        {
            int sumOfEvens = 0;
            int sumOfOdds = 0;

            int sumOfEvensAndOdds = 0;

            int checkDigit = 0;

            for (int i = 1; i <= possibleBarCode.Length; i++)
            {
                if (i % 2 == 0)
                {
                    sumOfEvens += possibleBarCode[i - 1];
                }
                else
                {
                    if (i != 13)
                        sumOfOdds += possibleBarCode[i - 1];
                    else
                    {
                        sumOfEvens = sumOfEvens * 3;
                        sumOfEvensAndOdds = sumOfEvens + sumOfOdds;

                        checkDigit = sumOfEvensAndOdds % 10;
                        if (checkDigit == 0)
                            break;
                        else
                            checkDigit = 10 - checkDigit;
                    }

                }
            }


            //If control number is different than last digit of barcode, barcode is invalid
            if (checkDigit == possibleBarCode[12])
                return true;

            return false;

        }

        private static void GenerateBarcode(int[] a)
        {
            Random r = new Random();
            int y = 0;
            for (int i = 0; i < a.Length; i++)
            {
                lock (syncLock)
                {
                    y = r.Next(0, 10);
                }
                a[i] = y;
            }
        }

    }
}
