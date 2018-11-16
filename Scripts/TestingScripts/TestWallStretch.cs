using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestWallStretch : MonoBehaviour {

	private Vector3 _currentScale;
	public bool doubleScale=true,invertX=false,invertY=false,useY=false;
	private void Start()
	{
		Calculate();
	}

	private void Update()
	{
		Calculate();
	}

	public void Calculate()
	{
		if (_currentScale == transform.localScale) return;
		if (CheckForDefaultSize()) return;

		_currentScale = transform.localScale;
		Material[] mats = GetComponent<Renderer> ().materials;

		if (useY == false) {
			if (doubleScale == false) {
				foreach (Material m in mats) {
					float x = _currentScale.x;
					float y = _currentScale.z;
					if (invertX == true) {
						//	_currentScale.x = y;
					}

					if (invertY == true) {
						//	_currentScale.z = x;
					}

					m.mainTextureScale = new Vector2 (_currentScale.x, _currentScale.z);
				}
			} else {
				foreach (Material m in mats) {
					float x = _currentScale.x;
					float y = _currentScale.z;
					if (invertX == true) {
						//	_currentScale.x = y;
					}

					if (invertY == true) {
						//	_currentScale.z = x;
					}
					m.mainTextureScale = new Vector2 (_currentScale.x * 2, _currentScale.z * 2);
				}
			}
		} else {
			if (doubleScale == false) {
				foreach (Material m in mats) {
					float x = _currentScale.x;
					float y = _currentScale.y;
					if (invertX == true) {
						//	_currentScale.x = y;
					}

					if (invertY == true) {
						//	_currentScale.z = x;
					}

					m.mainTextureScale = new Vector2 (_currentScale.x, _currentScale.y);
				}
			} else {
				foreach (Material m in mats) {
					float x = _currentScale.x;
					float y = _currentScale.y;
					if (invertX == true) {
						//	_currentScale.x = y;
					}

					if (invertY == true) {
						//	_currentScale.z = x;
					}
					m.mainTextureScale = new Vector2 (_currentScale.x * 2, _currentScale.y * 2);
				}
			}
		}

		if (GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode != TextureWrapMode.Repeat)
		{
			GetComponent<Renderer>().sharedMaterial.mainTexture.wrapMode = TextureWrapMode.Repeat;
		}
	}


	private bool CheckForDefaultSize()
	{
		if (_currentScale != Vector3.one) return false;

	//	var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

	//	DestroyImmediate(GetComponent<MeshFilter>());
	//	gameObject.AddComponent<MeshFilter>();
	//	GetComponent<MeshFilter>().sharedMesh = cube.GetComponent<MeshFilter>().sharedMesh;

	//	DestroyImmediate(cube);

		return false;
	}
}
