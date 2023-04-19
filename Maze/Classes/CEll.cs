using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Maze.Classes
{
	public enum CellType
	{
		Free,
		Blocked,
		Start,
		End1,
		End2,
		Path1, 
		Path2,
		PathCommon
	}

	public enum Direction
	{
		Left,
		Right,
		Up, 
		Down
	}

	public class Cell
	{
		public Maze Maze;
		public int Row = 0;
		public int Column = 0;
		public CellType Type = CellType.Free;
		public Rectangle Rectangle = new Rectangle();

		public Cell(Maze parent, int column, int row)
		{
			Maze = parent;
			Column = column;
			Row = row;

			Random random = new Random();
			double blockedPossibility = random.NextDouble();
			Type = Maze.BlockedPossibility > blockedPossibility ? CellType.Blocked : CellType.Free;

			SetRectangle();
        }

		public override string ToString()
		{
			string result = $"({Row}, {Column})";
			return result;

		}

		/// <summary>
		/// Set the sizes of the rectangle based on the current sizes of the screen
		/// </summary>
		public void SetRectangle()
		{
			Rectangle.StrokeThickness = 1;
			SetBorder(Colors.Black);

            if (Type == CellType.Free)
				Rectangle.Fill = new SolidColorBrush(Colors.White);
			else if (Type == CellType.Blocked)
                Rectangle.Fill = new SolidColorBrush(Colors.DarkBlue);
			else if (Type == CellType.Start)
                Rectangle.Fill = new SolidColorBrush(Colors.Bisque);
			else if (Type == CellType.End1)
                Rectangle.Fill = new SolidColorBrush(Colors.GreenYellow);
			else if (Type == CellType.End2)
                Rectangle.Fill = new SolidColorBrush(Colors.YellowGreen);
			else if (Type == CellType.Path1)
                Rectangle.Fill = new SolidColorBrush(Colors.MediumPurple);
			else if (Type == CellType.Path2)
                Rectangle.Fill = new SolidColorBrush(Colors.MediumPurple);
			else if (Type == CellType.PathCommon)
                Rectangle.Fill = new SolidColorBrush(Colors.Purple);

			Rectangle.Width = Maze.CellWidth;
			Rectangle.Height = Maze.CellHeight;

			int left, top;
			if (Column == 0)
				left = 0;
			else
				left = Maze.CellWidth * Column;

			if (Row == 0)
				top = 0;
			else
				top = Maze.CellHeight * Row;

			Canvas.SetLeft(Rectangle, left);
			Canvas.SetTop(Rectangle, top);
		}

		/// <summary>
		/// Revert the type cell from bloked to unblocked and vice versa
		/// </summary>
		public void RevertBlocked()
		{
			if (Type == CellType.Free)
			{
				Type = CellType.Blocked;
				Rectangle.Fill = new SolidColorBrush(Colors.DarkBlue);
			}
			else if (Type == CellType.Blocked)
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

		/// <summary>
		/// Set the color of the border
		/// </summary>
		/// <param name="color"></param>
        public void SetBorder(Color color)
        {
            Rectangle.Stroke = new SolidColorBrush(color);
        }

		/// <summary>
		/// Get the sibling cell in the maze to a specific direction if is unblocked
		/// </summary>
		/// <param name="direction"></param>
		/// <returns></returns>
		public Cell? GetSibling(Direction direction)
		{
			try
			{
				Cell? sibling = null;

				if (direction == Direction.Left)
					sibling = this.Maze.GetCell(this.Row, this.Column - 1);
				else if (direction == Direction.Right)
					sibling = this.Maze.GetCell(this.Row, this.Column + 1);
				else if (direction == Direction.Down)
					sibling = this.Maze.GetCell(this.Row + 1, this.Column);
				else if (direction == Direction.Up)
					sibling = this.Maze.GetCell(this.Row - 1, this.Column);

				if (sibling != null && sibling.Type != CellType.Blocked)
					return sibling;
				else
					return null;
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
	}
}
