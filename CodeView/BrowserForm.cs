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
using System.Xml.Linq;
using CodeView.Nodes;

namespace CodeView
{
	public partial class BrowserForm : Form
	{
		//Stack<TreeNode> History = new Stack<TreeNode>();

		public BrowserForm()
		{
			InitializeComponent();
		}

		private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			//if (e.Node is DataNode)
			//{
			//	TreeView.BeginUpdate();
			//	((DataNode)e.Node).Reload();
			//	TreeView.EndUpdate();
			//}
		}

		private void treeView_AfterExpand(object sender, TreeViewEventArgs e)
		{
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//if (e.Node is DataNode)
			//{
			//	PropertyGrid.SelectedObject = ((DataNode)e.Node).GetProperties();
			//}
			//else
			//	PropertyGrid.SelectedObject = null;

			//if (e.Node is NotableNode)
			//{
			//	NotesTextBox.Text = ((NotableNode)e.Node).Notes;
			//	NotesTextBox.Enabled = true;
			//}
			//else
			//{
			//	NotesTextBox.Text = string.Empty;
			//	NotesTextBox.Enabled = false;
			//}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Open();
		}

		private void Open()
		{
			//if (openFileDialog.ShowDialog() == DialogResult.OK)
			//{
			//	if (openFileDialog.FileName.EndsWith(".cvproj"))
			//	{
			//		saveFileDialog.FileName = openFileDialog.FileName;

			//		LoadProject(openFileDialog.FileName);
			//	}
			//	else
			//		LoadRom(openFileDialog.FileName);
			//}
		}

		private void LoadProject(string fileName)
		{
			//var document = XDocument.Load(fileName);

			//var project = new Project
			//{
			//	Text = document.Root.Attribute("Name").Value,
			//	Description = document.Root.Attribute("Description").Value,
			//	Notes = document.Root.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty,
			//};

			//project.Tables = document.Root.Element("Tables").Elements("Table").Select(x => new Table
			//{
			//	Text = x.Attribute("Name").Value,
			//	Description = x.Attribute("Description").Value,
			//	Address = int.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
			//	Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty,
			//	Project = project
			//})
			//.OrderBy(x => x.Text)
			//.ToDictionary(x => x.Address);

			//project.Functions = document.Root.Element("Functions").Elements("Function").Select(x => new Function
			//{
			//	Text = x.Attribute("Name").Value,
			//	Description = x.Attribute("Description").Value,
			//	Address = int.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
			//	Flags = byte.Parse(x.Attribute("Flags").Value, System.Globalization.NumberStyles.HexNumber),
			//	Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty,
			//	Project = project
			//})
			//.OrderBy(x => x.Text)
			//.ToDictionary(x => x.Address);

			//project.Variables = document.Root.Element("Variables").Elements("Variable").Select(x => new Variable
			//{
			//	Text = x.Attribute("Name").Value,
			//	Description = x.Attribute("Description").Value,
			//	Address = int.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
			//	Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty,
			//	Project = project
			//})
			//.OrderBy(x => x.Text)
			//.ToDictionary(x => x.Address);
			
			//TreeView.Nodes.Clear();

			//TreeView.Nodes.Add(project);
			//TreeView.SelectedNode = project;
			//project.Expand();
		}

		private void SaveProject(string fileName)
		{
			//var project = TreeView.Nodes[0] as Project;

			//var document = new XDocument();

			//var root = new XElement("Project");

			//root.Add(new XAttribute("Name", project.Text));
			//root.Add(new XAttribute("Description", project.Description));
			//root.Add(new XText(project.Notes));

			//document.Add(root);

			//var tables = new XElement("Tables");
			//var variables = new XElement("Variables");
			//var functions = new XElement("Functions");

			//tables.Add(project.Tables
			//	.Select(x => new XElement("Table",
			//		new XAttribute("Name", x.Value.Text),
			//		new XAttribute("Description", x.Value.Description),
			//		new XAttribute("Address", x.Value.Address.ToString("X")),
			//		new XText(x.Value.Notes))));

			//variables.Add(project.Variables
			//	.Select(x => new XElement("Variable",
			//		new XAttribute("Name", x.Value.Text),
			//		new XAttribute("Description", x.Value.Description),
			//		new XAttribute("Address", x.Value.Address.ToString("X")),
			//		new XText(x.Value.Notes))));

			//functions.Add(project.Functions
			//	.Select(x => new XElement("Function",
			//		new XAttribute("Name", x.Value.Text),
			//		new XAttribute("Description", x.Value.Description),
			//		new XAttribute("Address", x.Value.Address.ToString("X")),
			//		new XAttribute("Flags", x.Value.Flags.ToString("X")),
			//		new XText(x.Value.Notes))));

			//root.Add(tables, variables, functions);

			//document.Save(fileName);
		}

		private void LoadRom(string path)
		{
			//var project = TreeView.SelectedNode as Project;

			//if (project == null)
			//	return;

			//project.Memory = new byte[16 * 1024 * 1024];

			//using (var stream = File.OpenRead(path))
			//{
			//	for (var bank = 0x00; bank < 0x7E; bank++)
			//	{
			//		stream.Position = bank * 0x8000;
			//		stream.Read(project.Memory, (bank * 0x10000) + 0x8000, 0x8000);
			//	}

			//	for (var bank = 0xFE; bank < 0x100; bank++)
			//	{
			//		stream.Position = bank * 0x8000;
			//		stream.Read(project.Memory, (bank * 0x10000) + 0x8000, 0x8000);
			//	}

			//	Array.Copy(project.Memory, 0, project.Memory, 0x800000, 0x7E0000);
			//}
		}

