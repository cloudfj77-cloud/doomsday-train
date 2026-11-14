using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MultiEventHandler
{
	public List<Func<PointerEventData, bool>> mousePotentialDragFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseBeginDragFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseDragFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseDropFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseEndDragFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseClickFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseUpFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseDownFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseMoveFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseEnterFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseExitFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseScrollFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<PointerEventData, bool>> mouseDoubleClickFunc = new List<Func<PointerEventData, bool>>();

	public List<Func<BaseEventData, bool>> selectFunc = new List<Func<BaseEventData, bool>>();

	public List<Func<BaseEventData, bool>> deselectFunc = new List<Func<BaseEventData, bool>>();

	public List<Func<BaseEventData, bool>> updateSelectedFunc = new List<Func<BaseEventData, bool>>();
}
