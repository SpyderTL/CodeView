using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CodeView
{
	public static class ProjectFile
	{
		public static void Load(string filePath)
		{
			var document = XDocument.Load(filePath);

			Project.Name = document.Root.Attribute("Name").Value;
			Project.Description = document.Root.Attribute("Description").Value;
			Project.Length = ulong.Parse(document.Root.Attribute("Length").Value, System.Globalization.NumberStyles.HexNumber);
			Project.Notes = document.Root.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty;

			Project.Files = document.Root.Element("Files").Elements("File").Select(x => new Project.File
			{
				Path = x.Attribute("Path").Value,
				Segments = x.Element("Segments").Elements("Segment").Select(y => new Project.File.Segment
				{
					Offset = ulong.Parse(y.Attribute("Offset").Value, System.Globalization.NumberStyles.HexNumber),
					Address = ulong.Parse(y.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
					Length = ulong.Parse(y.Attribute("Length").Value, System.Globalization.NumberStyles.HexNumber),
				}).ToList()
			})
			.OrderBy(x => x.Path)
			.ToList();

			Project.Tables = document.Root.Element("Tables").Elements("Table").Select(x => new Project.Table
			{
				Name = x.Attribute("Name").Value,
				Description = x.Attribute("Description").Value,
				Address = ulong.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
				Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty
			})
			.OrderBy(x => x.Address)
			.ToList();

			Project.Functions = document.Root.Element("Functions").Elements("Function").Select(x => new Project.Function
			{
				Name = x.Attribute("Name").Value,
				Description = x.Attribute("Description").Value,
				Address = ulong.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
				Properties = new Dictionary<string, string>
				{
					{ "Flags", x.Attribute("Flags").Value }
				},
				Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty
			})
			.OrderBy(x => x.Address)
			.ToList();

			Project.Variables = document.Root.Element("Variables").Elements("Variable").Select(x => new Project.Variable
			{
				Name = x.Attribute("Name").Value,
				Description = x.Attribute("Description").Value,
				Address = ulong.Parse(x.Attribute("Address").Value, System.Globalization.NumberStyles.HexNumber),
				Notes = x.Nodes().OfType<XText>().Select(y => y.Value.Replace("\r\n", "\n").Replace("\n", Environment.NewLine)).FirstOrDefault() ?? string.Empty
			})
			.OrderBy(x => x.Address)
			.ToList();
		}

		public static void Save(string filePath)
		{
			var document = new XDocument();

			var root = new XElement("Project");

			root.Add(new XAttribute("Name", Project.Name));
			root.Add(new XAttribute("Description", Project.Description));
			root.Add(new XAttribute("Length", Project.Length));
			root.Add(new XText(Project.Notes));

			document.Add(root);

			var files = new XElement("Files");
			var tables = new XElement("Tables");
			var variables = new XElement("Variables");
			var functions = new XElement("Functions");

			files.Add(Project.Files
				.Select(x => new XElement("File",
					new XAttribute("Path", x.Path),
					new XElement("Segments", x.Segments.Select(y => new XElement("Segment",
						new XAttribute("Offset", y.Offset.ToString("X")),
						new XAttribute("Address", y.Address.ToString("X")),
						new XAttribute("Length", y.Length.ToString("X"))))))));

			tables.Add(Project.Tables
				.Select(x => new XElement("Table",
					new XAttribute("Name", x.Name),
					new XAttribute("Description", x.Description),
					new XAttribute("Address", x.Address.ToString("X")),
					new XText(x.Notes))));

			variables.Add(Project.Variables
				.Select(x => new XElement("Variable",
					new XAttribute("Name", x.Name),
					new XAttribute("Description", x.Description),
					new XAttribute("Address", x.Address.ToString("X")),
					new XText(x.Notes))));

			functions.Add(Project.Functions
				.Select(x => new XElement("Function",
					new XAttribute("Name", x.Name),
					new XAttribute("Description", x.Description),
					new XAttribute("Address", x.Address.ToString("X")),
					new XAttribute("Flags", x.Properties["Flags"]),
					new XText(x.Notes))));

			root.Add(files, tables, variables, functions);

			document.Save(filePath);
		}
	}
}
