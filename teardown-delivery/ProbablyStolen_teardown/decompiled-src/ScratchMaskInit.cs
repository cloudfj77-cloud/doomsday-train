using UnityEngine;

public class ScratchMaskInit : MonoBehaviour
{
	[HideInInspector]
	public RenderTexture mask;

	private void Awake()
	{
		mask = new RenderTexture(56, 56, 0, RenderTextureFormat.ARGB32);
		mask.filterMode = FilterMode.Point;
		mask.wrapMode = TextureWrapMode.Clamp;
		mask.Create();
		RenderTexture.active = mask;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = null;
	}
}