		private void treeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			//if (e.Node is FunctionPointer)
			//{
			//	var pointer = e.Node as FunctionPointer;

			//	var project = TreeView.Nodes[0] as Project;

			//	if (!project.Functions.TryGetValue(pointer.Address, out Function function))
			//	{
			//		function = new Function { Text = pointer.Address.ToString("X6"), Address = pointer.Address, Project = project, Flags = pointer.Flags };

			//		project.Functions.Add(pointer.Address, function);

			//		pointer.Parent.Parent.Nodes.Add(function);
			//	}
			//	else
			//	{
			//		History.Push(e.Node);

			//		TreeView.SelectedNode = function;
			//	}
			//}
			//else if (e.Node is VariablePointer)
			//{
			//	var pointer = e.Node as VariablePointer;

			//	var project = TreeView.Nodes[0] as Project;

			//	if (!project.Variables.TryGetValue(pointer.Address, out Variable variable))
			//	{
			//		variable = new Variable { Text = pointer.Address.ToString("X6"), Address = pointer.Address, Project = project };

			//		project.Variables.Add(pointer.Address, variable);

			//		pointer.Parent.Parent.Parent.Nodes[1].Nodes.Add(variable);
			//	}
			//	else
			//	{
			//		History.Push(e.Node);

			//		TreeView.SelectedNode = variable;
			//	}
			//}
			//else if (e.Node is TablePointer)
			//{
			//	var pointer = e.Node as TablePointer;

			//	var project = TreeView.Nodes[0] as Project;

			//	if (!project.Tables.TryGetValue(pointer.Address, out Table table))
			//	{
			//		table = new Table { Text = pointer.Address.ToString("X6"), Address = pointer.Address, Project = project };

			//		project.Tables.Add(pointer.Address, table);

			//		pointer.Parent.Parent.Parent.Nodes[0].Nodes.Add(table);
			//	}
			//	else
			//	{
			//		History.Push(e.Node);

			//		TreeView.SelectedNode = table;
			//	}
			//}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//Save();
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//SaveAs();
		}

		private void Save()
		{
			//if (string.IsNullOrWhiteSpace(saveFileDialog.FileName))
			//	SaveAs();
			//else
			//	SaveProject(saveFileDialog.FileName);
		}

		private void SaveAs()
		{
			//if (saveFileDialog.ShowDialog() == DialogResult.OK)
			//{
			//	switch (Path.GetExtension(saveFileDialog.FileName))
			//	{
			//		case ".cvproj":
			//			openFileDialog.FileName = saveFileDialog.FileName;

			//			SaveProject(saveFileDialog.FileName);
			//			break;

			//		case ".txt":
			//			SaveText(saveFileDialog.FileName);
			//			break;
			//	}
			//}
		}

		private void SaveText(string fileName)
		{
			//using (var writer = File.CreateText(fileName))
			//{
			//	var node = TreeView.SelectedNode;

			//	foreach (var child in node.Nodes.OfType<TreeNode>())
			//		writer.WriteLine(child.Text);

			//	writer.Flush();
			//	writer.Close();
			//}
		}

		private void treeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			//if (e.Node is Project)
			//	return;

			//if (e.Node is Table)
			//	return;

			//if (e.Node is Function)
			//	return;

			//if (e.Node is Variable)
			//	return;

			//e.CancelEdit = true;
		}

		private void treeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
		}


		private void treeView_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.KeyCode == Keys.Delete)
			//	Delete();

			//if (e.KeyCode == Keys.OemMinus &&
			//	e.Control &&
			//	History.Count != 0)
			//	TreeView.SelectedNode = History.Pop();
		}

		private void Delete()
		{
			//if (TreeView.SelectedNode is Table)
			//{
			//	var table = TreeView.SelectedNode as Table;

			//	table.Remove();

			//	var project = TreeView.Nodes[0] as Project;

			//	project.Tables.Remove(table.Address);
			//}
			//else if (TreeView.SelectedNode is Variable)
			//{
			//	var variable = TreeView.SelectedNode as Variable;

			//	variable.Remove();

			//	var project = TreeView.Nodes[0] as Project;

			//	project.Variables.Remove(variable.Address);
			//}
			//else if (TreeView.SelectedNode is Function)
			//{
			//	var function = TreeView.SelectedNode as Function;

			//	function.Remove();

			//	var project = TreeView.Nodes[0] as Project;

			//	project.Functions.Remove(function.Address);
			//}
		}

		private void newTableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//var project = TreeView.Nodes[0] as Project;

			//var table = new Table { Text = "Table", Address = 0, Project = project };

			//project.Tables.Add(table.Address, table);

			//project.Nodes[0].Nodes.Add(table);
			//TreeView.SelectedNode = table;
			//table.BeginEdit();
		}

		private void newVariableToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//var variable = new Variable { Text = "Variable", Address = 0 };

			//var project = TreeView.Nodes[0] as Project;

			//project.Variables.Add(variable.Address, variable);

			//project.Nodes[1].Nodes.Add(variable);
			//TreeView.SelectedNode = variable;
			//variable.BeginEdit();
		}

		private void newFunctionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//var project = TreeView.Nodes[0] as Project;

			//var function = new Function { Text = "Function", Address = 0, Flags = 0x00, Project = project };

			//project.Functions.Add(function.Address, function);

			//project.Nodes[2].Nodes.Add(function);
			//TreeView.SelectedNode = function;
			//function.BeginEdit();
		}

		private void notesTextBox_TextChanged(object sender, EventArgs e)
		{
			//var notable = TreeView.SelectedNode as NotableNode;

			//if (notable == null)
			//	return;

			//notable.Notes = NotesTextBox.Text;
		}
	}
}
