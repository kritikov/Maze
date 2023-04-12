using System;
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
		//public List<Cell> Cells = new List<Cell>();

		public Cell[,] Cells;

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
			Cells = new Cell[Rows, Columns];
			Canvas.Children.Clear();

			for (int i = 0; i < Columns; i++)
			{
				for (int j = 0; j < Rows; j++)
				{
					CreateCell(i, j);
				}
			}
		}

		// Create one cell of the maze
		public void CreateCell(int column, int row)
		{
			try
			{
				if (row >= Rows || column >= Columns)
					return;

				Cell cell = new Cell(this, column, row);
				Cells[row, column] = cell;
				Canvas.Children.Add(cell.Rectangle);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Draw the cells in the screen.Should run every time we change the size of the window
		/// </summary>
		public void Redraw()
		{
			for (int i=0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					Cells[i,j].SetRectangle();

				}
			}
		}

		public Cell GetCell(int posX, int posY)
		{
			try
			{
				int column = (int)(posX / CellWidth);
				int row = (int)(posY / CellHeight);

				var cell = Cells[row, column];
				cell.SetColor(Colors.AliceBlue);

				return cell;
			}
			catch (Exception ex)
			{
				throw ex;
			}
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
