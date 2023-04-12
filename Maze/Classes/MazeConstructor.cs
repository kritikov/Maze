using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Maze.Classes
{

	public class MazeConstructor
	{
		#region VARIABLES AND NESTED CLASSES

		public int Rows = 5;
		public int Columns = 5;
		public double Width => Canvas.ActualWidth;
		public int CellWidth => (int)(Width / Columns);
		public double Height => Canvas.ActualHeight;
		public int CellHeight => (int)(Height / Rows);
		public Brush Background = new SolidColorBrush(Colors.White);
		public Canvas Canvas;
		public List<Cell> Cells = new List<Cell>();

		#endregion


		#region CONSTRUCTORS

		public MazeConstructor(Canvas canvas)
		{
			Canvas = canvas;
		}
		
		#endregion


		#region METHODS

		/// <summary>
		/// Create the maze. This is used only once and when we change the number of cells
		/// </summary>
		public void Construct()
		{
			Canvas.Background = Background;
			Canvas.Children.Clear();

			CreateCells();
		}

		// Create the cells of the maze
		public void CreateCells()
		{
			Cells.Clear();
			Canvas.Children.Clear();

			for (int i = 1; i <= Columns; i++)
			{
				for (int j = 1; j <= Rows; j++)
				{
					CreateCell(i, j);
				}
			}
		}

		// Create one cell of the maze
		public void CreateCell(int column, int row)
		{
			if (row > Rows || column > Columns)
				return;

			Cell cell = new Cell(this, column, row);
			Cells.Add(cell);
			Canvas.Children.Add(cell.Rectangle);
		}

		/// <summary>
		/// Draw the cells in the screen.Should run every time we change the size of the window
		/// </summary>
		public void Redraw()
		{
			foreach(var cell in Cells)
			{
				cell.SetRectangle();
			}
		}

		public Cell GetCell(int posX, int posY)
		{
			int column = (int)(posX / CellWidth) + 1;
			int row = (int)(posY / CellHeight) + 1;

			var cell = this.Cells.Where(p => p.Row == row && p.Column == column).FirstOrDefault();
			cell.SetColor(Colors.AliceBlue);

			return cell;
		}

		public void RevertCellType(int posX, int posY)
		{
			Cell cell = GetCell(posX, posY);
			cell.RevertType();
		}



		public void Set (int rows, int columns)
		{
			Rows = rows;
			Columns = columns;
		}

		#endregion
	}
}
