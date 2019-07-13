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

			var project = new Project { Text = "New Project", ProjectName = "New Project" };

			treeView.Nodes.Add(project);
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
	}
}
