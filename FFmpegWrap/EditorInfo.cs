using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	class EditorInfo
	{
		public String Filename { get; set; }
		public SelectInfo<EncoderInfo> VideoEncoders { get; set; }
			= new SelectInfo<EncoderInfo>()
			{
				Index = 2,
				Items =
				{
					new EncoderInfo{ Encoder = "copy", Option = "" },
					new EncoderInfo{ Encoder = "libx264", Option = "" },
					new EncoderInfo{ Encoder = "libx265", Option = "" },
					new EncoderInfo{ Encoder = "h264_qsv", Option = "" }
				}
			};
		public SelectInfo<EncoderInfo> AudioEncoders { get; set; }
			= new SelectInfo<EncoderInfo>()
			{
				Index = 2,
				Items =
				{
					new EncoderInfo{ Encoder = "copy", Option = "" },
					new EncoderInfo{ Encoder = "ac3", Option = " -b:a 192k" },
					new EncoderInfo{ Encoder = "aac", Option = " -b:a 192k" }
				}
			};
		public SelectInfo<String> Resolutions { get; set; }
			= new SelectInfo<String>() { Items = { "1920x1080", "1280x720", "720x480" } };
		public VideoInfo VideoInfo { get; set; }
		public String StartTime { get; set; }
		public String TotalTime { get; set; }
	}
}
