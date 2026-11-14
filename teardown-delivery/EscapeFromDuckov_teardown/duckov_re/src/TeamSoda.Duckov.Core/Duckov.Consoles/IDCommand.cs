namespace Duckov.Consoles;

public interface IDCommand
{
	string CommandWord { get; }

	string Execute(DConsole console, string[] args);
}
