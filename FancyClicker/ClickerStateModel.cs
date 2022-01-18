using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FancyClicker
{
	internal class ClickerStateModel : INotifyPropertyChanged
	{
		private UInt64 _clickCount;

		public string ClickCount
		{
			get { return String.Format("{0:n0}", _clickCount); }
		}

		public UInt64 incrementClickCount()
		{
			_clickCount++;
			RaisePropertyChanged(nameof(ClickCount));

			return _clickCount;
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void RaisePropertyChanged(string propertyName)
		{
			// take a copy to prevent thread issues
			PropertyChangedEventHandler? handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
