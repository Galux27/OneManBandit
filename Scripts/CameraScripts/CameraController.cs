using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CameraController : MonoBehaviour {

	/// <summary>
	/// Controls the camera following the player, camera offset when hit by a bullet and the blood red effect when injured.
	/// </summary>

	public static CameraController me;
	public bool doWeFollowPlayer = false;
	public Vector3 cameraRecoilOffset = Vector3.zero;
	public float recoilReset = 0.1f;

	public RawImage bloodTexDisp;
	public Vector2 bulletImpactOffset = Vector2.zero;
	public float bloodAlphaVal=0.7f;

	public Color bulletImpactColor = Color.red;
	void Awake()
	{
		me = this;
	}
	
	// Update is called once per frame

	void FixedUpdate()
	{
		

	}

	void LateUpdate(){
		if (doWeFollowPlayer == true) {
			followPlayer ();
		} else {
			freeCam ();
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			doWeFollowPlayer = !doWeFollowPlayer;
		}

		if (doWeFollowPlayer == false) {
			
		} else {
			resetCameraPos ();
			bloodTexDisp.texture = getBloodEffectTexture ();
			countDownAlpha ();
		}
		//if (Input.GetKeyDown (KeyCode.T)) {
		//	hitByBullet(new Vector2(Random.Range(-1.0f,1.0f),Random.Range(-1.0f,1.0f)));
		//}
	}

	public void bulletRecoilEffect(Weapon w)
	{
		recoilReset = 0.1f;
		float x = Random.Range ((w.recoilMax / 10) * -1, (w.recoilMax / 10));
		float y = Random.Range ((w.recoilMax / 10) * -1, (w.recoilMax / 10));
		cameraRecoilOffset = new Vector3 (x, y, 0);
	}

	void resetRecoil()
	{
		if (recoilReset > 0) {
			recoilReset -= Time.deltaTime;
			if (recoilReset <= 0) {
				cameraRecoilOffset = Vector3.zero;
			}
		}
	}

	void freeCam(){
		if (Input.GetKey (KeyCode.W)) {
			transform.Translate (Vector3.up * 20 * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.S)) {
			transform.Translate (Vector3.up * -20 * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.A)) {
			transform.Translate (Vector3.right * -20 * Time.deltaTime);
		}

		if (Input.GetKey (KeyCode.D)) {
			transform.Translate (Vector3.right * 20 * Time.deltaTime);
		}
	}


	public void hitByBullet(Vector2 bulletPosition)
	{

		if (bloodTexDisp.IsActive () == false) {
			bloodTexDisp.gameObject.SetActive (true);
		}
		bloodAlphaVal += 0.3f;
		//bullet position to be converted so the camera gets moved in the direction of the bullets path
		//do math.abs(bul pos - pl pos) 


		if (bloodAlphaVal > 0.8f) {
			bloodAlphaVal = 0.8f;
		}

		cameraRecoilOffset = transform.InverseTransformDirection(bulletPosition.normalized) * 3;
	}
	float cameraResetTimer = 0.7f;
	void resetCameraPos()
	{
		if (cameraRecoilOffset != Vector3.zero) {
			cameraResetTimer -= Time.deltaTime;
			if (cameraResetTimer <= 0) {
				cameraRecoilOffset = Vector3.zero;
				cameraResetTimer = 0.0f;
			}
		}
	}

	void followPlayer()
	{
		Vector3 newPos = new Vector3 (CommonObjectsStore.player.transform.position.x + cameraRecoilOffset.x, CommonObjectsStore.player.transform.position.y + cameraRecoilOffset.y, -10);
		this.transform.position = newPos;//Vector3.Slerp (this.transform.position,newPos,5*Time.deltaTime);// new Vector3 (CommonObjectsStore.player.transform.position.x + cameraRecoilOffset.x, CommonObjectsStore.player.transform.position.y + cameraRecoilOffset.y, -10);
	}

	float bloodTimer = 0.01f;
	void countDownAlpha()
	{
		if (bloodAlphaVal > 0) {
			bloodTimer -= Time.deltaTime;

			if (bloodTimer <= 0) {
				bloodAlphaVal -= 0.01f;
				bloodTimer = 0.01f;
			}
		}
	}

	public Texture2D getBloodEffectTexture()
	{
		Texture2D t = new Texture2D (1, 1, TextureFormat.RGBA32, false);
		Color c = new Color (bulletImpactColor.r, bulletImpactColor.g, bulletImpactColor.b, bloodAlphaVal);
		t.SetPixel (0, 0, c);
		t.Apply ();
		return t;
	}
	public LayerMask lm;
	public void moveCameraInMouseDir()
	{
		
		Vector2 camPos = new Vector2 (this.transform.position.x, this.transform.position.y);
		Vector2 plPos = new Vector2 (CommonObjectsStore.player.transform.position.x, CommonObjectsStore.player.transform.position.y);

		Vector3 mousePos = Vector3.zero;

		if (CommonObjectsStore.me.mainCam.orthographic == true) {
			mousePos = CommonObjectsStore.me.mainCam.ScreenToWorldPoint (Input.mousePosition);
		} else {
			mousePos = getMousePositionWithPerspective (Input.mousePosition, CommonObjectsStore.player.transform.position.z);
		}

		Vector2 mp = new Vector2 (mousePos.x, mousePos.y);

		Vector2 dir = mp - plPos;
		RaycastHit2D[] hits = Physics2D.RaycastAll (plPos, dir, 8.0f,lm);
		foreach (RaycastHit2D hit in hits) {


			Debug.DrawRay (plPos, dir, Color.green);
			if (hit.collider == null) {
				
			} else if (hit.collider.gameObject.tag == "Building"|| hit.collider.gameObject.transform.root.tag=="Building") {
				Debug.Log ("Hit building");
				this.gameObject.transform.position = Vector3.Slerp (new Vector3(CommonObjectsStore.player.transform.position.x,CommonObjectsStore.player.transform.position.y,-10), new Vector3 (hit.point.x, hit.point.y, -10), 0.5f); //new Vector3 (hit.point.x - (dir.x*2), hit.point.y-(dir.y*2), -10);
				return;
			}
			else if (Vector2.Distance (plPos, mp) >= 8.0f && hit.collider != null &&hit.collider.tag!="Player" || hit.collider != null&&hit.collider.tag!="Player" ) {
				Debug.Log (hit.collider.gameObject.tag + " || " + hit.collider.gameObject.name);

				this.gameObject.transform.position = new Vector3 (hit.point.x, hit.point.y, -10);
				return;
			} 
		}

		float dist = Vector2.Distance (mp, plPos);

		if (dist > 8) {
			dist = 8;
		}

		Vector2 nonColPos = plPos + ((dir.normalized) * dist);
		this.gameObject.transform.position = new Vector3 (nonColPos.x, nonColPos.y, -10);
	}

	Vector3 getMousePositionWithPerspective(Vector3 screenPos, float z)
	{
		Ray ray = Camera.main.ScreenPointToRay (screenPos);
		Plane xy = new Plane (Vector3.forward, new Vector3 (0, 0, z));
		float distance;
		xy.Raycast (ray, out distance);
		return ray.GetPoint (distance);
	}

}
