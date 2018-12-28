using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace FFmpegWrap
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
		}

		private void Window_Drop(object sender, DragEventArgs e)
		{
			String[] files = e.Data.GetData(DataFormats.FileDrop) as String[];
			if (files == null) return;

			DataContext dc= DataContext as DataContext;
			foreach (var file in files)
			{
				var info = VideoAnalysis.Analysis(file);
				if (info == null) continue;
				dc.EditorInfos.Add(info);
			}
		}

		private void MenuItemPreview_Click(object sender, RoutedEventArgs e)
		{
			if (ListBoxJob.SelectedIndex < 0) return;

			VideoAnalysis.Preview(ListBoxJob.SelectedItem as EditorInfo);
		}

		private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
		{
			if (ListBoxJob.SelectedIndex < 0) return;

			DataContext dc = DataContext as DataContext;
			dc.EditorInfos.RemoveAt(ListBoxJob.SelectedIndex);
		}

		private void ButtonOutputPath_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new System.Windows.Forms.FolderBrowserDialog();
			if (dlg.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;

			DataContext dc = DataContext as DataContext;
			dc.OutputPath = dlg.SelectedPath;
		}

		private async void ButtonRun_ClickAsync(object sender, RoutedEventArgs e)
		{
			if (ListBoxJob.Items.Count == 0) return;

			// 出力先チェック
			DataContext dc = DataContext as DataContext;
			if (String.IsNullOrEmpty(dc.OutputPath) || !System.IO.Directory.Exists(dc.OutputPath))
			{
				MessageBox.Show("出力先を指定して下さい");
				return;
			}

			// ファイル重複チェック
			HashSet<String> filenames = new HashSet<String>();
			foreach (var item in ListBoxJob.Items)
			{
				String filename = (item as EditorInfo).Filename;
				if (filenames.Contains(filename))
				{
					MessageBox.Show("ファイル名が重複しています");
					return;
				}
				filenames.Add(filename);
			}

			// エンコード
			if (dc.Jobs.Info().Mode != Job.eMode.Combination)
			{
				foreach (var item in ListBoxJob.Items)
				{
					await VideoAnalysis.Encode(dc.OutputPath, item as EditorInfo);
				}
			}

			// 結合
			if (dc.Jobs.Info().Mode != Job.eMode.Encode)
			{
				if (!String.IsNullOrEmpty(dc.CombinationFile))
				{
					VideoAnalysis.Combination(dc);
				}
			}

			System.Diagnostics.Process.Start(dc.OutputPath);
		}
	}
}
