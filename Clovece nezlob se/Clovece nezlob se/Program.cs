using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace coviknezlob
{
    class Program
    {
        public static Random r = new Random();

        public static int gridSize = 0;
        private static int halfOfGridSize;


        private static int[] xCoord;
        private static int[] yCoord;

        private static int playerA_X;
        private static int playerA_Y;
        private static int playerB_X;
        private static int playerB_Y;


        private static int currentPosA_X;
        private static int currentPosA_Y;
        private static int currentPosB_X;
        private static int currentPosB_Y;
        private static int playerAStartPosition_X;
        private static int playerAStartPosition_Y;

        private static int playerBStartPosition_Y;

        private static int playerBStartPosition_X;

        private static bool playerATurn = true;

        private static bool playerBStarted = false;

        private static bool playerAStarted = false;


        private static int[,] arrayOfPawns_A;
        private static int[,] arrayOfPawns_B;

        private static int pawnCount_A = 0;
        private static int pawnCount_B = 0;

        private static int maxPawns = 0;

        static char[,] mainGrid;


        private static bool playerAWon = false;

        static void Main(string[] args)
        {

            Console.WriteLine("Input grid size : ");

            gridSize = int.Parse(Console.ReadLine()) + 1;
            halfOfGridSize = (gridSize) / 2;
            mainGrid = new char[gridSize, gridSize];

            
            arrayOfPawns_A = new int[(gridSize - 4) / 2, 2];
            arrayOfPawns_B = new int[(gridSize - 4) / 2, 2];

            maxPawns = (gridSize - 4) / 2;

            // Fill grid with numbers
            FillGridWithNumbers(mainGrid);

            // Fill grid with symbols
            FillGridWithSymbols(mainGrid);


            int numOfAsterixes = CountAsterix(mainGrid);

            xCoord = new int[numOfAsterixes];
            yCoord = new int[numOfAsterixes];
            int[,] coordinateArray = GetMovablePositions(mainGrid);

            for (int i = 0; i < numOfAsterixes; i++)
            {
                xCoord[i] = coordinateArray[i, 0];
                yCoord[i] = coordinateArray[i, 1];
            }

            currentPosA_X = 0;
            currentPosA_Y = 0;

            playerAStartPosition_X = currentPosA_X;
            playerAStartPosition_Y = currentPosA_Y;


            currentPosB_X = numOfAsterixes / 2;
            currentPosB_Y = numOfAsterixes / 2;
            playerBStartPosition_X = currentPosB_X;
            playerBStartPosition_Y = currentPosB_Y;

            while (!EndGame())
             {
                 DoTurns();
                 if (EndGame() && playerAWon)
                 {
                     Console.WriteLine("Game is over. Player A won! Congrats.");
                 }
                 else if (EndGame() && !playerAWon)
                 {
                     Console.WriteLine("Game is over. Player B won! Congrats.");
                 }
             }


            for(int i = 0; i < numOfAsterixes; i++)
            {
                Console.WriteLine(xCoord[i] + " "+ yCoord[i]); 
            }
        }

        private static bool EndGame()
        {

            int aCount = CountAsterix(mainGrid);

            //Player A is on his last position
            if(currentPosA_X >= aCount - 1)
            {
                return true;
            }
            else if(currentPosB_X == (aCount / 2) - 1)
            {
                return true;
            }

            return false;
        }

        
        private static void DoTurns()
        {

            //Opening turn player A
            if (!playerAStarted && playerATurn)
            {

                int triesA = 2;
                int diceRollA = r.Next(1, 7);

                if(diceRollA == 6)
                {
                    MovePlayerA(xCoord[currentPosA_X], yCoord[currentPosA_Y]);

                    Console.WriteLine("Player A rolled 6, putting his pawn on table. Player B has turn now.");
                    playerATurn = false;
                    playerAStarted = true;

                }
                else
                {
                    while (diceRollA != 6)
                    {
                        diceRollA = r.Next(1, 7);
                        Console.WriteLine("Player A rolled {0} , tries left {1}", diceRollA, triesA);

                        if (diceRollA == 6)
                        {
                            MovePlayerA(xCoord[currentPosA_X], yCoord[currentPosA_Y]);

                            Console.WriteLine("Player A rolled 6, putting his pawn on table. Player B has turn now.");
                            playerATurn = false;
                            playerAStarted = true;
                            break;
                        }

                        triesA--;

                        if (triesA == 0)
                        {
                            Console.WriteLine("Player A run out of tries, player B is now on turn.");
                            playerATurn = false;
                            break;
                        }
                    }
                }

            }
            // After opening
            else if (playerAStarted && playerATurn)
            {
                int diceRollA = r.Next(1, 7);
                Console.WriteLine("Player A rolled {0}", diceRollA);


                int lastPosX = currentPosA_X;
                int lastPosY = currentPosA_Y;



                //Old position becomes *

                currentPosA_X += diceRollA;
                currentPosA_Y += diceRollA;
                if (currentPosA_X >= 31)
                {
                    currentPosA_X = 31;
                    currentPosA_Y = 31;

                }

                if(lastPosX >= 31)
                {
                    lastPosX = 31;
                    lastPosY = 31;

                }
                mainGrid[yCoord[lastPosY],xCoord[lastPosX]] = '*';
                MovePlayerA(xCoord[currentPosA_Y], yCoord[currentPosA_X]);

                playerATurn = false;
            }

            //Opening turn player B
            if (!playerBStarted && !playerATurn)
            {

                int diceRollB = r.Next(1, 7);
                int triesB = 2;


                if(diceRollB == 6)
                {
                    MovePlayerB(xCoord[currentPosB_X], yCoord[currentPosB_Y]);
                    Console.WriteLine("Player B rolled 6, putting his pawn on table. Player A has turn now.");
                    playerATurn = true;
                    playerBStarted = true;


                    if (pawnCount_B < maxPawns)
                    {
                        arrayOfPawns_B[pawnCount_B, 0] = xCoord[playerBStartPosition_X];
                        arrayOfPawns_B[pawnCount_B, 1] = yCoord[playerBStartPosition_Y];

                        pawnCount_A++;
                    }
                }
                else
                {
                    while (diceRollB != 6)
                    {
                        diceRollB = r.Next(1, 7);
                        Console.WriteLine("Player B rolled {0} , tries left {1}", diceRollB, triesB);

                        if (diceRollB == 6)
                        {
                            MovePlayerB(xCoord[currentPosB_X], yCoord[currentPosB_Y]);
                            Console.WriteLine("Player B rolled 6, putting his pawn on table. Player A has turn now.");
                            playerATurn = true;
                            playerBStarted = true;


                            if (pawnCount_B < maxPawns)
                            {
                                arrayOfPawns_B[pawnCount_B, 0] = xCoord[playerBStartPosition_X];
                                arrayOfPawns_B[pawnCount_B, 1] = yCoord[playerBStartPosition_Y];

                                pawnCount_A++;
                            }
                            break;
                        }

                        triesB--;

                        if (triesB == 0)
                        {
                            Console.WriteLine("Player B run out of tries, player A is now on turn.");
                            playerATurn = true;
                            break;
                        }
                    }

                }

            }
            //After opening
            else if(playerBStarted && !playerATurn)
            {
                int diceRollB = r.Next(1, 7);
                Console.WriteLine("Player B rolled {0}", diceRollB);

                //Old position becomes *
                mainGrid[yCoord[currentPosB_Y],xCoord[currentPosB_X]] = '*';

                currentPosB_X += diceRollB;
                currentPosB_Y += diceRollB;

                if(currentPosB_X >= 32)
                {
                    currentPosB_X = 0;
                    currentPosB_Y = 0;

                }
                MovePlayerB(xCoord[currentPosB_Y], yCoord[currentPosB_X]);

                playerATurn = true;

            }

            PrintGrid(mainGrid);

        }

        private static void MovePlayerA(int x, int y)
        {
            mainGrid[y, x] = 'A';
        }

        private static void MovePlayerB(int x, int y)
        {
            mainGrid[y, x] = 'B';
        }

        private static int[,] GetMovablePositions(char[,] grid)
        {
            int numofAsterix = CountAsterix(grid);


            int[] xArray = new int[numofAsterix];
            int[] yArray = new int[numofAsterix];

            int[,] main = new int[gridSize, gridSize];

            int x = halfOfGridSize + 1;
            int y = 1;

            xArray[0] = x;
            yArray[0] = y;


            for (int counter = 1; counter < numofAsterix; counter++)
            {


                if (y == gridSize - 2 && counter <= numofAsterix / 2)
                {
                    xArray[counter] = x;
                    yArray[counter] = y + 1;

                    counter++;
                    xArray[counter] = x - 1;
                    yArray[counter] = y + 1;

                    counter++;
                    xArray[counter] = x - 2;
                    yArray[counter] = y + 1;

                    x = halfOfGridSize - 1;
                    y = gridSize - 1;

                }
                else if (x == gridSize - 2 && counter < numofAsterix / 4)
                {
                    xArray[counter] = x + 1;
                    yArray[counter] = y;

                    counter++;
                    xArray[counter] = x + 1;
                    yArray[counter] = y + 1;

                    counter++;
                    xArray[counter] = x + 1;
                    yArray[counter] = y + 2;

                    x = gridSize - 1;
                    y = y + 2;
                }
                else
                {

                    if (counter < numofAsterix / 2 && grid[x, y + 1] == '*' && x > halfOfGridSize)
                    {
                        xArray[counter] = x;
                        y++;
                        yArray[counter] = y;
                    }

                    else if (x != gridSize - 1 && y != halfOfGridSize + 1 && counter < (numofAsterix / 2) - 2)
                    {
                        if (x == gridSize - 1 && grid[x, y] == '*' && x == 13)
                        {
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }
                        else if (grid[x + 1, y] == '*' && x > halfOfGridSize && x < gridSize - 1)
                        {
                            x++;
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }


                    }
                    else
                    {
                        if (grid[x - 1, y] == '*' && x > halfOfGridSize && x > 1 && y >= halfOfGridSize)
                        {
                            x--;
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }
                        else if (grid[x, y - 1] == '*' && x < halfOfGridSize && x > 0)
                        {
                            y--;
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }
                        else if (grid[x - 1, y] == '*' && x < halfOfGridSize && x > 0 && y > halfOfGridSize)
                        {
                            x--;
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }
                        else if (grid[x + 1, y] == '*' && x < halfOfGridSize && x > 0)
                        {
                            x++;
                            xArray[counter] = x;
                            yArray[counter] = y;
                        }
                    }
                }

            }

            var result = new int[xArray.Length, 2];

            for (int i = 0; i < xArray.Length; i++)
            {
                result[i, 0] = xArray[i];
                result[i, 1] = yArray[i];
            }

            return result;

        }

        private static int CountAsterix(char[,] grid)
        {
            int counter = 0;

            foreach (char i in grid)
            {
                if (i == '*')
                    counter++;
                else if (i == 'A' || i == 'B')
                    counter++;
            }
            return counter;
        }

        private static void FillGridWithSymbols(char[,] grid)
        {
            for (int i = 1; i < gridSize; i++)
            {

                for (int j = 1; j < gridSize; j++)
                {
                    //Center X
                    if (j == halfOfGridSize && i == halfOfGridSize)
                    {
                        grid[i, j] = 'X';

                    }
                    // Outer lines I
                    else if ((i == halfOfGridSize && j == 1) || (i == halfOfGridSize && j == gridSize - 1))
                    {
                        grid[i, j] = '*';
                        grid[i - 1, j] = '*';
                        grid[i + 1, j] = '*';
                    }
                    // Outer lines J
                    else if ((j == halfOfGridSize && i == 1) || (j == halfOfGridSize && i == gridSize - 1))
                    {
                        grid[i, j] = '*';
                        grid[i, j - 1] = '*';
                        grid[i, j + 1] = '*';
                    }
                    //Main X skeleton
                    else if (i == halfOfGridSize)
                    {
                        grid[i, j] = 'D';
                        grid[i + 1, j] = '*';
                        grid[i - 1, j] = '*';
                    }
                    //Main Y skeleton
                    else if (j == halfOfGridSize)
                    {
                        grid[i, j] = 'D';
                        grid[i, j + 1] = '*';
                        grid[i, j - 1] = '*';

                    }

                }
            }
        }

        private static void FillGridWithNumbers(char[,] grid)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (j < gridSize - 1)
                {
                    int num = j;
                    //Ovo verovatno može u pythonu bez provere a ovde mora ovako ybg
                    if (num >= 10)
                        num = j % 10; // pošto char prima samo jedan karater stavljam poslednji broj


                    grid[0, j + 1] = char.Parse(num.ToString());
                    grid[j + 1, 0] = char.Parse(num.ToString());
                }
                else
                {
                    int num = (j - 1) % 10;

                    grid[j, 0] = char.Parse((num).ToString());
                    grid[0, j] = char.Parse((num).ToString());
                }

            }
        }



        //Prints main grid array
        private static void PrintGrid(char[,] grid)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Console.Write(string.Format("{0} ", grid[i, j]));
                }
                Console.WriteLine();
            }
        }
    }
}