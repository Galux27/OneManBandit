using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor {
	LevelEditorTask currentTask;
	bool edit=false,rotation=false;
	Vector3 pos;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnInspectorGUI ()
	{
		DrawDefaultInspector ();
	}
	Vector2 menuScroll=Vector2.zero;

	void OnSceneGUI()
	{
		pos = Event.current.mousePosition;
		pos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - pos.y;
		pos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint (pos);

		Handles.BeginGUI ();
		menuScroll = GUILayout.BeginScrollView (menuScroll, GUILayout.Width (1000), GUILayout.Height (75));
		GUILayout.BeginHorizontal ();

		if (GUILayout.Button ("Place Roads", GUILayout.Width (100), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.roads;
		}

		if (GUILayout.Button ("Place Enemies", GUILayout.Width (100), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.enemies;
		}

		if (GUILayout.Button ("Place Police", GUILayout.Width (100), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.police;
		}

		if (GUILayout.Button ("Create Patrol Routes", GUILayout.Width (150), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.patrolRoutes;
		}

		if (GUILayout.Button ("Create Civilian Spawns", GUILayout.Width (150), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.civSpawns;
		}

		if (GUILayout.Button ("Create Civilian Actions", GUILayout.Width (150), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.civActions;
		}

		if (GUILayout.Button ("Place Light Sources", GUILayout.Width (150), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.lightSources;
		}

		if (GUILayout.Button ("Place CCTV Cameras", GUILayout.Width (150), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.cctv;
		}

		if (GUILayout.Button ("Place OOB Marker", GUILayout.Width (125), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.outOfBounds;
		}

		if (GUILayout.Button ("Place Nature", GUILayout.Width (125), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.nature;
		}

		GUILayout.EndHorizontal ();
		GUILayout.EndScrollView ();
		GUILayout.BeginVertical ();
		if (currentTask == LevelEditorTask.civActions) {
			GUILayout.Label ("Currently Editing Civilian Actions", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.civSpawns) {
			GUILayout.Label ("Currently Editing Civilian Spawns", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.roads) {
			GUILayout.Label ("Currently Editing Civilian Roads", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.police) {
			GUILayout.Label ("Currently Editing Police", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.patrolRoutes) {
			GUILayout.Label ("Currently Editing Patrol Routes", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.enemies) {
			GUILayout.Label ("Currently Placing Enemies", GUILayout.Width (300), GUILayout.Height (25));

		} else if (currentTask == LevelEditorTask.lightSources) {
			GUILayout.Label ("Placing Light Sources", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.cctv) {
			GUILayout.Label ("Placing CCTV Cameras", GUILayout.Width (300), GUILayout.Height (25));
		} else if (currentTask == LevelEditorTask.outOfBounds) {
			GUILayout.Label ("Placing OOB Areas", GUILayout.Width (300), GUILayout.Height (25));

		} else if (currentTask == LevelEditorTask.nature) {
			GUILayout.Label ("Placing Nature", GUILayout.Width (300), GUILayout.Height (25));

		}

		if (edit == true) {
			GUILayout.Label ("Editing is enabled", GUILayout.Width (150));
			if(GUILayout.Button("Disable Editing",GUILayout.Width(100))){
				edit = false;
			}
		} else {
			GUILayout.Label ("Editing is disabled", GUILayout.Width (150));
			if(GUILayout.Button("Enable Editing",GUILayout.Width(100))){
				edit = true;
			}
		}
			
		GUILayout.EndVertical ();

		if (currentTask == LevelEditorTask.civActions) {
			civilianActionStuff ();
		} else if (currentTask == LevelEditorTask.civSpawns) {
			civilianSpawnStuff ();
		} else if (currentTask == LevelEditorTask.roads) {
			RoadControl ();
		} else if (currentTask == LevelEditorTask.police) {
			policePlacementController ();
		} else if (currentTask == LevelEditorTask.patrolRoutes) {
			patrolRouteControl ();
		} else if (currentTask == LevelEditorTask.enemies) {
			placeEnemiesControl ();
			drawEnemyHandles ();

		} else if (currentTask == LevelEditorTask.lightSources) {
			lightControl ();
			drawLightUI ();
		} else if (currentTask == LevelEditorTask.cctv) {
			inputDetect ();
			drawCCTVGUI ();
		} else if (currentTask == LevelEditorTask.nature) {
			natureGUI ();	
		}
	


		Handles.EndGUI ();

		if (currentTask == LevelEditorTask.civActions) {
			if (toEdit == null) {

			} else {
				if (edit == true) {
					drawCivilianActionHandles ();
				}
			}
			civilianActionHandles ();

		} else if (currentTask == LevelEditorTask.roads) {
			drawRoadLines ();
			DrawRoadGizmos ();
		} else if (currentTask == LevelEditorTask.police) {
			drawPoliceHandles ();
		} else if (currentTask == LevelEditorTask.patrolRoutes) {
			drawPatrolrouteGizmos ();
		} else if (currentTask == LevelEditorTask.enemies) {
			drawEnemyGizmos ();
			drawPatrolrouteGizmos ();

		} else if (currentTask == LevelEditorTask.lightSources) {
			drawLightHandles ();
		} else if (currentTask == LevelEditorTask.cctv) {
			drawCCTVHandles ();
		} else if (currentTask == LevelEditorTask.outOfBounds) {
			drawAreaForPoints ();
			inputDetect ();
			drawOOBHandles ();
		}else if (currentTask == LevelEditorTask.nature) {
			natureHandles ();

		}
	}

	CivilianAction toEdit;

	void civilianActionHandles()
	{
		LevelEditor le = (LevelEditor)target;

		foreach (GameObject g in le.civilianActionsCreated) {
			if (toEdit == null || toEdit.gameObject == g) {
				CivilianAction ca = g.GetComponent<CivilianAction> ();
				Handles.Label (g.transform.position + new Vector3 (0, -0.5f, 0), "Action " + ca.actionName);

				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.civilianActionsCreated.Remove (g);
					EditorUtility.SetDirty (le);
					DestroyImmediate (g);
					return;
				}

				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position+new Vector3(1,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if(toEdit==null){
						toEdit = g.GetComponent<CivilianAction> ();
						edit = true;
					}
					else{
						toEdit=null;
						edit = false;
					}
				}
			}
			Handles.color = Color.white;
		}
	}

	void civilianActionStuff()
	{
		if (edit == false) {
			GUILayout.Label ("Press CTRL + Right Click to create a new civilian action",GUILayout.Width(350),GUILayout.Height(25));
			inputDetect ();
		} else {
			if (toEdit == null) {
				GUILayout.BeginVertical ();
				LevelEditor le = (LevelEditor)target;
				foreach (GameObject g in le.civilianActionsCreated) {
					CivilianAction ca = g.GetComponent<CivilianAction> ();

					if (GUILayout.Button ("Action: " + ca.actionName, GUILayout.Width (125), GUILayout.Height (25))) {
						toEdit = ca;
					}
				}

				GUILayout.EndVertical ();
			} else {
				GUILayout.BeginVertical ();

				GUILayout.Label ("Editing Civilian Action " + toEdit.actionName, GUILayout.Width (200), GUILayout.Height (25));

				if (GUILayout.Button ("Stop Editing", GUILayout.Width (100), GUILayout.Height (25))) {
					toEdit = null;
				}

				if (toEdit == null) {
					return;
				}

				GUILayout.Label ("Action time:", GUILayout.Width (100), GUILayout.Height (25));
				toEdit.resetValue = float.Parse (GUILayout.TextField (toEdit.resetValue.ToString (), GUILayout.Width (100), GUILayout.Height (25)));

				GUILayout.Label ("Action Name:", GUILayout.Width (100), GUILayout.Height (25));
				toEdit.actionName = GUILayout.TextField (toEdit.actionName, GUILayout.Width (100), GUILayout.Height (25));

				if (GUILayout.Button ("Is sitting action " + toEdit.sitting.ToString (), GUILayout.Width (150), GUILayout.Height (25))) {
					toEdit.sitting = !toEdit.sitting;
				}

				if (rotation == false) {
					if (GUILayout.Button ("Change rotation of action", GUILayout.Width (200), GUILayout.Height (25))) {
						rotation = true;
					}
				} else {
					if (GUILayout.Button ("Change position of action", GUILayout.Width (200), GUILayout.Height (25))) {
						rotation = false;
					}
				}

				if (toEdit.positionForAction == null) {
					if (GUILayout.Button ("Create seperate position for action", GUILayout.Width (250), GUILayout.Height (25))) {
						GameObject g = new GameObject ();
						g.transform.parent = toEdit.transform;
						g.name = toEdit.actionName + " action location";
						g.transform.localPosition = new Vector3 (0, 1, 0);
						toEdit.positionForAction = g.transform;
					}
				}



				if (GUILayout.Button ("Destroy Action", GUILayout.Width (100), GUILayout.Height (25))) {
					destroyAction (toEdit.gameObject);
				}



				//will need to add sprite renderer
				EditorUtility.SetDirty(toEdit);
				EditorUtility.SetDirty (toEdit.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());
				GUILayout.EndVertical ();
			}
		}
	}

	void civilianSpawnStuff()
	{
		inputDetect ();
		drawSpawnPointHandles ();
	}

	void createCivilianAction()
	{
		LevelEditor le = (LevelEditor)target;

		GameObject g = (GameObject)Instantiate (le.civilianActionPrefab, getMousePos (), Quaternion.Euler (0, 0, 0));
		g.transform.parent = le.gameObject.transform;
		le.addCivilanAction (g);
		EditorUtility.SetDirty (g);
		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (le.gameObject);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());

	}

	void drawCivilianActionHandles()
	{
		if (rotation == false) {
			EditorGUI.BeginChangeCheck ();
			//Handles.Label (toEdit.transform.position, toEdit.gameObject.name);
			Vector3 pos2 = Handles.PositionHandle (toEdit.transform.position, toEdit.transform.rotation);

			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				toEdit.transform.position = pos2;

				EditorUtility.SetDirty (toEdit);
				EditorUtility.SetDirty (toEdit.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			if (toEdit.positionForAction == null) {

			} else {
				EditorGUI.BeginChangeCheck ();
				//Handles.Label (toEdit.transform.position, toEdit.gameObject.name);
				Vector3 pos3 = Handles.PositionHandle (toEdit.positionForAction.transform.position, toEdit.positionForAction.transform.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					Handles.Label (pos3, toEdit.actionName + " position");
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					toEdit.positionForAction.transform.position = pos3;

					EditorUtility.SetDirty (toEdit);
					EditorUtility.SetDirty (toEdit.gameObject);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}

			}
		} else {
			EditorGUI.BeginChangeCheck ();

			Quaternion rot = Handles.RotationHandle (toEdit.transform.rotation, toEdit.transform.position);

			if (EditorGUI.EndChangeCheck ()) {
				toEdit.transform.rotation = rot;
				EditorUtility.SetDirty (toEdit);
				EditorUtility.SetDirty (toEdit.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			if (toEdit.positionForAction == null) {

			} else {
				EditorGUI.BeginChangeCheck ();
				Handles.Label (toEdit.positionForAction.transform.position, toEdit.actionName + " rotation");

				Quaternion rot2 = Handles.RotationHandle (toEdit.positionForAction.transform.rotation, toEdit.positionForAction.transform.position);

				if (EditorGUI.EndChangeCheck ()) {
					toEdit.positionForAction.transform.rotation = rot2;
					EditorUtility.SetDirty (toEdit);
					EditorUtility.SetDirty (toEdit.positionForAction.gameObject);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}

			}
		}
	}

	void createCivilianSpawnPoint(){
		LevelEditor le = (LevelEditor)target;

		GameObject g = new GameObject ();
		g.transform.parent = le.gameObject.transform;
		g.name = "Civilian Spawn Point";

		g.transform.position = getMousePos ();
		CivilianController c = FindObjectOfType<CivilianController> ();
		c.addSpawnPoint (g);
		EditorUtility.SetDirty (c);
		EditorUtility.SetDirty (c.gameObject);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}
	CivilianController c;
	void drawSpawnPointHandles()
	{
		if (c == null) {
			c = FindObjectOfType<CivilianController> ();
		}

		foreach (Transform t in c.spawnPoints) {
			EditorGUI.BeginChangeCheck ();
			Handles.Label (t.position, "Civilian Spawn " + t.position.ToString());
			Vector3 pos2 = Handles.PositionHandle (t.position, t.rotation);
			Handles.Label (t.position+ new Vector3(0,-1,0), "Destroy");

			if (Handles.Button (t.position + new Vector3(0,-1,0), Quaternion.Euler (0, 0, 0), 1, 1,Handles.RectangleHandleCap)) {
				c.spawnPoints.Remove (t);
				DestroyImmediate (t.gameObject);
				EditorUtility.SetDirty (c);
				EditorUtility.SetDirty (c.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				return;
			}

			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				t.position = pos2;

				EditorUtility.SetDirty (c);
				EditorUtility.SetDirty (t.gameObject);

				EditorUtility.SetDirty (c.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}
	}

	void destroyAction(GameObject g)
	{
		LevelEditor le = (LevelEditor)target;
		le.civilianActionsCreated.Remove (g);
		DestroyImmediate (g);


		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	void inputDetect()
	{
		Event e = Event.current;

		switch (e.type) {
		case EventType.MouseUp:
			if (e.button == 1 && e.shift == true) {
				if (currentTask == LevelEditorTask.civActions) {
					createCivilianAction ();
				} else if (currentTask == LevelEditorTask.civSpawns) {
					createCivilianSpawnPoint ();
				} else if (currentTask == LevelEditorTask.roads) {
					createPointForRoad ();
				} else if (currentTask == LevelEditorTask.police) {
					if (placingCopRoute == false) {
						createPoliceCar ();
					} else {
						addNewPointToPoliceRoute ();
					}
				} else if (currentTask == LevelEditorTask.patrolRoutes) {
					if (editingRoute == true) {
						addNewPointToRoute ();
					} else {
						createNewPatrolRoute ();
					}
				} else if (currentTask == LevelEditorTask.enemies) {
					if (editingEnemy == false) {
						placeEnemy ();
					}
				} else if (currentTask == LevelEditorTask.lightSources) {
					if (editingLight == false) {
						placeLight ();
					} else {
						if (lightToEdit.GetComponent<Lightswitch> () == true) {
							setLightSwitchToSource ();
						}
					}
				} else if (currentTask == LevelEditorTask.cctv) {
					if (editCCTV == false) {
						createCCTV ();
					}
				} else if (currentTask == LevelEditorTask.outOfBounds) {
					if (pointsSet == false) {
						if (pointA == Vector3.zero) {
							setPointA ();
						} else {
							setPointB ();
						}
					} else {
						createOutOfBoundsMarker ();
					}
				} else if (currentTask == LevelEditorTask.nature) {
					if (eraseNature == true) {
						eraseNatureMethod ();
					}
				}
			} 


			return;
		}
	}

	NewRoad road;
	void RoadControl()
	{
		if (road == null) {
			if (FindObjectOfType<NewRoad> () == null) {
				GameObject g = new GameObject ();
				g.transform.position = Vector3.zero;
				g.name = "Road Controller";
				road = g.AddComponent<NewRoad> ();
			} else {
				road = FindObjectOfType<NewRoad> ();
			}
		}

		if (road.sectionsInTheRoad == null) {
			road.sectionsInTheRoad = new List<NewRoadJunction> ();
			NewRoadJunction r1 = createRoadSection (Vector3.zero);
			NewRoadJunction  r2 = createRoadSection (Vector3.zero + new Vector3(0,1,0));
			road.sectionsInTheRoad.Add (r1);
			road.sectionsInTheRoad.Add (r2);
			r1.potentialPoints.Add (r2);
		//	r2.potentialPoints.Add (r1);
		}
		if (areWeLinkingExisting == false) {
			nearestRoadSection = getNearestJunction ();
		}

		inputDetect ();
	}


	NewRoadJunction createRoadSection(Vector3 position)
	{
		GameObject g = new GameObject ();
		g.name = "Road Junction";
		NewRoadJunction r = g.AddComponent<NewRoadJunction> ();
		g.transform.parent = road.transform;
		g.transform.position = position;
		r.startPoint = r.transform;
		r.potentialPoints = new List<NewRoadJunction> ();

		EditorUtility.SetDirty (g);
		//EditorUtility.SetDirty (r);
		//EditorUtility.SetDirty (r.gameObject);
		EditorUtility.SetDirty (road);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());

		return r;
	}



	void DrawRoadGizmos()
	{
		foreach (NewRoadSection rs in road.sectionsInTheRoad) {
			EditorGUI.BeginChangeCheck ();
			//Handles.Label (toEdit.transform.position, toEdit.gameObject.name);
			Vector3 pos2 = Handles.PositionHandle (rs.startPoint.transform.position,rs.startPoint.transform.rotation);

			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				rs.startPoint.transform.position = pos2;

				EditorUtility.SetDirty (rs);
				EditorUtility.SetDirty (rs.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}
	}
	bool areWeLinkingExisting=false;
	void drawRoadLines()
	{

		foreach (NewRoadJunction rs in road.sectionsInTheRoad) {
			List<Transform> roadLines = rs.getPoints ();

			if (nearestRoadSection == rs) {
				Handles.color = Color.red;
			} else {
				if (areWeLinkingExisting == false) {
					Handles.color = Color.white;
				} else {
					Handles.color = Color.blue;
				}
			}

			if (rs.isJunction () == false) {
				Handles.Label (rs.startPoint.position, "Road Point " + road.sectionsInTheRoad.IndexOf (rs));
			} else {
				Handles.Label (rs.startPoint.position, "Road Junction " + road.sectionsInTheRoad.IndexOf (rs));

			}

			Handles.color = Color.blue;
			if (Handles.Button (rs.startPoint.position + new Vector3 (0, 1, 0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
				areWeLinkingExisting = true;
			}
			if (areWeLinkingExisting == false) {
				Handles.color = Color.red;
				if (Handles.Button (rs.startPoint.position + new Vector3 (1, 1, 0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					foreach (NewRoadJunction r in road.sectionsInTheRoad) {
						if (r.potentialPoints.Contains (rs)) {
							r.potentialPoints.Remove (rs);
						}
					}
					road.sectionsInTheRoad.Remove (rs);
					DestroyImmediate (rs.gameObject);
					return;
				}
				Handles.color = Color.white;
			}
			foreach (Transform t in roadLines) {
				Handles.DrawLine (rs.startPoint.position, t.position);
			}
		}

		Handles.Label (getRoadPos (), "Nearest Point To Mouse");
	}

	NewRoadJunction getNearestJunction()
	{
		float distance = 9999999.0f;
		NewRoadJunction retVal = null;

		foreach (NewRoadJunction rj in road.sectionsInTheRoad) {
			float d = Vector2.Distance (rj.startPoint.position, getMousePos ());
			if (d < distance) {
				distance = d;
				retVal = rj;
			}
		}
		return retVal;
	}

	NewRoadJunction nearestRoadSection;
	Vector3 getRoadPos()
	{
		float distance = 999999.0f;
		Vector3 point = Vector3.zero;
		foreach (NewRoadJunction rs in road.sectionsInTheRoad) {
			List<Transform> connections = rs.getPoints ();

			foreach (Transform t in connections) {
				Vector3 pos = getNearestPointOnLine (rs.startPoint.position, t.position, getMousePos ());
				float d2 = Vector3.Distance (getMousePos (), pos);

				if (d2 < distance) {
					point = pos;
					distance = d2;
				}
			}
		}
		return point;
	}

	void createPointForRoad()
	{
		if (areWeLinkingExisting == false) {
			Vector3 pos2 = getRoadPos ();

			NewRoadJunction r = createRoadSection (pos2);
			if (r.potentialPoints == null) {
				r.potentialPoints = new List<NewRoadJunction> ();
			}

			road.sectionsInTheRoad.Add (r);
			nearestRoadSection.potentialPoints.Add (r);
		} else {
			NewRoadJunction j = null;
			float d = 9999999.0f;

			foreach (NewRoadJunction rj in road.sectionsInTheRoad) {
				float d2 = Vector2.Distance (getMousePos (), rj.startPoint.position);
				if (d2 < d && d2 < 5) {
					j = rj;
					d = d2;
				}
			}

			if (j == null) {
				areWeLinkingExisting = false;
				return;
			}

			if (j.potentialPoints == null) {
				j.potentialPoints = new List<NewRoadJunction> ();
			}

			if (nearestRoadSection.potentialPoints == null) {
				nearestRoadSection.potentialPoints = new List<NewRoadJunction> ();
			}

			nearestRoadSection.potentialPoints.Add (j);
			areWeLinkingExisting = false;
		}
		//r.potentialPoints.Add (nearestRoadSection);


		/*int index = road.sectionsInTheRoad.IndexOf (nearestRoadSection);
		road.sectionsInTheRoad.Insert (index + 1, r);
		if (road.sectionsInTheRoad [road.sectionsInTheRoad.Count - 1] == r) {
			r.nextPoint = road.sectionsInTheRoad [0];
			r.potentialPoints.Add (road.sectionsInTheRoad [0]);
		} else {
			r.nextPoint = road.sectionsInTheRoad [road.sectionsInTheRoad.IndexOf (r) + 1];
			r.potentialPoints.Add (road.sectionsInTheRoad [road.sectionsInTheRoad.IndexOf (r) + 1]);
		}
		road.sectionsInTheRoad [index].nextPoint = r;*/
		

	}

	PoliceController pc;
	void policePlacementController()
	{
		if (pc == null) {
			pc = FindObjectOfType<PoliceController> ();
		}
		inputDetect ();

	}

	bool placingCopRoute=false;
	GameObject copAddingRouteTo;

	void drawPoliceHandles()
	{
		foreach (GameObject g in pc.copCarsAvailable) {
			EditorGUI.BeginChangeCheck ();

			if (placingCopRoute == false) {
				Handles.color = Color.red;

				if (Handles.Button (g.transform.position + new Vector3 (0, 1, 0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					pc.copCarsAvailable.Remove (g);
					DestroyImmediate (g);
					EditorUtility.SetDirty (pc);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					return;
				}
			}

			if (placingCopRoute == false || placingCopRoute == true && copAddingRouteTo == g) {
				Handles.color = Color.blue;

				if (Handles.Button (g.transform.position + new Vector3 (1, 1, 0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if (placingCopRoute == false) {
						copAddingRouteTo = g;
						placingCopRoute = true;
					} else {
						copAddingRouteTo = null;
						placingCopRoute = false;
					}
				}
			}


			Handles.color = Color.white;

			EditorGUI.BeginChangeCheck ();
			//Handles.Label (toEdit.transform.position, toEdit.gameObject.name);
			Vector3 pos2 = Handles.PositionHandle (g.transform.position,g.transform.rotation);

			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				g.transform.position = pos2;

				EditorUtility.SetDirty (g);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			if (placingCopRoute == true) {


				PoliceCarScript pc = copAddingRouteTo.GetComponent<PoliceCarScript> ();

				if (pc.myRoute == null) {

				} else {

					if (pc.myRoute.Count >= 2) {
						Handles.DrawLine (copAddingRouteTo.transform.position, pc.myRoute [0].transform.position);

						for (int x = 0; x < pc.myRoute.Count - 1; x++) {
							Handles.DrawLine (pc.myRoute [x].position, pc.myRoute [x + 1].position);
						}
					}


					foreach (Transform t in pc.myRoute) {
						EditorGUI.BeginChangeCheck ();

						Vector3 pos3 = Handles.PositionHandle (t.position, t.rotation);

						if (EditorGUI.EndChangeCheck ()) {
							//Undo.RecordObject (r, "Moved position of " + rs.roomName);
							t.position = pos3;

							EditorUtility.SetDirty (t);
							EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
						}
					}
				}
			}
		}


	}

	void addNewPointToPoliceRoute()
	{
		LevelEditor le = (LevelEditor)target;
		PoliceCarScript pc = copAddingRouteTo.GetComponent<PoliceCarScript> ();
		if (pc.myRoute == null) {
			pc.myRoute = new List<Transform> ();
		}

		GameObject g = new GameObject ();
		g.name = "Police route point" + pc.myRoute.Count;
		g.transform.parent = le.transform;
		g.transform.position = getMousePos ();
		pc.myRoute.Add (g.transform);
		EditorUtility.SetDirty (pc);

	}

	void createPoliceCar()
	{
		LevelEditor le = (LevelEditor)target;

		GameObject g = (GameObject)Instantiate (le.policeCarPrefab, getMousePos (), Quaternion.Euler (0, 0, 0));
		if (pc.copCarsAvailable == null) {
			pc.copCarsAvailable = new List<GameObject> ();
		}

		pc.copCarsAvailable.Add (g);
		PoliceCarScript pcs = g.GetComponent<PoliceCarScript> ();
		pcs.myRoute.Clear ();

		g.SetActive (false);
		EditorUtility.SetDirty (pc);
		EditorUtility.SetDirty (pc.gameObject);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());
	}
	LevelController lc;

	void patrolRouteControl()
	{
		if (lc == null) {
			lc = FindObjectOfType<LevelController> ();
		}
		inputDetect ();
	}

	void createNewPatrolRoute()
	{
		GameObject g = new GameObject ();
		g.transform.position = getMousePos ();
		g.name = "Patrol Route Parent";

		if (lc.patrolRoutesInLevel == null) {
			lc.patrolRoutesInLevel = new List<PatrolRoute> ();
		}

		PatrolRoute pr = g.AddComponent<PatrolRoute> ();
		GameObject point1 = new GameObject ();
		point1.transform.position = getMousePos ();
		point1.name = "Patrol route point";
		pr.pointsInRoute = new List<GameObject> ();
		pr.pointsInRoute.Add (point1);
		g.transform.parent = lc.gameObject.transform;
		point1.transform.parent = g.transform;
		lc.patrolRoutesInLevel.Add (pr);


		EditorUtility.SetDirty (lc);
		EditorUtility.SetDirty (lc.gameObject);
		EditorUtility.SetDirty (pr);
		EditorUtility.SetDirty (pr.gameObject);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene());

	}

	bool editingRoute=false;
	PatrolRoute routeEditing;

	void addNewPointToRoute()
	{
		GameObject g = new GameObject ();
		g.transform.position = getMousePos ();
		g.name = "Patrol Point";
		g.transform.parent = routeEditing.transform;
		routeEditing.pointsInRoute.Add (g);
		EditorUtility.SetDirty (routeEditing);
	}

	void drawPatrolrouteGizmos()
	{
		
		foreach (PatrolRoute p in lc.patrolRoutesInLevel) {

			if (currentTask == LevelEditorTask.enemies) {
				Handles.Label (p.pointsInRoute[0].transform.position, "Patrol route " + lc.patrolRoutesInLevel.IndexOf (p));
				if (p.pointsInRoute.Count >= 2) {
					for (int x = 0; x < p.pointsInRoute.Count - 1; x++) {
						Handles.DrawLine (p.pointsInRoute [x].transform.position, p.pointsInRoute [x + 1].transform.position);
					}
				}
			} else {
				Handles.Label (p.transform.position,"Patrol route " + lc.patrolRoutesInLevel.IndexOf (p));

				Handles.color = Color.red;
				if (editingRoute == false) {
					if (Handles.Button (p.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
						lc.patrolRoutesInLevel.Remove (p);
						DestroyImmediate (p.gameObject);
						EditorUtility.SetDirty (lc);
						EditorUtility.SetDirty (lc.gameObject);
						EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
						return;
					}
				}
				if (editingRoute == false || editingRoute == true && routeEditing == p) {
					Handles.color = Color.blue;

					if (Handles.Button (p.transform.position+new Vector3(1,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
						if (editingRoute == true) {
							editingRoute = false;
							routeEditing = null;
						} else {
							editingRoute = true;
							routeEditing = p;
						}
					}
				}
				Handles.color = Color.white;
			}
		}

		if (editingRoute == true) {
			foreach (GameObject g in routeEditing.pointsInRoute) {
				Transform t = g.transform;
				EditorGUI.BeginChangeCheck ();

				Vector3 pos3 = Handles.PositionHandle (t.position, t.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					t.position = pos3;

					EditorUtility.SetDirty (t);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}


			if(routeEditing.pointsInRoute.Count>=2)
			{
				for(int x = 0;x<routeEditing.pointsInRoute.Count-1;x++){
					Handles.DrawLine (routeEditing.pointsInRoute [x].transform.position, routeEditing.pointsInRoute [x+1].transform.position);
				}
			}
		}

	}


	Vector3 getNearestPointOnLine(Vector3 startOfLine, Vector3 endOfLine, Vector3 currentPoint){
		Vector3 vVector1 = currentPoint - startOfLine;
		Vector3 vVector2 = (endOfLine - startOfLine).normalized;

		float d = Vector3.Distance (startOfLine, endOfLine);
		float t = Vector3.Dot (vVector2, vVector1);

		if (t <= 0) {
			return startOfLine;
		}

		if (t >= d) {
			return endOfLine;
		}

		Vector3 vVector3 = vVector2 * t;

		Vector3 vClosestPoint = startOfLine + vVector3;

		return vClosestPoint;
	}
	bool editingEnemy=false,rotEnemy=false;
	GameObject enemyToEdit,enemyIAmAdding;
	void placeEnemiesControl()
	{
		inputDetect ();
	}
	Vector2 scrollPos = new Vector2();
	void drawEnemyHandles()
	{
		LevelEditor le = (LevelEditor)target;

		if (editingEnemy == false) {
			GUILayout.BeginVertical ();
			if (enemyIAmAdding == null) {
				GUILayout.Label ("No enemy to add", GUILayout.Width (200), GUILayout.Height (25));
			} else {
				GUILayout.Label ("Adding " + enemyIAmAdding.name, GUILayout.Width (250), GUILayout.Height (25));
			}

			foreach (GameObject g in le.enemies) {
				if (GUILayout.Button ("Place " + g.name, GUILayout.Width (200), GUILayout.Height (25))) {
					enemyIAmAdding = g;
				}
			}
			GUILayout.EndVertical();
		} else {
			NPCController npc = enemyToEdit.GetComponent<NPCController> ();
			NPCBehaviourDecider npcb = enemyToEdit.GetComponent<NPCBehaviourDecider> ();
			PersonMovementController pmc = enemyToEdit.GetComponent<PersonMovementController> ();
			NPCItemInitialiser npci = enemyToEdit.GetComponent<NPCItemInitialiser> ();
			NPCMemory npcm = enemyToEdit.GetComponent<NPCMemory> ();
			GUILayout.BeginVertical ();
			GUILayout.Label ("Current Enemy is " + enemyToEdit.name, GUILayout.Width (150), GUILayout.Height (25));
			if (rotEnemy == false) {
				if (GUILayout.Button ("Rotate Enemy", GUILayout.Width (100), GUILayout.Height (25))) {
					rotEnemy = true;
				}
			} else {
				if (GUILayout.Button ("Move Enemy", GUILayout.Width (100), GUILayout.Height (25))) {
					rotEnemy = false;
				}
			}

			scrollPos = GUILayout.BeginScrollView (scrollPos, GUILayout.Width (250),GUILayout.Height(300));
			GUILayout.Label ("AI Type = " + npc.myType.ToString());
			foreach (AIType ait in System.Enum.GetValues(typeof(AIType))) {
				if (GUILayout.Button ("Set Type : " + ait.ToString(), GUILayout.Width (200), GUILayout.Height (25))) {
					npc.myType = ait;
					npcb.myType = ait;
					EditorUtility.SetDirty (npc);
					EditorUtility.SetDirty (npcb);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}

			GUILayout.Label ("NPC Speed = " + pmc.movementSpeed.ToString (), GUILayout.Width (150), GUILayout.Height (25));
			try{
				pmc.movementSpeed= float.Parse (GUILayout.TextField (pmc.movementSpeed.ToString(),GUILayout.Width (150), GUILayout.Height (25))); 
				EditorUtility.SetDirty(pmc);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

			}
			catch{
				pmc.movementSpeed= float.Parse (GUILayout.TextField ("0",GUILayout.Width (150), GUILayout.Height (25))); 
				EditorUtility.SetDirty(pmc);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			//stuff for ID's
			GUILayout.Label("NPC ID",GUILayout.Width(100),GUILayout.Height(25));
			npcb.myID = GUILayout.TextField (npcb.myID, GUILayout.Width (100), GUILayout.Height (25));

			GUILayout.Label ("Freindly ID's", GUILayout.Width (100), GUILayout.Height (25));
			foreach (string st in npcb.freindlyIDs) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label (st, GUILayout.Width (100), GUILayout.Height (25));
				if (GUILayout.Button ("X", GUILayout.Width (25), GUILayout.Height (25))) {
					npcb.freindlyIDs.Remove (st);
					EditorUtility.SetDirty (npcb);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					return;
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.BeginHorizontal ();
			newID = GUILayout.TextField (newID, GUILayout.Width (100), GUILayout.Height (25));
			if (GUILayout.Button ("Add ID", GUILayout.Width (100), GUILayout.Height (25))) {
				if (npcb.freindlyIDs == null) {
					npcb.freindlyIDs = new List<string> ();
				}
				npcb.freindlyIDs.Add (newID);
				newID = "";
				EditorUtility.SetDirty (npcb);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
			GUILayout.EndHorizontal ();


			GUILayout.Label ("Items NPC Gets", GUILayout.Width (150), GUILayout.Height (25));
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Weapon NPC Gets = ", GUILayout.Width (150), GUILayout.Height (25));
			npci.weaponToGet = GUILayout.TextArea (npci.weaponToGet, GUILayout.Width (150), GUILayout.Height (25));
			EditorUtility.SetDirty (npci);

			GUILayout.EndHorizontal ();
			GUILayout.Label ("_________________");
			GUILayout.Label ("Items to add to NPC");
			foreach (string st in npci.itemsToAdd) {
				GUILayout.BeginHorizontal ();
				//npci = GUILayout.TextArea (st, GUILayout.Width (150), GUILayout.Height (25));
				GUILayout.Label(st,GUILayout.Width(100),GUILayout.Height(25));
				if (GUILayout.Button ("X", GUILayout.Width (25), GUILayout.Height (25))) {
					npci.itemsToAdd.Remove (st);
					EditorUtility.SetDirty (npci);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.BeginHorizontal ();
			newItem = GUILayout.TextField (newItem, GUILayout.Width (100), GUILayout.Height (25));
			if (GUILayout.Button ("Add Item", GUILayout.Width (100), GUILayout.Height (25))) {
				if (npci.itemsToAdd == null) {
					npci.itemsToAdd = new List<string> ();
				}
				npci.itemsToAdd.Add (newItem);
				EditorUtility.SetDirty (npci);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

			}

			GUILayout.EndHorizontal ();
			GUILayout.Label ("_________________");
			GUILayout.Label ("Random Items For NPC");
			foreach (string st in npci.randomItems) {
				GUILayout.BeginHorizontal ();
				//st = GUILayout.TextArea (st, GUILayout.Width (150), GUILayout.Height (25));
				GUILayout.Label(st,GUILayout.Width(100),GUILayout.Height(25));

				if (GUILayout.Button ("X", GUILayout.Width (25), GUILayout.Height (25))) {
					npci.randomItems.Remove (st);
					EditorUtility.SetDirty (npci);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.BeginHorizontal ();
			newRandom = GUILayout.TextField (newRandom, GUILayout.Width (100), GUILayout.Height (25));
			if (GUILayout.Button ("Add Item", GUILayout.Width (100), GUILayout.Height (25))) {
				if (npci.randomItems == null) {
					npci.randomItems = new List<string> ();
				}
				npci.randomItems.Add (newRandom);
				EditorUtility.SetDirty (npci);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

			}

			GUILayout.EndHorizontal ();

			if (npcb.patrol == true) {
				if (GUILayout.Button ("Set guard to stationary", GUILayout.Width (100), GUILayout.Height (25))) {
					npcb.patrol = false;
					npcm.myRoute = null;
					EditorUtility.SetDirty (npcb);
					EditorUtility.SetDirty (npcm);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

				}
				if (lc == null) {
					lc = FindObjectOfType<LevelController> ();
				}
				int ind = 0;
				foreach (PatrolRoute pr in lc.patrolRoutesInLevel) {
					if (pr == npcm.myRoute) {
						GUILayout.Label ("Patrol Route = " + ind, GUILayout.Width (150), GUILayout.Height (25));
					} else {
						if (GUILayout.Button ("Set route to " + ind, GUILayout.Width (200), GUILayout.Height (25))) {
							npcm.myRoute = pr;
							EditorUtility.SetDirty (npcm);
							EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

						}
					}
					ind++;
				}

			} else {
				if (GUILayout.Button ("Set guard to Patrol", GUILayout.Width (100), GUILayout.Height (25))) {
					npcb.patrol = true;
					EditorUtility.SetDirty (npcb);
				}
			}


			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();
		}
	}



	string newID = "";
	string newItem="",newRandom="";
	void drawEnemyGizmos()
	{
		LevelEditor le = (LevelEditor)target;
		if (lc == null) {
			lc = FindObjectOfType<LevelController> ();
		}
		foreach (GameObject g in le.enemiesCreated) {
			if (editingEnemy == false) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.enemiesCreated.Remove (g);
					DestroyImmediate (g);
					EditorUtility.SetDirty (le);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					return;
				}
			}

			if (editingEnemy == false || editingEnemy == true && g == enemyToEdit) {
				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position + new Vector3 (1, 0, 0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if (editingEnemy == false) {
						editingEnemy = true;
						enemyToEdit = g;
					} else {
						editingEnemy = false;
						enemyToEdit = null;
					}
				}
			}
			Handles.color = Color.white;
			if (lc == null) {
				lc = FindObjectOfType<LevelController> ();
			}
			if (lc.itemDepositLoc == null) {
				lc.itemDepositLoc = new GameObject ().transform;
				lc.itemDepositLoc.name = "Item Deposit Location";
				lc.itemDepositLoc.parent = lc.transform;
			}

			Handles.Label (lc.itemDepositLoc.position,"Item deposit location, items found by security get deposited here");

			EditorGUI.BeginChangeCheck ();

			Vector3 pos3 = Handles.PositionHandle (lc.itemDepositLoc.position, lc.itemDepositLoc.rotation);

			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				lc.itemDepositLoc.position = pos3;

				EditorUtility.SetDirty (lc);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}

		if (lc.raiseAlarmLoc == null) {
			lc.raiseAlarmLoc = new GameObject ().transform;
			lc.raiseAlarmLoc.name = "Raise Alarm Location";
			lc.raiseAlarmLoc.transform.parent = lc.transform;
		}
		Handles.Label (lc.raiseAlarmLoc.position,"Raise alarm location, guards will go here to call the police");
		EditorGUI.BeginChangeCheck ();

		Vector3 pos4 = Handles.PositionHandle (lc.raiseAlarmLoc.position, lc.raiseAlarmLoc.rotation);

		if (EditorGUI.EndChangeCheck ()) {
			//Undo.RecordObject (r, "Moved position of " + rs.roomName);
			lc.raiseAlarmLoc.position = pos4;

			EditorUtility.SetDirty (lc);
			EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
		}

		if (editingEnemy == true) {
			GameObject g = enemyToEdit;
			if (rotEnemy == false) {
				EditorGUI.BeginChangeCheck ();

				Vector3 pos3 = Handles.PositionHandle (g.transform.position, g.transform.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					g.transform.position = pos3;

					EditorUtility.SetDirty (g);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			} else {
				EditorGUI.BeginChangeCheck ();
				Quaternion q = Handles.RotationHandle (g.transform.rotation, g.transform.position);
				//Vector3 pos3 = Handles.PositionHandle (g.transform.position, g.transform.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					g.transform.rotation = q;

					EditorUtility.SetDirty (g);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}
			NPCMemory npcm = enemyToEdit.GetComponent<NPCMemory> ();
			NPCBehaviourDecider npcb = enemyToEdit.GetComponent<NPCBehaviourDecider> ();

			if (npcb.patrol == true) {
				if (npcm.myRoute == null) {

				} else {
					foreach (GameObject g2 in npcm.myRoute.pointsInRoute) {
						Transform t = g2.transform;
						EditorGUI.BeginChangeCheck ();

						Vector3 pos3 = Handles.PositionHandle (t.position, t.rotation);

						if (EditorGUI.EndChangeCheck ()) {
							//Undo.RecordObject (r, "Moved position of " + rs.roomName);
							t.position = pos3;

							EditorUtility.SetDirty (t);
							EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
						}
					}


					if(npcm.myRoute.pointsInRoute.Count>=2)
					{
						for(int x = 0;x<npcm.myRoute.pointsInRoute.Count-1;x++){
							Handles.DrawLine (npcm.myRoute.pointsInRoute [x].transform.position, npcm.myRoute.pointsInRoute [x+1].transform.position);
						}
					}
				}
			}
		}
	}

	void placeEnemy()
	{
		if (enemyIAmAdding == null) {
			return;
		}

		LevelEditor le = (LevelEditor)target;
		GameObject g = (GameObject)Instantiate (enemyIAmAdding, getMousePos (), Quaternion.Euler (0, 0, 0));
		le.addEnemy (g);

		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (g.gameObject);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	bool placeSource=true;

	void placeLight()
	{
		LevelEditor le = (LevelEditor)target;

		if (placeSource == true) {
			GameObject g = (GameObject)Instantiate (le.lightPrefab, getMousePos (), Quaternion.Euler (0, 0, 0));
			g.transform.parent = le.transform;
			le.addLightSource (g); 
			EditorUtility.SetDirty (le);
		} else {
			GameObject g = (GameObject)Instantiate (le.lightswitchPrefab, getMousePos (), Quaternion.Euler (0, 0, 0));
			g.transform.parent = le.transform;
			le.addLightSwitch (g); 
			EditorUtility.SetDirty (le);
		}
	}

	void lightControl()
	{
		inputDetect ();
	}

	GameObject lightToEdit;
	bool editingLight=false;
	void drawLightUI()
	{
		if (editingLight == false) {
			if (placeSource == true) {
				if (GUILayout.Button ("Switch To Lightswitches", GUILayout.Width (150), GUILayout.Height (25))) {
					placeSource = false;
				}
			} else {
				if (GUILayout.Button ("Switch To Lightsources", GUILayout.Width (150), GUILayout.Height (25))) {
					placeSource = true;
				}
			}
		} else {
			if (rotation == false) {
				if (GUILayout.Button ("Rotate Light/Switch", GUILayout.Width (150), GUILayout.Height (25))) {
					rotation = true;
				}
			} else {
				if (GUILayout.Button ("Move Light/Switch", GUILayout.Width (150), GUILayout.Height (25))) {
					rotation = false;
				}
			}
			if (lightToEdit.GetComponent<LightSource> () == true) {

				LightSource l = lightToEdit.GetComponent<LightSource> ();
				if (GUILayout.Button ("Calculate Light Area", GUILayout.Width (150), GUILayout.Height (25))) {
					l.editorGetTilesLightTouches ();
					EditorUtility.SetDirty (l);

					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}

				if (GUILayout.Button ("Clear Light Area", GUILayout.Width (150), GUILayout.Height (25))) {
					l.lightingTilePositions.Clear();
					EditorUtility.SetDirty (l);

					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}

				GUILayout.BeginHorizontal ();
				GUILayout.Label ("Light Range = " + l.maxLightDistance.ToString (), GUILayout.Width (100), GUILayout.Height (25));
				try {
					l.maxLightDistance = float.Parse (GUILayout.TextField (l.maxLightDistance.ToString (), GUILayout.Width (100), GUILayout.Height (25)));
				} catch {
					l.maxLightDistance = float.Parse (GUILayout.TextField (l.maxLightDistance.ToString (), GUILayout.Width (100), GUILayout.Height (25)));
				}
				GUILayout.EndHorizontal ();
			}
		}
	}

	void drawLightHandles()
	{
		LevelEditor le = (LevelEditor)target;
		foreach (GameObject g in le.lightsourcesICreated) {
			if (editingLight == false) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.lightsourcesICreated.Remove (g);
					EditorUtility.SetDirty (le);
					DestroyImmediate (g);
				}
			}

			if (editingLight == false || editingLight == true && lightToEdit == g) {
				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position+new Vector3(1,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if (editingLight == false) {	
						lightToEdit = g;
						editingLight = true;
					} else {
						lightToEdit = null;
						editingLight = false;
					}

				}
			}
			Handles.color = Color.white;
		}

		foreach (GameObject g in le.lightswitchesICreated) {
			if (editingLight == false) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.lightswitchesICreated.Remove (g);
					EditorUtility.SetDirty (le);
					DestroyImmediate (g);
				}
			}

			if (editingLight == false || editingLight == true && lightToEdit == g) {
				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position+new Vector3(1,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if (editingLight == false) {	
						lightToEdit = g;
						editingLight = true;
					} else {
						lightToEdit = null;
						editingLight = false;
					}

				}
			}
			Handles.color = Color.white;
		}

		if (editingLight == true) {
			if (rotation == false) {
				EditorGUI.BeginChangeCheck ();

				Vector3 pos3 = Handles.PositionHandle (lightToEdit.transform.position, lightToEdit.transform.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					lightToEdit.transform.position = pos3;

					EditorUtility.SetDirty (lightToEdit);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			} else {
				EditorGUI.BeginChangeCheck ();

				//Vector3 pos3 = Handles.PositionHandle (lightToEdit.transform.position, lightToEdit.transform.rotation);
				Quaternion q = Handles.RotationHandle(lightToEdit.transform.rotation,lightToEdit.transform.position);
				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					lightToEdit.transform.rotation = q;

					EditorUtility.SetDirty (lightToEdit);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}

			if (lightToEdit.GetComponent<Lightswitch> () == true) {
				Lightswitch ls = lightToEdit.GetComponent<Lightswitch> ();
				foreach (LightSource l in ls.sourcesIAlter) {
					Handles.color = Color.yellow;
					Handles.DrawLine (ls.transform.position, l.transform.position);
					Handles.color = Color.white;
				}
			}
		}
	}


	void setLightSwitchToSource()
	{
		Lightswitch ls = lightToEdit.GetComponent<Lightswitch> ();

		if (ls == null) {
			return;
		} else {
			LevelEditor le = (LevelEditor)target;

			GameObject toAdd=null;
			float dist = 9999999.0f;
			foreach (GameObject g in le.lightsourcesICreated) {
				float d2 = Vector3.Distance (g.transform.position, getMousePos ());
				if (d2 < dist && d2<3.0f) {
					toAdd = g;
					dist = d2;
				}
			}

			if (toAdd == null) {

			} else {
				if (ls.sourcesIAlter == null) {
					ls.sourcesIAlter = new List<LightSource> ();
				} 
				ls.sourcesIAlter.Add (toAdd.GetComponent<LightSource> ());
				EditorUtility.SetDirty (ls);
			}
		}
	}

	void lightSceneGUI()
	{
		if (lightToEdit == null) {

		} else {
			LightSource l = lightToEdit.GetComponent<LightSource> ();
				Vector3 bl, br, tl, tr;
				bl = new Vector3 (l.gameObject.transform.position.x - (l.sourceSize / 2), l.gameObject.transform.position.y - (l.sourceSize / 2), 0);
				br = new Vector3 (l.gameObject.transform.position.x + (l.sourceSize / 2), l.gameObject.transform.position.y - (l.sourceSize / 2), 0);
				tl = new Vector3 (l.gameObject.transform.position.x - (l.sourceSize / 2), l.gameObject.transform.position.y + (l.sourceSize / 2), 0);
				tr = new Vector3 (l.gameObject.transform.position.x + (l.sourceSize / 2), l.gameObject.transform.position.y + (l.sourceSize / 2), 0);
				Color c = l.sourceColor;
				Vector3[] verts = new Vector3[4];
				verts [0] = bl;
				verts [1] = br;
				verts [3] = tl;
				verts [2] = tr;
				Handles.DrawSolidRectangleWithOutline (verts, new Color (c.r, c.g, c.b, 0.1f), Color.black);

				Handles.color = Color.red;
				foreach (Vector3 pos in l.lightingTilePositions) {
					Handles.DrawWireCube(pos + new Vector3(0.5f,0.5f,0),new Vector3(1,1,0.1f));
				}
				Handles.color = Color.white;
		} 
	}

	Vector3 getRoundedMousePos()
	{
		return new Vector3 (Mathf.Round (pos.x), Mathf.Round(pos.y), 0);
	}

	Vector3 getMousePos()
	{
		return new Vector3 (pos.x, pos.y, 0);
	}

	void createCCTV(){
		LevelEditor le = (LevelEditor)target;

		Vector3 pos = getMousePos ();
		pos = new Vector3 (pos.x, pos.y, -1);
		GameObject g = (GameObject)Instantiate (le.cctvPrefab, pos, Quaternion.Euler (0, 0, 0));
		g.transform.parent = le.transform;
		le.addCCTVCamera (g);
		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (g);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	bool editCCTV=false;
	GameObject cctvToEdit;

	void drawCCTVGUI()
	{
		if (editCCTV == true) {
			if (rotation == false) {
				if (GUILayout.Button ("Rotate CCTV Camera", GUILayout.Width (150), GUILayout.Height (25))) {
					rotation = true;
				}
			} else {
				if (GUILayout.Button ("Move CCTV Camera", GUILayout.Width (150), GUILayout.Height (25))) {
					rotation = false;
				}
			}
		}
	}

	void drawCCTVHandles()
	{
		LevelEditor le = (LevelEditor)target;
		foreach (GameObject g in le.cctvCameras) {
			if (editCCTV == false) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.cctvCameras.Remove (g);
					DestroyImmediate (g);
					EditorUtility.SetDirty (le);
				}
			}

			if (editCCTV == false || editCCTV == true && g == cctvToEdit) {
				Handles.color = Color.blue;

				if (Handles.Button (g.transform.position+new Vector3(1,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					if (editCCTV == false) {
						editCCTV = true;
						cctvToEdit = g;
					} else {
						editCCTV = false;
						cctvToEdit = null;
					}
				}
				Handles.color = Color.white;
			}
		}

		if (editCCTV == true) {
			if (rotation == false) {
				EditorGUI.BeginChangeCheck ();

				Vector3 pos3 = Handles.PositionHandle (cctvToEdit.transform.position, cctvToEdit.transform.rotation);

				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					cctvToEdit.transform.position = pos3;

					EditorUtility.SetDirty (cctvToEdit);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			} else {
				EditorGUI.BeginChangeCheck ();

				//Vector3 pos3 = Handles.PositionHandle (lightToEdit.transform.position, lightToEdit.transform.rotation);
				Quaternion q = Handles.RotationHandle(cctvToEdit.transform.rotation,cctvToEdit.transform.position);
				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					cctvToEdit.transform.rotation = q;

					EditorUtility.SetDirty (cctvToEdit);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}
		}
	}


	Vector3 pointA=Vector3.zero,pointB=Vector3.zero;
	bool pointsSet=false;

	void setPointA()
	{
		pointA = getMousePos ();
	}

	void setPointB()
	{
		pointB = getMousePos ();
		pointsSet = true;
	}

	void drawAreaForPoints()
	{
		if (pointA == Vector3.zero) {
			return;
		}
		if (pointsSet == false) {
			pointB = getMousePos ();
		}

		float lowX, lowY, highX, highY;
		if (pointA.x < pointB.x) {
			lowX = pointA.x;
			highX = pointB.x;
		} else {
			lowX = pointB.x;
			highX = pointA.x;
		}

		if (pointA.y < pointB.y) {
			highY = pointB.y;
			lowY = pointA.y;
		} else {
			highY = pointA.y;
			lowY = pointB.y;
		}

		List<Vector3> verts = new List<Vector3> ();
		verts.Add (new Vector3 (lowX, lowY, 0));
		verts.Add (new Vector3 (highX, lowY, 0));
		verts.Add (new Vector3 (highX, highY, 0));
		verts.Add (new Vector3 (lowX, highY, 0));

		Handles.DrawSolidRectangleWithOutline (verts.ToArray(), new Color (1, 1, 1, 0.5f), Color.black);
	}

	void createOutOfBoundsMarker()
	{
		LevelEditor le = (LevelEditor)target;

		float lowX, lowY, highX, highY;
		if (pointA.x < pointB.x) {
			lowX = pointA.x;
			highX = pointB.x;
		} else {
			lowX = pointB.x;
			highX = pointA.x;
		}

		if (pointA.y < pointB.y) {
			highY = pointB.y;
			lowY = pointA.y;
		} else {
			highY = pointA.y;
			lowY = pointB.y;
		}

		GameObject g = new GameObject ();
		g.AddComponent<BoxCollider2D> ();
		g.name = "Out of bounds collider";
		Vector3 p1 = new Vector3 (lowX, lowY, 0);
		Vector3 p2 = new Vector3 (highX, highY, 0);

		g.transform.position = p2 - ((p2 - p1)/2);
		g.transform.localScale = new Vector3 (Mathf.Abs (highX- lowX),Mathf.Abs( highY - lowY),1);
		g.transform.parent = le.gameObject.transform;
		le.addOutOfBoundsMarker (g);
		g.layer = 22;

		pointA = Vector3.zero;
		pointB = Vector3.zero;
		pointsSet = false;
		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (g);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	void drawOOBHandles()
	{
		LevelEditor le = (LevelEditor)target;

		foreach (GameObject g in le.outOfBoundsMarkers) {
			Handles.color = Color.red;
			if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
				le.outOfBoundsMarkers.Remove (g);
				EditorUtility.SetDirty (g);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				DestroyImmediate (g);
			}
		}
	}

	Vector3 naturePos1=Vector3.zero,naturePos2 = Vector3.zero;
	void natureHandles()
	{
		if (eraseNature == false) {
			EditorGUI.BeginChangeCheck ();

			Vector3 p1 = Handles.PositionHandle (naturePos1, Quaternion.Euler (0, 0, 0));
			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				naturePos1 = p1;

				//EditorUtility.SetDirty (cctvToEdit);
				//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}

			EditorGUI.BeginChangeCheck ();

			Vector3 p2 = Handles.PositionHandle (naturePos2, Quaternion.Euler (0, 0, 0));
			if (EditorGUI.EndChangeCheck ()) {
				//Undo.RecordObject (r, "Moved position of " + rs.roomName);
				naturePos2 = p2;

				//EditorUtility.SetDirty (cctvToEdit);
				//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
			drawNatureLabel ();
		} else {
			Handles.color = new Color (1, 0, 0, 0.1f);
			Handles.DrawSolidDisc (new Vector3(pos.x,pos.y,-1), Vector3.forward, eraseRadius);
			Handles.color = Color.white;
		}
	}

	void drawNatureLabel()
	{
		float highX, highY, lowX, lowY;
		if (naturePos1.x > naturePos2.x) {
			highX = naturePos1.x;
			lowX = naturePos2.x;
		} else {
			highX = naturePos2.x;
			lowX = naturePos1.x;
		}

		if (naturePos1.y > naturePos2.y) {
			highY = naturePos1.y;
			lowY = naturePos2.y;
		} else {
			highY = naturePos2.y;
			lowY = naturePos1.y;
		}
		List<Vector3> verts = new List<Vector3> ();
		verts.Add (new Vector3 (lowX, lowY, 0));
		verts.Add (new Vector3 (highX, lowY, 0));
		verts.Add (new Vector3 (highX, highY, 0));
		verts.Add (new Vector3 (lowX, highY, 0));

		Handles.DrawSolidRectangleWithOutline (verts.ToArray(), new Color (1, 1, 1, 0.5f), Color.cyan);
		Vector3 mid = new Vector3 (highX - ((highX - lowX)/2), highY - ((highY - lowY)/2), 0);
		Handles.Label (mid, "Nature mid point");

	}


	float natureDensity = 1;
	void natureGUI()
	{
		if (eraseNature == false) {
			if (GUILayout.Button ("Erase Nature", GUILayout.Width (125), GUILayout.Height (25))) {
				eraseNature = true;
			}
		} else {
			if (GUILayout.Button ("Create Nature", GUILayout.Width (125), GUILayout.Height (25))) {
				eraseNature = false;
			}
		}

		if (eraseNature == false) {
			GUILayout.Label ("Nature Density " + natureDensity.ToString ());
			natureDensity = GUILayout.HorizontalSlider (natureDensity, 1.0f, 10.0f, GUILayout.Width (100.0f), GUILayout.Height (25.0f));

			if (GUILayout.Button ("Create Nature", GUILayout.Width (125), GUILayout.Height (25))) {
				createNature ();
			}

			if (GUILayout.Button ("Clear nature", GUILayout.Width (125), GUILayout.Height (25))) {
				debugClearNature ();
			}

		} else {
			GUILayout.Label ("Erase Size " + eraseRadius.ToString ());
			eraseRadius = GUILayout.HorizontalSlider (eraseRadius, 1.0f, 10.0f, GUILayout.Width (100.0f), GUILayout.Height (25.0f));
			inputDetect ();
		}
	}

	void debugClearNature()
	{
		LevelEditor le = (LevelEditor)target;

		foreach (GameObject g in le.natureObjectsCreated) {
			DestroyImmediate (g);
		}
		le.natureObjectsCreated.Clear ();
		le.natureObjectsCreated = new List<GameObject> ();
		EditorUtility.SetDirty (le);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	void createNature()
	{
		LevelEditor le = (LevelEditor)target;

		float highX, highY, lowX, lowY;
		if (naturePos1.x > naturePos2.x) {
			highX = naturePos1.x;
			lowX = naturePos2.x;
		} else {
			highX = naturePos2.x;
			lowX = naturePos1.x;
		}

		if (naturePos1.y > naturePos2.y) {
			highY = naturePos1.y;
			lowY = naturePos2.y;
		} else {
			highY = naturePos2.y;
			lowY = naturePos1.y;
		}

		for(float x = lowX;x <highX;x++)
		{
			for (float y = lowY; y < highY; y++) {
				Vector3 createAt = new Vector3 (x, y, 0);
				float r = Random.Range (0.0f, 30.0f);
				if (natureDensity > r) {
					GameObject g = (GameObject)Instantiate (le.objectsForNature [Random.Range (0, le.objectsForNature.Count)], createAt, Quaternion.Euler (0, 0, 0));
					le.addNatureObjects (g);
					g.transform.parent = le.transform;
				}
			
			}

		}
		EditorUtility.SetDirty (le);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}

	bool eraseNature = false;
	float eraseRadius = 5.0f;

	void eraseNatureMethod()
	{
		LevelEditor le = (LevelEditor)target;
		List<GameObject> toErase = new List<GameObject> ();
		foreach (GameObject g in le.natureObjectsCreated) {
			if (Vector2.Distance (g.transform.position, pos) < eraseRadius / 2) {
				toErase.Add (g);
			}
		}
		for (int x = 0; x < toErase.Count; x++) {
			le.natureObjectsCreated.Remove (toErase [x]);
			DestroyImmediate (toErase [x]);
		}
		EditorUtility.SetDirty (le);
		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());

	}
}
