using System;

public class ItemShapeModifier : GridShapeBuilder
{
	public readonly Func<byte, byte, byte> operation;

	public readonly int priority;

	public ItemShapeModifier(Func<byte, byte, byte> operation = null, int priority = 0)
	{
		if (operation == null)
		{
			operation = DefaultOperation;
		}
		this.operation = operation;
		this.priority = priority;
	}

	private byte DefaultOperation(byte sourceData, byte targetData)
	{
		if (targetData <= 0)
		{
			return sourceData;
		}
		return targetData;
	}
}
