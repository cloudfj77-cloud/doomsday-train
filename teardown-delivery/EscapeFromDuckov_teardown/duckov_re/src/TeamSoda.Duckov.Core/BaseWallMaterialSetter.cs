using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BaseWallMaterialSetter : MonoBehaviour
{
	public Renderer renderer;

	public BaseWallRendererInfo mainInfo;

	public BaseWallRendererInfo restroomInfo;

	public BaseWallPaper mainDefault;

	public BaseWallPaper restroomDefault;

	private static BaseWallPaper mainWallPaper;

	private static BaseWallPaper restroomWallPaper;

	private static bool dirty = true;

	public static Action<BaseWallPaper, BaseRoomTypes> SetPresetEvent;

	private void Update()
	{
		if (mainWallPaper == null)
		{
			mainWallPaper = mainDefault;
			dirty = true;
		}
		if (restroomWallPaper == null)
		{
			restroomWallPaper = restroomDefault;
			dirty = true;
		}
		if (dirty)
		{
			Sync();
		}
	}

	private void OnDestroy()
	{
	}

	public static void SetWallPaperStatic(BaseWallPaper wallpaper, BaseRoomTypes roomType)
	{
		switch (roomType)
		{
		case BaseRoomTypes.main:
			mainWallPaper = wallpaper;
			break;
		case BaseRoomTypes.restroom:
			restroomWallPaper = wallpaper;
			break;
		default:
			throw new ArgumentOutOfRangeException("roomType", roomType, null);
		}
		dirty = true;
	}

	public void Sync()
	{
		dirty = false;
		List<Material> materials = renderer.sharedMaterials.ToList();
		SetMaterialsRefrence(mainInfo, mainWallPaper.preset, ref materials);
		SetMaterialsRefrence(restroomInfo, restroomWallPaper.preset, ref materials);
		renderer.SetSharedMaterials(materials);
	}

	private void SetMaterialsRefrence(BaseWallRendererInfo rendererInfo, BaseWallMaterialPreset preset, ref List<Material> materials)
	{
		SetRendererMaterial(rendererInfo.wall, preset.wall, ref materials);
		SetRendererMaterial(rendererInfo.celling, preset.celling, ref materials);
		SetRendererMaterial(rendererInfo.floor, preset.floor, ref materials);
	}

	private void SetRendererMaterial(List<int> indexes, Material material, ref List<Material> materials)
	{
		foreach (int index in indexes)
		{
			renderer.sharedMaterials[index] = material;
			materials[index] = material;
		}
	}
}
