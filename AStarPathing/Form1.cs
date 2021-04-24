using AStar;
using EllerAlg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AStarPathing
{
    public partial class Form1 : Form
    {
        private int _width;
        private int _height;
        private Grid _grid;
        private int _renderCount = 0;


        public Form1()
        {
            InitializeComponent();

        }

        public void CreateGridImage(int scalar)
        {
            Random rnd = new Random();
            using (var image = new Bitmap(_width * scalar, _height * scalar)) // , PixelFormat.Format32bppArgb
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);

                    int closedCount = 0;

                    for (var x = 0; x < _width; x++)
                    {
                        for (var y = 0; y < _height; y++)
                        {
                            if (_grid[new Vector2Int(x, y)].Blocked)
                            {
                                graphics.FillRectangle(new SolidBrush(Color.Black), x * scalar - scalar / 2,
                                    y * scalar - scalar / 2, scalar, scalar);
                            }
                            else if (_grid[new Vector2Int(x, y)].Closed)
                            {
                                closedCount++;
                                CircleAtPoint(graphics,
                                    new PointF(x * scalar, y * scalar), 4,
                                    Color.IndianRed);
                            }
                        }
                    }
                }
                image.Save(
                        $"Paths\\start.png", ImageFormat.Png);
                pictureBox1.Image = Image.FromFile($"Paths\\start.png");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var runs = (int)numericUpDown1.Value;


            var search = new AStarSearch(_grid);
            var timer = new Stopwatch();
            var times = new List<long>();
            var stepTimes = new List<double>();

            for (var runIndex = 0; runIndex < runs; runIndex++)
            {
                var start = new Vector2Int((int)numericUpDown2.Value, (int)numericUpDown3.Value);
                var finish = new Vector2Int((int)numericUpDown4.Value, (int)numericUpDown5.Value);

                timer.Start();

                var greedyPath = search.GreedyFind(start, finish);
                var path = search.Find(start, finish);

                timer.Stop();

                if (!greedyPath.Any())
                    throw new Exception("Any");
                if (!greedyPath.Last().Location.Equals(finish))
                {
                    finish = greedyPath.Last().Location;
                }
                if (!greedyPath.First().Location.Equals(start))
                    throw new Exception("Start");
                RenderPath(_width, _height, start, finish, greedyPath, path, search, _grid, runIndex, timer.Elapsed);

                times.Add(timer.ElapsedMilliseconds);
                stepTimes.Add((double)timer.ElapsedMilliseconds / greedyPath.Length);

                timer.Reset();
            }
        }

        #region Private Methods

        private async void EllerAsync(Cell[,] cells)
        {
            await Task.Run(() =>
            {
                MazeCreator mc = new MazeCreator((cells.GetUpperBound(1) + 1) / 2, (cells.GetUpperBound(0) + 1) / 2);
                mc.Generate();
                for (int i = 0; i < cells.GetUpperBound(0); i++)
                {
                    for (int j = 0; j < cells.GetUpperBound(1); j++)
                    {
                        if (cells[i, j].Blocked)
                            continue;
                        else
                        {
                            if (mc.Maze[i / 2][j / 2].Bottom && i >= 1)
                                cells[i + 1, j].Blocked = true;
                            if (mc.Maze[i / 2][j / 2].Right && j >= 1)
                                cells[i, j + 1].Blocked = true;
                            if (mc.Maze[i / 2][j / 2].Top && mc.Maze[i / 2][j / 2].Left && i >= 1 && j >= 1)
                                cells[i - 1, j - 1].Blocked = true;
                            if (mc.Maze[i / 2][j / 2].Bottom && mc.Maze[i / 2][j / 2].Right)
                                cells[i + 1, j + 1].Blocked = true;
                        }
                    }
                }
                _grid = new Grid(cells);
            });
            CreateGridImage(10);
        }

        private Vector2Int GetRandomCell(int maxX, int maxY, Random rnd)
        {
            var ix = rnd.Next(0, maxX);
            var iy = rnd.Next(0, maxY);
            return new Vector2Int(iy, ix);
        }

        private void RenderPath(int width, int height, Vector2Int start, Vector2Int goal, IList<Cell> greedyPath, IList<Cell> path,
            AStarSearch search, IGridProvider grid, int runIndex, TimeSpan elapsed)
        {
            var scalar = 10;

            var verdana = new FontFamily("Verdana");
            var statsFont = new Font(verdana, 36, FontStyle.Bold, GraphicsUnit.Pixel);

            using (var image = new Bitmap(width * scalar, height * scalar, PixelFormat.Format32bppArgb))
            {
                using (var graphics = Graphics.FromImage(image))
                {
                    graphics.Clear(Color.White);

                    int closedCount = 0;

                    for (var x = 0; x < width; x++)
                    {
                        for (var y = 0; y < height; y++)
                        {
                            if (grid[new Vector2Int(x, y)].Blocked)
                            {
                                graphics.FillRectangle(new SolidBrush(Color.Black), x * scalar - scalar / 2,
                                    y * scalar - scalar / 2, scalar, scalar);
                            }
                            else
                            {
                                if (grid[new Vector2Int(x, y)].Value == 1)
                                    graphics.FillRectangle(new SolidBrush(Color.DarkGray), x * scalar - scalar / 2,
                                        y * scalar - scalar / 2, scalar, scalar);
                                else if (grid[new Vector2Int(x, y)].Value == 2)
                                    graphics.FillRectangle(new SolidBrush(Color.DarkGreen), x * scalar - scalar / 2,
                                        y * scalar - scalar / 2, scalar, scalar);
                                else if (grid[new Vector2Int(x, y)].Value == 3)
                                    graphics.FillRectangle(new SolidBrush(Color.LightYellow), x * scalar - scalar / 2,
                                        y * scalar - scalar / 2, scalar, scalar);
                            }
                        }
                    }
                    Console.WriteLine(closedCount);
                    PointF[] greedyPathh = greedyPath.Select(n => new PointF(n.Location.X * scalar, n.Location.Y * scalar)).ToArray();

                    if (greedyPathh.Count() > 1)
                        graphics.DrawLines(new Pen(new SolidBrush(Color.Red), 4),
                        greedyPathh);

                    if (path != null)
                    {

                        PointF[] pathh = path.Select(n => new PointF(n.Location.X * scalar, n.Location.Y * scalar)).ToArray();

                        if (pathh.Count() > 1)
                            graphics.DrawLines(new Pen(new SolidBrush(Color.Blue), 4),
                            pathh);
                    }

                    CircleAtPoint(graphics, new PointF(start.X * scalar, start.Y * scalar), 10, Color.Red);
                    CircleAtPoint(graphics, new PointF(goal.X * scalar, goal.Y * scalar), 10, Color.Blue);

                    graphics.DrawString(
                        $"Elapsed: {elapsed.TotalMilliseconds:000}ms Closed: {closedCount:00000}",
                        statsFont, new SolidBrush(Color.Black), 2, height * scalar - (statsFont.GetHeight(graphics) + 2));
                    image.Save(
                        $"Paths\\{start.X}_{start.Y}-{goal.X}_{goal.Y}_{runIndex}_{_renderCount}.png",
                        ImageFormat.Png);
                    pictureBox1.Image = Image.FromFile($"Paths\\{start.X}_{start.Y}-{goal.X}_{goal.Y}_{runIndex}_{_renderCount}.png");
                    _renderCount++;
                }
            }
        }


        private void CircleAtPoint(Graphics graphics, PointF center, float radius, Color color)
        {
            var shifted = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
            graphics.FillEllipse(new SolidBrush(color), shifted);
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void ToBlocked()
        {
            var cells = _grid.Cells;
            for (int i = 0; i < cells.GetUpperBound(0); i++)
            {
                for (int j = 0; j < cells.GetUpperBound(1); j++)
                {
                    if (i % 2 != 0 && j % 2 != 0)
                        cells[i, j].Blocked = false;
                    else
                        cells[i, j].Blocked = true;

                }
            }
        }

        #endregion

        #region Resize Buttons
        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = ResizeImage(pictureBox1.Image, pictureBox1.Width - 475, pictureBox1.Height - 350);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = ResizeImage(pictureBox1.Image, pictureBox1.Width + 475, pictureBox1.Height + 350);
        }
        #endregion
        private void CreateBut_Click(object sender, EventArgs e)
        {
            _grid = new Grid((int)GrNumeric.Value, (int)GrNumeric2.Value);
            _width = _grid.Width;
            _height = _grid.Height;
            //ToBlocked();
            EllerAsync(_grid.Cells);
            numericUpDown1.Minimum = 1;
            panel1.MaximumSize = panel1.Size;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.MaximumSize = panel1.Size;

            var runs = 1;

            var image = (Bitmap)Image.FromFile("cavern.gif");

            var width = image.Width;
            var height = image.Height;

            _grid = new Grid(width, height);

            if (!Directory.Exists("Paths"))
                Directory.CreateDirectory("Paths");

            var timer = new Stopwatch();

            var times = new List<long>();
            var stepTimes = new List<double>();

            var search = new AStarSearch(_grid);

            for (var runIndex = 0; runIndex < runs; runIndex++)
            {

                if (numericUpDown2.Value > _grid.Width)
                    numericUpDown2.Value = _grid.Width - 10;

                if (numericUpDown3.Value > _grid.Width)
                    numericUpDown3.Value = _grid.Height - 10;

                if (numericUpDown4.Value > _grid.Width)
                    numericUpDown4.Value = _grid.Width - 10;

                if (numericUpDown5.Value > _grid.Width)
                    numericUpDown5.Value = _grid.Height - 10;

                var start = new Vector2Int((int)numericUpDown2.Value, (int)numericUpDown3.Value);
                var finish = new Vector2Int((int)numericUpDown4.Value, (int)numericUpDown5.Value);

                for (var x = 0; x < width; x++)
                    for (var y = 0; y < height; y++)
                    {
                        _grid[x, y].Blocked = (image.GetPixel(x, y).R + image.GetPixel(x, y).G + image.GetPixel(x, y).B) < 120;
                    }

                timer.Start();

                var greedyPath = search.GreedyFind(start, finish);

                var path = search.Find(start, finish);

                timer.Stop();

                if (!greedyPath.Any() || !greedyPath.Last().Location.Equals(finish) || !greedyPath.First().Location.Equals(start))
                    MessageBox.Show(
                    "Скорее всего вы указали стену, алгоритм не смог найти заданную точку",
                    "Введите новое значение",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.DefaultDesktopOnly);
                else
                {
                    RenderPath(width, height, start, finish, greedyPath, path, search, _grid, runIndex, timer.Elapsed); //greedy

                    times.Add(timer.ElapsedMilliseconds);
                    stepTimes.Add((double)timer.ElapsedMilliseconds / greedyPath.Length);

                    timer.Reset();
                }
            }
            if (times.Count > 0)
                Console.WriteLine("Elapsed: Avg: {0:000}ms Min: {1:000}ms Max: {2:000}ms StepAvg:{3:0.000000}ms",
                    times.Average(), times.Min(), times.Max(), stepTimes.Average());
        }
    }
}

