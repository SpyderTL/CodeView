using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeView.Nodes;

namespace CodeView
{
	public partial class BrowserForm : Form
	{
		public BrowserForm()
		{
			InitializeComponent();
			Fonts.Bold = new Font(Font, FontStyle.Bold);

			var project = new Project
			{
				Text = "New Project"
			};

			project.Functions.Add(0xff96, new Function { Text = "_ResetInterruptHandler", Address = 0xff96, Project = project });

			treeView.Nodes.Add(project);

			treeView.SelectedNode = project;

			LoadProject("../../Examples/StarFoxUsa10.cvproj");
			LoadRom("../../Examples/StarFoxUsa10.bin");
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
			if (e.Node is DataNode)
			{
				treeView.BeginUpdate();
				((DataNode)e.Node).Reload();
				treeView.EndUpdate();
			}
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node is DataNode)
			{
				propertyGrid.SelectedObject = ((DataNode)e.Node).GetProperties();
			}
			else
				propertyGrid.SelectedObject = null;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open();
		}

		private void Open()
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (openFileDialog.FileName.EndsWith(".cvproj"))
				{
					saveFileDialog.FileName = openFileDialog.FileName;

					LoadProject(openFileDialog.FileName);
				}
				else
					LoadRom(openFileDialog.FileName);
			}
		}

		private void LoadProject(string fileName)
		{
			using (var reader = File.OpenText(fileName))
			{
				var project = new Project();

				project.Text = reader.ReadLine();
				reader.ReadLine();

				while (true)
				{
					var line = reader.ReadLine();

					if (string.IsNullOrWhiteSpace(line))
						break;

					var address = int.Parse(line, System.Globalization.NumberStyles.HexNumber);
					var name = reader.ReadLine();

					project.Tables.Add(address, new Table { Address = address, Text = name });
				}

				while (true)
				{
					var line = reader.ReadLine();

					if (string.IsNullOrWhiteSpace(line))
						break;

					var address = int.Parse(line, System.Globalization.NumberStyles.HexNumber);
					var name = reader.ReadLine();

					project.Variables.Add(address, new Variable { Address = address, Text = name });
				}

				while (true)
				{
					var line = reader.ReadLine();

					if (string.IsNullOrWhiteSpace(line))
						break;

					var address = int.Parse(line, System.Globalization.NumberStyles.HexNumber);
					var name = reader.ReadLine();
					var flags = byte.Parse(reader.ReadLine(), System.Globalization.NumberStyles.HexNumber);

					project.Functions.Add(address, new Function { Address = address, Text = name, Flags = flags, Project = project });
				}

				treeView.Nodes.Clear();

				treeView.Nodes.Add(project);
				treeView.SelectedNode = project;
				project.Expand();
			}
		}

		private void SaveProject(string fileName)
		{
			var project = treeView.Nodes[0] as Project;

			using (var writer = File.CreateText(fileName))
			{
				writer.WriteLine(project.Text);

				writer.WriteLine();

				foreach (var table in project.Tables)
				{
					writer.WriteLine(table.Key.ToString("X"));
					writer.WriteLine(table.Value.Text);
				}

				writer.WriteLine();

				foreach (var variable in project.Variables)
				{
					writer.WriteLine(variable.Key.ToString("X"));
					writer.WriteLine(variable.Value.Text);
				}

				writer.WriteLine();

				foreach (var function in project.Functions)
				{
					writer.WriteLine(function.Key.ToString("X"));
					writer.WriteLine(function.Value.Text);
					writer.WriteLine(function.Value.Flags.ToString("X"));
				}
			}
		}

		private void LoadRom(string path)
		{
			var project = treeView.SelectedNode as Project;

			if (project == null)
				return;

			project.Memory = new byte[16 * 1024 * 1024];

			using (var stream = File.OpenRead(path))
			{
				for (var bank = 0x00; bank < 0x7E; bank++)
				{
					stream.Position = bank * 0x8000;
					stream.Read(project.Memory, (bank * 0x10000) + 0x8000, 0x8000);
				}

				for (var bank = 0xFE; bank < 0x100; bank++)
				{
					stream.Position = bank * 0x8000;
					stream.Read(project.Memory, (bank * 0x10000) + 0x8000, 0x8000);
				}

				Array.Copy(project.Memory, 0, project.Memory, 0x800000, 0x7E0000);
			}
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			if (e.Node is FunctionPointer)
			{
				var pointer = e.Node as FunctionPointer;

				var project = treeView.Nodes[0] as Project;

				if (!project.Functions.TryGetValue(pointer.Address, out Function function))
				{
					function = new Function { Text = pointer.Address.ToString("X6"), Address = pointer.Address, Project = project, Flags = pointer.Flags };

					project.Functions.Add(pointer.Address, function);

					pointer.Parent.Parent.Nodes.Add(function);
				}
				else
					treeView.SelectedNode = function;
			}
			else if (e.Node is VariablePointer)
			{
				var pointer = e.Node as VariablePointer;

				var project = treeView.Nodes[0] as Project;

				if (!project.Variables.TryGetValue(pointer.Address, out Variable variable))
				{
					variable = new Variable { Text = pointer.Address.ToString("X6"), Address = pointer.Address };

					project.Variables.Add(pointer.Address, variable);

					pointer.Parent.Parent.Parent.Nodes[1].Nodes.Add(variable);
				}
				else
					treeView.SelectedNode = variable;
			}
			else if (e.Node is TablePointer)
			{
				var pointer = e.Node as TablePointer;

				var project = treeView.Nodes[0] as Project;

				if (!project.Tables.TryGetValue(pointer.Address, out Table table))
				{
					table = new Table { Text = pointer.Address.ToString("X6"), Address = pointer.Address };

					project.Tables.Add(pointer.Address, table);

					pointer.Parent.Parent.Parent.Nodes[0].Nodes.Add(table);
				}
				else
					treeView.SelectedNode = table;
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void Save()
		{
			if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
				SaveAs();
			else
				SaveProject(saveFileDialog.FileName);
		}

		private void SaveAs()
		{
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				openFileDialog.FileName = saveFileDialog.FileName;

				SaveProject(saveFileDialog.FileName);
			}
		}

		private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			if (e.Node is Project)
				return;

			if (e.Node is Function)
				return;

			if (e.Node is Variable)
				return;

			e.CancelEdit = true;
		}

		private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Delete();
		}

		private void Delete()
		{
			if (ActiveControl == splitContainer &&
				splitContainer.ActiveControl == treeView)
			{
				if (treeView.SelectedNode is Table)
				{
					var table = treeView.SelectedNode as Table;

					table.Remove();

					var project = treeView.Nodes[0] as Project;

					project.Tables.Remove(table.Address);
				}
				else if (treeView.SelectedNode is Variable)
				{
					var variable = treeView.SelectedNode as Variable;

					variable.Remove();

					var project = treeView.Nodes[0] as Project;

					project.Variables.Remove(variable.Address);
				}
			}
		}

		private void newTableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var table = new Table { Text = "Table", Address = 0 };

			var project = treeView.Nodes[0] as Project;

			project.Tables.Add(table.Address, table);

			project.Nodes[0].Nodes.Add(table);
			treeView.SelectedNode = table;
			table.BeginEdit();
		}

		private void newVariableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var variable = new Variable { Text = "Variable", Address = 0 };

			var project = treeView.Nodes[0] as Project;

			project.Variables.Add(variable.Address, variable);

			project.Nodes[1].Nodes.Add(variable);
			treeView.SelectedNode = variable;
			variable.BeginEdit();
		}

		private void newFunctionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var project = treeView.Nodes[0] as Project;

			var function = new Function { Text = "Function", Address = 0, Flags = 0x00, Project = project };

			project.Functions.Add(function.Address, function);

			project.Nodes[2].Nodes.Add(function);
			treeView.SelectedNode = function;
			function.BeginEdit();
		}
	}
}
