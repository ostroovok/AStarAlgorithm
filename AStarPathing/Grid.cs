using AStar;
using System;

namespace AStarPathing
{
    public class Grid : IGridProvider
    {
        public Cell[,] Cells { get; }

        public int Width { get; set; }
        public int Height { get; set; }

        public Cell this[int x, int y] => Cells[x, y];

        public Vector2Int Size => new Vector2Int(Width, Height);
        public Cell this[Vector2Int location] => Cells[location.X, location.Y];

        public Grid(Cell[,] cells)
        {
            Cells = cells;
            Width = cells.GetUpperBound(1) + 1;
            Height = cells.GetUpperBound(0) + 1;
        }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;

            Cells = new Cell[width, height];
            Reset();
        }

        public void Reset()
        {
            Random rnd = new Random();
            for (var x = 0; x <= Cells.GetUpperBound(0); x++)
            {
                for (var y = 0; y <= Cells.GetUpperBound(1); y++)
                {

                    var cell = Cells[x, y];

                    if (cell == null)
                    {
                        Cells[x, y] = new Cell(new Vector2Int(x, y), rnd.Next(1, 4));
                    }
                    else
                    {
                        cell.Value = rnd.Next(1, 4);
                        cell.G = 0;
                        cell.H = 0;
                        cell.F = 0;
                        cell.Closed = false;
                        cell.Parent = null;
                    }
                }
            }
        }

        public void Print()
        {
            for (var x = 0; x <= Cells.GetUpperBound(0); x++)
            {
                for (var y = 0; y <= Cells.GetUpperBound(1); y++)
                {
                    System.Console.Write(Cells[x, y].Blocked + ", ");
                }
                System.Console.WriteLine();
            }
        }

        public int GetNodeId(Vector2Int location) => location.X * Width + location.Y;
    }
}