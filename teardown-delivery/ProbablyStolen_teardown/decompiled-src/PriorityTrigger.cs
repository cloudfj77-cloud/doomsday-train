using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public abstract class PriorityTrigger
{
	public virtual int priority { get; }

	public static void InitBehaviours<T>(List<T> list) where T : PriorityTrigger
	{
		foreach (Type item2 in (from t in Assembly.GetExecutingAssembly().GetTypes()
			where t?.IsClass ?? false
			where t.IsSubclassOf(typeof(T)) && !t.IsAbstract
			select t).Cast<Type>())
		{
			T item = (T)Activator.CreateInstance(item2);
			list.Add(item);
		}
		list.Sort((T i1, T i2) => i2.priority.CompareTo(i1.priority));
	}
}
