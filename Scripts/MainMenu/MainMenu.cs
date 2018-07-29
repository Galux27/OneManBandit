using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class MainMenu : MonoBehaviour {
	/// <summary>
	/// Class that controls the GUI for the main menu, profile selection & creation. Also controls the creating of profiles and their directories. 
	/// </summary>


	public Dropdown myDropdown;
	public InputField textInput;
	public MainMenuSection mySection;
	public GameObject selectProfile,newProfile,menu;
	public string profileName = "";
	string[] sep;
	// Use this for initialization
	void Start () {
		sep = new string[1];
		sep[0] = "\\";
		findProfiles ();
	}
	
	// Update is called once per frame
	void Update () {
		setMenuSection ();
		decideProfile ();

		if (mySection == MainMenuSection.menu) {
			shouldWeDrawContinueButton ();
		}
	}

	void decideProfile()
	{
		if (mySection == MainMenuSection.selectProfile) {
			profileName = profilesFound [myDropdown.value];
		} else if (mySection == MainMenuSection.newProfile) {
			profileName = textInput.text;
			textInput.text = profileName;
		}
	}

	void setMenuSection()
	{
		if (mySection == MainMenuSection.selectProfile) {
			selectProfile.SetActive (true);
			newProfile.SetActive (false);
			menu.SetActive (false);
		} else if (mySection == MainMenuSection.newProfile) {
			selectProfile.SetActive (false);
			newProfile.SetActive (true);
			menu.SetActive (false);
		} else {
			selectProfile.SetActive (false);
			newProfile.SetActive (false);
			menu.SetActive (true);
		}
	}

	public void setSelectProfile()
	{
		mySection = MainMenuSection.selectProfile;
	}

	public void setNewProfile()
	{
		mySection = MainMenuSection.newProfile;
	}

	public void setMenu()
	{
		PlayerPrefs.SetString ("Profile", profileName);
		mySection = MainMenuSection.menu;
		findProfiles ();
		IDManager.me.setID ();
	}

	public void Quit()
	{
		Application.Quit ();
	}

	public void createProfile()
	{
		if(profileName == "" || profilesFound.Contains(profileName) || profileName.IndexOfAny(Path.GetInvalidPathChars())!=-1){
			return;//need some 
		}
		string 	folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		folderPath = Path.Combine (folderPath, profileName);
		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
			setMenu ();

		}
	}

	public List<string> profilesFound;
	void findProfiles()
	{
		string 	folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");

		if (Directory.Exists (folderPath) == false) {
			Directory.CreateDirectory (folderPath);
		}

		string[] dirs = Directory.GetDirectories (folderPath);
		List<string> dirsMod = new List<string> ();
		foreach (string st in dirs) {
			string[] arr = st.Split (sep, System.StringSplitOptions.RemoveEmptyEntries);
			dirsMod.Add (arr[arr.Length-1]);
		}
		profilesFound = dirsMod;
		myDropdown.ClearOptions ();
		myDropdown.AddOptions (profilesFound);
	}

	bool doesFileExist()
	{
		string folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		folderPath = Path.Combine (folderPath, PlayerPrefs.GetString ("Profile"));
		folderPath = Path.Combine (folderPath, "PlayerData");

		string filePath = Path.Combine (folderPath, "MiscData.txt");
		if (File.Exists (filePath) == false) {
			return false;
		}
		return true;
	}

	void eraseExistingData()
	{
		string folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		folderPath = Path.Combine (folderPath, PlayerPrefs.GetString ("Profile"));
		if (Directory.Exists (folderPath)) {
			Directory.Delete (folderPath, true);
		}
	}

	public void NewGame()
	{
		eraseExistingData ();
		LoadScreen.loadScreen.loadSceneWithoutStoringData ("HomeLevel");
	}

	public void ContinueGame(){
		string folderPath = Path.Combine( System.Environment.GetFolderPath (System.Environment.SpecialFolder.MyDocuments),"OneManBanditSaves");
		folderPath = Path.Combine (folderPath, PlayerPrefs.GetString ("Profile"));
		folderPath = Path.Combine (folderPath, "PlayerData");

		//string filePath = Path.Combine (folderPath, "MiscData.txt");
		List<string> miscData = readFile (folderPath, "MiscData.txt");
		LoadScreen.loadScreen.loadSceneWithoutStoringData (miscData [2]);
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
		return retVal;
	}


	public GameObject continueButton;
	void shouldWeDrawContinueButton()
	{
		continueButton.SetActive (doesFileExist ());
	}
}

public enum MainMenuSection{
	selectProfile,
	newProfile,
	menu
}
