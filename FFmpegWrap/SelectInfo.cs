using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	// 複数から単一選択
	class SelectInfo<Type>
		where Type : class
	{
		public int Index { get; set; }
		public List<Type> Items { get; set; } = new List<Type>();

		public Type Info()
		{
			return Items[Index];
		}
	}
}
