using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCopsOnCall : MonoBehaviour {
	PlayerCarController myPCC;
	NewRoadFollower myRF;
	bool triggered=false;
	public GameObject cop;
	public copSpawnType myType;
   public float spawnTimer = 5.0f;
	// Use this for initialization
	void Start () {
		myRF = this.GetComponent<NewRoadFollower> ();
		myPCC = this.GetComponent<PlayerCarController> ();
        spawnTimer = Random.Range(5.0f, 10.0f);


    }

    // Update is called once per frame
    void Update () {
		if (myPCC.stolen == true || myPCC.playerInCar == true) {
			Destroy (this);
		}
		if (shouldWeSpawnCops ()) {
			spawnCops ();
		}
	}

	bool shouldWeSpawnCops()
	{
		if (triggered == true) {
			return false;
		}

		if (canWeSpawnCops () == false) {
			return false;
		}
		if (myType == copSpawnType.patrol) {
			if (PoliceController.me.copsCalled == true || PoliceController.me.backupCalled == true || PoliceController.me.swatCalled == true) {
				return true;
			}
		} else if (myType == copSpawnType.normal) {
			if (PoliceController.me.copsHere == true || PoliceController.me.backupHere == true) {
				return true;
			}
		} else {
			if (PoliceController.me.swatHere == true) {
				return true;
			}
		}
		return false;
	}

	NewRoad r;
	bool canWeSpawnCops()
	{
		


		if (r == null) {
			r = FindObjectOfType<NewRoad> ();
		}

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
           // if (myRF.areBothJunctionsPlayerAccessable())
          //  {
             //   return true;
          //  }

            foreach (NewRoadJunction nrj in r.sectionsInTheRoad)
            {
                if (nrj.playerCarSpawn == true)
                {
                    if (Vector2.Distance(this.transform.position, nrj.startPoint.position) < 3.0f)
                    {

                        return true;
                    }
                }
            }
        }
		return false;
	}

	public void spawnCops()
	{
		if (myType == copSpawnType.normal) {
			GameObject g = (GameObject)Instantiate (cop, myPCC.driversDoor.transform.position, Quaternion.Euler (0, 0, 0));
			GameObject g2 = (GameObject)Instantiate (cop, myPCC.altCarDoors [0].transform.position, Quaternion.Euler (0, 0, 0));
		} else if (myType == copSpawnType.patrol) {
			for (int x = 0; x < 4; x++) {
				Vector3 pos = Vector3.zero;
				if (x == 0) {
					pos = myPCC.driversDoor.transform.position;
				} else {
					pos= myPCC.altCarDoors[x-1].transform.position;
				}
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.cop, pos, Quaternion.Euler (0, 0, 0));
				PoliceController.me.copsInLevel.Add (g);
			}
		} else {
			for (int x = 0; x < 4; x++) {
				Vector3 pos = Vector3.zero;
				if (x == 0) {
					pos = myPCC.driversDoor.transform.position;
				} else {
					pos= myPCC.altCarDoors[x-1].transform.position;
				}
				GameObject g = (GameObject)Instantiate (CommonObjectsStore.me.swat, pos, Quaternion.Euler (0, 0, 0));
				PoliceController.me.copsInLevel.Add (g);
			}
		}
		myRF.hasDriver = false;
		triggered = true;
		Destroy (this);

	}
}

public enum copSpawnType{
	patrol,
	normal,
	swat
}
