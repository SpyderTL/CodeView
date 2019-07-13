using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeView.Nodes
{
	class Memory : DataNode
	{
		public byte[] Data;

		public override object GetProperties()
		{
			return new
			{
				Size = Data == null ? -1 : Data.Length
			};
		}

		public override void Reload()
		{
			Nodes.Clear();
		}
	}
}
