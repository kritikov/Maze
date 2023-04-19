using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Classes
{
	public class AStarResults
	{
		public State InitialState;
		public State? FinalState;
		public long TotalTime = 0;
		public ulong StatesOpened = 0;

		public AStarResults(State initialState)
		{
			InitialState = initialState;
		}

		/// <summary>
		/// Get the results of the analysis as a list of strings
		/// </summary>
		public List<string> GetResults()
		{
			List<string> messages = new List<string>();

			messages.Add($"Searching path from ({InitialState.Cell}) to {InitialState.Destination}");
			if (FinalState != null)
			{
				messages.Add($"Path found with final state: {FinalState.DisplayValue} with g={FinalState?.g}, h={FinalState?.h}, f={FinalState?.f}");
				messages.Add($"Total cost = {FinalState?.g}");
				messages.Add($"States opened: {StatesOpened}");
				messages.Add($"Total time: {TotalTime} ms");

				List<State> statesInPath = FinalState.GetPath();

				messages.Add($"Path until reach destination: ");
				foreach (var state in statesInPath)
				{
					if (state.Parent == null)
						messages.Add($"initial state {state?.Cell} with g={state?.g}, h={state?.h}, f={state?.f}");
					else
						messages.Add($"move at state {state?.Cell} with g={state?.g}, h={state?.h}, f={state?.f}");
				}
			}
			else
			{
				messages.Add($"No path found");
			}

			return messages;
		}
	}
}
