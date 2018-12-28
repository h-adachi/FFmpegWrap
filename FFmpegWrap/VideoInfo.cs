using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	class VideoInfo
	{
		public String FileName { get; set; }
		public SelectInfo<StreamInfo> VideoStreams { get; set; } = new SelectInfo<StreamInfo>();
		public SelectInfo<ChoiceInfo<StreamInfo>> AudioStreams { get; set; } = new SelectInfo<ChoiceInfo<StreamInfo>>();
		public SelectInfo<ChoiceInfo<StreamInfo>> SubTitleStreams { get; set; } = new SelectInfo<ChoiceInfo<StreamInfo>>();
	}
}
