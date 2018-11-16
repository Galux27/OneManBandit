using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
[CustomEditor(typeof(ItemInWorld))]
public class ItemInWorldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
    Vector2 menuScroll = Vector2.zero;

    ItemInWorld[] items;
    ItemInWorld editing;
    Vector3 pos;
    int respawnRate = 0;

    void OnSceneGUI()
    {
        if(items==null)
        {
            refreshItemsInWorld();
        }

        pos = Event.current.mousePosition;
        pos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - pos.y;
        pos = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(pos);
        pos.z = -0.01f;

        if(editing==null)
        {
            CreateInputDetect();
        }

        Handles.BeginGUI();

        foreach(ItemInWorld i in items)
        {
            if(i==editing)
            {
                if (inworldButton(i.transform.position, Color.cyan))
                {
                    editing = null;
                }

                if (inworldButton(i.transform.position+new Vector3(1,0,0), Color.red))
                {
                    DestroyImmediate(editing.gameObject);
                    refreshItemsInWorld();
                    editing = null;
                    return;
                }
            }
            else
            {
                if (inworldButton(i.transform.position, Color.blue))
                {
                    editing = i;
                }
            }
        }

        if(editing==null)
        {
            label("No item editing");
        }
        else
        {

            string item = getItems();

            if (item != "None")
            {
                setItem(id.getItem(item).GetComponent<Item>());
            }

            GUILayout.BeginHorizontal();
            if(button("-"))
            {
                editing.GetComponent<SpriteRenderer>().sortingOrder = editing.GetComponent<SpriteRenderer>().sortingOrder - 1;
                EditorUtility.SetDirty(editing.gameObject);
                EditorUtility.SetDirty(editing);
            }
            label(editing.GetComponent<SpriteRenderer>().sortingOrder.ToString());

            if (button("+"))
            {
                editing.GetComponent<SpriteRenderer>().sortingOrder = editing.GetComponent<SpriteRenderer>().sortingOrder + 1;
                EditorUtility.SetDirty(editing.gameObject);
                EditorUtility.SetDirty(editing);

            }
            GUILayout.EndHorizontal();




            if (editRot==true)
            {
                if (button("Edit Position"))
                {
                    editRot = false;
                }
              //  rotationHandle(editing.gameObject);
            }
            else
            {
                if (button("Edit Rotation"))
                {
                    editRot = true ;
                }
               // rotationHandle(editing.gameObject);
            }

            if(button("Item Respawns = " + editing.respawn.ToString(),150,30))
            {
                editing.respawn = !editing.respawn;
            }

            if (editing.respawn == true)
            {
                GUILayout.BeginHorizontal();
                label("Respawn Rate (hours) = " + respawnRate.ToString(),200,30);
                int.TryParse(textInput(respawnRate.ToString()),out respawnRate);
                editing.respawnRate = respawnRate;
                EditorUtility.SetDirty(editing);
                GUILayout.EndHorizontal();
            }

        }
        Handles.EndGUI();

        if(editing==null)
        {

        }
        else
        {
            if (editRot == true)
            {

                rotationHandle(editing.gameObject);
            }
            else
            {

                positionHandle(editing.gameObject);
            }
        }

       
    }

    void CreateInputDetect()
    {
        Event e = Event.current;
        // Debug.Log(e.ToString());
        switch (e.type)
        {  
            case EventType.KeyUp:
                if (e.control == true || e.keyCode == KeyCode.LeftControl)
                {
                    GameObject g = new GameObject();
                    g.transform.position = pos;
                    editing = g.AddComponent<ItemInWorld>();
                    g.name = "ItemInWorld - ";
                    g.AddComponent<SpriteRenderer>();
                    refreshItemsInWorld();
                    
                }
                return;
        }
    }

    void setItem(Item i)
    {
        editing.myItem = i;
        editing.gameObject.GetComponent<SpriteRenderer>().sprite = i.inWorldUnequiped;
        editing.gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1,1, 0.75f);
        editing.gameObject.name = "ItemInWorld - " + i.itemName;
        EditorUtility.SetDirty(editing.gameObject);
        EditorUtility.SetDirty(editing);
    }

    void refreshItemsInWorld()
    {
        items = FindObjectsOfType<ItemInWorld>();

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
        if (Handles.Button(pos, Quaternion.Euler(0, 0, 0), 0.5f, 0.5f, Handles.RectangleHandleCap))
        {
            Handles.BeginGUI();
            return true;
        }
        Handles.BeginGUI();
        return false;
    }

    bool editRot = false;
    void positionHandle(GameObject toMove)
    {
        EditorGUI.BeginChangeCheck();

        Vector3 pos = Handles.PositionHandle(toMove.transform.position+new Vector3(0,2,0), toMove.transform.rotation);

        if (EditorGUI.EndChangeCheck())
        {
            //Undo.RecordObject (r, "Moved position of " + rs.roomName);
            toMove.transform.position = pos - new Vector3(0,2,0);
        }
    }

    void rotationHandle(GameObject toMove)
    {
        EditorGUI.BeginChangeCheck();

        Quaternion rot = Handles.RotationHandle(toMove.transform.rotation, toMove.transform.position + new Vector3(0, 2, 0));

        if(EditorGUI.EndChangeCheck())
        {
            toMove.transform.rotation = rot;
        }
    }
}