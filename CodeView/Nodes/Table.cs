namespace CodeView.Nodes
{
	public class Table : NotableNode
	{
		public Project Project;
		public int Address;
		public string Description = string.Empty;

		public override object GetProperties()
		{
			return new TableProperties { Table = this };
		}

		public override void Reload()
		{
		}

		public class TableProperties
		{
			public Table Table;

			public string Name
			{
				get => Table.Text;
				set => Table.Text = value;
			}

			public string Description
			{
				get => Table.Description;
				set => Table.Description = value;
			}

			public string Address
			{
				get => Table.Address.ToString("X");
				set
				{
					Table.Project.Functions.Remove(Table.Address);

					Table.Address = int.Parse(value, System.Globalization.NumberStyles.HexNumber);

					Table.Project.Tables.Add(Table.Address, Table);
				}
			}
		}
	}
}