using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
[CustomEditor(typeof(NPCIDManager))]
public class UniqueNPCEditor : Editor {//need to have a way to record ID's and what scene they are in

    //TODO:
    //test out unique NPC serialization
    //test out editor saving & loading of t

    //Add a file that controls what unique NPCs should be active
    //have a class that allows the activation of NPCs in the conversation tree.
    //have shops record whether they have been robbed by the player/shopkeeper attacked

    Vector3 pos = Vector3.zero;
    NPCID[] npcsInWorld;

    uniqueNPCSetting mySetting;

    GameObject npcEditing;
    NPCController controllerEditing;
    NPCBehaviourDecider deciderEditing;
    PersonClothesController clothesEditing;
    editingTask currentTask;
    Inventory inventoryEditing;
    NPCItemInitialiser npcItems;
    NPCID idEditing;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
    CommonObjectsStore c = null;
    IDManager im = null;
    Vector2 taskScroll = Vector2.zero, viewScroll = Vector2.zero;
    void OnSceneGUI()
    {
        pos = Event.current.mousePosition;
        pos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - pos.y;
        pos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(pos);
        pos.z = 0;
        Selection.selectionChanged += OnDeselect;

        if (im == null)
        {
            im = FindObjectOfType<IDManager>();
        }

        if (c == null)
        {
            c = FindObjectOfType<CommonObjectsStore>();
        }
        if (id == null)
        {
            id = FindObjectOfType<ItemDatabase>();
        }

        Handles.BeginGUI();
        /*  string[] options = System.Enum.GetNames(typeof( uniqueNPCSetting));
          GUILayout.BeginHorizontal();

          foreach(string st in options)
          {
              if (button(st))
              {
                  mySetting = getSettingFromString(st) ;
              }
          }
          GUILayout.EndHorizontal();*/
        
        if (mySetting==uniqueNPCSetting.menu)
        {
            //draw buttons on each of the NPCID's found in the level, 
            if(npcsInWorld==null || npcsInWorld.Length==0)
            {
                refreshNPCS();
            }

            foreach(NPCID npc in npcsInWorld)
            {
                inworldLabel(npc.transform.position, npc.firstName + " " + npc.lastName);
                if(inworldButton(npc.gameObject.transform.position,Color.cyan))
                {
                    npcEditing = npc.gameObject;
                    controllerEditing = npcEditing.GetComponent<NPCController>();
                    clothesEditing = npcEditing.GetComponent<PersonClothesController>();
                    deciderEditing = npcEditing.GetComponent<NPCBehaviourDecider>();
                    inventoryEditing = npcEditing.GetComponent<Inventory>();
                    idEditing = npc;
                    npcItems = npcEditing.GetComponent<NPCItemInitialiser>();
                    mySetting = uniqueNPCSetting.editing;
                }
            }

            if(button("New Unique NPC"))
            {
                GameObject g = (GameObject)Instantiate(c.civilian,new Vector3( SceneView.currentDrawingSceneView.camera.transform.position.x, SceneView.currentDrawingSceneView.camera.transform.position.y,0), Quaternion.Euler(0,0,0));
                NPCID createdID = g.AddComponent<NPCID>();
                createdID.myId = im.getNPCID();
                g.GetComponent<NPCBehaviourDecider>().myType = AIType.neutral;
                g.GetComponent<NPCController>().myType = AIType.neutral;
                refreshNPCS();
                EditorUtility.SetDirty(id);
                EditorUtility.SetDirty(createdID);
            }
        }else if (mySetting == uniqueNPCSetting.editing)
        {
            if (button("Menu"))
            {
                EditorUtility.SetDirty(npcEditing);
                EditorUtility.SetDirty(controllerEditing);
                EditorUtility.SetDirty(clothesEditing);
                EditorUtility.SetDirty(deciderEditing);
                EditorUtility.SetDirty(idEditing);
                EditorUtility.SetDirty(inventoryEditing);
                EditorUtility.SetDirty(npcItems);
                npcEditing = null;
                controllerEditing = null;
                clothesEditing = null;
                deciderEditing = null;

                mySetting = uniqueNPCSetting.menu;
            }
            string[] editOptions = System.Enum.GetNames(typeof(editingTask));
            string info = "Editing NPC with the ID " + idEditing.myId;
            label(info, info.Length * 8, 30);
            taskScroll = GUILayout.BeginScrollView(taskScroll, GUILayout.Width(500), GUILayout.Height(50));
            GUILayout.BeginHorizontal();
            foreach (string st in editOptions)
            {
                if (button(st))
                {
                    currentTask = getTaskFromString(st);   
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.Label(currentTask.ToString());
            if(currentTask == editingTask.name)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                GUILayout.BeginHorizontal();
                string st = "Name: " + idEditing.firstName + " " + idEditing.lastName;
                label(st, st.Length * 8, 30);
                idEditing.firstName = textInput(idEditing.firstName);
                idEditing.lastName = textInput(idEditing.lastName);
                EditorUtility.SetDirty(idEditing);
                EditorUtility.SetDirty(npcEditing);
                GUILayout.EndHorizontal();

                if(button("NPC Always Active" + idEditing.alwaysActive.ToString()))
                {
                    idEditing.alwaysActive = !idEditing.alwaysActive;

                }

                GUILayout.EndScrollView();

            }
            else if(currentTask==editingTask.position)
            {
                if(editRotation==false)
                {
                    if( button("Change Rotation"))
                    {
                        editRotation = true;
                    }
                    drawPositionHandle(ref npcEditing);
                    EditorUtility.SetDirty(npcEditing);
                }
                else
                {
                    if (button("Change Position"))
                    {
                        editRotation = false;
                    }
                    drawRotationHandle(ref npcEditing);
                    EditorUtility.SetDirty(npcEditing);

                }
            }else if(currentTask==editingTask.items)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                label("Items To add:");
                string item = getItems();
                if(item!="None")
                {
                    if (npcItems.itemsToAdd == null)
                    {
                        npcItems.itemsToAdd = new List<string>();
                    }
                    npcItems.itemsToAdd.Add(item);
                    EditorUtility.SetDirty(npcItems);
                }

                label("Add random item:");
                string randItem = getItems();
                if(randItem!="None")
                {
                    if(npcItems.randomItems==null)
                    {
                        npcItems.randomItems = new List<string>();
                    }
                    npcItems.randomItems.Add(randItem);
                    EditorUtility.SetDirty(npcItems);

                }
                label("Items Being Given");
                foreach(string st in npcItems.itemsToAdd)
                {
                    GUILayout.BeginHorizontal();
                    label(st, st.Length * 8, 30);
                    if (button("X"))
                    {
                        npcItems.itemsToAdd.Remove(st);
                        EditorUtility.SetDirty(npcItems);

                        return;
                    }
                    GUILayout.EndHorizontal();
                }
                label("Items that may be given");
                foreach (string st in npcItems.randomItems)
                {
                    GUILayout.BeginHorizontal();
                    label(st, st.Length * 8, 30);
                    if (button("X"))
                    {
                        npcItems.randomItems.Remove(st);
                        EditorUtility.SetDirty(npcItems);

                        return;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();

            }
            else if(currentTask==editingTask.aesthetics)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                if (clothesEditing.clothesToWearAtStart == null)
                {
                    clothesEditing.clothesToWearAtStart = new List<string>();
                }

                if(button("Random Clothes " + clothesEditing.getRandomClothes.ToString()))
                {
                    clothesEditing.getRandomClothes = !clothesEditing.getRandomClothes;
                }
                label("Clothes you could add:");
                if (clothesEditing.getRandomClothes == false)
                {
                    string st = getClothes();
                    if(st!="None")
                    {
                        if(clothesEditing.clothesToWearAtStart.Contains(st)==false)
                        {
                            clothesEditing.clothesToWearAtStart.Add(st);
                        }
                    }
                }

                foreach(string st in clothesEditing.clothesToWearAtStart)
                {
                    GUILayout.BeginHorizontal();
                    label(st, st.Length * 8, 30);
                    if (button("X"))
                    {
                        clothesEditing.clothesToWearAtStart.Remove(st);
                        return;
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }
            else if (currentTask == editingTask.whenToActivate)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));
                label("Activate NPC:");
               /* label("On Mission Start");
                string missionStart = getMission();
                if(missionStart!="None")
                {
                    if (idEditing.started_missionsToActivateOn == null)
                    {
                        idEditing.started_missionsToActivateOn = new List<string>();
                    }
                    if (idEditing.started_missionsToActivateOn.Contains(missionStart) == false)
                    {
                        idEditing.started_missionsToActivateOn.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }

                label("On Mission Finish");
                string missionEnd = getMission();
                if(missionEnd!="None")
                {
                    if (idEditing.finished_missionsToActivateOn == null)
                    {
                        idEditing.finished_missionsToActivateOn = new List<string>();
                    }
                    if (idEditing.finished_missionsToActivateOn.Contains(missionStart) == false)
                    {
                        idEditing.finished_missionsToActivateOn.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }
                */
                label("Unique NPC's Dead");
                int val = getUniquieNPCID();
                if(val!=-1)
                {
                    if(idEditing.idsToActivateOnIfDead==null)
                    {
                        idEditing.idsToActivateOnIfDead = new List<int>();
                    }

                    if (idEditing.idsToActivateOnIfDead.Contains(val) == false)
                    {
                        idEditing.idsToActivateOnIfDead.Add(val);
                    }
                }

                label("------------------------");
               
                GUILayout.EndScrollView();
            }else if (currentTask == editingTask.whenToDeactivate)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                label("Deactivate NPC:");
               /* label("On Mission Start");
                string missionStart = getMission();
                if (missionStart != "None")
                {
                    if (idEditing.started_missionsToDeactivateOn == null)
                    {
                        idEditing.started_missionsToDeactivateOn = new List<string>();
                    }
                    if (idEditing.started_missionsToDeactivateOn.Contains(missionStart) == false)
                    {
                        idEditing.started_missionsToDeactivateOn.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }

                label("On Mission Finish");
                string missionEnd = getMission();
                if (missionEnd != "None")
                {
                    if (idEditing.finished_missionsToDeactivateOn == null)
                    {
                        idEditing.finished_missionsToDeactivateOn = new List<string>();
                    }
                    if (idEditing.finished_missionsToDeactivateOn.Contains(missionStart) == false)
                    {
                        idEditing.finished_missionsToDeactivateOn.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }*/

                label("Unique NPC's Dead");
                int val = getUniquieNPCID();
                if (val != -1)
                {
                    if (idEditing.idsToDeactivateOnIfDead == null)
                    {
                        idEditing.idsToDeactivateOnIfDead = new List<int>();
                    }

                    if (idEditing.idsToDeactivateOnIfDead.Contains(val) == false)
                    {
                        idEditing.idsToDeactivateOnIfDead.Add(val);
                        EditorUtility.SetDirty(idEditing);
                    }
                }

                GUILayout.EndScrollView();
            }else if (currentTask == editingTask.peacefulConditions)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                label("Make NPC Peaceful:");
                /*label("On Mission Start");
                string missionStart = getMission();
                if (missionStart != "None")
                {
                    if (idEditing.started_missionsToMakeNotHostile == null)
                    {
                        idEditing.started_missionsToMakeNotHostile = new List<string>();
                    }
                    if (idEditing.started_missionsToMakeNotHostile.Contains(missionStart) == false)
                    {
                        idEditing.started_missionsToMakeNotHostile.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }

                label("On Mission Finish");
                string missionEnd = getMission();
                if (missionEnd != "None")
                {
                    if (idEditing.finished_missionsToMakeNotHostile == null)
                    {
                        idEditing.finished_missionsToMakeNotHostile = new List<string>();
                    }
                    if (idEditing.finished_missionsToMakeNotHostile.Contains(missionStart) == false)
                    {
                        idEditing.finished_missionsToMakeNotHostile.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }
                */
                label("Unique NPC's Dead");
                int val = getUniquieNPCID();
                if (val != -1)
                {
                    if (idEditing.idsThatMakeNotHostileOnDeath == null)
                    {
                        idEditing.idsThatMakeNotHostileOnDeath = new List<int>();
                    }

                    if (idEditing.idsThatMakeNotHostileOnDeath.Contains(val) == false)
                    {
                        idEditing.idsThatMakeNotHostileOnDeath.Add(val);
                        EditorUtility.SetDirty(idEditing);

                    }
                }

                GUILayout.EndScrollView();
            }
            else if(currentTask==editingTask.hostileConditions)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                label("Make NPC Hostile:");
               /* label("On Mission Start");
                string missionStart = getMission();
                if (missionStart != "None")
                {
                    if (idEditing.started_missionsToMakeHostile == null)
                    {
                        idEditing.started_missionsToMakeHostile = new List<string>();
                    }
                    if (idEditing.started_missionsToMakeHostile.Contains(missionStart) == false)
                    {
                        idEditing.started_missionsToMakeHostile.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }

                label("On Mission Finish");
                string missionEnd = getMission();
                if (missionEnd != "None")
                {
                    if (idEditing.finished_missionsToMakeHostile == null)
                    {
                        idEditing.finished_missionsToMakeHostile = new List<string>();
                    }
                    if (idEditing.finished_missionsToMakeHostile.Contains(missionStart) == false)
                    {
                        idEditing.finished_missionsToMakeHostile.Add(missionStart);
                        EditorUtility.SetDirty(idEditing);
                        return;
                    }
                }*/

