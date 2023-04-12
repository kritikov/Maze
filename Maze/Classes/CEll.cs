using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maze.Classes
{
	public enum CellType
	{
		Free,
		Blocked
	}

	public class Cell
	{
		public MazeConstructor Parent;
		public int Row = 0;
		public int Column = 0;
		public CellType Type = CellType.Free;
		public Rectangle Rectangle = new Rectangle();

		public Cell(MazeConstructor parent, int column, int row)
		{
			Parent = parent;
			Column = column;
			Row = row;

			Random random = new Random();
			double blockedPossibility = random.NextDouble();
			Type = Parent.BlockedPossibility > blockedPossibility ? CellType.Blocked : CellType.Free;

			SetRectangle();
		}

		/// <summary>
		/// Set the sizes of the rectangle based on the current sizes of the screen
		/// </summary>
		public void SetRectangle()
		{
			Rectangle.Stroke = new SolidColorBrush(Colors.Black);
			Rectangle.StrokeThickness = 1;

			if (Type == CellType.Free)
				Rectangle.Fill = new SolidColorBrush(Colors.White);
			else
				Rectangle.Fill = new SolidColorBrush(Colors.DarkBlue);

			Rectangle.Width = Parent.CellWidth;
			Rectangle.Height = Parent.CellHeight;

			int left, top;
			if (Column == 0)
				left = 0;
			else
				left = Parent.CellWidth * Column;

			if (Row == 0)
				top = 0;
			else
				top = Parent.CellHeight * Row;

			Canvas.SetLeft(Rectangle, left);
			Canvas.SetTop(Rectangle, top);
		}

		/// <summary>
		/// Redraw the cell in the screen with the current sizes
		/// </summary>
		public void Redraw()
		{
			SetRectangle();
		}

		/// <summary>
		/// Revert the type cell from bloked to unblocked and vice versa
		/// </summary>
		public void RevertType()
		{
			if (Type == CellType.Free)
			{
				Type = CellType.Blocked;
				Rectangle.Fill = new SolidColorBrush(Colors.DarkBlue);
			}
			else
			{
				Type = CellType.Free;
				Rectangle.Fill = new SolidColorBrush(Colors.White);
			}
		}

		/// <summary>
		/// Set the background color of the cell
		/// </summary>
		/// <param name="color"></param>
		public void SetColor(Color color)
		{
			Rectangle.Fill = new SolidColorBrush(color);
		}
	}
}
