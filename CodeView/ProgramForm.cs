using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CodeView
{
	public static class ProgramForm
	{
		public static BrowserForm Form;

		public static void Load()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			Form = new BrowserForm();

			Form.NewToolStripMenuItem.Click += NewToolStripMenuItem_Click;
			Form.OpenToolStripMenuItem.Click += OpenToolStripMenuItem_Click;
			Form.SaveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
			Form.SaveAsToolStripMenuItem.Click += SaveAsToolStripMenuItem_Click;

			Fonts.Bold = new Font(Form.Font, FontStyle.Bold);
		}

		private static void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private static void SaveToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private static void OpenToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		private static void NewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			NewProject();
		}

		public static void NewProject()
		{
			Program.File = null;
			Program.Undo.Clear();
			Program.Redo.Clear();

			Project.Functions.Clear();
			Project.Tables.Clear();
			Project.Variables.Clear();

			Project.Functions.Add(new Project.Function { Name = "00FF96 ResetInterruptHandler", Address = 0xff96 });

			Refresh();
		}

		public static void Refresh()
		{
			Form.TreeView.Nodes.Clear();

			var project = new TreeNode(Project.Name);

			var files = new TreeNode("Files");

			files.Nodes.AddRange(Project.Files.Select(File).ToArray());

			var tables = new TreeNode("Tables");

			tables.Nodes.AddRange(Project.Tables.Select(Table).ToArray());

			var variables = new TreeNode("Variables");

			variables.Nodes.AddRange(Project.Variables.Select(Variable).ToArray());

			var functions = new TreeNode("Functions");

			functions.Nodes.AddRange(Project.Functions.Select(Function).ToArray());

			project.Nodes.Add(files);
			project.Nodes.Add(tables);
			project.Nodes.Add(variables);
			project.Nodes.Add(functions);

			Form.TreeView.Nodes.Add(project);
		}

		private static TreeNode File(Project.File file)
		{
			var node = new TreeNode
			{
				Text = file.Path,
				Tag = file
			};

			node.Nodes.Add("Loading...");

			return node;
		}

		private static TreeNode Table(Project.Table table)
		{
			var node = new TreeNode
			{
				Text = table.Name,
				Tag = table
			};

			return node;
		}

		private static TreeNode Variable(Project.Variable variable)
		{
			var node = new TreeNode
			{
				Text = variable.Name,
				Tag = variable
			};

			return node;
		}

		private static TreeNode Function(Project.Function function)
		{
			var node = new TreeNode
			{
				Text = function.Name,
				Tag = function
			};

			node.Nodes.Add("Loading...");

			return node;
		}
	}
}