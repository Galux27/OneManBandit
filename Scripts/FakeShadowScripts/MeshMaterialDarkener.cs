using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshMaterialDarkener : MonoBehaviour {
	/// <summary>
	/// Adds a color to a material based on the suns color in the scene, added to all 3d models. 
	/// </summary>
	public List<Material> materialsOnObject;
	public List<Color> originalMaterialColor;
	public bool isWindow=false;
	void Awake()
	{
		materialsOnObject = new List<Material> ();
		originalMaterialColor = new List<Color> ();

		MeshRenderer[] renderers = this.gameObject.GetComponentsInChildren<MeshRenderer> ();

		foreach (MeshRenderer r in renderers) {
			foreach (Material m in r.materials) {
				materialsOnObject.Add (m);
				originalMaterialColor.Add (m.color);
			}
		}
		if (this.transform.parent == null) {
			if (this.gameObject.tag=="Window") {
				isWindow = true;
			}
		} else {
			if (this.gameObject.tag=="Window"||this.transform.parent.tag=="Window") {
				isWindow = true;
			}
		}
			


	}

	// Use this for initialization
	void Start () {
		StartCoroutine (setMatColor ());

	}
	bool whatWasSunLastFrame=false;


	IEnumerator setMatColor()
	{
		if (shouldWeCalculateColour () == true) {
			if (isWindow == false) {
				for (int x = 0; x < materialsOnObject.Count; x++) {
					Color c = TimeScript.me.getSunColor ();
					Color c2 = originalMaterialColor [x];
					materialsOnObject [x].color = new Color ((c.r + c2.r) / 2, (c.g + c2.g) / 2, (c.b + c2.b) / 2, 1.0f);
				}
			} else {
				for (int x = 0; x < materialsOnObject.Count; x++) {
					Color c = TimeScript.me.getSunColor ();
					Color c2 = originalMaterialColor [x];
					materialsOnObject [x].color = new Color ((c.r + c2.r) / 2, (c.g + c2.g) / 2, (c.b + c2.b) / 2, 0.5f);
				}
			}
		}
		yield return new WaitForSeconds (0.5f);
		StartCoroutine (setMatColor ());
	}


	bool shouldWeCalculateColour()
	{
		Vector2 myPos = new Vector2 (this.transform.position.x, this.transform.position.y);
		Vector2 camPos = new Vector2 (CommonObjectsStore.me.mainCam.transform.position.x, CommonObjectsStore.me.mainCam.transform.position.y);

		if (Vector2.Distance (myPos, camPos) < 35) {
			return true;
		} else {
			return false;
		}
	}
}
