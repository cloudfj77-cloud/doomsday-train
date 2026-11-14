using Cysharp.Threading.Tasks;

namespace Duckov.UI;

public interface IClosureView
{
	UniTask Task(bool evacuated);
}
