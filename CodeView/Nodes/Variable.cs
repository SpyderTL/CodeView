using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace CodeView.Nodes
{
	public class Variable : NotableNode
	{
		public Project Project;
		public int Address;
		public string Description = string.Empty;

		public override object GetProperties()
		{
			return new VariableProperties { Variable = this };
		}

		public override void Reload()
		{
		}

		public class VariableProperties
		{
			public Variable Variable;

			public string Name
			{
				get => Variable.Text;
				set => Variable.Text = value;
			}

			public string Description
			{
				get => Variable.Description;
				set => Variable.Description = value;
			}

			public string Address
			{
				get => Variable.Address.ToString("X");
				set
				{
					Variable.Project.Functions.Remove(Variable.Address);

					Variable.Address = int.Parse(value, System.Globalization.NumberStyles.HexNumber);

					Variable.Project.Variables.Add(Variable.Address, Variable);
				}
			}
		}
	}
}