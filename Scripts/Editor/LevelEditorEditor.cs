using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

using System.IO;
[CustomEditor(typeof(LevelEditor))]
public class LevelEditorEditor : Editor {
	LevelEditorTask currentTask;
	bool edit=false,rotation=false;
	Vector3 pos;
	

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
		menuScroll = GUILayout.BeginScrollView (menuScroll, GUILayout.Width (500), GUILayout.Height (75));
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

		if (GUILayout.Button ("Add Conversations", GUILayout.Width (125), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.conversations;
		}


		if (GUILayout.Button ("Place Shops", GUILayout.Width (125), GUILayout.Height (25))) {
			currentTask = LevelEditorTask.shops;
		}

        if(GUILayout.Button("Place Items",GUILayout.Width(125),GUILayout.Height(25)))
        {
            currentTask = LevelEditorTask.items;
        }

        if(GUILayout.Button("Place Teleports", GUILayout.Width(125), GUILayout.Height(25)))
        {
            currentTask = LevelEditorTask.teleport;
        }

        if(button("Add Events", 125, 25))
        {
            currentTask = LevelEditorTask.events;
        }

        if(button("Add lore objects", 125, 25))
        {
            currentTask = LevelEditorTask.background;
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

		} else if (currentTask == LevelEditorTask.conversations) {
			GUILayout.Label ("Adding Conversations", GUILayout.Width (300), GUILayout.Height (25));

		} else if (currentTask == LevelEditorTask.shops) {
			GUILayout.Label ("Adding Shops", GUILayout.Width (300), GUILayout.Height (25));

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
		} else if (currentTask == LevelEditorTask.conversations) {
			conversationUI ();
			//conversationHandles ();
		} else if (currentTask == LevelEditorTask.shops) {
			shopUI ();
		}else if(currentTask==LevelEditorTask.items)
        {
            GUILayout.Label("Click To Add Items");
        }else if(currentTask==LevelEditorTask.teleport)
        {
            drawTeleportUI();
        }
        else if(currentTask==LevelEditorTask.events)
        {
            drawEventUI();
        }else if(currentTask==LevelEditorTask.background)
        {
            drawBackgroundUI();
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
		}else if (currentTask == LevelEditorTask.conversations) {
			inputDetect ();

			conversationHandles ();

		} else if (currentTask == LevelEditorTask.shops) {
			inputDetect ();
            drawShopHandles();

            shopHandles ();
		}
        else if(currentTask==LevelEditorTask.items)
        {
            inputDetect();
        }else if(currentTask==LevelEditorTask.teleport)
        {
            drawTeleportHandles();
            inputDetect();
        }else if(currentTask==LevelEditorTask.events)
        {
            drawEventHandles();
            inputDetect();
        }else if(currentTask==LevelEditorTask.background)
        {
            drawBackgroundHandles();
            inputDetect();
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
                    if (currentTask == LevelEditorTask.civActions)
                    {
                        createCivilianAction();
                    }
                    else if (currentTask == LevelEditorTask.civSpawns)
                    {
                        createCivilianSpawnPoint();
                    }
                    else if (currentTask == LevelEditorTask.roads)
                    {
                        createPointForRoad();
                    }
                    else if (currentTask == LevelEditorTask.police)
                    {
                        if (placingCopRoute == false)
                        {
                            createPoliceCar();
                        }
                        else
                        {
                            addNewPointToPoliceRoute();
                        }
                    }
                    else if (currentTask == LevelEditorTask.patrolRoutes)
                    {
                        if (editingRoute == true)
                        {
                            addNewPointToRoute();
                        }
                        else
                        {
                            createNewPatrolRoute();
                        }
                    }
                    else if (currentTask == LevelEditorTask.enemies)
                    {
                        if (editingEnemy == false)
                        {
                            placeEnemy();
                        }
                    }
                    else if (currentTask == LevelEditorTask.lightSources)
                    {
                        if (editingLight == false)
                        {
                            placeLight();
                        }
                        else
                        {
                            if (lightToEdit.GetComponent<Lightswitch>() == true)
                            {
                                setLightSwitchToSource();
                            }
                        }
                    }
                    else if (currentTask == LevelEditorTask.cctv)
                    {
                        if (editCCTV == false)
                        {
                            createCCTV();
                        }
                    }
                    else if (currentTask == LevelEditorTask.outOfBounds)
                    {
                        if (pointsSet == false)
                        {
                            if (pointA == Vector3.zero)
                            {
                                setPointA();
                            }
                            else
                            {
                                setPointB();
                            }
                        }
                        else
                        {
                            createOutOfBoundsMarker();
                        }
                    }
                    else if (currentTask == LevelEditorTask.nature)
                    {
                        if (eraseNature == true)
                        {
                            eraseNatureMethod();
                        }
                    }
                    else if (currentTask == LevelEditorTask.conversations)
                    {
                        if (addPersonToTalkTo == true)
                        {
                            createPersonToTalkTo();
                        }
                    }
                    else if (currentTask == LevelEditorTask.shops)
                    {
                        if (shopEditing == null)
                        {
                            createShop();
                        }
                    }
                    else if (currentTask == LevelEditorTask.items)
                    {
                        GameObject g = new GameObject();
                        g.transform.position = pos;
                        g.AddComponent<ItemInWorld>();
                        g.name = "ItemInWorld - ";
                        g.AddComponent<SpriteRenderer>();
                        Selection.SetActiveObjectWithContext(g, g);
                    }
                    else if(currentTask==LevelEditorTask.teleport)
                    {
                        teleportInput();
                    }else if(currentTask==LevelEditorTask.background)
                    {
                        if(lorePrefab==null)
                        {

                        }
                        else
                        {
                            GameObject g = (GameObject) Instantiate(lorePrefab, pos, Quaternion.Euler(0, 0, 0));
                            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y, 0);
                            g.GetComponent<SpriteRenderer>().sortingOrder = 3;
                            setBackgroundObjects();
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
    NPCID[] ids;
	GameObject npcConvoToEdit;
	ConversationManager conversationEditing;
	ConversationChoice choiceEditing;
	bool addPersonToTalkTo=false;
	void conversationHandles()
	{
		LevelEditor le = (LevelEditor)target;
        if (ids == null)
        {
            ids = FindObjectsOfType<NPCID>();
        }
		if (addPersonToTalkTo == false) {
			foreach (GameObject g in le.peopleToTalkTo) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.peopleToTalkTo.Remove (g);
					EditorUtility.SetDirty (g);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					DestroyImmediate (g);
				}

				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position+new Vector3(1.0f,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					npcConvoToEdit = g;
					conversationEditing = g.GetComponent<ConversationManager> ();
					addPersonToTalkTo = false;
				}

			}

            foreach (NPCID npc in ids)
            {
                GameObject g = npc.gameObject;
                Handles.color = Color.red;
                if (Handles.Button(g.transform.position, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                {
                    //le.peopleToTalkTo.Remove(g);
                   // EditorUtility.SetDirty(g);
                   // EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                   // DestroyImmediate(g);
                }

                Handles.color = Color.blue;
                if (Handles.Button(g.transform.position + new Vector3(1.0f, 0, 0), Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                {
                    le.peopleToTalkTo.Add(g);
                    npcConvoToEdit = g;
                    if (g.GetComponent<ConversationManager>() == null)
                    {
                        conversationEditing = g.AddComponent<ConversationManager>();
                    }
                    else
                    {
                        conversationEditing = g.GetComponent<ConversationManager>();

                    }
                    addPersonToTalkTo = false;
                }

            }

            if (npcConvoToEdit != null) {
				if (rotation == false) {
					EditorGUI.BeginChangeCheck ();

					Vector3 p1 = Handles.PositionHandle (npcConvoToEdit.transform.position, Quaternion.Euler (0, 0, 0));
					if (EditorGUI.EndChangeCheck ()) {
						//Undo.RecordObject (r, "Moved position of " + rs.roomName);
						npcConvoToEdit.transform.position = p1;

						//EditorUtility.SetDirty (cctvToEdit);
						//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					}
				} else {
					EditorGUI.BeginChangeCheck ();
					Quaternion q = Handles.RotationHandle ( npcConvoToEdit.transform.rotation,npcConvoToEdit.transform.position);
					if (EditorGUI.EndChangeCheck ()) {
						//Undo.RecordObject (r, "Moved position of " + rs.roomName);
						npcConvoToEdit.transform.rotation = q;

						EditorUtility.SetDirty (npcConvoToEdit);
						EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					}
				}
			}
		}
	}

	void createPersonToTalkTo()
	{
		LevelEditor le = (LevelEditor)target;

		GameObject g = (GameObject)Instantiate (FindObjectOfType<CommonObjectsStore> ().civilian, getMousePos (), Quaternion.Euler (0, 0, 0));
		NPCController npc = g.GetComponent<NPCController> ();
		ConversationManager cm = g.AddComponent<ConversationManager> ();
		cm.setID ();
		npc.myType = AIType.talk;
		NPCBehaviourDecider npcb = g.GetComponent<NPCBehaviourDecider> ();
		npcb.myType = AIType.talk;
		le.addToTalkTo (g);
		EditorUtility.SetDirty (g);
		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (cm);
		EditorUtility.SetDirty (le.gameObject);

		EditorUtility.SetDirty (npc);
		EditorUtility.SetDirty (npcb);

		EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
	}
	Vector2 convoVec = Vector2.zero;
	void conversationUI()
	{
		GUILayout.BeginVertical ();
		if (addPersonToTalkTo == false) {
			if (GUILayout.Button ("Add people to talk to",GUILayout.Width (175), GUILayout.Height (25))) {
				addPersonToTalkTo = true;
			}
		} else {
			if (GUILayout.Button ("Edit people to talk to",GUILayout.Width (175), GUILayout.Height (25))) {
				addPersonToTalkTo = false;
			}
		}

		if (addPersonToTalkTo == false) {
			if (conversationEditing == null) {

			} else {
				GUILayout.Label ("___________ MANIP NPC ___________");

				if (rotation == false) {
					if (GUILayout.Button ("Rotate",GUILayout.Width (125), GUILayout.Height (25))) {
						rotation = true;
					}
				} else {
					if (GUILayout.Button ("Move",GUILayout.Width (125), GUILayout.Height (25))) {
						rotation = false;
					}
				}


				if (choiceEditing == null ) {

					convoVec = GUILayout.BeginScrollView (convoVec, GUILayout.Width (500),GUILayout.Height(500));

					GUILayout.Label ("___________ CONVO START ___________");

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Person Speaking To:",GUILayout.Width (175), GUILayout.Height (25));
					conversationEditing.personSpeakingTo = GUILayout.TextField (conversationEditing.personSpeakingTo,GUILayout.Width (125), GUILayout.Height (25));
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Opening Line:",GUILayout.Width (175), GUILayout.Height (25));
					conversationEditing.openingLine = GUILayout.TextField (conversationEditing.openingLine,GUILayout.Width (125), GUILayout.Height (25));
					GUILayout.EndHorizontal ();
					//if (conversationEditing.getChoices().Count < 3) {
						if (GUILayout.Button ("Add conversation choice",GUILayout.Width (175), GUILayout.Height (25))) {
							ConversationChoice c = npcConvoToEdit.AddComponent<ConversationChoice> ();
							conversationEditing.addChoice (c);
							EditorUtility.SetDirty (c);
							EditorUtility.SetDirty (conversationEditing);

						}
					//} else {
					//	GUILayout.Label ("Can't have more than three options, remove one");
					//}

					GUILayout.Label ("___________ CHOICES ___________");

					foreach (ConversationChoice c in conversationEditing.getChoices()) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Player Text:",GUILayout.Width (175), GUILayout.Height (25));
						c.playerText = GUILayout.TextField (c.playerText,GUILayout.Width (175), GUILayout.Height (25));
						GUILayout.EndHorizontal ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("NPC Response:",GUILayout.Width (175), GUILayout.Height (25));
						c.NPCResponse = GUILayout.TextField (c.NPCResponse,GUILayout.Width (175), GUILayout.Height (25));
						GUILayout.EndHorizontal ();

						//need to add some condition for adding code triggers.

						if (GUILayout.Button ("Edit Choice",GUILayout.Width (175), GUILayout.Height (25))) {
							choiceEditing = c;
						}

						if (GUILayout.Button ("Remove Choice",GUILayout.Width (175), GUILayout.Height (25))) {
							conversationEditing.initialOptions.Remove (c);
							DestroyImmediate (c);

							EditorUtility.SetDirty (conversationEditing);
							EditorUtility.SetDirty (conversationEditing.gameObject);
						}
					}
					GUILayout.EndScrollView ();
				} else {
					convoVec = GUILayout.BeginScrollView (convoVec, GUILayout.Width (500),GUILayout.Height(500));

					GUILayout.Label ("___________ EDIT CHOICE ___________");

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("Player Text:",GUILayout.Width (175), GUILayout.Height (25));
					choiceEditing.playerText = GUILayout.TextField (choiceEditing.playerText,GUILayout.Width (175), GUILayout.Height (25));
					GUILayout.EndHorizontal ();

					GUILayout.BeginHorizontal ();
					GUILayout.Label ("NPC Response:",GUILayout.Width (175), GUILayout.Height (25));
					choiceEditing.NPCResponse = GUILayout.TextField (choiceEditing.NPCResponse,GUILayout.Width (175), GUILayout.Height (25));
					GUILayout.EndHorizontal ();

					if (GUILayout.Button ("Choice is repeatable = " + choiceEditing.repeatable.ToString (), GUILayout.Width (175), GUILayout.Height (25))) {
						choiceEditing.repeatable = !choiceEditing.repeatable;
						EditorUtility.SetDirty (choiceEditing);
						EditorUtility.SetDirty (choiceEditing.gameObject);
					}

					string[] choiceTypes = System.Enum.GetNames (typeof(typeOfTrigger));
					if (choiceEditing.myTrigger == null) {
						foreach (string st in choiceTypes) {
							if (GUILayout.Button ("Set Trigger to " + st, GUILayout.Width (175), GUILayout.Height (25))) {
								setCurrentChoiceTrigger (st);
							}
						}
					} else {
						if (GUILayout.Button ("Remove Choice", GUILayout.Width (175), GUILayout.Height (25))) {
							DestroyImmediate (choiceEditing.myTrigger);
							EditorUtility.SetDirty (choiceEditing);
						}

						drawUIForCurrentChoiceTrigger ();
					}



					//if(choiceEditing.getChoices().Count<3){
						if(GUILayout.Button("Add new choice",GUILayout.Width (175), GUILayout.Height (25))){
							ConversationChoice c = npcConvoToEdit.AddComponent<ConversationChoice> ();
							choiceEditing.addChoice (c);
							EditorUtility.SetDirty (c);
							EditorUtility.SetDirty (choiceEditing);
						}
					//}else{
					//	GUILayout.Label ("Can't have more than 3 choices");
					//}

					GUILayout.Label ("___________ CHOICES ___________");


					foreach (ConversationChoice c in choiceEditing.getChoices()) {
						GUILayout.BeginHorizontal ();
						GUILayout.Label ("Player Text:",GUILayout.Width (175), GUILayout.Height (25));
						c.playerText = GUILayout.TextField (c.playerText,GUILayout.Width (175), GUILayout.Height (25));
						GUILayout.EndHorizontal ();

						GUILayout.BeginHorizontal ();
						GUILayout.Label ("NPC Response:",GUILayout.Width (175), GUILayout.Height (25));
						c.NPCResponse = GUILayout.TextField (c.NPCResponse,GUILayout.Width (175), GUILayout.Height (25));
						GUILayout.EndHorizontal ();

						//need to add some condition for adding code triggers.

						if (GUILayout.Button ("Edit Choice",GUILayout.Width (175), GUILayout.Height (25))) {
							choiceEditing = c;
						}

						if (GUILayout.Button ("Remove Choice",GUILayout.Width (175), GUILayout.Height (25))) {
							choiceEditing.nextChoices.Remove (c);
							DestroyImmediate (c);

							EditorUtility.SetDirty (conversationEditing);
							EditorUtility.SetDirty (conversationEditing.gameObject);
						}
					}

					if (GUILayout.Button ("Return to conversation beginning",GUILayout.Width (175), GUILayout.Height (25))) {
						choiceEditing = null;
					}
					EditorUtility.SetDirty (choiceEditing);
					GUILayout.EndScrollView ();

				}


				EditorUtility.SetDirty (conversationEditing);
				EditorUtility.SetDirty (conversationEditing.gameObject);
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}
		GUILayout.EndVertical ();
	}

	void setCurrentChoiceTrigger(string val)
	{
		if (choiceEditing.myTrigger == null) {

		} else {
			DestroyImmediate (choiceEditing.myTrigger);
		}

		if (val == "None") {
			
		} else if (val == "Add_Item") {
			choiceEditing.myTrigger = choiceEditing.gameObject.AddComponent<ConversationTrigger_AddItem> ();
			choiceEditing.myTrigger.myType = typeOfTrigger.Add_Item;
		} else if (val == "Give_Money") {
			choiceEditing.myTrigger = choiceEditing.gameObject.AddComponent<ConversationTrigger_GiveMoney> ();
			choiceEditing.myTrigger.myType = typeOfTrigger.Give_Money;
		} else if (val == "Start_Mission") {
			choiceEditing.myTrigger = choiceEditing.gameObject.AddComponent<ConversationTrigger_StartMission> ();
			choiceEditing.myTrigger.myType = typeOfTrigger.Start_Mission;
		}else if (val == "setIDActive")
        {
            choiceEditing.myTrigger = choiceEditing.gameObject.AddComponent<ConversationTrigger_SetNPCIDActive>();
            choiceEditing.myTrigger.myType = typeOfTrigger.setIDActive;
        }
		if (choiceEditing.myTrigger != null) {
			EditorUtility.SetDirty (choiceEditing.myTrigger);
		}

		EditorUtility.SetDirty (choiceEditing);
	}
	Vector2 itemScroll2 = Vector2.zero;
	Vector2 allItemScroll = Vector2.zero;
	Vector2 combScroll = Vector2.zero;
	void drawUIForCurrentChoiceTrigger()
	{
		if (choiceEditing.myTrigger == null) {

		} else if (choiceEditing.myTrigger.myType == typeOfTrigger.Add_Item) {
			ConversationTrigger_AddItem ctai = (ConversationTrigger_AddItem)choiceEditing.myTrigger;
			if (id == null) {
				id = FindObjectOfType<ItemDatabase> ();
			}

			combScroll = GUILayout.BeginScrollView (combScroll);
			GUILayout.Label ("Items to be given", GUILayout.Width (175), GUILayout.Height (25));
			itemScroll2 = GUILayout.BeginScrollView (itemScroll2, GUILayout.Width (300));
			foreach (string st in ctai.getItemsToAdd()) {
				GUILayout.BeginHorizontal ();

				GUILayout.Label (st, GUILayout.Width (175), GUILayout.Height (25));

				if (GUILayout.Button ("X", GUILayout.Width (35), GUILayout.Height (25))) {
					ctai.itemsToAdd.Remove (st);
					break;
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.EndScrollView ();
			GUILayout.Label ("Items that could be given", GUILayout.Width (175), GUILayout.Height (25));
			allItemScroll = GUILayout.BeginScrollView (allItemScroll, GUILayout.Width (300));

			foreach (GameObject g in id.items) {
				Item i = g.GetComponent<Item> ();
				GUILayout.BeginHorizontal ();

				GUILayout.Label (i.itemName, GUILayout.Width (175), GUILayout.Height (25));
				if (GUILayout.Button ("Add", GUILayout.Width (75), GUILayout.Height (25))) {
					ctai.addItem (i.itemName);
				}
				GUILayout.EndHorizontal ();

			}
			GUILayout.EndScrollView ();
			GUILayout.EndScrollView ();
			EditorUtility.SetDirty (ctai);

		} else if (choiceEditing.myTrigger.myType == typeOfTrigger.Give_Money) {
			ConversationTrigger_GiveMoney ctgm = (ConversationTrigger_GiveMoney)choiceEditing.myTrigger;

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Money to give:",GUILayout.Width(125),GUILayout.Height(30));
			ctgm.moneyToAdd = int.Parse (GUILayout.TextField(ctgm.moneyToAdd.ToString(),GUILayout.Width(125),GUILayout.Height(30)));
			GUILayout.EndHorizontal ();
			EditorUtility.SetDirty (ctgm);

		}else if(choiceEditing.myTrigger.myType==typeOfTrigger.Start_Mission) {
			ConversationTrigger_StartMission ctsm = (ConversationTrigger_StartMission)choiceEditing.myTrigger;
			if (missCont == null) {
				missCont = FindObjectOfType<MissionController> ();
			}

			missionScroll = GUILayout.BeginScrollView (missionScroll,GUILayout.Width(400),GUILayout.Height(400));

			foreach (Mission m in missCont.missions) {
				if (ctsm.missionToStart == m.missionName) {
					GUILayout.Label ("Current Mission : " + m.missionName, GUILayout.Width (300), GUILayout.Height (30));
				} else {
					if (GUILayout.Button (m.missionName, GUILayout.Width (300), GUILayout.Height (30))) {
						ctsm.missionToStart = m.missionName;
						EditorUtility.SetDirty (ctsm);

					}
				}
			}

			GUILayout.EndScrollView ();
		}else if(choiceEditing.myTrigger.myType == typeOfTrigger.setIDActive)
        {
            drawIDEdit((ConversationTrigger_SetNPCIDActive)choiceEditing.myTrigger);
        }
	}

    List<string> IDsCreated;
    Vector2 idScroll = Vector2.zero;

   List<string> readFile(string fileName)
    {
        List<string> retVal = new List<string>();
        string path = fileName;
        StreamReader sr = new StreamReader(path);
        string st = sr.ReadLine();
        while (st != null)
        {
            retVal.Add(st);
            st = sr.ReadLine();
        }
        sr.Close();
        return retVal;
    }

    void drawIDEdit(ConversationTrigger_SetNPCIDActive trigger)
    {
        if(IDsCreated==null)
        {
            string folderPath = Path.Combine(Application.dataPath, "NPCIDData");

            string path = Path.Combine(folderPath, "UniquieNPCData.txt");
            if (Directory.Exists(folderPath) == false)
            {
                Directory.CreateDirectory(folderPath);
            }
            if (File.Exists(path) == true)
            {
                IDsCreated =readFile(path);
                //writeIDToFile(folderPath);
            }
        }

        if(trigger.idsToAdd==null)
        {
            trigger.idsToAdd = new List<int>();
        }
        idScroll = GUILayout.BeginScrollView(idScroll,GUILayout.Width(200),GUILayout.Height(200));

        foreach(string st in IDsCreated)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(st, GUILayout.Width(st.Length * 8), GUILayout.Height(30));
            string[] dat = st.Split(';');
            int num = int.Parse(dat[0]);
            if (trigger.idsToAdd.Contains(num) == false)
            {
                if (GUILayout.Button("Add"))
                {
                    trigger.idsToAdd.Add(num);
                    EditorUtility.SetDirty(trigger);
                }
            }
            else
            {
                if (GUILayout.Button("Remove"))
                {
                    trigger.idsToAdd.Remove(num);
                    EditorUtility.SetDirty(trigger);

                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

	Vector2 missionScroll = Vector2.zero;
	MissionController missCont=null;

	void createShop()
	{
		LevelEditor le = (LevelEditor)target;

		GameObject g = new GameObject ();
		g.transform.position = getMousePos ();
		Shop s = g.AddComponent<Shop> ();
		g.AddComponent<PlayerActionShop> ();
		le.addShop (g);
		EditorUtility.SetDirty (le);
		EditorUtility.SetDirty (le.gameObject);
	}

	Shop shopEditing;

	void shopHandles()
	{
		LevelEditor le = (LevelEditor)target;

		if (shopEditing == null) {
			foreach (GameObject g in le.shops) {
				Handles.color = Color.red;
				if (Handles.Button (g.transform.position, Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					le.shops.Remove (g);
					EditorUtility.SetDirty (g);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
					DestroyImmediate (g);
				}

				Handles.color = Color.blue;
				if (Handles.Button (g.transform.position+new Vector3(1.0f,0,0), Quaternion.Euler (0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap)) {
					shopEditing = g.GetComponent<Shop> ();
				}
			}
		} else {
			if (rotation == false) {
				EditorGUI.BeginChangeCheck ();

				Vector3 p1 = Handles.PositionHandle (shopEditing.transform.position, Quaternion.Euler (0, 0, 0));
				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					shopEditing.transform.position = p1;

					EditorUtility.SetDirty (shopEditing);
					//EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			} else {
				EditorGUI.BeginChangeCheck ();
				Quaternion q = Handles.RotationHandle ( shopEditing.transform.rotation,shopEditing.transform.position);
				if (EditorGUI.EndChangeCheck ()) {
					//Undo.RecordObject (r, "Moved position of " + rs.roomName);
					shopEditing.transform.rotation = q;

					EditorUtility.SetDirty (shopEditing);
					EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
				}
			}
		}
	}
	ItemDatabase id;

	Vector2 itemScroll = Vector2.zero;
	void shopUI(){
		if (id == null) {
			id = FindObjectOfType<ItemDatabase> ();
		}
		GUILayout.BeginVertical ();
		if (shopEditing == null) {

		} else {
			if (rotation == false) {
				if (GUILayout.Button ("Rotate",GUILayout.Width (125), GUILayout.Height (25))) {
					rotation = true;
				}
			} else {
				if (GUILayout.Button ("Move",GUILayout.Width (125), GUILayout.Height (25))) {
					rotation = false;
				}
			}

			if(GUILayout.Button("Stop editing",GUILayout.Width (125), GUILayout.Height (25))){
				EditorUtility.SetDirty (shopEditing);
				EditorUtility.SetDirty (shopEditing.gameObject);

				shopEditing=null;

			}

            if (GUILayout.Button("Should Shop link to existing " + shopEditing.linkedToExisting, GUILayout.Width(155), GUILayout.Height(25)))
            {
                shopEditing.linkedToExisting = !shopEditing.linkedToExisting;
            }
                GUILayout.BeginHorizontal ();
			GUILayout.Label ("Chance of shop having an item", GUILayout.Width (175), GUILayout.Height (25));
			shopEditing.chanceOfHavingItem = GUILayout.HorizontalSlider (shopEditing.chanceOfHavingItem, 1, 100, GUILayout.Width (125), GUILayout.Height (25));
			GUILayout.Label (shopEditing.chanceOfHavingItem.ToString(), GUILayout.Width (125), GUILayout.Height (25));

			GUILayout.EndHorizontal ();
			itemScroll = GUILayout.BeginScrollView (itemScroll,GUILayout.Width (355));

			foreach (GameObject g in id.items) {
				Item i = g.GetComponent<Item> ();

				GUILayout.BeginHorizontal ();
				GUILayout.Label (i.itemName, GUILayout.Width (125));
				if (shopEditing.getItemsISell().Contains (i.itemName) == false) {
					if (GUILayout.Button ("Set Sell Item", GUILayout.Width (110))) {
						shopEditing.addItemToShop (i.itemName);
						EditorUtility.SetDirty (shopEditing);
						EditorUtility.SetDirty (shopEditing.gameObject);
					}
				} else {
					GUILayout.Label ("Shop Sells", GUILayout.Width (125));

				}

				if (shopEditing.getItemsIBuy().Contains (i.itemName) == false) {
					if (GUILayout.Button ("Set Buy Item", GUILayout.Width (110))) {
						shopEditing.addItemICouldBuy (i.itemName);
						EditorUtility.SetDirty (shopEditing);
						EditorUtility.SetDirty (shopEditing.gameObject);

					}
				} else {
					GUILayout.Label ("Shop Buys", GUILayout.Width (125));
				}
				GUILayout.EndHorizontal ();
			}


			GUILayout.Label ("ITEMS SHOP SELLS");

			foreach (string st in shopEditing.itemsICouldSell) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label(st,GUILayout.Width (125), GUILayout.Height (25));
				if(GUILayout.Button("Remove",GUILayout.Width (125), GUILayout.Height (25))){
					shopEditing.itemsICouldSell.Remove(st);
					EditorUtility.SetDirty (shopEditing);
					EditorUtility.SetDirty (shopEditing.gameObject);
					GUILayout.EndHorizontal ();

					break;
				}
				GUILayout.EndHorizontal ();
			}
			GUILayout.Label ("ITEMS SHOP BUYS");
			foreach (string st in shopEditing.itemsIBuy) {
				GUILayout.BeginHorizontal ();
				GUILayout.Label(st,GUILayout.Width (125), GUILayout.Height (25));
				if(GUILayout.Button("Remove",GUILayout.Width (125), GUILayout.Height (25))){
					shopEditing.itemsIBuy.Remove(st);
					EditorUtility.SetDirty (shopEditing);
					EditorUtility.SetDirty (shopEditing.gameObject);
					GUILayout.EndHorizontal ();

					break;
				}
				GUILayout.EndHorizontal ();
			}

			GUILayout.EndScrollView ();
		}

		GUILayout.EndHorizontal ();
	}

    void drawShopHandles()
    {
       

        if (shopEditing == null)
        {
            return;
        } 
        if (npcsInWorld == null||npcsInWorld.Length==0)
        {
            npcsInWorld = FindObjectsOfType<NPCController>();
        }
        Debug.Log("Drawing shop handles.");
        foreach (NPCController npc in npcsInWorld)
        {
            if (shopEditing.myKeeper == npc.gameObject)
            {
                Handles.color = Color.white;
                if (inworldButton(npc.transform.position))
                {
                    shopEditing.myKeeper = null;
                    EditorUtility.SetDirty(shopEditing);
                }
            }
            else
            {
                Handles.color = Color.blue;
                if (inworldButton(npc.transform.position))
                {
                    shopEditing.myKeeper = npc.gameObject;
                    EditorUtility.SetDirty(shopEditing);

                }
            }
        }
    }

    int teleportDecide = 0;
    bool rotTeleport = false;
    GameObject telEdit;
    Teleport[] teleportsInWorld;
    AreaTransition[] transitionsInWorld;
    int teleportType = 0;
    void drawTeleportHandles()
    {
        if (telEdit != null)
        {
            if (rotTeleport == true)
            {/*
                    EditorGUI.BeginChangeCheck();

                    Vector3 p1 = Handles.PositionHandle(telEdit.transform.position, Quaternion.Euler(0, 0, 0));
                    if (EditorGUI.EndChangeCheck())
                    {
                        //Undo.RecordObject (r, "Moved position of " + rs.roomName);
                        telEdit.transform.position = p1;

                        EditorUtility.SetDirty(telEdit);
                        //EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
                    }*/

                EditorGUI.BeginChangeCheck();
                Vector3 scale = Handles.ScaleHandle(telEdit.transform.localScale,telEdit.transform.position, telEdit.transform.rotation, 2.0f);
              //  Vector3 p1 = Handles.PositionHandle(telEdit.transform.position, Quaternion.Euler(0, 0, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    //Undo.RecordObject (r, "Moved position of " + rs.roomName);
                    // telEdit.transform.position = p1;
                    telEdit.transform.localScale = scale;
                    EditorUtility.SetDirty(telEdit);
                    //EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
                }
                
            }
            else
            {
                EditorGUI.BeginChangeCheck();

                Vector3 p1 = Handles.PositionHandle(telEdit.transform.position, Quaternion.Euler(0, 0, 0));
                if (EditorGUI.EndChangeCheck())
                {
                    //Undo.RecordObject (r, "Moved position of " + rs.roomName);
                    telEdit.transform.position = p1;

                    EditorUtility.SetDirty(telEdit);
                    //EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
                }
                
            }
        }
       if(teleportDecide==1)
       {
            if(telEdit==null)
            {
                foreach(Teleport t in teleportsInWorld)
                {
                    Handles.color = Color.blue;
                    if (Handles.Button(t.transform.position, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                    {
                        telEdit = t.gameObject;
                        EditorUtility.SetDirty(t);
                        return;
                    }

                    Handles.color = Color.red;
                    if (Handles.Button(t.transform.position+new Vector3(1,0,0), Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                    {
                        DestroyImmediate(t.gameObject);
                        teleportsInWorld = FindObjectsOfType<Teleport>();

                        return;
                    }
                }
            }
            else
            {
                foreach (Teleport t in teleportsInWorld)
                {
                    if (t == telEdit.GetComponent<Teleport>())
                    {
                        
                      /*  Handles.color = Color.blue;
                        if (Handles.Button(t.transform.position, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                        {
                            telEdit = t.gameObject;
                            EditorUtility.SetDirty(t);
                            return;
                        }*/

                        Handles.color = Color.red;
                        if (Handles.Button(t.transform.position + new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                        {
                            DestroyImmediate(t.gameObject);
                            teleportsInWorld = FindObjectsOfType<Teleport>();

                            return;
                        }

                        if (t.toGoTo == null)
                        {

                        }
                        else
                        {
                            Handles.color = Color.cyan;
                            Handles.DrawLine(t.transform.position, t.toGoTo.transform.position);
                        }
                    }
                    else
                    {
                        Handles.color = Color.green;
                        if (Handles.Button(t.transform.position + new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                        {
                            //DestroyImmediate(t.gameObject);
                            telEdit.GetComponent<Teleport>().toGoTo = t;
                            t.toGoTo = telEdit.GetComponent<Teleport>();
                            teleportsInWorld = FindObjectsOfType<Teleport>();
                            EditorUtility.SetDirty(t);
                            EditorUtility.SetDirty(telEdit.GetComponent<Teleport>());
                            return;
                        }


                    }
                }
            }
        }else if(teleportDecide==3)
        {
            if(transitionsInWorld==null)
            {
                transitionsInWorld = FindObjectsOfType<AreaTransition>();
            }

            if(telEdit==null)
            {
                foreach(AreaTransition a in transitionsInWorld)
                {
                    Handles.Label(a.transform.position, a.sceneToGoTo);
                    Handles.Label(a.transform.position - new Vector3(0, 1, 0), a.startID.ToString());
                    Handles.Label(a.transform.position - new Vector3(0, 2, 0), a.sceneName);

                    Handles.color = Color.blue;
                    if (Handles.Button(a.transform.position, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                    {
                        telEdit = a.gameObject;
                        EditorUtility.SetDirty(a);
                        return;
                    }

                    Handles.color = Color.red;
                    if (Handles.Button(a.transform.position + new Vector3(1, 0, 0), Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                    {
                        DestroyImmediate(a.gameObject);
                        transitionsInWorld = FindObjectsOfType<AreaTransition>();

                        return;
                    }
                }
            }
            else
            {
                foreach (AreaTransition a in transitionsInWorld)
                {
                    Handles.Label(a.transform.position, a.sceneToGoTo);
                    Handles.Label(a.transform.position - new Vector3(0, 1, 0),a.startID.ToString());
                    Handles.Label(a.transform.position - new Vector3(0, 2, 0), a.sceneName);

                    Handles.color = Color.cyan;
                    if (Handles.Button(a.transform.position, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
                    {
                        telEdit = null;
                        EditorUtility.SetDirty(a);
                        return;
                    }

                    
                }
            }
        }
    }

    void drawTeleportUI()
    {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Place Teleports",GUILayout.Width(150),GUILayout.Height(25)))
        {
            teleportDecide = 0;
            telEdit = null;
        }

        if (GUILayout.Button("Edit Teleports", GUILayout.Width(150), GUILayout.Height(25)))
        {
            teleportDecide = 1;
            telEdit = null;
            teleportsInWorld = FindObjectsOfType<Teleport>();
        }

        if (GUILayout.Button("Place Level Transitions", GUILayout.Width(150), GUILayout.Height(25)))
        {
            teleportDecide = 2;
            telEdit = null;

        }

        if (GUILayout.Button("Edit Level Transitions", GUILayout.Width(150), GUILayout.Height(25)))
        {
            teleportDecide = 3;
            telEdit = null;

        }

        if(telEdit!=null)
        {
            EditorUtility.SetDirty(telEdit);
            Debug.Log("Tel edit is not null, marked dirty");
            if (telEdit.GetComponent<AreaTransition>() != null)
            {
                EditorUtility.SetDirty(telEdit.GetComponent<AreaTransition>());
                Debug.Log("got transition, marked dirty");

            }

            if (telEdit.GetComponent<Teleport>() != null)
            {
                EditorUtility.SetDirty(telEdit.GetComponent<Teleport>());
                Debug.Log("got teleport, marked dirty");

            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        

        if (teleportDecide==0)
        {
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Normal Teleport", GUILayout.Width(150), GUILayout.Height(25)))
            {
               teleportType = 0;
            }
            if (GUILayout.Button("Sewer Surface", GUILayout.Width(150), GUILayout.Height(25)))
            {
                teleportType = 1;
            }
            if (GUILayout.Button("Sewer Ladder", GUILayout.Width(150), GUILayout.Height(25)))
            {
                teleportType = 2;
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            GUILayout.EndHorizontal();
            if (teleportDecide == 1)
            {
                if(GUILayout.Button("Rot/Move Switch", GUILayout.Width(150)))
                {
                    rotTeleport = !rotTeleport;
                }
            }else if(teleportDecide==3)
            {
                if (telEdit != null)
                {
                    AreaTransition at = telEdit.GetComponent<AreaTransition>();
                    if (GUILayout.Button("Rot/Move Switch", GUILayout.Width(150)))
                    {
                        rotTeleport = !rotTeleport;
                    }

                    string sceneForEdit = getScene(at.sceneToGoTo);

                    if (sceneForEdit != "None")
                    {
                       at.sceneToGoTo = sceneForEdit;
                        EditorUtility.SetDirty(telEdit);
                        EditorUtility.SetDirty(at);

                    }
                    int areaToGoTo = at.startID;
                    at.startID = int.Parse(GUILayout.TextArea(areaToGoTo.ToString(),GUILayout.Width(50)));

                    at.sceneName = GUILayout.TextArea(at.sceneName, GUILayout.Width(150));

                    EditorUtility.SetDirty(telEdit);
                    EditorUtility.SetDirty(at);


                }
            }
        }

    }
    CommonObjectsStore common;
    void teleportInput()
    {
        if(common==null)
        {
            common = FindObjectOfType<CommonObjectsStore>();
        }

        

        if(teleportDecide==0)
        {
            Vector3 vector3 = new Vector3(pos.x,pos.y,0);
            
            if(teleportType==0)
            {
                GameObject teleport = (GameObject)Instantiate(common.teleport, vector3, Quaternion.Euler(0, 0, 0));

            }
            else if(teleportType==1)
            {
                GameObject teleport = (GameObject)Instantiate(common.sewerEntrance, vector3, Quaternion.Euler(0, 0, 0));

            }
            else if(teleportType==2)
            {
                GameObject teleport = (GameObject)Instantiate(common.sewerExit, vector3, Quaternion.Euler(0, 0, 0));

            }
            teleportsInWorld = FindObjectsOfType<Teleport>();
        }else if(teleportDecide==1)
        {
           
        }else if(teleportDecide==2)
        {
            Vector3 vector3 = new Vector3(pos.x, pos.y, 0);

            GameObject teleport = (GameObject)Instantiate(common.levelTransition, vector3, Quaternion.Euler(0, 0, 0));
           
        }
        else if(teleportDecide==3)
        {
            
        }
    }

    IngameEventManager ie;
    IngameEvent eventEditing;
    IngameEvent[] events;
    Vector2 eventScroll = Vector2.zero,conditionScroll=Vector2.zero;
    TimeScript ts;
    IngameEventCondition conditionEditing;
    void drawEventUI()
    {
        if(ie==null)
        {
            ie = FindObjectOfType<IngameEventManager>();
        }

        if(events==null)
        {
            events = FindObjectsOfType<IngameEvent>();
        }

        if(ts==null)
        {
            ts = FindObjectOfType<TimeScript>();
        }

        if(eventEditing==null)
        {
            label("Select an event to edit");
            eventScroll = GUILayout.BeginScrollView(eventScroll, GUILayout.Width(200), GUILayout.Height(300));
            foreach(IngameEvent e in events)
            {
                GUILayout.BeginHorizontal();
                if (button(e.eventName, 125, 25))
                {
                    eventEditing = e;
                }
                if(button("X",35,35))
                {
                    foreach(IngameEventCondition cond in e.conditionsToTrigger)
                    {
                        DestroyImmediate(cond);
                    }
                    DestroyImmediate(e);
                    events = FindObjectsOfType<IngameEvent>();
                    return;
                }
                GUILayout.EndHorizontal();
            }
            if(button("Add New Event",125,25))
            {
                eventEditing = ie.gameObject.AddComponent<IngameEvent>();
                eventEditing.myID = FindObjectOfType<IDManager>().getEventID();
                EditorUtility.SetDirty(ie);
                EditorUtility.SetDirty(eventEditing);
            }
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.BeginHorizontal();
            label("Event Name");
            eventEditing.eventName = textInput(eventEditing.eventName, 200, 30);
            GUILayout.EndHorizontal();

            if(button("Event repeatable " + eventEditing.Repeatable.ToString(),125,25))
            {
                eventEditing.Repeatable = !eventEditing.Repeatable;
            }

            if(eventEditing.Repeatable)
            {
                GUILayout.BeginHorizontal();
                label("Hours till repeat: ");
                eventEditing.hoursTillRepeat = numberInput(eventEditing.hoursTillRepeat);

                GUILayout.EndHorizontal();
            }

            label("Event Conditions");
            if(eventEditing.conditionsToTrigger==null)
            {
                eventEditing.conditionsToTrigger = new List<IngameEventCondition>();
            }
            conditionScroll = GUILayout.BeginScrollView(conditionScroll, GUILayout.Width(300), GUILayout.Height(400));

            label("Conditions to trigger:");
            if(button("Time of day", 125, 25))
            {
                IngameEventCondition adding = eventEditing.gameObject.AddComponent<IngameEventCondition_TimeOfDay>();
                eventEditing.conditionsToTrigger.Add(adding);
                EditorUtility.SetDirty(eventEditing);
                EditorUtility.SetDirty(adding);


            }

            if (button("Unique NPC Condition", 125, 25))
            {
                IngameEventCondition adding = eventEditing.gameObject.AddComponent<IngameEventCondition_UniqueNPCState>();

                eventEditing.conditionsToTrigger.Add(adding);
                EditorUtility.SetDirty(eventEditing);
                EditorUtility.SetDirty(adding);

            }

            if(button("Random Chance", 125, 25))
            {
                IngameEventCondition adding = eventEditing.gameObject.AddComponent<IngameEventConditionRandomChance>();
                eventEditing.conditionsToTrigger.Add(adding);
                EditorUtility.SetDirty(eventEditing);
                EditorUtility.SetDirty(adding);

            }

            label("Edit condition");
            foreach (IngameEventCondition cond in eventEditing.conditionsToTrigger)
            {
                GUILayout.BeginHorizontal();
                if (button(cond.GetType().ToString(),225,25)){
                    conditionEditing = cond;
                }
                if(button("X",30,30))
                {
                    eventEditing.conditionsToTrigger.Remove(cond);
                    DestroyImmediate(cond);
                    EditorUtility.SetDirty(eventEditing);
                    return;
                }

                GUILayout.EndHorizontal();
            }

            if(conditionEditing==null)
            {
                if (button("Finished Editing", 125, 25))
                {
                    EditorUtility.SetDirty(eventEditing);
                    eventEditing = null;
                }
            }else if(conditionEditing.GetType()==typeof(IngameEventCondition_TimeOfDay))
            {
                label("Editing time of day");

                IngameEventCondition_TimeOfDay cond = (IngameEventCondition_TimeOfDay)conditionEditing;
                GUILayout.BeginHorizontal();
                label("Start Hour: "+cond.Shour.ToString());
                cond.Shour = sliderInput(cond.Shour, 0, 23);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                label("Start Min: "+cond.Smin.ToString());
                cond.Smin = sliderInput(cond.Smin, 0, 59);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                label("End Hour: "+cond.Ehour.ToString());
                cond.Ehour = sliderInput(cond.Ehour, 0, 23);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                label("End Min: "+cond.Emin.ToString());
                cond.Emin = sliderInput(cond.Emin, 0, 59);
                GUILayout.EndHorizontal();

                EditorUtility.SetDirty(cond);
            }
            else if(conditionEditing.GetType()==typeof(IngameEventCondition_UniqueNPCState))
            {
                label("Editing unique NPC state");
                IngameEventCondition_UniqueNPCState cond = (IngameEventCondition_UniqueNPCState)conditionEditing;

                label("ID = " + cond.IDToCheck);
                if(cond.deadState==true)
                {
                    if(button("has to be dead to trigger", 150, 25))
                    {
                        cond.deadState = !cond.deadState;
                    }
                }
                else
                {
                    if (button("has to be alive to trigger", 150, 25))
                    {
                        cond.deadState = !cond.deadState;
                    }
                }

                displayGlobalNPCData();
                int id = getUniquieNPCID(cond.IDToCheck);
                if(id!=-1)
                {
                    cond.IDToCheck = id;
                    EditorUtility.SetDirty(cond);
                }
            }else if (conditionEditing.GetType() == typeof(IngameEventConditionRandomChance))
            {
                label("Editing random chance");

                label("Max Chance:");
                IngameEventConditionRandomChance cond = (IngameEventConditionRandomChance)conditionEditing;

                cond.maxValue = sliderInput(cond.maxValue, 0, 1000);
                label("Threshold (value must be lower to be true)");
                cond.threshold = sliderInput(cond.threshold, 0, cond.maxValue);
                EditorUtility.SetDirty(cond);

            }

            if (button("Done Editing Condition", 155, 25))
            {
                EditorUtility.SetDirty(conditionEditing);
                conditionEditing = null;
            }
            GUILayout.EndScrollView();
            EditorUtility.SetDirty(eventEditing);

           

            
        }
    }

    int sliderInput(int val,int min,int max)
    {
        return(int) GUILayout.HorizontalSlider(val, min, max, GUILayout.Width(200), GUILayout.Height(30));
    }

    int numberInput(int content)
    {
        try
        {
            return int.Parse(GUILayout.TextField(content.ToString(), GUILayout.Width(125), GUILayout.Height(30)));
        }
        catch
        {
            return 0;
        }
    }
    int numberInput(int content,int width,int height)
    {
        try
        {
            return int.Parse(GUILayout.TextField(content.ToString(), GUILayout.Width(width), GUILayout.Height(height)));
        }
        catch
        {
            return 0;
        }
    }

    string textInput(string content)
    {
        return GUILayout.TextField(content, GUILayout.Width(125), GUILayout.Height(30));
    }

    string textInput(string content, int width, int height)
    {
        return GUILayout.TextField(content, GUILayout.Width(width), GUILayout.Height(height));
    }

    ItemInWorld[] itemInWorlds;
    NPCController[] npcsInWorld;
    void drawEventHandles()
    {
        if (eventEditing == null)
        {

        }
        else
        {
            if (eventEditing.objectsToEnable == null)
            {
                eventEditing.objectsToEnable = new List<GameObject>();
            }
            if(itemInWorlds==null)
            {
                itemInWorlds = FindObjectsOfType<ItemInWorld>();
            }

            if (npcsInWorld == null)
            {
                npcsInWorld = FindObjectsOfType<NPCController>();
            }

            foreach(ItemInWorld i in itemInWorlds)
            {
                if(eventEditing.objectsToEnable.Contains(i.gameObject))
                {
                    Handles.color = Color.red;
                    if (inworldButton(i.transform.position))
                    {
                        eventEditing.objectsToEnable.Remove(i.gameObject);
                    }
                }
                else
                {
                    Handles.color = Color.blue;
                    if(inworldButton(i.transform.position))
                    {
                        eventEditing.objectsToEnable.Add(i.gameObject);
                    }
                }
            }

            foreach (NPCController i in npcsInWorld)
            {
                if (eventEditing.objectsToEnable.Contains(i.gameObject))
                {
                    Handles.color = Color.red;
                    if (inworldButton(i.transform.position))
                    {
                        eventEditing.objectsToEnable.Remove(i.gameObject);
                    }
                }
                else
                {
                    Handles.color = Color.blue;
                    if (inworldButton(i.transform.position))
                    {
                        eventEditing.objectsToEnable.Add(i.gameObject);
                    }
                }
            }
        }
    }

    bool inworldButton(Vector3 pos)
    {

        if (Handles.Button(pos, Quaternion.Euler(0, 0, 0), 1.0f, 1.0f, Handles.RectangleHandleCap))
        {
            Handles.BeginGUI();
            return true;
        }
      
        return false;
    }
    public GameObject lorePrefab,loreEditing;
    public List<GameObject> emailObjects,phoneObjects;
    Email[] emailsEditing;
    PhoneMessage[] phoneEditing;
    Email EmailEditing;
    PhoneMessage PhoneEditing;
    bool editingEmail = false;
    void setBackgroundObjects()
    {
        emailObjects = new List<GameObject>();
        phoneObjects = new List<GameObject>();
        Email[] emails = FindObjectsOfType<Email>();
        foreach(Email e in emails)
        {
            if (emailObjects.Contains(e.gameObject) == false)
            {
                emailObjects.Add(e.gameObject);
            }
        }

        PhoneMessage[] phoneMessages = FindObjectsOfType<PhoneMessage>();
        foreach(PhoneMessage p in phoneMessages)
        {
            if (phoneObjects.Contains(p.gameObject) == false)
            {
                phoneObjects.Add(p.gameObject);
            }
        }
    }

    void drawBackgroundUI()
    {
        if(common==null)
        {
            common = FindObjectOfType<CommonObjectsStore>();
        }

        if (loreEditing == null)
        {
            foreach (GameObject g in common.loreItems)
            {
                if (button(g.name, 125, 25))
                {
                    lorePrefab = g;
                }
            }
        }
        else
        {
            if(editingEmail==true)
            {
                if (EmailEditing == null)
                {
                    if(button("Add new Email",125,25))
                    {
                        loreEditing.AddComponent<Email>();
                        emailsEditing = loreEditing.GetComponents<Email>();
                    }

                    foreach (Email e in emailsEditing)
                    {
                        if (button("Edit " + e.sender, 125, 25))
                        {
                            EmailEditing = e;
                        }
                    }
                }
                else
                {
                    if(button("Done Editing",125,25))
                    {
                        EditorUtility.SetDirty(EmailEditing);
                        EmailEditing = null;
                        
                    }
                    label("Email Contents");
                    EmailEditing.contents = GUILayout.TextArea(EmailEditing.contents, GUILayout.Width(150), GUILayout.Height(75));

                    label("Email Sender");
                    EmailEditing.sender = textInput(EmailEditing.sender);
                    label("Subject");
                    EmailEditing.subject = textInput(EmailEditing.subject);

                    if (ts==null)
                    {
                        ts = FindObjectOfType<TimeScript>();
                    }
                    label("Date Sent " + EmailEditing.daySend+"/"+EmailEditing.monthSend+"/"+EmailEditing.yearSend);
                    EmailEditing.daySend = sliderInput(EmailEditing.daySend, 0, ts.daysInMonth[EmailEditing.monthSend-1]);
                    EmailEditing.monthSend = sliderInput(EmailEditing.monthSend, 1, 12);
                    EmailEditing.yearSend = numberInput(EmailEditing.yearSend);
                }
            }
            else
            {
                if (PhoneEditing == null)
                {
                    if(button("Add new text message", 125, 25))
                    {
                        loreEditing.AddComponent<PhoneMessage>();
                        phoneEditing = loreEditing.GetComponents<PhoneMessage>();
                    }

                    foreach (PhoneMessage p in phoneEditing)
                    {
                        if (button("Edit phone message from " + p.sender, 200, 25))
                        {
                            PhoneEditing = p;
                        }

                    }
                }
                else
                {
                    if (button("Done Editing", 125, 25))
                    {
                        EditorUtility.SetDirty(PhoneEditing);
                        PhoneEditing = null;

                    }
                    label("Text Contents");
                    PhoneEditing.messageText = GUILayout.TextArea(PhoneEditing.messageText, GUILayout.Width(150), GUILayout.Height(75));

                    label("Text Sender");
                    PhoneEditing.sender = textInput(PhoneEditing.sender);
                   // label("Subject");
                   // PhoneEditing.subject = textInput(PhoneEditing.subject);

                    if (ts == null)
                    {
                        ts = FindObjectOfType<TimeScript>();
                    }
                    label("Date Sent " + PhoneEditing.daySend + "/" + PhoneEditing.monthSend + "/" + PhoneEditing.yearSend);
                   
                    PhoneEditing.daySend = sliderInput(PhoneEditing.daySend, 0, ts.daysInMonth[PhoneEditing.monthSend - 1]);

                    PhoneEditing.monthSend = sliderInput(PhoneEditing.monthSend, 1, 12);
                    PhoneEditing.yearSend = numberInput(PhoneEditing.yearSend);
                }
            }
        }
    }

    void drawBackgroundHandles()
    {
        if(emailObjects==null || phoneObjects==null)
        {
            setBackgroundObjects();
        }

        foreach (GameObject g in emailObjects)
        {
            if (loreEditing != g)
            {
                Handles.color = Color.blue;
                if (inworldButton(g.transform.position))
                {
                    loreEditing = g;
                    emailsEditing = g.GetComponents<Email>();
                    editingEmail = true;
                }
            }
            else
            {
                Handles.color = Color.white;
                if (inworldButton(g.transform.position))
                {
                    loreEditing = null;

                }

                Handles.color = Color.red;
                if (inworldButton(g.transform.position + new Vector3(2, 0, 0)))
                {
                    foreach (Email pm in loreEditing.GetComponents<Email>())
                    {
                        DestroyImmediate(pm);
                    }

                    DestroyImmediate(loreEditing);
                    setBackgroundObjects();
                    return;
                }
            }
            
        }

        foreach (GameObject g in phoneObjects)
        {
            if (loreEditing != g)
            {
                Handles.color = Color.blue;
                if (inworldButton(g.transform.position))
                {
                    loreEditing = g;
                    phoneEditing = g.GetComponents<PhoneMessage>();
                    editingEmail = false;
                }
            }
            else
            {
                Handles.color = Color.white;
                if (inworldButton(g.transform.position))
                {
                    loreEditing = null;

                }

                Handles.color = Color.red;
                if (inworldButton(g.transform.position + new Vector3(2, 0, 0)))
                {
                    foreach (PhoneMessage pm in loreEditing.GetComponents<PhoneMessage>())
                    {
                        DestroyImmediate(pm);
                    }

                    DestroyImmediate(loreEditing);
                    setBackgroundObjects();
                    return;
                }
            }
           
        }

    }


    Vector2 sceneScroll = Vector2.zero;
    string getScene(string currentScene)
    {
        string retVal = currentScene;
        int numScenes = EditorSceneManager.sceneCountInBuildSettings;
        sceneScroll = GUILayout.BeginScrollView(sceneScroll, GUILayout.Width(300), GUILayout.Height(200));
        for (int x = 0; x < numScenes; x++)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(x));

            if (button(name, 200, 30))
            {
                retVal = name;
            }
        }
        GUILayout.EndScrollView();
        return retVal;
    }
    Vector2 misScroll = Vector2.zero;
    bool button(string content, int width, int height)
    {
        if (GUILayout.Button(content, GUILayout.Width(width), GUILayout.Height(height)))
        {
            return true;
        }
        return false;
    }

    bool isSceneInBuild(string st)
    {
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
        {
            if (s.path.Contains(st))
            {
                return true;
            }
        }
        return false;
    }

    void label(string content)
    {
        GUILayout.Label(content, GUILayout.Width(125), GUILayout.Height(30));
    }
    void label(string content, int width, int height)
    {
        GUILayout.Label(content, GUILayout.Width(width), GUILayout.Height(height));
    }

    void inworldLabel(GameObject gameObject, string label)
    {
        Handles.EndGUI();
        // Handles.color = Color.red;
        Handles.Label(gameObject.transform.position, label);

        Handles.BeginGUI();
    }
    void inworldLabel(Vector3 position, string label)
    {
        Handles.EndGUI();
        // Handles.color = Color.red;
        Handles.Label(position, label);
        Handles.BeginGUI();
    }

    List<string> uniquieNPCDataFromFile;
    void getGlobalNPCData()
    {
        string folderPath = Path.Combine(Application.dataPath, "NPCIDData");

        string path = Path.Combine(folderPath, "UniquieNPCData.txt");
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        if (File.Exists(path) == true)
        {
            uniquieNPCDataFromFile = readFile(path);
            //writeIDToFile(folderPath);
        }
    }
    Vector2 globalVec = Vector2.zero;
    bool disp = false;
    void displayGlobalNPCData()
    {
        if (uniquieNPCDataFromFile == null || uniquieNPCDataFromFile.Count == 0)
        {
            getGlobalNPCData();
        }
        if (button("Display Global",125,25))
        {
            disp = !disp;
        }
        globalVec = GUILayout.BeginScrollView(globalVec);
        if (disp == true)
        {
            foreach (string st in uniquieNPCDataFromFile)
            {
                label(st, st.Length * 8, 30);
            }
        }
        GUILayout.EndScrollView();
    }
    Vector2 uniqueScroll = Vector2.zero;
    int getUniquieNPCID(int id)
    {
        int retVal = -1;
        if (uniquieNPCDataFromFile == null || uniquieNPCDataFromFile.Count == 0)
        {
            getGlobalNPCData();
        }
        uniqueScroll = GUILayout.BeginScrollView(uniqueScroll, GUILayout.Width(200), GUILayout.Height(200));

        foreach (string st in uniquieNPCDataFromFile)
        {
            if (button(st, st.Length * 8, 30))
            {
                string[] dat = st.Split(';');
                int val = int.Parse(dat[0]);
                if (val != id)
                {
                    retVal = val;
                }
            }
        }

        GUILayout.EndScrollView();
        return retVal;
    }
}
