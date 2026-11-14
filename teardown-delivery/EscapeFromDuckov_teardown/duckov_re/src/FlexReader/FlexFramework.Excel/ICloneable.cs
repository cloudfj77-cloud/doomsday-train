namespace FlexFramework.Excel;

public interface ICloneable<T> where T : class
{
	T DeepClone();

	T ShallowClone();
}
