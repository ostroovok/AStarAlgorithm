using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EllerAlg
{
    public class MazeCreator
    {
        public int Width { get; }
        public int Height { get; private set; }
        public ECell[][] Maze { get; private set; }

        private Random _rnd;

        public MazeCreator(int width, int height)
        {
            Width = width;
            Height = height;
            _rnd = new Random();
            Maze = new ECell[Height][];
            for (int i = 0; i < Height; i++)
            {
                Maze[i] = new ECell[Width];
                for (int j = 0; j < Width; j++)
                {
                    Maze[i][j] = new ECell();
                }
            }
        }

        public ECell[][] Generate()
        {

            Maze = new ECell[Height][];
            for (int i = 0; i < Height; i++)
            {
                Maze[i] = new ECell[Width];
                for (int j = 0; j < Width; j++)
                {
                    Maze[i][j] = new ECell();
                }

            }

            var temp = new int[Width];
            var bot = new int[Width];
            for (int i = 0; i < Width; i++)
            {
                temp[i] = i;
                bot[i] = i;
            }

            for (int i = 0; i < Height - 1; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    if (j != Width - 1 && j + 1 != temp[j] && _rnd.NextDouble() < 0.5)
                    {
                        Maze[i][j].Right = false;

                        temp[bot[j + 1]] = temp[j];

                        bot[temp[j]] = bot[j + 1];

                        temp[j] = j + 1;
                        
                        bot[j + 1] = j;
                    }

                    if (j != temp[j] && _rnd.NextDouble() < 0.5)
                    {
                        temp[bot[j]] = temp[j];

                        bot[temp[j]] = bot[j];

                        temp[j] = j;

                        bot[j] = j;
                    } 
                    else
                        Maze[i][j].Bottom = false;
                }

                for (int c = 0; c < temp.Length; c++)
                {
                    Console.Write(temp[c]+ "  ");
                }
                Console.WriteLine();
            }

            for (int j = 0; j < Width; j++)
            {
                if (j != Width - 1 && j + 1 != temp[j] && (j == temp[j] || _rnd.NextDouble() < 0.5))
                {
                    Maze.Last()[j].Right = false;

                    temp[bot[j + 1]] = temp[j];

                    bot[temp[j]] = bot[j + 1];

                    temp[j] = j + 1;

                    bot[j + 1] = j;
                }
                temp[bot[j]] = temp[j];

                bot[temp[j]] = bot[j];

                temp[j] = j;

                bot[j] = j;
            }
            if(Console.ReadKey().Key == ConsoleKey.W)
                PrintWithOutNumbers(0, Height, Maze);
            return Maze;
        }

        public void PrintWithOutNumbers(int start, int h, ECell[][] maze)
        {
            for (int i = 0; i < maze[0].Length; i++)
            {
                Console.Write($"__");
                ConsoleColor color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write(".");
                Console.ForegroundColor = color;
            }
            Console.WriteLine();
            for (int i = start; i < h; i++)
            {
                for (int j = 0; j < maze[0].Length; j++)
                {
                    if (maze[i][j].Bottom)
                    {
                        Console.Write("__");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    if (maze[i][j].Right)
                    {
                        Console.Write("|");
                    }
                    else
                    {
                        ConsoleColor color = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(".");
                        Console.ForegroundColor = color;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
