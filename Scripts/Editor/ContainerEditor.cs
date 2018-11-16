using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
[CustomEditor(typeof(Container))]
public class ContainerEditor : Editor {
    ItemDatabase id;
    Container editing;
    Container[] allContainers;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
    Vector2 menuScroll = Vector2.zero;

    void OnSceneGUI()
    {
        if(id==null)
        {
            id = FindObjectOfType<ItemDatabase>();
        }

        if(allContainers==null)
        {
            allContainers = FindObjectsOfType<Container>();
        }

       // editing = (Container)target;


        Handles.BeginGUI();

        foreach(Container c in allContainers)
        {
            if(c==null)
            {
                continue;
            }

            if(c==editing)
            {
                inworldLabel(c.gameObject,"Editing");
                inworldButton(c.transform.position, Color.red);
            }
            else
            {
                if(inworldButton(c.transform.position,Color.cyan))
                {
                    if (editing != null)
                    {
                        EditorUtility.SetDirty(editing);
                    }
                    editing = c;
                }
            }
        }

        menuScroll = GUILayout.BeginScrollView(menuScroll, GUILayout.Width(400), GUILayout.Height(300));
        if(editing==null)
        {
            GUILayout.EndScrollView();
            return;
        }
        if (editing.itemsICouldAdd == null)
        {
            editing.itemsICouldAdd = new List<string>();
            editing.chanceOfItem = new List<float>();
        }
        string st = getItems();
        if(st!="None")
        {
            if(editing.itemsICouldAdd.Contains(st)==false)
            {
                editing.itemsICouldAdd.Add(st);
                editing.chanceOfItem.Add(50.0f);
            }
        }

        for(int x = 0;x<editing.itemsICouldAdd.Count;x++)
        {
            GUILayout.BeginHorizontal();
            label(editing.itemsICouldAdd[x] + ":" + editing.chanceOfItem[x].ToString());
            editing.chanceOfItem[x] = GUILayout.HorizontalSlider(editing.chanceOfItem[x], 0.01f, 100.0f, GUILayout.Width(200));
            if(button("X"))
            {
                editing.itemsICouldAdd.RemoveAt(x);
                editing.chanceOfItem.RemoveAt(x);
                return;
            }
            GUILayout.EndHorizontal();
            
        }
        EditorUtility.SetDirty(editing);
        GUILayout.EndScrollView();

        Handles.EndGUI();

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
}
