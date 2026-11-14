using UnityEngine;

namespace Duckov.Consoles;

public abstract class DCommand : ScriptableObject, IDCommand
{
	public abstract string CommandWord { get; }

	public abstract string Execute(DConsole console, string[] args);
}
