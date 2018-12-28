using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace FFmpegWrap
{
	class DataContext : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		public ObservableCollection<EditorInfo> EditorInfos { get; set; } = new ObservableCollection<EditorInfo>();
		public String OutputPath
		{
			get { return mOutputPath; }
			set
			{
				mOutputPath = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputPath)));
			}
		}

		public String CombinationFile { get; set; }
		public SelectInfo<Job> Jobs { get; set; } = new SelectInfo<Job>
		{
			Index = 0,
			Items =
			{
				new Job() { Mode = Job.eMode.Encode, Discription = "Encode" },
				new Job() { Mode = Job.eMode.Combination, Discription = "Combination" },
				new Job() { Mode = Job.eMode.ALL, Discription = "ALL" }
			}
		};

		private String mOutputPath;
	}
}
