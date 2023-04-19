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
		public Cell Cell;
		public Cell Destination;

		public string DisplayValue
		{
			get { return this.ToString(); }
		}
		
		#endregion


		#region CONSTRUCTORS

		public State()
		{
		}

		public State(Cell cell, Cell destinationCell)
		{
			Cell = cell;
			Destination = destinationCell;
		}

		#endregion


		#region METHODS

		public override string ToString()
		{
			string result = $"({Cell.Row}, {Cell.Column})";
			return result;
		}

		/// <summary>
		/// Returns true if the state is final
		/// </summary>
		/// <returns></returns>
		public bool IsFinal()
		{
			if (Cell == Destination)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Check if a state has not appears before in its path
		/// </summary>
		/// <returns></returns>
		public bool IsUniqueDescendant()
		{
			State? parent = this.Parent;

			// TODO: search by CurrentCell
			while (parent != null)
			{
				if (parent.Cell.Row == this.Cell.Row && parent.Cell.Column == this.Cell.Column)
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
		public State GetChild(Cell cell, double cost)
		{
			State childState = new State();
			childState.Parent = this;
			childState.Cell = cell;
			childState.Destination = Destination;
			childState.Weight = cost;
			childState.g = this.g + cost;

			return childState;
		}

		/// <summary>
		/// Evaluate the state using the heuristic function
		/// </summary>
		public void Evaluate()
		{
			if (Parent != null)
				g = Parent.g + Weight;

			h = HeuristicDistance(Cell.Row, Cell.Column, Destination.Row, Destination.Column);
			f = g + h;
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
				bestState = states.MinBy(p => p.f);
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
		public static AStarResults ASTARAnalysis(State initialState, CancellationToken cancellationToken)
		{
			AStarResults results = new AStarResults(initialState);
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
				if (initialState.IsFinal())
					finalState = initialState;

				// evaluate the initial state
				initialState.Evaluate();

				while (finalState == null && openStates.Count != 0)
				{
					// stop the process if the user has cancel it
					cancellationToken.ThrowIfCancellationRequested();

					// get child state when moving right
					var rightCell = selectedState.Cell.GetSibling(Direction.Right);
					if (rightCell != null)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(rightCell, 0.5);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.Evaluate();

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// get child state when moving down
					var downCell = selectedState.Cell.GetSibling(Direction.Down);
					if (downCell != null)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(downCell, 1);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.Evaluate();

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// get child state when moving left
					var leftCell = selectedState.Cell.GetSibling(Direction.Left);
					if (leftCell != null)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(leftCell, 0.5);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.Evaluate();

							// add it to list with the opened states
							openStates.Add(childState);
							statesOpened++;
							Logs.Write($"opening child {childState.DisplayValue} with g={childState.g}, h={childState.h}, f={childState.f}");
						}
					}

					// get child state when moving up
					var upCell = selectedState.Cell.GetSibling(Direction.Up);
					if (upCell != null)
					{
						// create the child state with the proper cost
						State childState = selectedState.GetChild(upCell, 1);

						// if the child hasnt appears before in its anchestors then use it
						if (childState.IsUniqueDescendant())
						{
							// evaluate child
							childState.Evaluate();

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
					if (selectedState.IsFinal())
					{
						finalState = selectedState;
						Logs.Write($"found final state {finalState?.DisplayValue} with g={finalState?.g}, h={finalState?.h}, f={finalState?.f}");
					}
				}

				Logs.Write($"Analyzing initial state {initialState.DisplayValue} with A* algorithm ended");

				// if a solution is found
				if (finalState != null)
				{
					results.FinalState = finalState;
					results.TotalTime = watch.ElapsedMilliseconds;
					results.StatesOpened = statesOpened;
				}

				// write the results in the log
				var messages = results.GetResults();
				foreach (var message in messages)
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
