using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
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
		/// Get the analytic results of the analysis as a list of strings
		/// </summary>
		public List<string> GetResults()
		{
			List<string> messages = new List<string>();

			messages.Add($"Searching path from {InitialState.Cell} to {InitialState.Destination}");
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

		/// <summary>
		/// Return the summirized results as a list with strings
		/// </summary>
		/// <returns></returns>
		public List<string> GetSumResults()
		{
			List<string> messages = new List<string>();

			if (FinalState == null)
			{
				messages.Add($"No path found from {InitialState.Cell} to {InitialState.Destination}");
			}
			else
			{
				messages.Add($"Path found from {InitialState.Cell} to {InitialState.Destination}");
				messages.Add($"Cost of this path = {FinalState?.g}");
				messages.Add($"States opened: {StatesOpened}");
				messages.Add($"Search time: {TotalTime} ms");
			}

			messages.Add("");

			return messages;
		}

		/// <summary>
		/// Return a list with the path to the destination
		/// </summary>
		/// <returns></returns>
		public List<string> GetPathResults()
		{
			List<string> path = new List<string>();

			if (FinalState != null)
			{
				List<State> statesInPath = FinalState.GetPath();

				foreach (var state in statesInPath)
				{
					if (state.Parent != null)
						path.Add($"move at state {state?.Cell} with g={state?.g}, h={state?.h}, f={state?.f}");
				}
			}

			return path;
		}
	
		/// <summary>
		/// Get the combined results from two other results
		/// </summary>
		/// <param name="results1"></param>
		/// <param name="results2"></param>
		/// <returns></returns>
		public static List<string> GetCombinedResults(AStarResults results1, AStarResults results2)
		{
			List<string> result = new List<string>();

			result.Add($"As first destination was chosen the {results1.InitialState.Destination} and as second the {results2.InitialState.Destination}");
			result.Add("");
			result.Add($"Total cost= {results1?.FinalState?.g + results2?.FinalState?.g}");
			result.Add($"Total states opened: {results1?.StatesOpened + results2?.StatesOpened}");
			result.Add($"Total search time: {results1?.TotalTime + results2?.TotalTime} ms");
			result.Add("");

			if (results1 != null)
				result.AddRange(results1.GetSumResults());

			if (results2 != null)
				result.AddRange(results2.GetSumResults());

			result.Add("Path found:");

			if (results1 != null)
				result.AddRange(results1.GetPathResults());

			if (results2 != null)
				result.AddRange(results2.GetPathResults());

			return result;
		}
	
	}
}
