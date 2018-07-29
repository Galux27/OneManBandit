using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

	/// <summary>
	/// Class that controls all kinds of door
	/// </summary>

	public GameObject endLeft, endRight, center;
	public float size = 1.0f;

	public Vector3 startingRotation;
	public bool doorOpen=false;
	public bool autoClose = true;
	float timer=10.0f;
	public float doorOpenSpeed;
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

	/// <summary>
	/// The note in world that contains the keycode for the door (if locked by keycode) 
	/// </summary>
	public GameObject myCode;
	public Sprite fallenSprite;
	public GameObject doorMesh;
	public bool canKickIn = true;
	public AudioClip doorSoundEffect,doorDestroyed,impact;

	/// <summary>
	/// When doors are generated they get an ID number that is serialized so that it can be linked with a key item taht  
	/// </summary>
	public int myID;
	public bool initialised=false;

	/// <summary>
	/// Marks the door as temporary so its not serialized with the rest of the level. 
	/// </summary>
	public bool isTempDoor=false;
	public string doorIncidentName;
	void Awake()
	{
		startingRotation = this.transform.rotation.eulerAngles;
		closeRotation = new Vector3 (0, 0, startingRotation.z - 90);
		openRotation = new Vector3 (0, 0, startingRotation.z + 90);
	}



	// Use this for initialization
	void Start () {
		if (initialised == false) {
		//	Debug.Log ("DOOR " + this.gameObject.name + " WASNT INITILISED");
			myID = IDManager.me.getID ();
			initialiseLockActions ();
			initialised = true;
		}
	//	this.gameObject.AddComponent<PlayerAction_InteractWithDoor> ();
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
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			GameObject key = Instantiate (CommonObjectsStore.me.keyBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			Item i = key.GetComponent<Item> ();
			i.myID = myID;
			Container toSpawnIn;

			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			toSpawnIn.addItemToContainer (i);
			//////Debug.Log ("Spawned key in " + toSpawnIn.gameObject.name);
			myKey = key;
			i.itemDescription += myRoomText;
			this.gameObject.AddComponent<PlayerAction_UnlockWithKey> ();
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			//key.transform.position = new Vector3 (0, 0, 0);

		}
		else if(wayIAmLocked == lockedWith.keycard)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}			GameObject key = Instantiate (CommonObjectsStore.me.keycardBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			KeycardItem i = key.GetComponent<KeycardItem> ();
			i.myID = myID;

			Container toSpawnIn;
			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			key.transform.position = toSpawnIn.transform.position;

			toSpawnIn.addItemToContainer (i);
			if (i.securityClearance == 0) { //TODO add way to manually set security tier
				securityTier = Random.Range (1, 4);
			}
			i.securityClearance = securityTier;
			//i.itemDescription += myRoomText;

			//////Debug.Log ("Spawned keycard in " + toSpawnIn.gameObject.name);
			myKey = key;
			this.gameObject.AddComponent<PlayerAction_OpenWithKeycard> ();
			//key.transform.position =  new Vector3 (0, 0, 0);
		}
		else if(wayIAmLocked == lockedWith.passcode)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}			if (keycodeNumber < 1000) {
				keycodeNumber = Random.Range (1000, 9999);
			}

			//create note for door
			//////Debug.Log ("Keycode number was " + keycodeNumber);
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithKeycode> ();

			GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.noteBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			myCode = g;
			NoteItem i = g.GetComponent<NoteItem> ();
			i.myID = myID;

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
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			toSpawnIn.addItemToContainer (i);
			//g.transform.position = new Vector3 (0, 0, 0);

			//////Debug.Log ("Created note with passcode in " + toSpawnIn.name);
		}
		else if(wayIAmLocked == lockedWith.vaultDoor)
		{

		}
		else if(wayIAmLocked == lockedWith.computer)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}		}
		else if(wayIAmLocked == lockedWith.none)
		{
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}
		}

	}

	public void setDoorActions(){
		if (wayIAmLocked == lockedWith.key) 
		{
			this.gameObject.AddComponent<PlayerAction_UnlockWithLockpick> ();
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			this.gameObject.AddComponent<PlayerAction_UnlockWithKey> ();
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}			//key.transform.position = new Vector3 (0, 0, 0);

		}
		else if(wayIAmLocked == lockedWith.keycard)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}
			this.gameObject.AddComponent<PlayerAction_OpenWithKeycard> ();
			//key.transform.position =  new Vector3 (0, 0, 0);
		}
		else if(wayIAmLocked == lockedWith.passcode)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}
			this.gameObject.AddComponent<PlayerAction_OpenDoorWithKeycode> ();
		}
		else if(wayIAmLocked == lockedWith.vaultDoor)
		{

		}
		else if(wayIAmLocked == lockedWith.computer)
		{
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}		}
		else if(wayIAmLocked == lockedWith.none)
		{
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			if (this.GetComponent<PlayerAction_OpenDoorWithExplosives> () == false) {
				this.gameObject.AddComponent<PlayerAction_OpenDoorWithExplosives> ();
			}
		}

	}
	bool playerHasLineOfSightToDoor()
	{

		Vector3 heading = center.transform.position - CommonObjectsStore.player.transform.position;
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
		if (Input.GetKeyDown (KeyCode.E) && Vector3.Distance(this.transform.position,CommonObjectsStore.player.transform.position)<1.5f && playerHasLineOfSightToDoor()) {
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
			this.gameObject.GetComponent<AudioController> ().playSound (doorSoundEffect);
		}
	}

	public void destroyDoor()
	{
		Destroy (endLeft);
		Destroy (endRight);
		center.GetComponent<SpriteRenderer> ().sprite = fallenSprite;
		center.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		Quaternion q = doorMesh.transform.rotation;
		doorMesh.SetActive (false);
		SpriteRenderer sr = this.gameObject.AddComponent<SpriteRenderer> ();
		if (sr == null) {

		} else {
			sr.sprite = fallenSprite;
			sr.flipX = true;
		}
		this.transform.rotation = q;
		this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
		this.gameObject.GetComponent<Collider2D> ().enabled = false;

		PlayerAction[] actions = this.gameObject.GetComponents<PlayerAction> ();
		for (int x = 0; x < actions.Length; x++) {
			Destroy (actions [x]);
		}

		this.enabled = false;
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
		if (doorHealth > 0) {
			this.gameObject.GetComponent<AudioController> ().playSound (impact);

		}else if(doorHealth <= 0) {

			if (doorIncidentName == "") {

			} else {
				LevelIncidentController.me.addIncident (doorIncidentName, this.transform.position);
			}

			this.gameObject.GetComponent<AudioController> ().playSound (doorDestroyed);

			foreach (GameObject g in NPCManager.me.npcsInWorld) {
				if (g == null) {
					continue;
				}
				NPCController npc = g.GetComponent<NPCController> ();
				npc.setHearedGunshot (this.transform.position, 14.0f);
			}
			PoliceController.me.setNoiseHeard (this.transform.position, 14.0f);

			Destroy (endLeft);
			Destroy (endRight);
			center.GetComponent<SpriteRenderer> ().sprite = fallenSprite;
			center.GetComponent<SpriteRenderer> ().sortingOrder = 0;
			Quaternion q = doorMesh.transform.rotation;
			doorMesh.SetActive (false);
			SpriteRenderer sr = this.gameObject.AddComponent<SpriteRenderer> ();
			sr.sprite = fallenSprite;
			sr.flipX = true;
			this.transform.rotation = q;
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, 0);
			this.gameObject.GetComponent<Collider2D> ().enabled = false;

			PlayerAction[] actions = this.gameObject.GetComponents<PlayerAction> ();
			for (int x = 0; x < actions.Length; x++) {
				Destroy (actions [x]);
			}

			this.enabled = false;
		} 
	}

	public bool canNPCOpenDoor(string id)
	{
		if (locked == true) {
			//return true;
			return idsThatCanGetThrough.Contains (id);
		} else {
			return true;
		}
	}

	public GameObject lastOpened;
	public Vector3 posOfLastOpened;
	void openDoor()
	{
		if (doorOpen == false) {
			doorOpen = true;
		}
	}

	void closeDoor()
	{
		if (doorOpen == true) {
			doorOpen = false;
		}
	}
	float openTimer = 0.2f;

	public void doorMoniter(GameObject opening)
	{
		float mod = 5;


		if (openRotation.z < 0) {
		//	mod *= -1;
		}

		if (doorOpen == true) {
			if (getDotProduct (opening) < 0) {
				if (this.gameObject.transform.eulerAngles.z != openRotation.z) {
					this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (openRotation), doorOpenSpeed * Time.deltaTime);
				}
			} else {
				if (this.gameObject.transform.eulerAngles.z != openRotation.z) {
					this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (closeRotation), doorOpenSpeed * Time.deltaTime);
				}
			}
		} else {
			if (this.gameObject.transform.eulerAngles.z != startingRotation.z) {
				this.gameObject.transform.rotation = Quaternion.Slerp (this.transform.rotation, Quaternion.Euler (startingRotation), doorOpenSpeed * Time.deltaTime);
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

	public void createMyKey()
	{
		myID = IDManager.me.getID ();
		string myRoomText = "";
		RoomScript myRoom = LevelController.me.getRoomPosIsIn (this.transform.position);
		if (myRoom == null) {

		} else {
			myRoomText = "The door is located in " + myRoom.roomName + ".";
		}

		if (wayIAmLocked == lockedWith.key) 
		{
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();
			GameObject key = Instantiate (CommonObjectsStore.me.keyBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			Item i = key.GetComponent<Item> ();
			i.myID = myID;
			Container toSpawnIn;

			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			toSpawnIn.addItemToContainer (i);
			//////Debug.Log ("Spawned key in " + toSpawnIn.gameObject.name);
			myKey = key;

			//key.transform.position = new Vector3 (0, 0, 0);

		}
		else if(wayIAmLocked == lockedWith.keycard)
		{
			GameObject key = Instantiate (CommonObjectsStore.me.keycardBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			KeycardItem i = key.GetComponent<KeycardItem> ();
			i.myID = myID;

			Container toSpawnIn;
			if (placesToSpawnUnlockItems.Count > 0) {
				toSpawnIn = placesToSpawnUnlockItems [Random.Range (0, placesToSpawnUnlockItems.Count)];
			} else {
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			key.transform.position = toSpawnIn.transform.position;

			toSpawnIn.addItemToContainer (i);
			if (i.securityClearance == 0) { //TODO add way to manually set security tier
				securityTier = Random.Range (1, 4);
			}
			i.securityClearance = securityTier;
			//i.itemDescription += myRoomText;

			//////Debug.Log ("Spawned keycard in " + toSpawnIn.gameObject.name);
			myKey = key;
			//key.transform.position =  new Vector3 (0, 0, 0);
		}
		else if(wayIAmLocked == lockedWith.passcode)
		{
			if (keycodeNumber < 1000) {
				keycodeNumber = Random.Range (1000, 9999);
			}

			//create note for door
			//////Debug.Log ("Keycode number was " + keycodeNumber);

			GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.noteBase, Vector3.zero, Quaternion.Euler (0, 0, 0));
			myCode = g;
			NoteItem i = g.GetComponent<NoteItem> ();
			i.myID = myID;

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
				toSpawnIn = LevelController.me.containersForEssentialItems [Random.Range (0, LevelController.me.containersForEssentialItems.Count)];
			}
			toSpawnIn.addItemToContainer (i);
			//g.transform.position = new Vector3 (0, 0, 0);

			//////Debug.Log ("Created note with passcode in " + toSpawnIn.name);
		}
		else if(wayIAmLocked == lockedWith.vaultDoor)
		{

		}
		else if(wayIAmLocked == lockedWith.computer)
		{
		}
		else if(wayIAmLocked == lockedWith.none)
		{
			//this.gameObject.AddComponent<PlayerAction_KickInDoor> ();

		}

	}
}

public enum lockedWith{
	none,
	key,
	keycard,
	passcode,
	vaultDoor,
	computer
}
