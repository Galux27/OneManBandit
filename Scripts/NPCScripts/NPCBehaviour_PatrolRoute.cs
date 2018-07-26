using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour_PatrolRoute : NPCBehaviour {

	public PatrolRoute myRoute;

	int counter = 0;
	public override void Initialise()
	{
		myController = this.gameObject.GetComponent<NPCController> ();

		if (myRoute == null) {
			if (myController.memory.myRoute == null) {
				myRoute = LevelController.me.getRandomRoute ();
			} else {
				myRoute = myController.memory.myRoute;
			}
		}
		myController.memory.myRoute = myRoute;
		myType = behaviourType.patrol;
		myController.pf.getPath (this.gameObject, myRoute.pointsInRoute [counter]);
		////Debug.LogError ("get path 1");
		radioMessageOnStart ();
		isInitialised = true;
	}

	public override void OnUpdate()
	{
		if (isInitialised == false) {
			Initialise ();
		}
		//do some shit
		pointMoniter();
		moveToCurrentPoint ();

	}

	void pointMoniter()
	{
		if (myController.pf.target != myRoute.pointsInRoute [counter]) {
			myController.pf.getPath (this.gameObject, myRoute.pointsInRoute [counter]);
			////Debug.LogError ("get path 2");

		}

		if (Vector2.Distance (this.transform.position, myRoute.pointsInRoute [counter].transform.position) < 2.5f) {
			if (counter < myRoute.pointsInRoute.Count - 1) {
				counter++;
				myController.pf.getPath (this.gameObject, myRoute.pointsInRoute [counter]);
				////Debug.LogError ("get path 3");

			} else if(counter>=myRoute.pointsInRoute.Count-1){
				counter = 0;
				myController.pf.getPath (this.gameObject, myRoute.pointsInRoute [counter]);
				////Debug.LogError ("get path 4");

			}
		}
	}

	void moveToCurrentPoint()
	{
		myController.pmc.moveToDirection (myController.pf.getCurrentPoint ());

		if (myController.detect.fov.foundPlayer == false) {
			myController.pmc.rotateToFacePosition (myController.pf.getCurrentPoint());

		} else {
			myController.pmc.rotateToFacePosition (CommonObjectsStore.player.transform.position);

		}
	}

	public override void OnComplete()
	{

	}

	public void setRoute(PatrolRoute route)
	{
		myRoute = route;
	}

	public override void passInGameobject(GameObject passIn)
	{
		myRoute = passIn.GetComponent<PatrolRoute> ();
	}

	public override void radioMessageOnStart ()
	{
		radioHackBand h = radioHackBand.buisness;

		if (myController.npcB.myType == AIType.civilian) {

		} else {
			if (myController.npcB.myType == AIType.cop) {
				h = radioHackBand.cop;
			} else if (myController.npcB.myType == AIType.swat) {
				h = radioHackBand.swat;
			}


				PhoneTab_RadioHack.me.setNewText ("This is "+this.gameObject.name+ ", returning to patrol",h);


		}
	}



}
