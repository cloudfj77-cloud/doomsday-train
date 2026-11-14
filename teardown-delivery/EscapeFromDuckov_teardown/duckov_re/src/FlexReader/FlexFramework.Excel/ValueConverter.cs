using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FlexFramework.Excel;

public static class ValueConverter
{
	private class Converter<T> : IConverter
	{
		private Converter<string, T> _converter;

		Type IConverter.Type => typeof(T);

		public Converter(Converter<string, T> converter)
		{
			if (converter == null)
			{
				throw new ArgumentNullException();
			}
			_converter = converter;
		}

		object IConverter.Convert(string input)
		{
			return _converter(input);
		}
	}

	private class Converter : IConverter
	{
		private Converter<string, object> _converter;

		private Type _type;

		Type IConverter.Type => _type;

		public Converter(Converter<string, object> converter, Type type)
		{
			if (converter == null || type == null)
			{
				throw new ArgumentNullException();
			}
			_converter = converter;
			_type = type;
		}

		object IConverter.Convert(string input)
		{
			return _converter(input);
		}
	}

	private static readonly List<IConverter> converters;

	static ValueConverter()
	{
		converters = new List<IConverter>();
		Reset();
	}

	public static void Reset()
	{
		converters.Clear();
		Register<ColorConverter>();
		Register<Color32Converter>();
		Register<RectConverter>();
		Register<Vector4Converter>();
		Register<Vector3Converter>();
		Register<Vector2Converter>();
		Register<ObjectConverter>();
		Register<ArrayConverter<string>>();
		Register<ArrayConverter<int>>();
		Register<ArrayConverter<float>>();
		Register<ListConverter<string>>();
		Register<ListConverter<int>>();
		Register<ListConverter<float>>();
		Register<DictionaryConverter<string, string>>();
		Register<DictionaryConverter<string, int>>();
		Register<DictionaryConverter<int, int>>();
		Register<DictionaryConverter<int, string>>();
	}

	public static void Empty()
	{
		converters.Clear();
	}

	public static object Convert(Type type, string value)
	{
		if (type == null)
		{
			throw new ArgumentNullException("type");
		}
		IConverter converter = converters.Find((IConverter c) => c.Type == type);
		if (converter == null)
		{
			TypeConverter @default = TypeDescriptor.GetConverter(type);
			if (@default == null)
			{
				throw new NotSupportedException("Converter not found for target type: " + type.Name);
			}
			converter = new Converter((string i) => @default.ConvertFromString(i), type);
			Register(converter);
		}
		try
		{
			return converter.Convert(value);
		}
		catch (FormatException)
		{
			throw;
		}
		catch (Exception ex2)
		{
			throw new FormatException(ex2.Message, ex2);
		}
	}

	public static T Convert<T>(string value)
	{
		return (T)Convert(typeof(T), value);
	}

	public static bool Register(IConverter converter)
	{
		if (converter == null)
		{
			return false;
		}
		converters.RemoveAll((IConverter c) => c.Type == converter.Type);
		converters.Add(converter);
		return true;
	}

	public static bool Register<T>() where T : IConverter, new()
	{
		return Register(new T());
	}

	public static bool Register<T>(Converter<string, T> converter)
	{
		return Register(new Converter<T>(converter));
	}

	public static bool Register(Converter<string, object> converter, Type type)
	{
		return Register(new Converter(converter, type));
	}

	public static bool Unregister(Type type)
	{
		return converters.RemoveAll((IConverter c) => c.Type == type) > 0;
	}

	public static bool Unregister<T>()
	{
		return Unregister(typeof(T));
	}
}
