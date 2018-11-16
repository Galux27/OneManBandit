using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Class used for generating IDs for linking doors & keys. 
/// </summary>
public class IDManager : MonoBehaviour {
	public static IDManager me;
	int curID = 0;
	int editorID=0;//only to be used in ID's that are set in the editor, not during gameplay
	int npcID=0;//used for identifying unique NPCs for missions and the like
    int eventID = 0;
	void Awake()
	{
		if (me == null) {
			me = this;
			DontDestroyOnLoad (this.gameObject);
			setID ();
			//setEditorID ();
		} else {
			Destroy (this);
		}
	}

	public int getID()
	{
		int retVal = curID;
		curID++;
		writeID ();
		return retVal;
	}

	public void writeID()
	{
		string folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		string profileDir = PlayerPrefs.GetString ("Profile");
		string IDDataPath = Path.Combine (folderPath, profileDir);
		IDDataPath = Path.Combine (IDDataPath, "Misc");
		if (Directory.Exists (IDDataPath) == false) {
			Directory.CreateDirectory (IDDataPath);
		}
		writeIDToFile (IDDataPath);
	}

	void writeIDToFile(string directory){
		string path = Path.Combine (directory, "ID.txt");
		StreamWriter writer = new StreamWriter (path,false);
		writer.WriteLine (curID.ToString());
		writer.Close ();
	}

	public void setID()
	{
		string folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		string profileDir = PlayerPrefs.GetString ("Profile");
		string IDDataPath = Path.Combine (folderPath, profileDir);
		string path = Path.Combine (IDDataPath,"ID.txt");
		if (Directory.Exists (IDDataPath) == false) {
			Directory.CreateDirectory (IDDataPath);
		}
		if (File.Exists (path) == false) {
			writeIDToFile (IDDataPath);
		}

		StreamReader sr = new StreamReader (path);
		string st = sr.ReadLine (); 
		curID = int.Parse (st);
		sr.Close ();
	}
	//Editor IDs

	void doesFileExist()
	{
		string folderPath = Path.Combine( Application.dataPath,"EditorIDData");

		string path = Path.Combine (folderPath,"EditorID.txt");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		if (File.Exists (path) == false) {
			writeEditorIDToFile (folderPath);
		}
	}

	public int getEditorID()
	{
		doesFileExist ();
		int retVal = curID;
		curID++;
		writeID ();
		return retVal;
	}

	public void writeEditorID()
	{
		string folderPath = Path.Combine( Application.dataPath,"EditorIDData");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		writeEditorIDToFile (folderPath);
	}

	void writeEditorIDToFile(string directory){
		string path = Path.Combine (directory, "EditorID.txt");
		StreamWriter writer = new StreamWriter (path,false);
		writer.WriteLine (curID.ToString());
		writer.Close ();
	}

	public void setEditorID()
	{
		string folderPath = Path.Combine( Application.dataPath,"EditorIDData");

		string path = Path.Combine (folderPath,"EditorID.txt");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		if (File.Exists (path) == false) {
			writeIDToFile (folderPath);
		}

		StreamReader sr = new StreamReader (path);
		string st = sr.ReadLine (); 
		curID = int.Parse (st);
		sr.Close ();
	}


	void doesNPCFileExist()
	{
		string folderPath = Path.Combine( Application.dataPath,"NPCIDData");

		string path = Path.Combine (folderPath,"NPCID.txt");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		if (File.Exists (path) == false) {
			writeEditorIDToFile (folderPath);
		}
	}


	public int getNPCID()
	{
		doesNPCFileExist ();
		int retVal = npcID;
		npcID++;
		writeNPCID ();
		return retVal;
	}

	public void writeNPCID()
	{
		string folderPath = Path.Combine( Application.dataPath,"NPCIDData");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		writeNPCIDToFile (folderPath);
	}

	void writeNPCIDToFile(string directory){
		string path = Path.Combine (directory, "NPCID.txt");
		StreamWriter writer = new StreamWriter (path,false);
		writer.WriteLine (npcID.ToString());
		writer.Close ();
	}

	public void setNPCID()
	{
		string folderPath = Path.Combine( Application.dataPath,"NPCIDData");

		string path = Path.Combine (folderPath,"NPCID.txt");
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}
		if (File.Exists (path) == false) {
			writeIDToFile (folderPath);
		}

		StreamReader sr = new StreamReader (path);
		string st = sr.ReadLine (); 
		npcID = int.Parse (st);
		sr.Close ();
	}


    //event ids

    void doesEventFileExist()
    {
        string folderPath = Path.Combine(Application.dataPath, "NPCIDData");

        string path = Path.Combine(folderPath, "NPCID.txt");
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        if (File.Exists(path) == false)
        {
            writeEditorIDToFile(folderPath);
        }
    }


    public int getEventID()
    {
        doesEventFileExist();
        int retVal = eventID;
        eventID++;
        writeEventID();
        return retVal;
    }

    public void writeEventID()
    {
        string folderPath = Path.Combine(Application.dataPath, "EventIDData");
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        writeEventIDToFile(folderPath);
    }

    void writeEventIDToFile(string directory)
    {
        string path = Path.Combine(directory, "EventID.txt");
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(eventID.ToString());
        writer.Close();
    }

    public void setEventID()
    {
        string folderPath = Path.Combine(Application.dataPath, "EventIDData");

        string path = Path.Combine(folderPath, "EventID.txt");
        if (Directory.Exists(folderPath) == false)
        {
            Directory.CreateDirectory(folderPath);
        }
        if (File.Exists(path) == false)
        {
            writeIDToFile(folderPath);
        }

        StreamReader sr = new StreamReader(path);
        string st = sr.ReadLine();
        eventID = int.Parse(st);
        sr.Close();
    }
}