                label("Unique NPC's Dead");
                int val = getUniquieNPCID();
                if (val != -1)
                {
                    if (idEditing.idsThatMakeHostileOnDeath == null)
                    {
                        idEditing.idsThatMakeHostileOnDeath = new List<int>();
                    }

                    if (idEditing.idsThatMakeHostileOnDeath.Contains(val) == false)
                    {
                        idEditing.idsThatMakeHostileOnDeath.Add(val);
                        EditorUtility.SetDirty(idEditing);

                    }
                }

                GUILayout.EndScrollView();
            }else if(currentTask==editingTask.whenToEnable)
            {

            }else if(currentTask==editingTask.whenToDisable)
            {

            }else if(currentTask==editingTask.editLists)
            {
                viewScroll = GUILayout.BeginScrollView(viewScroll, GUILayout.Width(400), GUILayout.Height(400));

                label("Hostile Conditions:");
                //hostile
                if (idEditing.idsThatMakeHostileOnDeath != null)
                {
                    foreach(int i in idEditing.idsThatMakeHostileOnDeath)
                    {
                        if(button("Remove " + i.ToString()))
                        {
                            idEditing.idsThatMakeHostileOnDeath.Remove(i);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.started_missionsToMakeHostile != null)
                {
                    foreach(string st in idEditing.started_missionsToMakeHostile)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.started_missionsToMakeHostile.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.finished_missionsToMakeHostile!= null)
                {
                    foreach(string st in idEditing.finished_missionsToMakeHostile)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.finished_missionsToMakeHostile.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                label("Peaceful Conditions:");
                //not hostile
                if (idEditing.idsThatMakeNotHostileOnDeath!=null)
                {
                    foreach(int i in idEditing.idsThatMakeNotHostileOnDeath)
                    {
                        if(button("Remove " + i.ToString()))
                        {
                            idEditing.idsThatMakeNotHostileOnDeath.Remove(i);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.started_missionsToMakeNotHostile != null)
                {
                    foreach(string st in idEditing.started_missionsToMakeNotHostile)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.started_missionsToMakeNotHostile.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.finished_missionsToMakeNotHostile != null)
                {
                    foreach(string st in idEditing.finished_missionsToMakeNotHostile)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.finished_missionsToMakeNotHostile.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                label("Activate Conditions:");
                //activate NPC
                if(idEditing.idsToActivateOnIfDead!=null)
                {
                    foreach(int i in idEditing.idsToActivateOnIfDead)
                    {
                        if(button("Remove " + i.ToString()))
                        {
                            idEditing.idsToActivateOnIfDead.Remove(i);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.started_missionsToActivateOn != null)
                {
                    foreach(string st in idEditing.started_missionsToActivateOn)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.started_missionsToActivateOn.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }


                if (idEditing.finished_missionsToActivateOn != null)
                {
                    foreach(string st in idEditing.finished_missionsToActivateOn)
                    {
                        if(button("Remove " + st))
                        {
                            idEditing.finished_missionsToActivateOn.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                label("Deactivate Conditions:");
                //deactivate NPC
                if (idEditing.idsToDeactivateOnIfDead != null)
                {
                    foreach(int i in idEditing.idsToDeactivateOnIfDead)
                    {
                        if(button("Remove " + i.ToString()))
                        {
                            idEditing.idsToDeactivateOnIfDead.Remove(i);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }

                if (idEditing.started_missionsToDeactivateOn != null)
                {
                    foreach (string st in idEditing.finished_missionsToDeactivateOn)
                    {
                        if (button("Remove " + st))
                        {
                            idEditing.finished_missionsToDeactivateOn.Remove(st);
                            EditorUtility.SetDirty(idEditing);

                            return;
                        }
                    }
                }


                if (idEditing.finished_missionsToDeactivateOn != null)
                {
                    foreach (string st in idEditing.finished_missionsToDeactivateOn)
                    {
                        if (button("Remove " + st))
                        {
                            idEditing.finished_missionsToDeactivateOn.Remove(st);
                            EditorUtility.SetDirty(idEditing);
                            return;
                        }
                    }
                }

                GUILayout.EndScrollView();

            }
            displayGlobalNPCData();

            //name
            //aesthetics (clothes, skin color)
            //Items
            //when to enable (What conditions have to be met for the NPC to start appearing in the world). (Mission done etc..)
            //when to disable (what conditions cause the NPC to stop beign displayed in the world) (not available at that time, mission done etc...)
            //hostile conditions (What conditions would cause the NPC to become hostile) (been attacked by player, at war with faction, wanted...)
            //peaceful conditions (what conditions would cause the NPC to become peaceful after turning hostile) (Mission complete, time passed)
            //stopping conditions (What conditions cause the NPC to stop existing in the world)(dead,mission done etc...)

        }

        Handles.EndGUI();
    }

    editingTask getTaskFromString(string st)
    {
        if (st == editingTask.name.ToString())
        {
            return editingTask.name;

        }
        else if (st == editingTask.aesthetics.ToString())
        {
            return editingTask.aesthetics;
        } else if (st == editingTask.whenToActivate.ToString())
        {
            return editingTask.whenToActivate;
        } else if (st == editingTask.whenToDeactivate.ToString())
        {
            return editingTask.whenToDeactivate;
        }
        else if (st == editingTask.whenToEnable.ToString())
        {
            return editingTask.whenToEnable;
        }
        else if (st == editingTask.whenToDisable.ToString())
        {
            return editingTask.whenToDisable;
        }
        else if (st == editingTask.hostileConditions.ToString())
        {
            return editingTask.hostileConditions;
        }
        else if (st == editingTask.peacefulConditions.ToString())
        {
            return editingTask.peacefulConditions;
        }else if (st == editingTask.position.ToString())
        {
            return editingTask.position;
        }else if(st==editingTask.items.ToString())
        {
            return editingTask.items;
        }else if(st==editingTask.editLists.ToString())
        {
            return editingTask.editLists;
        }
        return editingTask.name;
    }

    void refreshNPCS()
    {
        npcsInWorld = FindObjectsOfType<NPCID>();
    }

    uniqueNPCSetting getSettingFromString(string st)
    {
        if (st == "editing")
        {
            return uniqueNPCSetting.editing;
        }else if (st == "menu")
        {
            return uniqueNPCSetting.menu;
        }
        return uniqueNPCSetting.menu;
    } 

    bool button(string content)
    {
        if (GUILayout.Button(content, GUILayout.Width(125), GUILayout.Height(30)))
        {
            return true;
        }
        return false;
    }

    bool button(string content, int width, int height)
    {
        if (GUILayout.Button(content, GUILayout.Width(width), GUILayout.Height(height)))
        {
            return true;
        }
        return false;
    }

    bool inworldButton(Vector3 pos, Color color)
    {
        Handles.EndGUI();
        Handles.color = color;
        if (Handles.Button(pos, Quaternion.Euler(0, 0, 0), 1.0f, 1.0f, Handles.RectangleHandleCap))
        {
            Handles.BeginGUI();
            return true;
        }
        Handles.BeginGUI();
        return false;
    }

    string textInput(string content)
    {
        return GUILayout.TextField(content, GUILayout.Width(125), GUILayout.Height(30));
    }

    string textInput(string content, int width, int height)
    {
        return GUILayout.TextField(content, GUILayout.Width(width), GUILayout.Height(height));
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
    Vector3 getMouseClickInScene()
    {
        Handles.EndGUI();
        Debug.Log("Getting mouse click in scene");
        Event e = Event.current;
        // Debug.Log(e.ToString());
        switch (e.type)
        {
            case EventType.MouseUp:
                if (e.button == 1 && e.shift == true)
                {
                    Debug.Log("Got mouse click, returning " + pos.ToString());
                    Handles.BeginGUI();
                    return pos;
                }
                return Vector3.zero;
            case EventType.KeyUp:
                if (e.control == true || e.keyCode == KeyCode.LeftControl)
                {
                    Debug.Log("Got mouse click, returning " + pos.ToString());
                    Handles.BeginGUI();
                    return pos;
                }
                return Vector3.zero;
        }
        Handles.BeginGUI();
        return Vector3.zero;

    }
    ItemDatabase id;
    Vector2 itemScroll = Vector2.zero;
    string getItems()
    {
        string retVal = "None";
        if (id == null)
        {
            id = FindObjectOfType<ItemDatabase>();
        }
        itemScroll = GUILayout.BeginScrollView(itemScroll, GUILayout.Width(350), GUILayout.Height(350));
        foreach (GameObject g in id.items)
        {
            Item i = g.GetComponent<Item>();
            if (button(i.itemName, (i.getItemName().Length * 8) + 30, 35))
            {
                retVal = i.itemName;
            }
        }
        GUILayout.EndScrollView();
        return retVal;
    }

    string getClothes()
    {
        string retVal = "None";
        if (id == null)
        {
            id = FindObjectOfType<ItemDatabase>();
        }
        itemScroll = GUILayout.BeginScrollView(itemScroll, GUILayout.Width(350), GUILayout.Height(350));
        foreach (GameObject g in id.items)
        {
            ClothingItem i = g.GetComponent<ClothingItem>();
            if(i==null)
            {
                continue;
            }
            if (button(i.itemName, (i.getItemName().Length * 8) + 30, 35))
            {
                retVal = i.itemName;
            }
        }
        GUILayout.EndScrollView();
        return retVal;
    }

   

    bool editRotation = false;
    void drawPositionHandle(ref GameObject obj)
    {
        
        Handles.EndGUI();
        if (obj == null)
        {
            Handles.BeginGUI();

            return;
        }
        EditorGUI.BeginChangeCheck();
        Vector3 pos3 = Handles.PositionHandle(obj.transform.position, obj.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            Handles.Label(pos3, obj.name + " position");
            obj.transform.position = pos3;

            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
        Handles.BeginGUI();
    }

    void drawRotationHandle(ref GameObject obj)
    {
        Handles.EndGUI();
        if (obj == null)
        {
            Handles.BeginGUI();

            return;
        }
        EditorGUI.BeginChangeCheck();

        Quaternion rot = Handles.RotationHandle(obj.transform.rotation, obj.transform.position);

        if (EditorGUI.EndChangeCheck())
        {
            obj.transform.rotation = rot;
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
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
            uniquieNPCDataFromFile= readFile(path);
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
        if(button("Display Global"))
        {
            disp = !disp;
        }
        globalVec = GUILayout.BeginScrollView(globalVec);
        if(disp==true)
        {
            foreach(string st in uniquieNPCDataFromFile)
            {
                label(st, st.Length * 8, 30);
            }
        }
        GUILayout.EndScrollView();
    }
    Vector2 uniqueScroll = Vector2.zero;
    int getUniquieNPCID()
    {
        int retVal = -1;
        if (uniquieNPCDataFromFile == null || uniquieNPCDataFromFile.Count == 0)
        {
            getGlobalNPCData();
        }
        uniqueScroll = GUILayout.BeginScrollView(uniqueScroll, GUILayout.Width(200), GUILayout.Height(200));

        foreach(string st in uniquieNPCDataFromFile)
        {
            if (button(st, st.Length * 8, 30))
            {
                string[] dat = st.Split(';');
                int val = int.Parse(dat[0]);
                if(val!=idEditing.myId)
                {
                    retVal = val;
                }
            }
        }

        GUILayout.EndScrollView();
        return retVal;
    }

    void writeUniqueNPCData()
    {
        List<string> data = new List<string>();
       
        if(npcsInWorld==null||npcsInWorld.Length==0)
        {
            npcsInWorld = FindObjectsOfType<NPCID>();
        }

        if(uniquieNPCDataFromFile==null || uniquieNPCDataFromFile.Count==0)
        {
            getGlobalNPCData();
        }

        foreach(NPCID npc in npcsInWorld)
        {
            bool exists = false;
            int ind = 0;
            foreach(string st in uniquieNPCDataFromFile)
            {
                if (exists == false)
                {
                    string[] d = st.Split(';');

                    if (d[0] == npc.myId.ToString())
                    {
                        exists = true;
                        break;
                    }
                    else
                    {
                        ind++;
                    }
                }
            }
            if(exists==true)
            {
                uniquieNPCDataFromFile[ind] = npc.serializeNPCForEditor() + ";" + EditorSceneManager.GetActiveScene().name;
            }
            else
            {
                data.Add(npc.serializeNPCForEditor() + ";" + EditorSceneManager.GetActiveScene().name);
            }
        }
        foreach(string st in uniquieNPCDataFromFile)
        {
            data.Add(st);
        }
        writeListToFile(Path.Combine(Application.dataPath, "NPCIDData"), "UniquieNPCData.txt", data.ToArray());
    }

    public void writeListToFile(string directory, string fileName, string[] data)
    {
        string path = Path.Combine(directory, fileName);
        StreamWriter writer = new StreamWriter(path, false);
        foreach (string st in data)
        {
            writer.WriteLine(st);
        }
        writer.Close();
        //Debug.Log ("Wrote " + fileName);
    }

    List<string> readFile(string directory)
    {
        List<string> retVal = new List<string>();
      //  string path = Path.Combine(directory, fileName);
        StreamReader sr = new StreamReader(directory);
        string st = sr.ReadLine();
        while (st != null)
        {
            retVal.Add(st);
            st = sr.ReadLine();
        }
        sr.Close();
        return retVal;
    }

    void OnDeselect()
    {
        writeUniqueNPCData();
        Selection.selectionChanged -= OnDeselect;

    }

    MissionController m;
    Vector2 missionScroll = Vector2.zero;
    string getMission()
    {
        if(m==null)
        {
            m = FindObjectOfType<MissionController>();
        }

        string retval = "None";
        missionScroll = GUILayout.BeginScrollView(missionScroll,GUILayout.Width(200),GUILayout.Height(200));
        foreach(Mission miss in m.missions)
        {
           if(button(miss.missionName,miss.missionName.Length*8,30))
            {
                retval = miss.missionName;
            }
        }
        GUILayout.EndScrollView();
        return retval;
    }

}

enum uniqueNPCSetting
{
    menu,
    editing
    
}

enum editingTask
{
    name,
    position,
    aesthetics,
    items,
    whenToEnable,
    whenToDisable,
    whenToActivate,//enable = make appear in scene, activate is when the npc should first be spawned.
    whenToDeactivate,
    hostileConditions,
    peacefulConditions,
    editLists
}
