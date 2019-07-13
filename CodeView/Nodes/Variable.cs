namespace CodeView.Nodes
{
	public class Variable : DataNode
	{
		public Project Project;
		public int Address;

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