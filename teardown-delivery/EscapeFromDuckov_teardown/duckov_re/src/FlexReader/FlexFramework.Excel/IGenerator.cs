namespace FlexFramework.Excel;

public interface IGenerator
{
	object Instantiate(Row row);
}
public interface IGenerator<T>
{
	T Instantiate(Row row);
}
