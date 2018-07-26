using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ThrowableUI : MonoBehaviour {
	public static ThrowableUI me;
	public bool displayUI = false;
	public LayerMask myMask;
	Image myImage;
	public LineRenderer lr;
	void Awake()
	{
		me = this;
		lr = this.gameObject.AddComponent<LineRenderer> ();
		myImage = this.GetComponent<Image> ();
		myImage.enabled = false;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//myImage.enabled = displayUI;
		lr.enabled = displayUI;
	}

	public void setLine(Transform origin,float length)
	{
		displayUI = true;
		Vector3[] linePoints = new Vector3[2];

		RaycastHit2D ray = Physics2D.Raycast (origin.transform.position , origin.transform.up,length+1,myMask);

		if (ray.collider == null) {

		} else {
			length = Vector2.Distance (origin.transform.position, ray.point);
		}


		linePoints [0] = origin.position;
		linePoints [1] = origin.position + (origin.up * length);
		lr.SetPositions (linePoints);
		lr.startWidth = 0.1f;
		lr.endWidth = 0.1f;
		lr.material = CommonObjectsStore.me.suspicious;
		lr.startColor = Color.cyan;
		lr.endColor = Color.cyan;
	}



	/*public void setCursorPos(GameObject origin, float modifier)
	{
		displayUI = true;
		bool physic = Physics2D.queriesStartInColliders;
		Physics2D.queriesStartInColliders=false;
		RaycastHit2D ray = Physics2D.Raycast (origin.transform.position , origin.transform.up, modifier+1,myMask);

		if (ray.collider == null) {
			this.transform.position = CommonObjectsStore.me.mainCam.WorldToScreenPoint( origin.transform.position + (origin.transform.up * modifier));

		} else {
			this.transform.position = CommonObjectsStore.me.mainCam.WorldToScreenPoint(ray.point);

		}
		Physics2D.queriesStartInColliders=physic;

	}*/
}
