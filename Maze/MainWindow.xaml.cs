using Maze.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

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

		private CollectionViewSource resultsSource = new CollectionViewSource();
		public ICollectionView ResultsView
		{
			get
			{
				return resultsSource.View;
			}
		}

		public Classes.Maze Maze { get; set; }

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

		private CancellationTokenSource cancellationToken = new CancellationTokenSource();
		
		#endregion


		#region CONSTRUCTORS

		public MainWindow()
		{
			InitializeComponent();

			this.DataContext = this;
			logsSource.Source = Logs.List;
            Maze = new Classes.Maze(MazeCanvas);
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
			Application.Current.Shutdown();
		}

		private void CanvasClick(object sender, MouseButtonEventArgs e)
		{
			Message = "";

			try
			{
				// get the point where moise clicked
				Point point = Mouse.GetPosition(MazeCanvas);

				// get the corresponding cell
				var selectedSell = Maze.GetCell(point);

				// select the cell
				Maze.SelectCell(selectedSell);
                Message = $"cell position: {selectedSell.Row}, {selectedSell.Column}";

				// display the heuristic distance from start to the selected cell
				if (Maze.StartCell != null)
				{
					double h = selectedSell.HeuristicDistance(Maze.StartCell);
					Message += $", heuristic distance to selected cell: {h}";
				}

				// reverse block - unblock oh the cell
				if (Maze.EditCells == true)
					selectedSell.RevertBlocked();
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
				Maze.Create();
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
				Maze.Redraw();
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
				Maze.Create();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void ResetMaze(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				Maze.Reset();
			}
			catch (Exception ex)
			{
				Message = ex.Message;
			}
		}

		private void ASTARRun(object sender, RoutedEventArgs e)
		{
			Message = "";

			try
			{
				ValidateParameters();
				AlgorithmIsRunning = true;
				Maze.Reset();
				ASTARAnalysis();
			}
			catch (Exception ex)
			{
				Logs.Write(ex.Message);
				Message = ex.Message;
			}
		}

        private void SelectStartCell(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
			{
				SelectStartCell();
			}
			catch (Exception ex)
            {
                Message = ex.Message;
            }
        }

		private void SelectEnd1Cell(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
			{
				SelectEnd1Cell();
			}
			catch (Exception ex)
            {
                Message = ex.Message;
            }
        }
		
		private void SelectEnd2Cell(object sender, RoutedEventArgs e)
        {
            Message = "";

            try
			{
				SelectEnd2Cell();
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
			try
			{
				if (Maze.StartCell == null)
					throw new Exception("You must set the start cell");

				if (Maze.End1Cell == null)
					throw new Exception("You must set the end 1 cell");

				if (Maze.End2Cell == null)
					throw new Exception("You must set the end 2 cell");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Select the start position
		/// </summary>
		/// <exception cref="Exception"></exception>
        private void SelectStartCell()
        {
			try
			{
				if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
					Maze.SelectStartCell(Maze.SelectedCell);
				else
					throw new Exception("Select a valid cell");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Select the end 1 position
		/// </summary>
		/// <exception cref="Exception"></exception>
		private void SelectEnd1Cell()
        {
			try
			{
				if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
					Maze.SelectEnd1Cell(Maze.SelectedCell);
				else
					throw new Exception("Select a valid cell");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Select the end 2 position
		/// </summary>
		/// <exception cref="Exception"></exception>
		private void SelectEnd2Cell()
        {
			try
			{
				if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
					Maze.SelectEnd2Cell(Maze.SelectedCell);
				else
					throw new Exception("Select a valid cell");
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		/// <summary>
		/// Solve the initial state using the A* algorithm
		/// </summary>
		/// <returns></returns>
		private async Task ASTARAnalysis()
		{
			AStarResults results1 = null;
			AStarResults results2 = null;

			try
			{
				cancellationToken = new CancellationTokenSource();

				await Task.Run(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						Logs.Clear();
					});

					Cell destination1, destination2;
					if (Maze.End1Cell.HeuristicDistance(Maze.StartCell) <= Maze.End2Cell.HeuristicDistance(Maze.StartCell))
					{
						destination1 = Maze.End1Cell;
						destination2 = Maze.End2Cell;
					}
					else
					{
						destination1 = Maze.End2Cell;
						destination2 = Maze.End1Cell;
					}

					// search a path to the first destination
					State initialState1 = new State(Maze.StartCell, destination1);
					results1 = State.ASTARAnalysis(initialState1, cancellationToken.Token);

					// if a path to the first destination is found then search a path to the second destination
					if (results1.FinalState != null)
					{
						State initialState2 = new State(destination1, destination2);
						results2 = State.ASTARAnalysis(initialState2, cancellationToken.Token);
					}
				});
			}
			catch (Exception ex)
			{
				Logs.Write(ex.Message);
				Message = ex.Message;
			}
			finally
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					AlgorithmIsRunning = false;

					// if the first path is found then change their cells type to add color
					if (results1?.FinalState != null)
					{
						List<State> statesInPath = results1.FinalState.GetPath();
						foreach (var state in statesInPath)
						{
							if (state.Cell.Type == CellType.Free)
								state.Cell.Type = CellType.Path1;
						}
					}

					// if the second path is found then change their cells type to add color
					if (results2?.FinalState != null)
					{
						List<State> statesInPath = results2.FinalState.GetPath();
						foreach (var state in statesInPath)
						{
							if (state.Cell.Type == CellType.Free)
								state.Cell.Type = CellType.Path2;
							else if (state.Cell.Type == CellType.Path1)
								state.Cell.Type = CellType.PathCommon;
						}
					}

					Maze.Redraw();

					resultsSource.Source = AStarResults.GetCombinedResults(results1, results2);
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultsView)));
				});
			}
		}
		
		#endregion

		
	}
}
