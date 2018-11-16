using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonObjectsStore : MonoBehaviour {
	/// <summary>
	/// Script to store misc object that will be needed in more than one place.
	/// </summary>


	public static CommonObjectsStore me;
	public static GameObject player;
	public static PersonWeaponController pwc;
	public static PersonMovementController pm;
	public static PersonClothesController pcc;
	public GameObject bullet,progressBar,personText;
	public Material passive,suspicious,aggressive,fakeLight;
	public GameObject swat,cop,guard,civilian,patrolCop,detective,soldier;
	public GameObject keyBase,keycardBase,noteBase;
	public GameObject[] bloodEffects;
	public GameObject smokeEffect, tearGasEffect;
	public GameObject bloodSpurt;
	public Material spriteOutline;
	public Color spriteOutlineColor;
	public GameObject employeeGUIPrefab,cameraGUIPrefab,announcementGUIPrefab;
	public Shader lightOn,lightOff,normal,lightweight;
	public GameObject explosion,grenadeExplosion,fire,bulletImpact,bloodImpact,carFireEffect;
	public Camera mainCam;
	public Texture2D key,note,keycard,enemy;
	public LayerMask maskForPathSmoothing,maskForObjectAvoidance,maskForMelee;
	public ClothingItem defaultClothes;
	public GameObject stunHalo,fireEffect,speechIcon,shopIcon,seenPlayerIcon;
	public Sprite keyIcon,hotwireIcon;
	public GameObject policePatrolCar,policeBackupCar,swatVan,teleport,sewerEntrance,sewerExit,levelTransition;
    public GameObject[] loreItems;
	void Awake()
	{
		me = this;
		mainCam = Camera.main;
		player = GameObject.FindGameObjectWithTag ("Player");
		pwc = player.GetComponent<PersonWeaponController> ();
		pm = player.GetComponent<PersonMovementController> ();
		pcc = player.GetComponent<PersonClothesController> ();
		if (LoadingDataStore.me == null) {
			GameObject g = new GameObject ();
			g.name = "Load Data Store";
			g.AddComponent<LoadingDataStore> ();
		}
	}

	void Update()
	{
		if (CommonObjectsStore.player == null) {
			player = GameObject.FindGameObjectWithTag ("Player");
			pwc = player.GetComponent<PersonWeaponController> ();
			pm = player.GetComponent<PersonMovementController> ();
			pcc = player.GetComponent<PersonClothesController> ();
		}
	}

	public List<GameObject> cars;

	public GameObject getCars(string name)
	{
		foreach (GameObject g in cars) {
			PlayerCarController pcc = g.GetComponent<PlayerCarController> ();
			if (pcc.carName == name) {
				return g;
			}
		}
		return null;
	}

	public GameObject getRandomCar()
	{
		return cars [Random.Range (0, cars.Count)];
	}

}
