using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBulletSpawner : MonoBehaviour {
	float timer = 0.1f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		rotateToFacePosition (CommonObjectsStore.player.transform.position);
		timer -= Time.deltaTime;
		if (timer <= 0) {
			Instantiate (CommonObjectsStore.me.bullet, this.transform.position, this.transform.rotation);
			timer = 0.1f;
		}
	}

	public void rotateToFacePosition(Vector3 pos)
	{
		//hc.rotateToFacePosition (pos);
		Vector3 rot = new Vector3(0, 0, Mathf.Atan2((pos.y - transform.position.y),pos.x - transform.position.x)) * Mathf.Rad2Deg;
		rot = new Vector3(rot.x, rot.y, rot.z-90);//add 90 to make the player face the right way (yaxis = up)
		//rid.transform.eulerAngles = rot;
		this.transform.rotation = Quaternion.Euler(rot); //(INSTA ROTATION)

		//rotates player on Z axis to face cursor position
	}

}
