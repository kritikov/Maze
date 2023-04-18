using Maze.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
//using static System.Net.Mime.MediaTypeNames;

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

		public Classes.Maze Maze { get; set; }

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

		public Position StartPosition { get; set; } = new Position();
		public Position EndPosition1 { get; set; } = new Position();
		public Position EndPosition2 { get; set; } = new Position();

		private CancellationTokenSource cancellationToken = new CancellationTokenSource();
		public ObservableCollection<string> Results { get; set; } = new ObservableCollection<string>();
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
			System.Windows.Application.Current.Shutdown();
		}

		private void CanvasClick(object sender, MouseButtonEventArgs e)
		{
			Message = "";

			try
			{
				Point point = Mouse.GetPosition(MazeCanvas);
				var selectedSell = Maze.GetCell(point);
				Maze.SelectCell(selectedSell);

                Message = $"cell position: {selectedSell.Row + 1}, {selectedSell.Column + 1}";

				if (Maze.StartCell != null)
				{
					double h = Maze.HeuristicDistance(Maze.StartCell, selectedSell);
					Message += $", euristic distance to selected cell: {h}";
				}

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
				//MazeCanvas.Width = MazeScroller.ActualWidth - 17;
				//MazeCanvas.Height = MazeScroller.ActualHeight - 17;

				this.Maze.Construct();
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
				if (Maze.Cells != null)
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
				ValidateParameters();
				this.Maze.Construct();
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
				AlgorithmIsRunning = true;

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
			Message = "";

			try
			{
				if (Maze.N < 5 || Maze.N > 50)
					throw new Exception("The dimension of the maze are out of limits, the N must be between 5 and 50");

				if (Maze.BlockedPossibility < 0 || Maze.BlockedPossibility > 1)
					throw new Exception("he possibility of a cell to be blocked is out of limits, the P must be between 0 and 1");


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
            if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
            {
                this.StartPosition.Row = (int)Maze.SelectedCell?.Row;
                this.StartPosition.Column = (int)Maze.SelectedCell?.Column;
                Maze.SelectStartCell(Maze.SelectedCell);
            }
            else
            {
                throw new Exception("Select a valid cell");
            }
        }

        /// <summary>
        /// Select the end 1 position
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SelectEnd1Cell()
        {
            if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
            {
                this.EndPosition1.Row = (int)Maze.SelectedCell?.Row;
                this.EndPosition1.Column = (int)Maze.SelectedCell?.Column;
                Maze.SelectEnd1Cell(Maze.SelectedCell);
            }
            else
            {
                throw new Exception("Select a valid cell");
            }
        }

        /// <summary>
        /// Select the end 2 position
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void SelectEnd2Cell()
        {
            if (Maze.SelectedCell != null && Maze.SelectedCell.Type == CellType.Free)
            {
                this.EndPosition2.Row = (int)Maze.SelectedCell?.Row;
                this.EndPosition2.Column = (int)Maze.SelectedCell?.Column;
                Maze.SelectEnd2Cell(Maze.SelectedCell);
            }
            else
            {
                throw new Exception("Select a valid cell");
            }
        }

		/// <summary>
		/// Solve the initial state using the A* algorithm
		/// </summary>
		/// <returns></returns>
		private async Task ASTARAnalysis()
		{
			try
			{
				cancellationToken = new CancellationTokenSource();

				await Task.Run(() =>
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						Logs.Clear();
					});

					// TODO: temporary search manually in the the first end
					if (Maze.StartCell != null && Maze.End1Cell != null)
					{

						State initialState = new State(Maze.StartCell, Maze.End1Cell);
						Results = State.ASTARAnalysis(initialState, cancellationToken.Token);

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
					CommandManager.InvalidateRequerySuggested();
				});
			}
		}
		#endregion
	}
}
