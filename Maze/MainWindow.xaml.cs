﻿using Maze.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Maze
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region VARIABLES AND NESTED CLASSES

		private string message = "";
		public string Message
		{
			get { return message; }
			set
			{
				message = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Message"));
			}
		}

		private CollectionViewSource logsSource = new CollectionViewSource();
		public ICollectionView LogsView
		{
			get
			{
				return this.logsSource.View;
			}
		}

		public MazeConstructor MazeConstructor { get; set; }

		private bool stretchCanvas = true;
		public bool StretchCanvas
		{
			get { return stretchCanvas; }
			set
			{
				stretchCanvas = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(StretchCanvas)));
			}
		}

		private bool algorithmIsRunning = false;
		public bool AlgorithmIsRunning
		{
			get { return algorithmIsRunning; }
			set
			{
				algorithmIsRunning = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AlgorithmIsRunning)));
			}
		}

		public CellPosition StartPosition { get; set; } = new CellPosition();
		public CellPosition EndPosition1 { get; set; } = new CellPosition();
		public CellPosition EndPosition2 { get; set; } = new CellPosition();

		#endregion


		#region CONSTRUCTORS

		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = this;
			logsSource.Source = Logs.List;
			MazeConstructor = new MazeConstructor(MazeCanvas);

		}

		#endregion


		#region EVENTS

		public event PropertyChangedEventHandler PropertyChanged;

		private void ClearLogs(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				ClearLogs();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void ExitProgram(object sender, RoutedEventArgs e)
		{
			System.Windows.Application.Current.Shutdown();
		}

		private void CanvasClick(object sender, MouseButtonEventArgs e)
		{
			Message = "";

			try
			{
				Point point = Mouse.GetPosition(MazeCanvas);
				var selectedSell = MazeConstructor.GetCell(point);

				Message = $"cell position: {selectedSell.Row + 1}, {selectedSell.Column + 1}";

				if (MazeConstructor.EditCells == true)
					selectedSell.RevertType();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				//MazeCanvas.Width = MazeScroller.ActualWidth - 17;
				//MazeCanvas.Height = MazeScroller.ActualHeight - 17;

				this.MazeConstructor.Construct();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			Message = "";

			try
			{
				if (MazeConstructor.Cells != null)
					MazeConstructor.Redraw();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void RecreateMaze(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				ValidateParameters();
				this.MazeConstructor.Construct();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void SearchSolution(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				ValidateParameters();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		#endregion


		#region METHODS

		/// <summary>
		/// Clear the messages from the logs
		/// </summary>
		private void ClearLogs()
		{
			Logs.Clear();
		}

		/// <summary>
		/// Validate the parameters
		/// </summary>
		private void ValidateParameters()
		{
			Message = "";

			try
			{
				if (MazeConstructor.N < 5 || MazeConstructor.N > 50)
					throw new Exception("The dimension of the maze are out of limits, the N must be between 5 and 50");

				if (MazeConstructor.BlockedPossibility < 0 || MazeConstructor.BlockedPossibility > 1)
					throw new Exception("he possibility of a cell to be blocked is out of limits, the P must be between 0 and 1");


			}
			catch (Exception ex)
			{
				throw ex;
			}

		}

		#endregion

		
	}
}
