using System;
using System.Collections.Generic;

namespace AStar
{

    public class AStarSearch
    {

        private readonly IGridProvider _grid;
        private readonly FastPriorityQueue _open;

        public AStarSearch(IGridProvider grid)
        {

            _grid = grid;
            _open = new FastPriorityQueue(_grid.Size.X * _grid.Size.Y);
        }

        private double Heuristic(Cell cell, Cell goal)
        {

            var dX = Math.Abs(cell.Location.X - goal.Location.X);
            var dY = Math.Abs(cell.Location.Y - goal.Location.Y);

            // Octile distance
            return 1 * (dX + dY) + (Math.Sqrt(2) - 2 * 1) * Math.Min(dX, dY);
        }

        public void Reset()
        {

            _grid.Reset();
            _open.Clear();
        }

        public Cell[] GreedyFind(Vector2Int start, Vector2Int goal)
        {

            Reset();

            Cell startCell = _grid[start]; //first point
            Cell goalCell = _grid[goal];   //last point
            //Console.SetCursorPosition(1, 4);
            //Console.WriteLine($"{goalCell.Location.X}-----{goalCell.Location.Y} ---- Greedy");

            _open.Enqueue(startCell, 0);   // not checked

            var bounds = _grid.Size;

            Cell node = null;

            while (_open.Count > 0)
            {
                node = _open.Dequeue();

                node.Closed = true;

                var g = node.G + 1;

                if (goalCell.Location == node.Location) break;

                Vector2Int proposed = new Vector2Int(0, 0);

                for (var i = 0; i < PathingConstants.Directions.Length; i++)
                {
                    var direction = PathingConstants.Directions[i];
                    proposed.X = node.Location.X + direction.X;
                    proposed.Y = node.Location.Y + direction.Y;
                    if (proposed.X < 0 || proposed.X >= bounds.X ||
                        proposed.Y < 0 || proposed.Y >= bounds.Y)
                        continue;

                    Cell neighbour = _grid[proposed];

                    if (neighbour.Blocked) continue;

                    if (_grid[neighbour.Location].Closed) continue;

                    if (!_open.Contains(neighbour))
                    {

                        neighbour.G = g;
                        neighbour.H = Heuristic(neighbour, node);
                        neighbour.Parent = node;
                        // F will be set by the queue
                        _open.Enqueue(neighbour, neighbour.G + neighbour.H + neighbour.Value);

                    }
                    else if (g + neighbour.H + neighbour.Value < neighbour.F)
                    {
                        neighbour.G = g;
                        neighbour.F = neighbour.G + neighbour.H + neighbour.Value;
                        neighbour.Parent = node;
                    }
                }
            }

            var path = new Stack<Cell>();

            while (node != null)
            {
                path.Push(node);
                node = node.Parent;
            }

            return path.ToArray();
        }
        public Cell[] Find(Vector2Int start, Vector2Int goal)
        {

            Reset();

            Cell startCell = _grid[start]; //first point
            Cell goalCell = _grid[goal];   //last point\
            //Console.SetCursorPosition(1, 5);
            //Console.WriteLine($"{goalCell.Location.X}-----{goalCell.Location.Y} ---- Base");
            _open.Enqueue(startCell, 0);   // not checked

            var bounds = _grid.Size;

            Cell node = null;

            while (_open.Count > 0)
            {
                node = _open.Dequeue();

                node.Closed = true;

                var g = node.G + 1;

                if (goalCell.Location == node.Location) break;

                Vector2Int proposed = new Vector2Int(0, 0);

                for (var i = 0; i < PathingConstants.Directions.Length; i++)
                {
                    var direction = PathingConstants.Directions[i];
                    proposed.X = node.Location.X + direction.X;
                    proposed.Y = node.Location.Y + direction.Y;
                    if (proposed.X < 0 || proposed.X >= bounds.X ||
                        proposed.Y < 0 || proposed.Y >= bounds.Y)
                        continue;

                    Cell neighbour = _grid[proposed];

                    if (neighbour.Blocked) continue;

                    if (_grid[neighbour.Location].Closed) continue;

                    if (!_open.Contains(neighbour))
                    {

                        neighbour.G = g;
                        neighbour.H = Heuristic(neighbour, goalCell);
                        neighbour.Parent = node;
                        // F will be set by the queue
                        _open.Enqueue(neighbour, neighbour.G + neighbour.H + neighbour.Value);

                    }
                    else if (g + neighbour.H + neighbour.Value < neighbour.F)
                    {
                        neighbour.G = g;
                        neighbour.F = neighbour.G + neighbour.H + neighbour.Value;
                        neighbour.Parent = node;
                    }
                }
            }

            var path = new Stack<Cell>();

            while (node != null)
            {
                path.Push(node);
                node = node.Parent;
            }

            return path.ToArray();
        }
    }

}