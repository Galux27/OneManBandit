using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class IDManager : MonoBehaviour {
	public static IDManager me;
	int curID = 0;

	void Awake()
	{
		if (me == null) {
			me = this;
			DontDestroyOnLoad (this.gameObject);
			setID ();
		} else {
			Destroy (this.gameObject);
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
}
