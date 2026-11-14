using UnityEngine;

public class LetterboxBars : MonoBehaviour
{
	private Material material;

	private Rect area = new Rect(0f, 0f, 1f, 1f);

	public void SetArea(Rect normalizedArea)
	{
		area = normalizedArea;
	}

	private void EnsureMaterial()
	{
		if (!(material != null))
		{
			Shader shader = Shader.Find("Hidden/Internal-Colored");
			if (shader == null)
			{
				shader = Shader.Find("Sprites/Default");
			}
			material = new Material(shader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			if (material.HasProperty("_ZTest"))
			{
				material.SetInt("_ZTest", 8);
			}
			if (material.HasProperty("_ZWrite"))
			{
				material.SetInt("_ZWrite", 0);
			}
			if (material.HasProperty("_Cull"))
			{
				material.SetInt("_Cull", 0);
			}
		}
	}

	private void OnPostRender()
	{
		if (!(area.xMin <= 0f) || !(area.yMin <= 0f) || !(area.xMax >= 1f) || !(area.yMax >= 1f))
		{
			EnsureMaterial();
			GL.PushMatrix();
			material.SetPass(0);
			GL.LoadOrtho();
			GL.Begin(7);
			GL.Color(Color.black);
			if (area.xMin > 0f)
			{
				Quad(0f, 0f, area.xMin, 1f);
			}
			if (area.xMax < 1f)
			{
				Quad(area.xMax, 0f, 1f, 1f);
			}
			if (area.yMin > 0f)
			{
				Quad(area.xMin, 0f, area.xMax, area.yMin);
			}
			if (area.yMax < 1f)
			{
				Quad(area.xMin, area.yMax, area.xMax, 1f);
			}
			GL.End();
			GL.PopMatrix();
		}
	}

	private static void Quad(float x0, float y0, float x1, float y1)
	{
		GL.Vertex3(x0, y0, 0f);
		GL.Vertex3(x1, y0, 0f);
		GL.Vertex3(x1, y1, 0f);
		GL.Vertex3(x0, y1, 0f);
	}

	private void OnDestroy()
	{
		if (material != null)
		{
			Object.Destroy(material);
		}
	}
}
