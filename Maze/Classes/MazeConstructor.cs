using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Maze.Classes
{
	public enum DrawType
	{
		Stretch,
		Fixed
	}

	public class MazeConstructor: INotifyPropertyChanged
	{
		#region VARIABLES AND NESTED CLASSES

		public event PropertyChangedEventHandler PropertyChanged;

		public int Rows = 5;
		public int Columns = 5;
		public Cell[,] Cells;
		public double Width => Canvas.ActualWidth;
		public double Height => Canvas.ActualHeight;
		public int CellWidth => (int)(Width / Columns);
		public int CellHeight => (int)(Height / Rows);
		public Brush Background = new SolidColorBrush(Colors.White);
		public Canvas Canvas;

		private int n = 5;
		public int N
		{
			get { return n; }
			set
			{
				n = value;
				Rows = n;
				Columns = n;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(N)));
			}
		}

		private double blockedPossibility = 0.2;
		public double BlockedPossibility
		{
			get { return blockedPossibility; }
			set
			{
				blockedPossibility = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BlockedPossibility)));
			}
		}

		private int fixedCellDimension = 20;
		public int FixedCellDimension
		{
			get { return fixedCellDimension; }
			set
			{
				fixedCellDimension = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FixedCellDimension)));
			}
		}

		private bool editCells = false;
		public bool EditCells
		{
			get { return editCells; }
			set
			{
				editCells = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EditCells)));
			}
		}

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

		public Cell GetCell(Point point)
		{
			try
			{
				int column = (int)(point.X / CellWidth);
				int row = (int)(point.Y / CellHeight);
				var cell = Cells[row, column];

				return cell;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion
	}
}
