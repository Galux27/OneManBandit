using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeShadowGenerator : MonoBehaviour {

	/// <summary>
	/// Script used to work out if our 2d "Shadows" are being affected by one of the light sources. 
	/// </summary>

	public static FakeShadowGenerator me;

	public LightSource[] sourcesInLevel;

	void Awake()
	{
		me = this;
		sourcesInLevel = FindObjectsOfType<LightSource> ();
	}

	public LightSource getNearestLightSource(GameObject wanting){
		LightSource nearest = LightSource.sun;
		float distance = 99999999.0f;

		foreach (LightSource l in sourcesInLevel) {
			//yield return new WaitForEndOfFrame ();
			float d = Vector3.Distance (wanting.transform.position, l.gameObject.transform.position);
			if (d < distance) {
				if(lineOfSightToTargetWithNoCollider(l.gameObject,wanting)==true){
					distance = d;
					nearest = l;
				}
			}
		}


		return nearest;
	}

	public bool lineOfSightToTargetWithNoCollider(GameObject source,GameObject target){

		Vector3 origin = source.transform.position;


		Vector3 heading = target.transform.position - origin;
		//RaycastHit2D[] rays = Physics2D.RaycastAll (origin, heading,Vector3.Distance(source.transform.position,target.transform.position));
		RaycastHit2D ray = Physics2D.Raycast (origin, heading,Vector3.Distance(source.transform.position,target.transform.position));

		if (this.gameObject.tag == "Player") {
			//Debug.DrawRay (origin, heading,Color.cyan);
		}

		//foreach (RaycastHit2D ray in rays) {
			
			if (ray.collider == null) {
				
			} else {
				if (ray.collider.gameObject.tag == "Walls" || ray.collider.gameObject.tag=="Door") {
					return false;
				}
			}
	//	}
		return true;
	}
}
