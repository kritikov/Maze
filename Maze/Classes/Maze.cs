﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Maze.Classes
{
	public class Maze: INotifyPropertyChanged
	{
		#region VARIABLES AND NESTED CLASSES

		public event PropertyChangedEventHandler PropertyChanged;

		private Brush Background = new SolidColorBrush(Colors.White);
		private Canvas Canvas;

		public int Rows = 15;
		public int Columns = 15;
		public Cell[,] Cells;
		public double Width => Canvas.ActualWidth;
		public double Height => Canvas.ActualHeight;
		public int CellWidth => (int)(Width / Columns);
		public int CellHeight => (int)(Height / Rows);

		private int n = 15;
		public int N
		{
			get { return n; }
			set
			{
				n = value;
				Rows = n;
				Columns = n;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(N)));
				Create();
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

		private Cell? selectedCell;
		public Cell? SelectedCell
		{
			get { return selectedCell; }
			set { 
				selectedCell = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedCell)));
            }
		}

		private Cell? startCell;
		public Cell? StartCell
		{
			get { return startCell; }
			set {
                startCell = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StartCell)));
            }
		}

		private Cell? end1Cell;
		public Cell? End1Cell
        {
			get { return end1Cell; }
			set {
                end1Cell = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(End1Cell)));
            }
		}

		private Cell? end2Cell;
		public Cell? End2Cell
        {
			get { return end2Cell; }
			set {
                end2Cell = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(End2Cell)));
            }
		}

		#endregion


		#region CONSTRUCTORS

		public Maze(Canvas canvas)
		{
			Canvas = canvas;
		}

		#endregion


		#region METHODS

		/// <summary>
		/// Create the maze. This is used only once and when we change the number of cells
		/// </summary>
		public void Create()
		{
			try
			{
				if (N < 5 || N > 50)
					throw new Exception("The dimension of the maze are out of limits, the N must be between 5 and 50");

				if (BlockedPossibility < 0 || BlockedPossibility > 1)
					throw new Exception("The possibility of a cell to be blocked is out of limits, the P must be between 0 and 1");

				Canvas.Background = Background;
				Canvas.Children.Clear();
				SelectedCell = null;
				StartCell = null;
				End1Cell = null;
				End2Cell = null;

				CreateCells();

			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		/// <summary>
		/// Create the cells of the maze
		/// </summary>
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

		/// <summary>
		/// Create one cell of the maze
		/// </summary>
		/// <param name="column"></param>
		/// <param name="row"></param>
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
		/// Draw the cells in the screen based on the current sizes of the window
		/// </summary>
		public void Redraw()
		{
			if (Cells != null)
			{
				for (int i = 0; i < Rows; i++)
				{
					for (int j = 0; j < Columns; j++)
					{
						Cells[i, j].SetRectangle();
					}
				}
			}
		}

		/// <summary>
		/// Reset the maze to its initial condition before the A* runs
		/// </summary>
		public void Reset()
		{
			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					var cell = Cells[i, j];
					if (cell.Type == CellType.Path1 || cell.Type == CellType.Path2 || cell.Type == CellType.PathCommon)
						cell.Type = CellType.Free;
				}
			}

			Redraw();
		}

		/// <summary>
		/// Get the cell by a specific point
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Get the cell by a specific point
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public Cell? GetCell(int row, int column)
		{
			try
			{
				if (row < this.Rows && column < this.Columns && row >= 0 && column >= 0)
					return Cells[row, column];
				else
					return null;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Select a cell and change its border color as to have focus
		/// </summary>
		/// <param name="cell"></param>
		public void SelectCell(Cell cell)
		{
			SelectedCell?.Deselect();
			SelectedCell = cell;
			SelectedCell?.Select();
		}

		/// <summary>
		/// Select a cell as the start position
		/// </summary>
		/// <param name="cell"></param>
		public void SelectStartCell(Cell cell)
		{
			// previous start cell
			if (StartCell != null)
			{
                StartCell?.SetColor(Colors.White);
                StartCell.Type = CellType.Free;
            }

			// new start cell
            StartCell = cell;
            StartCell?.SetColor(Colors.Bisque);
            StartCell.Type = CellType.Start;

			Reset();
		}

		/// <summary>
		/// Select a cell as the end position 1
		/// </summary>
		/// <param name="cell"></param>
		public void SelectEnd1Cell(Cell cell)
		{
            // previous end cell 1
            if (End1Cell != null)
            {
                End1Cell.SetColor(Colors.White);
                End1Cell.Type = CellType.Free;
            }

            // new end cell 1
            End1Cell = cell;
            End1Cell?.SetColor(Colors.GreenYellow);
            End1Cell.Type = CellType.End1;

			Reset();
		}

        /// <summary>
        /// Select a cell as the end position 1
        /// </summary>
        /// <param name="cell"></param>
        public void SelectEnd2Cell(Cell cell)
        {
            // previous end cell 2
            if (End2Cell != null)
            {
                End2Cell.SetColor(Colors.White);
                End2Cell.Type = CellType.Free;
            }

            // new end cell 1
            End2Cell = cell;
            End2Cell?.SetColor(Colors.YellowGreen);
            End2Cell.Type = CellType.End2;

			Reset();
		}
		
		#endregion
	}
}
