using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	class VideoAnalysis
	{
		private const String Extension = ".mp4";
		public static EditorInfo Analysis(String filename)
		{
			if (!System.IO.File.Exists(filename)) return null;

			var psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = "ffmpeg.exe";
			psInfo.Arguments = "-i " + filename;
			psInfo.CreateNoWindow = true;
			psInfo.UseShellExecute = false;
			psInfo.RedirectStandardError = true;

			String output = System.Diagnostics.Process.Start(psInfo).StandardError.ReadToEnd();
			output = output.Replace("\r\n", "\n");

			VideoInfo vInfo = new VideoInfo();
			vInfo.FileName = filename;
			foreach (String oneline in output.Split('\n'))
			{
				int index = oneline.IndexOf("    Stream");
				if (index < 0) continue;

				index = oneline.IndexOf(":", index);
				if (index < 0) continue;

				index++;
				StreamInfo stInfo = new StreamInfo();
				for (; index < oneline.Length; index++)
				{
					int tmp;
					if (int.TryParse(oneline[index].ToString(), out tmp))
					{
						stInfo.Channel = stInfo.Channel * 10 + tmp;
					}
					else break;
				}

				index = oneline.IndexOf(":", index);
				if (index < 0) continue;
				String type = oneline.Substring(index + 2, oneline.IndexOf(":", index + 1) - index - 2);

				index = oneline.IndexOf(":", index + 1);
				if (index < 0) continue;
				stInfo.Name = oneline.Substring(index + 2);

				switch (type)
				{
					case "Video":
						vInfo.VideoStreams.Items.Add(stInfo);
						break;

					case "Audio":
						vInfo.AudioStreams.Items.Add(new ChoiceInfo<StreamInfo>() { Item = stInfo });
						break;

					case "Subtitle":
						vInfo.SubTitleStreams.Items.Add(new ChoiceInfo<StreamInfo>() { Item = stInfo });
						break;
				}
			}

			EditorInfo info = new EditorInfo();
			info.VideoInfo = vInfo;
			info.Filename = System.IO.Path.GetFileNameWithoutExtension(filename);
			return info;
		}

		public static void Preview(EditorInfo info)
		{
			var psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = "ffplay.exe";
			psInfo.Arguments = "-i " + info.VideoInfo.FileName
				+ " -vst " + info.VideoInfo.VideoStreams.Info().Channel
				+ " -ast " + info.VideoInfo.AudioStreams.Info().Item.Channel;

			if (!String.IsNullOrEmpty(info.StartTime))
			{
				psInfo.Arguments += " -ss " + info.StartTime;
			}
			if (!String.IsNullOrEmpty(info.TotalTime))
			{
				psInfo.Arguments += " -t " + info.TotalTime;
			}

			System.Diagnostics.Process.Start(psInfo);
		}

		public static async Task Encode(String output, EditorInfo info)
		{
			var psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = "ffmpeg.exe";
			StringBuilder args = new StringBuilder();

			args.Append("-y -vsync 1 -i " + info.VideoInfo.FileName
				+ " -c:v " + info.VideoEncoders.Info().Encoder
				+ " -map 0:" + info.VideoInfo.VideoStreams.Info().Channel);

			// デフォルトのオーディオ優先する
			List<StreamInfo> audios = new List<StreamInfo>();
			if(info.VideoInfo.AudioStreams.Items[info.VideoInfo.AudioStreams.Index].Enable == true)
			{
				audios.Add(info.VideoInfo.AudioStreams.Items[info.VideoInfo.AudioStreams.Index].Item);
			}
			foreach (var audio in info.VideoInfo.AudioStreams.Items)
			{
				if (audio.Enable == false) continue;
				if (audios.Count > 0 && audios[0] == audio.Item) continue;
				audios.Add(audio.Item);
			}
			foreach (var audio in audios)
			{
				args.Append(" -c:a " + info.AudioEncoders.Info().Encoder + info.AudioEncoders.Info().Option
					+ " -map 0:" + audio.Channel);
			}

			// 字幕操作したいけれど方法わからん、、、、
			/*
			foreach (var subtitle in info.VideoInfo.SubTitleStreams.Items)
			{
				if (subtitle.Enable == false) continue;
				args.Append(" -c:s mov_text");
			}
			*/

			args.Append(" -s " + info.Resolutions.Info() + " -aspect 16:9");
			if (!String.IsNullOrEmpty(info.StartTime))
			{
				args.Append(" -ss " + info.StartTime);
			}
			if (!String.IsNullOrEmpty(info.TotalTime))
			{
				args.Append(" -t " + info.TotalTime);
			}

			args.Append(" " + System.IO.Path.Combine(output, info.Filename) + Extension);

			psInfo.Arguments = args.ToString();
			await Task.Run(() => System.Diagnostics.Process.Start(psInfo).WaitForExit());
		}

		public static void Combination(DataContext dc)
		{
			using (var writer = new System.IO.StreamWriter("files.txt"))
			{
				foreach (var info in dc.EditorInfos)
				{
					String filename = System.IO.Path.Combine(dc.OutputPath, (info as EditorInfo).Filename);
					filename = filename.Replace('\\', '/');
					writer.WriteLine("file " + "'" + filename + Extension + "'");
				}
			}

			var psInfo = new System.Diagnostics.ProcessStartInfo();
			psInfo.FileName = "ffmpeg.exe";
			psInfo.Arguments = "-safe 0 -f concat -i files.txt -c copy " + System.IO.Path.Combine(dc.OutputPath, dc.CombinationFile) + Extension;
			System.Diagnostics.Process.Start(psInfo);
		}
	}
}
