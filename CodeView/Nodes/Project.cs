using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeView.Nodes
{
	class Project : DataNode
	{
		public string ProjectName;
		public List<Processor> Processors = new List<Processor>();
		public List<Memory> Memory = new List<Memory>();

		public override object GetProperties()
		{
			return new
			{
				ProjectName
			};
		}

		public override void Reload()
		{
			Nodes.Clear();

			Nodes.AddRange(Processors.ToArray());
			Nodes.AddRange(Memory.ToArray());
		}
	}
}
