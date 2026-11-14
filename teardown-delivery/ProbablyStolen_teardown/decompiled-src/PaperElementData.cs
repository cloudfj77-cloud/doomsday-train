using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class PaperElementData
{
	[DefaultValue("text")]
	public string type = "text";

	public string id;

	public float x;

	public float y;

	[DefaultValue(100f)]
	public float w = 100f;

	[DefaultValue(30f)]
	public float h = 30f;

	public string text;

	public string loc;

	public Dictionary<string, string> translations;

	[DefaultValue("body")]
	public string style = "body";

	public float fontSize;

	[DefaultValue("left")]
	public string align = "left";

	[DefaultValue("top")]
	public string valign = "top";

	public string color;

	[DefaultValue(true)]
	public bool wrap = true;

	public bool autoHeight;

	public string preview;

	public string sprite;

	[DefaultValue("stretch")]
	public string fit = "stretch";

	[DefaultValue(1f)]
	public float scale = 1f;

	public bool silhouette;

	public string fill;

	public float x2;

	public float y2;

	[DefaultValue(2f)]
	public float thickness = 2f;

	[DefaultValue("none")]
	public string arrow = "none";

	[DefaultValue(8f)]
	public float arrowSize = 8f;

	public string action;

	public string hoverSprite;

	public string slot;

	public string flow;

	[DefaultValue(6f)]
	public float spacing = 6f;

	[DefaultValue(true)]
	public bool visible = true;

	public bool locked;

	public bool raycast;

	[JsonExtensionData]
	public IDictionary<string, JToken> Extra;

	[JsonIgnore]
	public Rect Bounds
	{
		get
		{
			if (!IsLine)
			{
				return new Rect(x, y, w, h);
			}
			return LineBounds();
		}
	}

	[JsonIgnore]
	public bool IsText => type == "text";

	[JsonIgnore]
	public bool IsImage => type == "image";

	[JsonIgnore]
	public bool IsButton => type == "button";

	[JsonIgnore]
	public bool IsFlow => type == "flow";

	[JsonIgnore]
	public bool IsLine => type == "line";

	[JsonIgnore]
	public bool HasArrow
	{
		get
		{
			if (IsLine && !string.IsNullOrEmpty(arrow))
			{
				return arrow != "none";
			}
			return false;
		}
	}

	[JsonIgnore]
	public bool IsChromeAction => PaperAction.IsChrome(action);

	[JsonIgnore]
	public bool IsIconButton
	{
		get
		{
			if (IsButton)
			{
				return !string.IsNullOrEmpty(sprite);
			}
			return false;
		}
	}

	[JsonIgnore]
	public string DisplayName
	{
		get
		{
			if (!string.IsNullOrEmpty(id))
			{
				return id;
			}
			if (!string.IsNullOrEmpty(action))
			{
				return action;
			}
			if (IsText && !string.IsNullOrEmpty(text))
			{
				if (text.Length <= 18)
				{
					return text;
				}
				return text.Substring(0, 18) + "...";
			}
			if (!string.IsNullOrEmpty(sprite))
			{
				return sprite;
			}
			return type;
		}
	}

	public PaperElementData Clone()
	{
		return PaperJson.Clone(this);
	}

	public Rect LineBounds()
	{
		float num = Mathf.Max(1f, thickness) * 0.5f + (HasArrow ? Mathf.Max(0f, arrowSize) : 0f);
		float xmin = Mathf.Min(x, x2) - num;
		float ymin = Mathf.Min(y, y2) - num;
		float xmax = Mathf.Max(x, x2) + num;
		float ymax = Mathf.Max(y, y2) + num;
		return Rect.MinMaxRect(xmin, ymin, xmax, ymax);
	}

	public void Translate(float dx, float dy)
	{
		x += dx;
		y += dy;
		if (IsLine)
		{
			x2 += dx;
			y2 += dy;
		}
	}
}
