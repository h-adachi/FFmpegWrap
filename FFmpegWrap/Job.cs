using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	class Job
	{
		public enum eMode
		{
			Encode,
			Combination,
			ALL,
		}

		public eMode Mode { get; set; }
		public String Discription { get; set; }
	}
}
