using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeView.Nodes
{
	class Processor : DataNode
	{
		public override object GetProperties()
		{
			return new
			{
			};
		}

		public override void Reload()
		{
			Nodes.Clear();
		}
	}
}
