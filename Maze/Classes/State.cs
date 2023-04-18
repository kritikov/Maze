using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Maze.Classes
{
	public class State
	{
		#region VARIABLES

		public State? Parent = null;
		public double f = 0;
		public double g = 0;
		public double h = 0;
		public double Weight = 0;
		public int Row;
		public int Column;
		public string DisplayValue
		{
			get { return this.ToString(); }
		}
		#endregion


		#region CONSTRUCTORS

		public State()
		{
		}

		public State(int startRow, int startColumn)
		{
			Row = startRow;
			Column = startColumn;
		}

		#endregion


		#region METHODS

		public override string ToString()
		{
			string result = $"Position = ({Row}, {Column})";
			return result;
		}

		/// <summary>
		/// Check if a state has not appears before in its path
		/// </summary>
		/// <returns></returns>
		public bool IsUniqueDescendant()
		{
			State? parent = this.Parent;

			while (parent != null)
			{
				if (parent.Row == this.Row && parent.Column == this.Column)
					return false;

				parent = parent.Parent;
			}

			return true;
		}

		/// <summary>
		/// Get a list with all states in the path from the root to the current state
		/// </summary>
		/// <returns></returns>
		public List<State> GetPath()
		{
			List<State> states = new List<State>()
			{
				this
			};

			State? parent = this.Parent;
			while (parent != null)
			{
				states.Add(parent);
				parent = parent.Parent;
			}

			states.Reverse();

			return states;
		}

		/// <summary>
		/// Create a new state that is child of this state. The child has an index that splits its list of numbers at two parts.
		/// </summary>
		/// <param name="splitIndex"></param>
		/// <returns></returns>
		public State GetChild(int row, int column, double cost)
		{
			State childState = new State();
			childState.Parent = this;
			childState.Row = row;
			childState.Column = column;
			childState.Weight = cost;
			childState.g = this.g + cost;

			return childState;
		}

		/// <summary>
		/// The euritic function that calculates the cost between two cells
		/// </summary>
		/// <param name="startCell"></param>
		/// <param name="endCell"></param>
		/// <returns></returns>
		public static double HeuristicDistance(int startRow, int startColumn, int endRow, int endColumn)
		{
			double h = Math.Abs(endColumn - startColumn) * 0.5 + Math.Abs(endRow - startRow) * 1;

			return h;
		}

		/// <summary>
		/// Return the best state from a list of states
		/// </summary>
		/// <param name="states"></param>
		/// <returns></returns>
		public static State? GetBestState(List<State> states)
		{
			State? bestState;

			try
			{
				if (states.Count == 0)
					return null;
				else
				{
					bestState = states[0];
					foreach (var state in states)
					{
						if (state.f < bestState.f)
							bestState = state;
					}
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}

			return bestState;
		}

		/// <summary>
		/// Analyze a state using the A* algorithm
		/// </summary>
		public static ObservableCollection<string> ASTARAnalysis(State initialState, Maze maze, int endRow, int endColumn, CancellationToken cancellationToken)
		{
			ObservableCollection<string> results = new ObservableCollection<string>();
			var watch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				List<State> openStates = new List<State>();
				State? finalState = null;
				ulong statesOpened = 1;

				// open the initial state
				openStates.Add(initialState);
				State selectedState = initialState;
				Logs.Write($"**** Analyzing initial state {initialState.DisplayValue} with A* algorithm ****");
				Logs.Write($"Initial state: {initialState.DisplayValue} with g={initialState.g}, h={initialState.h}, f={initialState.f}");

				// if the initial state is the same as the destination then is the state we search
				if (initialState.Row == endRow && initialState.Column == endColumn)
					finalState = initialState;

				// evaluate the initial state
				initialState.g = initialState.Weight;
				initialState.h = HeuristicDistance(initialState.Row, initialState.Column, endRow, endColumn);
				initialState.f = initialState.g;

				while (finalState == null && openStates.Count != 0)
				{
					// stop the process if the user has cancel it
					cancellationToken.ThrowIfCancellationRequested();

					// open the childs of the selected state

					// left move child
					var rightCell = maze.GetCell(selectedState.Row, selectedState.Column + 1);
					if (rightCell != null && rightCell.Type != CellType.Blocked)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(rightCell.Row, rightCell.Column, 0.5);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.g = childState.Parent.g + childState.Weight;
							childState.h = HeuristicDistance(childState.Row, childState.Column, endRow, endColumn);
							childState.f = childState.g + childState.h;

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// down move child
					var downCell = maze.GetCell(selectedState.Row+1, selectedState.Column);
					if (downCell != null && downCell.Type != CellType.Blocked)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(downCell.Row, downCell.Column, 1);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.g = childState.Parent.g + childState.Weight;
							childState.h = HeuristicDistance(childState.Row, childState.Column, endRow, endColumn);
							childState.f = childState.g + childState.h;

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// left move child
					var leftCell = maze.GetCell(selectedState.Row, selectedState.Column - 1);
					if (leftCell != null && leftCell.Type != CellType.Blocked)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(leftCell.Row, leftCell.Column, 0.5);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.g = childState.Parent.g + childState.Weight;
							childState.h = HeuristicDistance(childState.Row, childState.Column, endRow, endColumn);
							childState.f = childState.g + childState.h;

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// up move child
					var upCell = maze.GetCell(selectedState.Row - 1, selectedState.Column);
					if (upCell != null && upCell.Type != CellType.Blocked)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(upCell.Row, upCell.Column, 1);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.g = childState.Parent.g + childState.Weight;
							childState.h = HeuristicDistance(childState.Row, childState.Column, endRow, endColumn);
							childState.f = childState.g + childState.h;

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// remove the current state from the opened states
					openStates.Remove(selectedState);
					Logs.Write($"closing state {selectedState?.DisplayValue}");

					// if there are no open states then exit
					if (openStates.Count == 0)
						break;

					// keep the unique opened states with the best f
					openStates = openStates.GroupBy(state => state.DisplayValue)
						.Select(state => state.OrderBy(state => state.f)
						.First())
						.ToList();

					// select the opened state with the best f
					selectedState = GetBestState(openStates);
					Logs.Write($"new selected state {selectedState?.DisplayValue} with g={selectedState?.g}, h={selectedState?.h}, f={selectedState?.f}");

					// check if the selected state is what we are searching for
					if (selectedState.Row == endRow && selectedState.Column == endColumn)
					{
						finalState = selectedState;
						Logs.Write($"found final state {finalState?.DisplayValue} with g={finalState?.g}, h={finalState?.h}, f={finalState?.f}");
					}
				}

				Logs.Write($"Analyzing initial state {initialState.DisplayValue} with A* algorithm ended");

				// if a solution is found
				if (finalState != null)
				{
					results.Add($"Final state found: {finalState.DisplayValue} with g={selectedState?.g}, h={selectedState?.h}, f={selectedState?.f}");
					results.Add($"Final cost = {selectedState?.g}");
					results.Add($"States opened: {statesOpened}");
					results.Add($"Total time: {watch.ElapsedMilliseconds} ms");

					List<State> statesInPath = finalState.GetPath();

					results.Add($"Path until final state found: ");
					foreach (var state in statesInPath)
					{
						if (state.Parent == null)
							results.Add($"initial state {state?.DisplayValue} with g={state?.g}, h={state?.h}, f={state?.f}");
						else
							results.Add($"move at state {state?.DisplayValue} with g={state?.g}, h={state?.h}, f={state?.f}");
					}

					// write the results in the log
				}
				else
				{
					results.Add($"No final state found");

				}

				foreach(var message in results)
				{
					Logs.Write(message);
				}

				return results;
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				watch.Stop();
			}
		}

		#endregion
	}

}
