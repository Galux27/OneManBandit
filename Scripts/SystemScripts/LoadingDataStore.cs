using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class LoadingDataStore : MonoBehaviour {
	public static LoadingDataStore me;

	public List<string> playerClothes,playerItems,playerEquipped,playerPortableContainer;
	public float playerHealth = 0;
	public int timeMin=0,timeHour=0;
	public bool playerBleeding=false;

	public string folderPath = "";

	public bool initialised = false;

	void Awake()
	{
		if (me == null) {
			me = this;
			DontDestroyOnLoad (this.gameObject);

		} else {
			Destroy (this.gameObject);
		}
		saveData ();

	}
	// Use this for initialization
	void Start () {
		if (initialised == false && doesSavegameExist ()) {
			readPlayerData ();
			recreatePlayerData ();
			recreateLevelData ();
			initialised = true;
		} 
	}
	
	// Update is called once per frame
	void Update () {
	//	if (Input.GetKeyDown (KeyCode.E)) {
			//storePlayerData ();
			//writePlayerData ();
	//	}

		//if (Input.GetKeyDown (KeyCode.Q)) {

		//	readPlayerData ();
		//}
	}

	public void storePlayerData()
	{
		playerClothes = new List<string> ();
		playerItems = new List<string> ();
		playerEquipped = new List<string> ();
		playerPortableContainer = new List<string> ();




		Inventory i = CommonObjectsStore.player.GetComponent<Inventory> ();
		foreach(Item item in i.inventoryItems)
		{
			playerItems.Add (serialiseItem(item));
		}

		if (i.head == null) {

		} else if (playerEquipped.Contains (i.head.itemName) == false) {
			playerEquipped.Add (i.head.itemName);
		}

		if (i.torso == null) {

		} else if (playerEquipped.Contains (i.torso.itemName) == false) {
			playerEquipped.Add (i.torso.itemName);
		}

		if (i.leftArm == null) {

		} else if (playerEquipped.Contains (i.leftArm.itemName) == false) {
			playerEquipped.Add (i.leftArm.itemName);
		}

		if (i.leftLeg == null) {

		} else if (playerEquipped.Contains (i.leftLeg.itemName) == false) {
			playerEquipped.Add (i.leftLeg.itemName);
		}

		if (i.rightArm == null) {

		} else if (playerEquipped.Contains (i.rightArm.itemName) == false) {
			playerEquipped.Add (i.rightArm.itemName);
		}

		if (i.rightLeg == null) {

		} else if (playerEquipped.Contains (i.rightLeg.itemName) == false) {
			playerEquipped.Add (i.rightLeg.itemName);
		}

		if (i.back == null) {

		} else if (playerEquipped.Contains (i.back.itemName) == false) {
			playerEquipped.Add (i.back.itemName);
		}

		PersonClothesController pcc = CommonObjectsStore.player.GetComponent<PersonClothesController> ();
		foreach (ClothingItem c in pcc.clothesBeingWorn) {
			playerClothes.Add (c.itemName);
		}

		PortableContainerItem pci = CommonObjectsStore.player.GetComponentInChildren<PortableContainerItem> ();
		if (pci == null) {

		} else {
			foreach(Item item in pci.gameObject.GetComponent<Container>().itemsInContainer)
			{
				playerPortableContainer.Add(serialiseItem(item));
			}
		}

		playerHealth = CommonObjectsStore.player.GetComponent<PersonHealth> ().healthValue;
		playerBleeding = CommonObjectsStore.player.GetComponent<BleedingEffect> ().bleeding;

		timeMin = TimeScript.me.minute;
		timeHour = TimeScript.me.hour;

		writePlayerData ();
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode){
		if (scene.name != "MainMenu") {
			if (doesSavegameExist () == true) {
				recreatePlayerData ();
				recreateLevelData ();
				changeActiveLevel ();
				IDManager.me.writeID ();
			}
		}
	}

	public void recreatePlayerData()
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		ItemDatabase id = FindObjectOfType<ItemDatabase> ();
		TimeScript ts = FindObjectOfType<TimeScript> ();
		Inventory pi = player.GetComponent<Inventory> ();
		PersonClothesController pcc = player.GetComponent<PersonClothesController> ();
		player.GetComponentInChildren<ArtemAnimationController> ().forceInitialise ();

		if (playerItems == null) {
			if (doesSavegameExist ()) {
				readPlayerData ();
			}
		}

		foreach (string st in playerItems) {
			if (getItemName(st) == "Backpack") {
				if (playerEquipped.Contains (getItemName(st))) {
					GameObject i = deserialiseItem (st);
					if (i == null) {

					} else {
						GameObject obj = i;//(GameObject)Instantiate (i, player.transform.position, player.transform.rotation);
						obj.transform.position = pi.gameObject.transform.position;
						Item item = obj.GetComponent<Item>();
						pi.addItemToInventory (item);
						item.equipItem ();
						pi.decideCapacityMod ();
						playerItems.Remove (getItemName (st));
						playerEquipped.Remove (getItemName (st));
						break;
					}
				}
			}
		}

		foreach (string st in playerItems) {
			GameObject i = deserialiseItem (st);
			if (i == null) {


			} else {
				GameObject obj = i;//(GameObject)Instantiate (i, player.transform.position, player.transform.rotation);
				obj.transform.position = pi.gameObject.transform.position;
				Item item = obj.GetComponent<Item>();
				pi.addItemToInventory (item);
			}
		}

		foreach (string st in playerEquipped) {
			foreach (Item i in pi.inventoryItems) {
				if (i == null) {

				} else {
					if (i.itemName == getItemName(st) || st==i.itemName) {
						i.equipItem ();
						//pi.setVisual(i, i.slot);
					}
				}
			}
		}

		foreach (string st in playerClothes) {
			foreach (Item i in pi.inventoryItems) {
				if (i == null) {

				} else {
					if (i.itemName == st) {
						i.equipItem ();

					}
				}
			}
		}

		foreach (string st in playerPortableContainer) {
			GameObject i = id.getItem (st);
			if (i == null) {


			} else {
				GameObject obj = (GameObject)Instantiate (i, player.transform.position, player.transform.rotation);
				Item item = obj.GetComponent<Item>();
				Container c = player.GetComponentInChildren<PortableContainerItem> ().GetComponent<Container> ();
				c.addItemToContainer (item);
			}
		}


		ts.initialiseSunPosition ();
		player.GetComponent<PersonHealth> ().healthValue = playerHealth;
		player.GetComponent<BleedingEffect> ().bleeding = playerBleeding;

		pcc.clothesSetFromLoading = true;
		pcc.aac.setAesthetics ();

		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");
		if (File.Exists (Path.Combine (playerDataPath, "DateTime.txt"))) {
			List<string> dateTime = readFile (playerDataPath, "DateTime.txt");
			int min = int.Parse (dateTime [0]);
			int hour = int.Parse (dateTime [1]);
			int day = int.Parse (dateTime [2]);
			int month = int.Parse (dateTime [3]);
			int year = int.Parse (dateTime [4]);

			ts.minute = min;
			ts.hour = hour;
			ts.day = day;
			ts.month = month;
			ts.year = year;
			ts.setDayOfWeek ();
			ts.initialisedDay = true;
		} else {
			Debug.LogError ("No timedate found");
		}

		Stash s = FindObjectOfType<Stash> ();
		if (s == null) {

		} else {
			List<string> stash = readFile(playerDataPath, "StashValue.txt");
			s.stashValue = int.Parse (stash [0]);

		}
	}

	public int getStashValue()
	{
		string playerDataPath = Path.Combine (folderPath, profileDir);

		if (File.Exists (Path.Combine (playerDataPath, "StashValue.txt")) == false) {
			List<string> stash2 = new List<string> ();
			stash2.Add ("0");
			writeListToFile (playerDataPath, "StashValue.txt", stash2.ToArray ());
		}

		playerDataPath = Path.Combine (playerDataPath, "PlayerData");
		List<string> stash = readFile(playerDataPath, "StashValue.txt");
		return int.Parse (stash [0]);
	}

	public void setStashValue(int value)
	{
		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");

		List<string> stash = new List<string> ();
		stash.Add (value.ToString ());
		writeListToFile (playerDataPath, "StashValue.txt", stash.ToArray ());


	}

	void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public string profileDir="";
	void initialiseFolderDirectory()
	{
		folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		profileDir = PlayerPrefs.GetString ("Profile");
	}

	bool doesSavegameExist()
	{
		string pathForDir = Path.Combine (folderPath, profileDir);
		if (Directory.Exists (pathForDir) == false) {
			return false;
		}

		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");

		string filePath = Path.Combine (playerDataPath, "Inventory.txt");
		if (File.Exists (filePath) == false) {
			return false;
		}

		return true;
	}

	void saveData()
	{
		initialiseFolderDirectory ();
		string pathForDir = Path.Combine (folderPath, profileDir);
		if (Directory.Exists (pathForDir) == false) {
			Directory.CreateDirectory (pathForDir);
			Debug.Log ("Created Profile Directory");
		} else {
			Debug.Log ("Profile Directory Exists");
		}
	}

	void writePlayerData(){
		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");
		if (Directory.Exists (playerDataPath) == false) {
			Directory.CreateDirectory (playerDataPath);
			Debug.Log ("Created Player Data Path");
		} else {
			Debug.Log ("Found Player Data Directory");
		}

		writeListToFile (playerDataPath, "Inventory.txt", playerItems.ToArray());
		writeListToFile (playerDataPath, "Clothes.txt", playerClothes.ToArray ());
		writeListToFile (playerDataPath, "Equiped.txt", playerEquipped.ToArray ());
		writeListToFile (playerDataPath, "PortableContainer.txt", playerPortableContainer.ToArray ());

		List<string> dateInfo =new List<string> ();
		//min,hour,day,month,year
		dateInfo.Add(TimeScript.me.minute.ToString());
		dateInfo.Add (TimeScript.me.hour.ToString());
		dateInfo.Add (TimeScript.me.day.ToString());
		dateInfo.Add (TimeScript.me.month.ToString());
		dateInfo.Add (TimeScript.me.year.ToString());

		writeListToFile (playerDataPath, "DateTime.txt", dateInfo.ToArray ());


		if (File.Exists (Path.Combine (playerDataPath, "StashValue.txt")) == false) {
			List<string> stash = new List<string> ();
			stash.Add ("0");
			writeListToFile (playerDataPath, "StashValue.txt", stash.ToArray ());
		}

		if (Stash.me == null) {
			
		} else {
			List<string> stash = new List<string> ();
			stash.Add (Stash.me.stashValue.ToString ());
			writeListToFile (playerDataPath, "StashValue.txt", stash.ToArray ());
		}

		changeActiveLevel ();
		storeLevelData ();
		//need files for inventory,clothes,equipped,
	}
	public List<string> miscData;
	void readPlayerData()
	{
		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");

		playerItems = readFile (playerDataPath, "Inventory.txt");
		playerClothes = readFile (playerDataPath, "Clothes.txt");
		playerEquipped = readFile (playerDataPath, "Equiped.txt");
		playerPortableContainer = readFile (playerDataPath, "PortableContainer.txt");

		List<string> misc = readFile (playerDataPath, "MiscData.txt");
		playerHealth = float.Parse (misc [0]);
		if (misc [1] == "True") {
			playerBleeding = true;
		} else {
			playerBleeding = false;
		}
		miscData = misc;
	}

	void writeListToFile(string directory,string fileName,string[] data){
		string path = Path.Combine (directory, fileName);
		StreamWriter writer = new StreamWriter (path,false);
		foreach (string st in data) {
			writer.WriteLine (st);
		}
		writer.Close ();
		//Debug.Log ("Wrote " + fileName);
	}

	List<string> readFile(string directory, string fileName)
	{
		List<string> retVal = new List<string> ();
		string path = Path.Combine (directory, fileName);
		StreamReader sr = new StreamReader (path);
		string st = sr.ReadLine (); 
		while (st!=null) {
			retVal.Add (st);
			st = sr.ReadLine ();
		}
		sr.Close ();
		return retVal;
	}

	void changeActiveLevel()
	{
		string playerDataPath = Path.Combine (folderPath, profileDir);
		playerDataPath = Path.Combine (playerDataPath, "PlayerData");
		List<string> miscPlayerInfo = new List<string> ();
		miscPlayerInfo.Add (playerHealth.ToString ());
		if (playerBleeding == true) {
			miscPlayerInfo.Add ("True");
		} else {
			miscPlayerInfo.Add ("False");
		}
		miscPlayerInfo.Add (SceneManager.GetActiveScene ().name);
		writeListToFile (playerDataPath, "MiscData.txt", miscPlayerInfo.ToArray ());
	}

	void storeLevelData()
	{
		string levelDataPath = Path.Combine (folderPath, profileDir);
		levelDataPath = Path.Combine (levelDataPath, SceneManager.GetActiveScene ().name);

		if (Directory.Exists (levelDataPath) == false) {
			Directory.CreateDirectory (levelDataPath);
		}

		//ESSENTIAL STUFF
		//Containers - store in their own folder, have a text file for each named with the position in the world, then just find the nearest container to that position and fill it with the items in the file.
		//Will need to filter specific items e.g. keys, notes from that container.

		string containerDataPath = Path.Combine (levelDataPath, "Containers");
		if (Directory.Exists (containerDataPath) == false) {
			Directory.CreateDirectory (containerDataPath);
		}

		List<Container> containers = new List<Container> ();
		Container[] allContainers = FindObjectsOfType<Container> ();

		foreach (Container c in allContainers) {
			if (c.transform.root.tag == "NPC" || c.transform.root.tag == "Player" || c.transform.root.tag=="Dead/Knocked") {
				continue;
			} else {
				containers.Add (c);
			}
		}
		List<string> itemsToNotKeep = new List<string> ();
		itemsToNotKeep.Add ("Key");
		itemsToNotKeep.Add ("Note");
		itemsToNotKeep.Add ("Keycard");
		foreach (Container c in containers) {
			List<string> itemsWeKeep = new List<string> ();
			foreach (Item i in c.itemsInContainer) {
				if (itemsToNotKeep.Contains (i.itemName) == false) {
					itemsWeKeep.Add (serialiseItem(i));
				}
			}

			string name = vectorToString(c.transform.position) + ".txt";
			writeListToFile(containerDataPath,name,itemsWeKeep.ToArray());
		}

		//Doors - Store in their own folder, have a text file named with their position (only if locked) then recording the method its locked with, the position of the relevant container & the item name of what is stored in it
		//will also have to record whether they are destroyed and when they were destroyed
		string doorDataPath = Path.Combine (levelDataPath, "Doors");

		if (Directory.Exists (doorDataPath) == false) {
			Directory.CreateDirectory (doorDataPath);
		}

		DoorScript[] doorsInLevel = getAllDoorsInLevel().ToArray();
		foreach (DoorScript ds in doorsInLevel) {
				List<string> doorData = new List<string> ();
				doorData.Add (ds.doorHealth.ToString ());

				//healthOfDoor
				if (ds.wayIAmLocked == lockedWith.none) {

				} else if (ds.wayIAmLocked == lockedWith.key) {
					//locationOfContainer
					//if (ds.myKey == null) {
					//	ds.createMyKey ();
					//}
						Item i = ds.myKey.GetComponent<Item> ();
						if (Inventory.playerInventory.inventoryItems.Contains (i) == false) {
							doorData.Add (vectorToString (getWorldPos(i.myContainer.gameObject)));
						} else {
							doorData.Add ("ON PLAYER");
						}
						doorData.Add (i.myID.ToString ());

				Debug.Log ("WROTE LOCKED DOOR TO FILE, ID WAS " + i.myID.ToString ());
				} else if (ds.wayIAmLocked == lockedWith.passcode) {
					//locationOfContainer
				//if (ds.myCode == null) {
				//	ds.createMyKey ();
			//	}
						Item i = ds.myCode.GetComponent<Item> ();
				doorData.Add (vectorToString (getWorldPos(i.myContainer.gameObject)));
						//passcode number
						doorData.Add (ds.keycodeNumber.ToString ());
						doorData.Add (ds.myCode.GetComponent<NoteItem> ().noteText);
					
				} else if (ds.wayIAmLocked == lockedWith.keycard) {
					//locationOfContainer
				///if (ds.myKey == null) {
				//	ds.createMyKey ();
				//}
					Item i = ds.myKey.GetComponent<Item> ();
					if (Inventory.playerInventory.inventoryItems.Contains (i) == false) {
						doorData.Add (vectorToString (getWorldPos(i.myContainer.gameObject)));
					} else {
						doorData.Add ("ON PLAYER");
					}
					doorData.Add (i.myID.ToString ());
				}
				string name = vectorToString (getWorldPos(ds.gameObject)) + ".txt";
				writeListToFile (doorDataPath, name, doorData.ToArray ());
			//will need to add some stuff to record when the door is destroyed
		}
		string windowDataPath = Path.Combine (levelDataPath, "Windows");
		if (Directory.Exists (windowDataPath)==false) {
			Directory.CreateDirectory (windowDataPath);
		}
		//windows - Store in their own folder, whether they are destroyed or not and what time they were destroyed
		WindowNew[] windows = getAllWindowsInLevel().ToArray();
		foreach (WindowNew w in windows) {
			List<string> windowData = new List<string> ();
			windowData.Add (w.windowDestroyed.ToString ());
			string name = vectorToString(getWorldPos(w.gameObject)) + ".txt";
			writeListToFile (windowDataPath, name, windowData.ToArray ());
		}


		serializeIncidents ();
		//LESS ESSENTIAL STUFF
		//Dead people, store the number of people killed in the level, have chalk outlines/police tape, flower memorials, closed buildings 
		//Buildings - have buildings be closed if they get damaged e.g. molotovs/explosives used, doors or windows destroyed. have workmen appear?
	}


	string serialiseItem(Item i)
	{
		string retval = i.itemName;
		if (i.gameObject.GetComponent<AmmoItem> () == true) {
			retval += ";" + i.GetComponent<AmmoItem> ().ammoCount;
		} else if (i.itemName.Contains ("Key")) {
			retval += ";" + i.myID;
		} else if(i.GetComponent<NoteItem>()==true){
			retval += ";"+i.GetComponent<NoteItem> ().noteText;
		}
		return retval;
	}

	string getItemName(string data)
	{
		string[] split = new string[1];
		split [0] = ";";
		string[] lines = data.Split (split, System.StringSplitOptions.RemoveEmptyEntries);
		return lines [0];
	}

	GameObject deserialiseItem(string data)
	{
		string[] split = new string[1];
		split [0] = ";";
		string[] lines = data.Split (split, System.StringSplitOptions.RemoveEmptyEntries);
		GameObject item = (GameObject)Instantiate(ItemDatabase.me.getItem(lines[0]),Vector3.zero,Quaternion.Euler(Vector3.zero));
		Item itemScript = item.GetComponent<Item> ();
		if (item.gameObject.GetComponent<AmmoItem> () == true) {
		//	retval += ";" + i.GetComponent<AmmoItem> ().ammoCount;
			AmmoItem ai = item.GetComponent<AmmoItem>();
			ai.ammoCount = int.Parse (lines [1]);
		} else if (itemScript.itemName.Contains ("Key")) {
		//	retval += ";" + i.myID;
			itemScript.myID = int.Parse(lines[1]);
		} else if(item.GetComponent<NoteItem>()==true){
		//	retval += i.GetComponent<NoteItem> ().noteText;
			item.GetComponent<NoteItem>().noteText = lines[1];
		}
		return item;
	}

	void recreateLevelData()
	{
		string levelDataPath = Path.Combine (folderPath, profileDir);
		levelDataPath = Path.Combine (levelDataPath, SceneManager.GetActiveScene ().name);
		Inventory playerInv = GameObject.FindGameObjectWithTag ("Player").GetComponent<Inventory> ();
		Item[] itemsInWorld =playerInv.inventoryItems.ToArray();
		if (Directory.Exists (levelDataPath) == false) {
			Debug.Log ("No level data found");
			return;
		}

		string containerDataPath = Path.Combine (levelDataPath, "Containers");
		if (Directory.Exists (containerDataPath) == false) {
			Debug.Log ("No containers found");
			return;
		}
		Container[] allContainers = FindObjectsOfType<Container> ();

		string[] files = Directory.GetFiles (containerDataPath);
		foreach (string st in files) {
			string s = st.Replace (containerDataPath, "");
//			Debug.Log ("Filename is " + s);
			Vector3 v = parseVector (s);
		//	Debug.Log ("Vector found is " + v.ToString ());
			Container c = null;
			float d = 9999999.0f;
			foreach(Container c2 in allContainers)
			{
				float dist = Vector2.Distance (v, getWorldPos(c2.gameObject));
				if (dist < d) {
					c = c2;
					d = dist;
				}
			}
			Debug.Log ("Container path = " + containerDataPath);
			List<string> items = readFile (containerDataPath, st);

			foreach(string it in items)
			{
				GameObject g = deserialiseItem (it);
				g.transform.position = c.transform.position;
				c.addItemToContainer (g.GetComponent<Item> ());
			}
			c.initialised = true;
		}

		string doorDataPath = Path.Combine (levelDataPath, "Doors");

		if (Directory.Exists (doorDataPath) == false) {
			return;
		}

		string[] doorFiles = Directory.GetFiles (doorDataPath);
		DoorScript[] doors = getAllDoorsInLevel().ToArray();
		foreach (string st in doorFiles) {
			string s = st.Replace (doorDataPath, "");
			Vector3 v = parseVector (s);

			float d = 999999.0f;
			DoorScript door = null;

			foreach (DoorScript ds in doors) {
				if (ds.isTempDoor == true) {

				} else {
					float d2 = Vector2.Distance (getWorldPos(ds.gameObject), v);
					if (d2 < d) {
						d = d2;
						door = ds;
					}
				}
			}


//			Debug.Log ("DOOR " + door.gameObject.name + " WAS FOUND FROM FILE " + st + " AT POSITION " + door.transform.position);
			List<string> doorDat = readFile (doorDataPath, st);
			int health = int.Parse (doorDat [0]);
			door.doorHealth = health;
			if (door.doorHealth == 0) {
				door.destroyDoor ();
			}
			if (door.wayIAmLocked == lockedWith.key) {
				//Debug.Log ("DOOR " + door.gameObject.name + " WAS LOCKED WITH KEY");
				if (doorDat [1] == "ON PLAYER") {
					int id = int.Parse (doorDat [2]);
					bool idAlreadyExists = false;
					foreach (Item i in itemsInWorld) {
						if (i.myID == id) {
							door.myKey = i.gameObject;
							idAlreadyExists = true;
						}
					}
					door.myID = id;
				} else {
					
					Vector3 containerPos = parseVector (doorDat [1]);
					int id = int.Parse (doorDat [2]);
					door.myID = id;
					bool idAlreadyExists = false;
					foreach (Item i in itemsInWorld) {
						if (i.myID == id) {
							door.myKey = i.gameObject;
							idAlreadyExists = true;
						}
					}
					if (idAlreadyExists == false) {
						GameObject g = (GameObject)Instantiate (ItemDatabase.me.getItem ("Key"), containerPos, Quaternion.Euler (0, 0, 0));
						Item i = g.GetComponent<Item> ();
						i.myID = id;
						Container containerForKey = null;
						float d2 = 999999.0f;
						foreach (Container c2 in allContainers) {
							float dist = Vector2.Distance (containerPos, getWorldPos(c2.gameObject));
							if (dist < d2) {
								containerForKey = c2;
								d2 = dist;
							}
						}
						door.myKey = i.gameObject;
						Debug.Log ("CONTAINER " + containerForKey.gameObject.name + " HAS KEY");
						containerForKey.addItemToContainer (i);
					}
				}
			} else if (door.wayIAmLocked == lockedWith.keycard) {
				if (doorDat [1] == "ON PLAYER") {
					int id = int.Parse (doorDat [2]);
					door.myID = id;

					bool idAlreadyExists = false;
					foreach (Item i in itemsInWorld) {
						if (i.myID == id) {
							door.myKey = i.gameObject;
							idAlreadyExists = true;
						}
					}
				} else {

					Vector3 containerPos = parseVector (doorDat [1]);
					int id = int.Parse (doorDat [2]);
					door.myID = id;
					bool idAlreadyExists = false;
					foreach (Item i in itemsInWorld) {
						if (i.myID == id) {
							door.myKey = i.gameObject;
							idAlreadyExists = true;
						}
					}
					if (idAlreadyExists == false) {
						GameObject g = (GameObject)Instantiate (ItemDatabase.me.getItem ("Keycard"), containerPos, Quaternion.Euler (0, 0, 0));
						Item i = g.GetComponent<Item> ();
						i.myID = id;
						Container containerForKey = null;
						float d2 = 999999.0f;
						foreach (Container c2 in allContainers) {
							float dist = Vector2.Distance (containerPos, getWorldPos(c2.gameObject));
							if (dist < d2) {
								containerForKey = c2;
								d2 = dist;
							}
						}
						containerForKey.addItemToContainer (i);
						door.myKey = g;
					}
				}
				//1 = container pos
				//2 = ID
			} else if (door.wayIAmLocked == lockedWith.passcode) {

				Debug.Log ("Door data found = " + doorDat.Count);
				string st3 = "";
				foreach(string st2 in doorDat)
				{
					st3 += st2;
				}
				Debug.Log("door date was " + st);
				Vector3 containerPos = parseVector (doorDat [1]);
				int code = int.Parse (doorDat [2]);
				door.keycodeNumber = code;
				//int id = int.Parse (doorDat [2]);
				bool idAlreadyExists = false;
				foreach (Item i in itemsInWorld) {
					if (i.GetComponent<NoteItem> () == true) {
						if (i.GetComponent<NoteItem> ().noteText == doorDat [3]) {
							door.myCode = i.gameObject;

							idAlreadyExists = true;
						}
					}
				
				}
				if (idAlreadyExists == false) {
					GameObject g = (GameObject)Instantiate (ItemDatabase.me.getItem ("Note"), containerPos, Quaternion.Euler (0, 0, 0));
					Item i = g.GetComponent<Item> ();
					//i.myID = id;
					Container containerForKey = null;
					float d2 = 999999.0f;
					foreach (Container c2 in allContainers) {
						float dist = Vector2.Distance (containerPos, getWorldPos(c2.gameObject));
						if (dist < d2) {
							containerForKey = c2;
							d2 = dist;
						}
					}
					door.myCode = i.gameObject;
					containerForKey.addItemToContainer (i);
					g.GetComponent<NoteItem> ().noteText = doorDat [3];
				}
				//1 = container pos
				//2 = ID
			} else {

			}
			if (door.doorHealth > 0) {
				door.setDoorActions ();
			}
			door.initialised = true;
		}

		string windowPath = Path.Combine (levelDataPath, "Windows");

		if (Directory.Exists (windowPath) == false) {
			return;
		}
		string[] windowFiles = Directory.GetFiles (windowPath);
		WindowNew[] windows = getAllWindowsInLevel().ToArray();

		foreach (string st in windowFiles) {
			string s = st.Replace (windowPath, "");
			Vector3 v = parseVector (s);

			float d = 999999.0f;
			WindowNew nearest = null;

			foreach (WindowNew w in windows) {
				float d2 = Vector2.Distance (getWorldPos(w.gameObject), v);
				if (d2 < d) {
					d = d2;
					nearest = w;
				}
			}

			List<string> windowData = readFile (windowPath, st);
			if (windowData [0] == "True") {
				nearest.windowDestroyed = true;
			} else {
				nearest.windowDestroyed = false;
			}

			if (nearest.windowDestroyed == true) {
				nearest.setDestroyed ();
			}

		}

		deserializeIncidents ();
	}

	void serializeIncidents()
	{
		string levelDataPath = Path.Combine (folderPath, profileDir);
		levelDataPath = Path.Combine (levelDataPath, SceneManager.GetActiveScene ().name);

		string incidentDataPath = Path.Combine (levelDataPath, "Incidents");
		if (Directory.Exists (incidentDataPath) == false) {
			Directory.CreateDirectory (incidentDataPath);
		}

		List<string> incidentData = LevelIncidentController.me.getIncidentsAsFile ();
		writeListToFile (incidentDataPath, "Incidents.txt", incidentData.ToArray ());
	}

	void deserializeIncidents()
	{
		//name;position;min;hour;day;month;year
		string levelDataPath = Path.Combine (folderPath, profileDir);
		levelDataPath = Path.Combine (levelDataPath, SceneManager.GetActiveScene ().name);
		string incidentDataPath = Path.Combine (levelDataPath, "Incidents");
		string[] semi = new string[]{ ";" };
		List<string> incidentData = readFile (incidentDataPath, "Incidents.txt");
		foreach (string st in incidentData) {
			string[] splits = st.Split (semi, System.StringSplitOptions.RemoveEmptyEntries);
			string name = splits [0];
			Vector3 pos = parseVector (splits [1]);
			int min = int.Parse (splits [2]);
			int hour = int.Parse (splits [3]);
			int day = int.Parse (splits [4]);
			int month = int.Parse (splits [5]);
			int year = int.Parse (splits [6]);
			LevelIncidentController.me.createIncidents (name, pos, min, hour, day, month, year);
		}
	}

	Vector3 parseVector(string st)
	{
		Vector3 retval = Vector3.zero;
		st = st.Remove (0,1);
		//st.Replace ("\\", "");
		st = st.Replace ("(", "");
		st = st.Replace (")", "");

		st = st.Replace (".txt", "");
		string[] com = new string[1];
		com [0] = ",";
		string[] split = st.Split (com, System.StringSplitOptions.RemoveEmptyEntries);

		retval = new Vector3 (float.Parse (split [0]), float.Parse (split [1]), float.Parse (split [2]));
		return retval;
	}

	string vectorToString(Vector3 v)
	{
		string retVal = "";
		string x = v.x.ToString();
		string y = v.y.ToString ();
		string z = v.z.ToString();

		retVal += "(" + x + "," + y + "," + z + ")";

		return retVal;
	}

	List<DoorScript> getAllDoorsInLevel()
	{
		List<DoorScript> retVal = new List<DoorScript> ();
		DoorScript[] doors = Resources.FindObjectsOfTypeAll<DoorScript> ();


		foreach (DoorScript d in doors) {
			if (d.gameObject.scene.IsValid() == false) {

			} else if (d.isTempDoor == true) {

			}else {
				retVal.Add (d);
			}
		}
		debugDispDoors = retVal;
		return retVal;
	}
	public List<DoorScript> debugDispDoors;
	List<WindowNew> getAllWindowsInLevel()
	{
		List<WindowNew> retVal = new List<WindowNew> ();
		WindowNew[] windows = Resources.FindObjectsOfTypeAll<WindowNew> ();

		foreach (WindowNew w in windows) {
			if (w.gameObject.scene.IsValid() == false) {

			} else {
				retVal.Add (w);
			}
		}

		return retVal;
	}

	Vector3 getWorldPos(GameObject g)
	{
		return g.transform.root.TransformPoint (g.transform.position);
	}
}
