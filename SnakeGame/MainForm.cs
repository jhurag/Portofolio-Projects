using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class MainForm : Form
    {

        private Point[] positionSnake = new Point[256];

        private static Point foodArray = new Point();

        private Snake snaker = new Snake();

        private static int score = 0;


        private static int snakeLenght = 0;

        private static bool toggle = false;

        private string timeElapsed;


        private MovingDirection movingDirection;

        private Color headColor = Color.Silver;
        private Color tailColor = Color.SeaGreen;

        private enum MovingDirection
        {
            Right,
            Left,
            Up,
            Down
        }

        private static double mseconds;


        private static bool endGame = false;



        private static readonly Random getRandom = new Random();
        private static readonly object syncLock = new object();

        private static double  seconds;
        private static double minute;
        private static bool canSpawnFood = true;

        /// <summary>
        /// Because Random class isn't so random i use this method.
        /// </summary>
        /// 
        private static int GetRandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return getRandom.Next(min, max);
            }
        }




        public MainForm()
        {
            InitializeComponent();

            ShowMenu(false);


            positionSnake[0] = new Point(mainPanel.Width / 2, mainPanel.Height / 2);
            Draw(0);
            mainPanel.Refresh();


            movingDirection = (MovingDirection)getRandom.Next(0, 4); // Da ne bude svaki put isti početni pravac kretanja
            SpawnFood(); //Stvaramo hranu odmah na početku
            
        }
        

        private void GetInput(KeyEventArgs e)
        {

            
            if (e.KeyCode == Keys.W)
            {
                movingDirection = MovingDirection.Up;
            }
           else if (e.KeyCode == Keys.S)
            {
                movingDirection = MovingDirection.Down;
            }
            else if (e.KeyCode == Keys.A)
            {
                movingDirection = MovingDirection.Left;
            }
            else if (e.KeyCode == Keys.D)
            {
                movingDirection = MovingDirection.Right;
            }
            else if(e.KeyCode == Keys.F)
            {
                if (toggle)
                {
                    toggle = !toggle;
                    timerForMoving.Stop();
                }
                else
                {
                    toggle = !toggle;
                    timerForMoving.Start();
                }
            }
            else
            {
                if(endGame)
                {
                    if(e.KeyCode == Keys.Space)
                    {
                        ShowMenu(false);

                        movingDirection = (MovingDirection)GetRandomNumber(0, 4);
                        timerForMoving.Start();
                        score = 0;
                        labelScore.Text = "Score : " + score.ToString();

                        Draw(2);
                        Draw(0);
                        for(int i = 1; i <= snakeLenght; i++)
                        {
                            positionSnake[i] = new Point();
                        }



                        positionSnake = new Point[256];
                        positionSnake[0] = new Point(mainPanel.Width / 2, mainPanel.Height / 2);
                        snakeLenght = 0;
                        endGame = !endGame;
                        SpawnFood();
                        seconds = 0;
                        mseconds = 0;
                        minute = 0;
                        timeElapsed = "Time : 00:00:00";
                        timerTimer.Start();

                    }
                    else if(e.KeyCode == Keys.Escape) 
                    {
                        Application.Exit();
                    }
                }
            }
        }

        private void MakeGameHarder()
        {
            if(score > 5)
            {
                if (timerForMoving.Interval > 100)
                    timerForMoving.Interval -= 5; // Speeding up timer interval which speeds up whole game.
                else
                    timerForMoving.Interval = 100;
            }
        }

        private void SpawnFood()
        {
            foodArray = GenerateLocation();
            CanSpawnFood(foodArray);
            while(!canSpawnFood)
            {
                foodArray = GenerateLocation();
                CanSpawnFood(foodArray);

                if (canSpawnFood)
                    break;
            }
            Draw(1);
            MakeGameHarder();
        }

    
        private Point GenerateLocation()
        {

            Point locationP;
            int x = GetRandomNumber(0, mainPanel.Width); 
            int y = GetRandomNumber(0, mainPanel.Height);

            while (x % 16 != 0) 
            {
                x = GetRandomNumber(0, mainPanel.Width);
                if (x % 16 == 0)
                    break;
            }
            while (y % 16 != 0) 
            {
                y = GetRandomNumber(0, mainPanel.Height);
                if (y % 16 == 0)
                    break;
            }
            locationP = new Point(x, y);
            return locationP;
        }

        private void ShowMenu(bool show)
        {
            if(show)
            {
                panelMenu.Show();
                labelMenu1.Show();
                labelMenu2.Show();
                labelMenu3.Show();
                panelMenu.BringToFront();
            }
            else
            {
                panelMenu.Hide();
                labelMenu1.Hide();
                labelMenu2.Hide();
                labelMenu3.Hide();
            }
        }


        private void SnakeMoving()
        {

            if(!SnakeOutOfBounds())
            {
                mainPanel.Refresh();

                PositionSnake();
            }
            else
            {
                EndGame();
            }
        }

        private void PositionSnake()
        {
            for(int i = snakeLenght; i >= 0; i--)
            {
                if(i == 0)
                {
                    switch(movingDirection) 
                    {
                        case MovingDirection.Right:
                            positionSnake[i] = new Point
                                (positionSnake[i].X + (16),positionSnake[i].Y);
                            break;
                        case MovingDirection.Left:
                            positionSnake[i] = new Point
                                (positionSnake[i].X - (16),positionSnake[i].Y);
                            break;
                        case MovingDirection.Up:
                            positionSnake[i] = new Point
                                (positionSnake[i].X, 
                                positionSnake[i].Y - 16 );
                            break;
                        case MovingDirection.Down:
                            positionSnake[i] = new Point
                                (positionSnake[i].X,
                                positionSnake[i].Y + 16); 
                            break;

                    }
                    Draw(0);
                    for (int j = 1; j <= snakeLenght; j++)
                    {
                        if (positionSnake[i] == positionSnake[j]) 
                            EndGame();
                    } 
                    if(foodArray == positionSnake[i]) //When snake eats food, food becomes snake
                    {
                        positionSnake[snakeLenght + 1] = foodArray;
                        snakeLenght++;
                        SpawnFood();
                        score++;
                        labelScore.Text = "Score : " + score.ToString();
                    }
                    Draw(0);
                }
                else 
                {
                    positionSnake[i] = new Point(positionSnake[i - 1].X, positionSnake[i - 1].Y);
                    
                    Draw(0);
                }

            }

        }

       

        private void Draw(int option)
        {
            Graphics g = mainPanel.CreateGraphics();
            if(option == 0) 
            {
                    for(int i = 0; i <= snakeLenght; i++)
                    {
                        if(i == 0)
                        {
                            g.FillRectangle(Brushes.Silver, positionSnake[i].X, positionSnake[i].Y, 16, 16);

                        }
                        else
                            g.FillRectangle(Brushes.SeaGreen, positionSnake[i].X, positionSnake[i].Y, 16, 16);

                    }
                
            }
            else if(option == 1)
                g.FillRectangle(Brushes.Aquamarine, foodArray.X, foodArray.Y, 16, 16);
            else if(option == 2) 
               mainPanel.BackColor = Color.FromArgb(255, mainPanel.BackColor);
        }

        private void EndGame()
        {
            timerTimer.Stop();
            endGame = true;
            timerForMoving.Interval = 200;
            timerForMoving.Stop();
            ShowMenu(true);
        }

        private bool SnakeOutOfBounds()
        {

            bool outOfBounds = false;
            if (positionSnake[0].X >= mainPanel.Width - 8 || positionSnake[0].X < 0)
                outOfBounds = true;
            else if (positionSnake[0].Y >= mainPanel.Height -8 || positionSnake[0].Y < 0)
                outOfBounds = true;
            return outOfBounds;
        }

        private bool CanSpawnFood(Point point)
        {
            bool boolean = true;
            for(int i = 0; i < snakeLenght; i++)
            {
                if(point == positionSnake[i])
                {
                    canSpawnFood = false;
                    boolean = false;
                    break;
                }
                else
                    canSpawnFood = true;
            }
            
            return boolean;
        }



        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            GetInput(e);
        }

        private void timerTimer_Tick(object sender, EventArgs e)
        {
            mseconds++;
            if (mseconds > 59)
            {
                mseconds = 00;
                seconds++;
            }
            if(seconds > 59)
            {
                seconds = 00;
                minute++;
            }
            timeElapsed = "Time : " + minute + ":" + seconds + ":" + mseconds;
            labelTimer.Text = timeElapsed;

       }

        private void mainPanel_Paint(object sender, PaintEventArgs e)
        {
            Draw(1);
           
        }
        private void timerForMoving_Tick(object sender, EventArgs e)
        {
            if(!endGame)
            {
                SnakeMoving();

            }
        }

    }
}
