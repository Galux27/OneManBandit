using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2 : MonoBehaviour {

	public Vector3 startingRotation;
	public bool doorOpen=false;
	public bool autoClose = true;
	float timer=3.0f;

	public bool locked=false;
	public List<string> idsThatCanGetThrough;
	public lockedWith wayIAmLocked;

	[Range(1000,9999)]
	public int keycodeNumber=0;

	public GameObject myKey;

	[Range(0,4)]
	public int securityTier = 0;

	public List<Container> placesToSpawnUnlockItems;
	public int doorHealth = 2;
	public Vector3 openRotation,closeRotation;
	public GameObject myCode;
	public Sprite fallenSprite;
	public bool canKickIn = true;
	void Awake()
	{
		startingRotation = this.transform.rotation.eulerAngles;
		closeRotation = new Vector3 (0, 0, startingRotation.z - 90);
		openRotation = new Vector3 (0, 0, startingRotation.z + 90);
	}

	// Use this for initialization
	void Start () {
		initialiseLockActions ();
	}

	// Update is called once per frame
	void Update () {
		if (locked == false) {
			isPlayerTryingToOpenDoor ();
		}

		if (doorOpen == true) {
			if (autoClose == true) {
				timer -= Time.deltaTime;
				if (timer <= 0) {
					closeDoor ();
					timer = 3.0f;
				}
			}
		}
		doorMoniter (lastOpened);
	}

	void initialiseLockActions()
	{
		RoomScript myRoom = LevelController.me.getRoomObjectIsIn (this.gameObject);
		string myRoomText = "";

		if (myRoom == null) {

		} else {
			myRoomText = "The door is located in " + myRoom.roomName + ".";
		}

		if (wayIAmLocked == lockedWith.key) 
		{
			this.gameObject.AddComponent<PlayerAction_UnlockWithLockpick> ();
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			GameObject key = Instantiate (CommonObjectsStore.me.keyBase, new Vector3 (0, 0, 0), Quaternion.Euler (0, 0, 0));
			Item i = key.GetComponent<Item> ();
			Container toSpawnIn;

			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = FindObjectOfType<Container> ();
			}
			key.transform.position = toSpawnIn.gameObject.transform.position;
			toSpawnIn.addItemToContainer (i);
			//////Debug.Log ("Spawned key in " + toSpawnIn.gameObject.name);
			myKey = key;
			i.itemDescription += myRoomText;
			this.gameObject.AddComponent<PlayerAction_UnlockWithKey> ();
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();

		}
		else if(wayIAmLocked == lockedWith.keycard)
		{
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			GameObject key = Instantiate (CommonObjectsStore.me.keycardBase, new Vector3 (0, 0, 0), Quaternion.Euler (0, 0, 0));
			KeycardItem i = key.GetComponent<KeycardItem> ();
			Container toSpawnIn;
			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = FindObjectOfType<Container> ();
			}
			toSpawnIn.addItemToContainer (i);
			if (i.securityClearance == 0) { //TODO add way to manually set security tier
				securityTier = Random.Range (1, 4);
			}
			i.securityClearance = securityTier;
			//i.itemDescription += myRoomText;

			//////Debug.Log ("Spawned keycard in " + toSpawnIn.gameObject.name);
			myKey = key;
			this.gameObject.AddComponent<PlayerAction_OpenWithKeycard> ();
			key.transform.position = toSpawnIn.transform.position;
		}
		else if(wayIAmLocked == lockedWith.passcode)
		{
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			if (keycodeNumber < 1000) {
				keycodeNumber = Random.Range (1000, 9999);
			}

			//create note for door
			//////Debug.Log ("Keycode number was " + keycodeNumber);
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithKeycode> ();

			GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.noteBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			myCode = g;
			NoteItem i = g.GetComponent<NoteItem> ();
			RoomScript r = LevelController.me.getRoomObjectIsIn (g);

			if (r == null) {
				i.noteText = "Here is the passcode for the door, its " + keycodeNumber.ToString () + ". Please destroy this note when you memorise it." + myRoomText;

			} else {
				i.noteText = "Here is the passcode for the door in the " + r.roomName +", its " + keycodeNumber.ToString () + ". Please destroy this note when you memorise it." + myRoomText;
			}

			Container toSpawnIn;
			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = FindObjectOfType<Container> ();
			}
			toSpawnIn.addItemToContainer (i);
			g.transform.position = toSpawnIn.transform.position;

			//////Debug.Log ("Created note with passcode in " + toSpawnIn.name);
		}
		else if(wayIAmLocked == lockedWith.vaultDoor)
		{

		}
		else if(wayIAmLocked == lockedWith.computer)
		{
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
		}
		else if(wayIAmLocked == lockedWith.none)
		{
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();

		}

	}

	bool playerHasLineOfSightToDoor()
	{

		Vector3 heading = transform.position - CommonObjectsStore.player.transform.position;
		RaycastHit2D[] rays= Physics2D.RaycastAll ( CommonObjectsStore.player.transform.position, heading,Vector3.Distance(this.transform.position,CommonObjectsStore.player.transform.position));
		Debug.DrawRay (CommonObjectsStore.player.transform.position, heading,Color.cyan);
		foreach (RaycastHit2D ray in rays) {
			if (ray.collider == null) {
				//return true;
			} else {
				//////Debug.Log (ray.collider.gameObject);

				if (ray.collider.gameObject.tag == "Player") {
					continue;
				}



				//if (ray.collider.gameObject == this.transform.parent.gameObject) {
				//return true;
				//} else {
				//	return false;
				//}
			}
		}
		return true;
	}


	void isPlayerTryingToOpenDoor()
	{
		if (Input.GetKeyDown (KeyCode.E) && Vector3.Distance(this.transform.position,CommonObjectsStore.player.transform.position)<2.0f && playerHasLineOfSightToDoor()) {
			interactWithDoor (CommonObjectsStore.player);
		}
	}

	public void interactWithDoor(GameObject openingDoor)
	{
		if (lastOpened != openingDoor) {
			lastOpened = openingDoor;
			posOfLastOpened = lastOpened.transform.position;
		}
		if (doorOpen == true) {
			closeDoor ();
		} else {
			openDoor ();
		}
	}

	public void kickInDoor()
	{
		if (canKickIn==false) {
			return;
		}

		doorHealth--;
		foreach (GameObject g in NPCManager.me.npcsInWorld) {
			if (g == null) {
				continue;
			}
			NPCController npc = g.GetComponent<NPCController> ();
			npc.setHearedGunshot (this.transform.position, 7.0f);
		}
		PoliceController.me.setNoiseHeard (this.transform.position, 7.0f);


		if (doorHealth == 0) {
			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}
				NPCController npc = g.GetComponent<NPCController> ();
				npc.setHearedGunshot (this.transform.position, 14.0f);
			}
			PoliceController.me.setNoiseHeard (this.transform.position, 14.0f);

			GameObject downed = new GameObject ();
			downed.name = "Destroyed door";
			downed.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
			downed.transform.rotation = this.transform.rotation;
			SpriteRenderer sr = downed.gameObject.AddComponent<SpriteRenderer> ();
			sr.sprite = fallenSprite;
			sr.sortingOrder = 1;
			Destroy (this.gameObject);

		} 
	}

	public bool canNPCOpenDoor(string id)
	{
		if (locked == true) {
			return true;
			//return idsThatCanGetThrough.Contains (id);
		} else {
			return true;
		}
	}

	public GameObject lastOpened;
	public Vector3 posOfLastOpened;
	void openDoor()
	{
		//this.gameObject.transform.rotation = Quaternion.Euler (new Vector3(0,0,this.transform.rotation.eulerAngles.z + 90));
		//	float timer = 3.0f;
		//while (timer > 0.0f) {
		////	this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation,Quaternion.Euler( new Vector3(0,0,this.transform.rotation.eulerAngles.z + 90)),5*Time.deltaTime);
		//	timer -= Time.deltaTime;
		//}
		//doorOpen = true;
		if (doorOpen == false) {
			doorOpen = true;
		}
	}

	void closeDoor()
	{
		if (doorOpen == true) {
			doorOpen = false;
		}
		//this.gameObject.transform.rotation = Quaternion.Euler (new Vector3(0,0,this.transform.rotation.eulerAngles.z - 90));
		//float timer = 3.0f;
		//while (timer > 0.0f) {
		//	this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (new Vector3 (0, 0, this.transform.rotation.eulerAngles.z - 90)), 5 * Time.deltaTime);
		//	timer -= Time.deltaTime;
		//}
	}
	float openTimer = 0.2f;

	public void doorMoniter(GameObject opening)
	{
		if (doorOpen == true) {
			if (getDotProduct (opening) < 0) {
				if (this.gameObject.transform.eulerAngles.z != openRotation.z) {
					this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (openRotation), 5 * Time.deltaTime);
				}
			} else {
				if (this.gameObject.transform.eulerAngles.z != openRotation.z) {
					this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (closeRotation), 5 * Time.deltaTime);
				}
			}
		} else {
			if (this.gameObject.transform.eulerAngles.z != startingRotation.z) {
				this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (startingRotation), 5 * Time.deltaTime);
			}
		}
	}

	public float getDotProduct(GameObject target)
	{
		if (target == null) {
			return 0.0f;
		}

		Vector3 heading = posOfLastOpened - this.transform.position;
		return Vector3.Dot (heading, this.transform.up);
	}


}