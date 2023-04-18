using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze.Classes
{
	public class Position : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private int row = -1;
		public int Row
		{
			get { return row; }
			set
			{
				row = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Row)));
			}
		}

		private int column = -1;
		public int Column
		{
			get { return column; }
			set
			{
				column = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Column)));
			}
		}
	}
}
