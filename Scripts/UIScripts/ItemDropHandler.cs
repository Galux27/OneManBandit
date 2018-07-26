using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ItemDropHandler : MonoBehaviour,IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	public void OnBeginDrag(PointerEventData eventData)
	{
		//////Debug.LogError("Beginning DRAG");
		itemBeingDragged = gameObject;
		startPosition = transform.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		//////Debug.LogError("DRAGGING");
		transform.position = Input.mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData){
		//////Debug.LogError("Ending DRAG");
		itemBeingDragged = null;
		transform.position = startPosition;
	}
}
