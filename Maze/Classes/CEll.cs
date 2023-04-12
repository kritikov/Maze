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

			SetRectangle();
		}

		/// <summary>
		/// Set the sizes of the rectangle based on the current sizes of the screen
		/// </summary>
		public void SetRectangle()
		{
			Rectangle.Stroke = new SolidColorBrush(Colors.Black);
			Rectangle.StrokeThickness = 1;
			//Rectangle.Fill = new SolidColorBrush(Colors.Black);
			Rectangle.Width = Parent.CellWidth;
			Rectangle.Height = Parent.CellHeight;

			int left, top;
			if (Column == 1)
				left = 0;
			else
				left = Parent.CellWidth * (Column - 1);

			if (Row == 1)
				top = 0;
			else
				top = Parent.CellHeight * (Row - 1);

			Canvas.SetLeft(Rectangle, left);
			Canvas.SetTop(Rectangle, top);
		}

		// Redraw the cell in the screen with the current sizes
		public void Redraw()
		{
			SetRectangle();
		}

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

		public void SetColor(Color color)
		{
			Rectangle.Fill = new SolidColorBrush(color);
		}
	}
}
