namespace AStar
{

    public class Cell
    {
        public bool Blocked { get; set; } //= true;
        public bool Visited { get; set; }
        public bool Closed { get; set; }
        public double F { get; set; }

        public int Value { get; set; }
        public double G { get; set; }
        public double H { get; set; }

        public Vector2Int Location { get; set; }

        public Cell Parent { get; set; }

        public int QueueIndex { get; set; }

        public Cell(Vector2Int location, int value)
        {
            Location = location;
            Value = value;
        }

        public override string ToString() => $"[{Location.X},{Location.Y}]";
    }

}