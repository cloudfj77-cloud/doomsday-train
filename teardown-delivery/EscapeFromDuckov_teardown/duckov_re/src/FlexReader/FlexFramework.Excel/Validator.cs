using System;
using System.Linq;
using System.Reflection;

namespace FlexFramework.Excel;

public static class Validator
{
	public static bool CanCast(Type from, Type to)
	{
		if (from.IsAssignableFrom(to))
		{
			return true;
		}
		if (HasImplicitConversion(from, to) || HasImplicitConversion(from, from, to) || HasImplicitConversion(to, from, to))
		{
			return true;
		}
		if (to.IsEnum)
		{
			return CanCast(from, Enum.GetUnderlyingType(to));
		}
		if (Nullable.GetUnderlyingType(to) != null)
		{
			return CanCast(from, Nullable.GetUnderlyingType(to));
		}
		return false;
	}

	private static bool HasImplicitConversion(Type definedOn, Type baseType, Type targetType)
	{
		return (from m in definedOn.GetMethods(BindingFlags.Static | BindingFlags.Public)
			where m.Name == "op_Implicit" && m.ReturnType == targetType
			select m).Any(delegate(MethodInfo m)
		{
			ParameterInfo parameterInfo = m.GetParameters().FirstOrDefault();
			return parameterInfo != null && parameterInfo.ParameterType == baseType;
		});
	}

	public static bool HasImplicitConversion(Type source, Type target)
	{
		TypeCode typeCode = Type.GetTypeCode(source);
		TypeCode typeCode2 = Type.GetTypeCode(target);
		switch (typeCode)
		{
		case TypeCode.SByte:
			switch (typeCode2)
			{
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		case TypeCode.Byte:
			if ((uint)(typeCode2 - 7) <= 8u)
			{
				return true;
			}
			return false;
		case TypeCode.Int16:
			switch (typeCode2)
			{
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return true;
			default:
				return false;
			}
		case TypeCode.UInt16:
			if ((uint)(typeCode2 - 9) <= 6u)
			{
				return true;
			}
			return false;
		case TypeCode.Int32:
			if (typeCode2 == TypeCode.Int64 || (uint)(typeCode2 - 13) <= 2u)
			{
				return true;
			}
			return false;
		case TypeCode.UInt32:
			if (typeCode2 == TypeCode.UInt32 || (uint)(typeCode2 - 12) <= 3u)
			{
				return true;
			}
			return false;
		case TypeCode.Int64:
		case TypeCode.UInt64:
			if ((uint)(typeCode2 - 13) <= 2u)
			{
				return true;
			}
			return false;
		case TypeCode.Char:
			if ((uint)(typeCode2 - 8) <= 7u)
			{
				return true;
			}
			return false;
		case TypeCode.Single:
			return typeCode2 == TypeCode.Double;
		default:
			return false;
		}
	}
}
