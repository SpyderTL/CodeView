using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeView.Nodes
{
	public class Project : NotableNode
	{
		public byte[] Memory;
		public Dictionary<int, Table> Tables = new Dictionary<int, Table>();
		public Dictionary<int, Variable> Variables = new Dictionary<int, Variable>();
		public Dictionary<int, Function> Functions = new Dictionary<int, Function>();
		public string Description = string.Empty;

		public override object GetProperties()
		{
			return null;
		}

		public override void Reload()
		{
			Nodes.Clear();

			var tables = new TreeNode("Tables");
			var variables = new TreeNode("Variables");
			var functions = new TreeNode("Functions");

			foreach (var table in Tables)
				tables.Nodes.Add(table.Value);

			foreach (var variable in Variables)
				variables.Nodes.Add(variable.Value);

			foreach (var function in Functions)
				functions.Nodes.Add(function.Value);

			Nodes.Add(tables);
			Nodes.Add(variables);
			Nodes.Add(functions);
		}
	}
}
