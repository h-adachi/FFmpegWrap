using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFmpegWrap
{
	// 複数から複数選択
    class ChoiceInfo<Type>
		where Type : class
    {
		public bool Enable { get; set; }
		public Type Item { get; set; }
    }
}
