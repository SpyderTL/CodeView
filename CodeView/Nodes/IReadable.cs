using System.IO;

namespace CodeView.Nodes
{
	public interface IReadable
	{
		Stream GetStream();
	}
}