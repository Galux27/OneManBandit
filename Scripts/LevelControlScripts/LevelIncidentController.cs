using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelIncidentController : MonoBehaviour {
	public static LevelIncidentController me;
	public List<string> incidents;
	public List<Vector3> incidentPositions;
	public List<DateTimeStore> datesOfIncidents;
	public Incident[] incidentExamples;
	void Awake()
	{
		me = this;
		incidentExamples = this.GetComponents<Incident> ();
	}
	// Use this for initialization
	void Start () {
		

	}
	
	public void addIncident(string name, Vector3 position){
		if (incidents == null) {
			incidents = new List<string> ();

		}

		if (incidentPositions == null) {
			incidentPositions = new List<Vector3> ();

		}

		if (datesOfIncidents == null) {
			datesOfIncidents = new List<DateTimeStore> ();
		}


		incidents.Add (name);
		incidentPositions.Add (position);
		DateTimeStore d = new DateTimeStore ();
		d.min = TimeScript.me.minute;
		d.hour = TimeScript.me.hour;
		d.day = TimeScript.me.day;
		d.month = TimeScript.me.month;
		d.year = TimeScript.me.year;
		datesOfIncidents.Add (d);
	}

	public void reAddIncident(string name, Vector3 position,DateTimeStore date){
		if (incidents == null) {
			incidents = new List<string> ();

		}

		if (incidentPositions == null) {
			incidentPositions = new List<Vector3> ();

		}

		if (datesOfIncidents == null) {
			datesOfIncidents = new List<DateTimeStore> ();
		}

		Debug.LogError ("Incident " + name + " re added");
		incidents.Add (name);
		incidentPositions.Add (position);

		datesOfIncidents.Add (date);
	}

	public List<string> getIncidentsAsFile()
	{
		List<string> retVal = new List<string> ();
		for (int x = 0; x < incidents.Count; x++) {
			string toAdd = "";
			DateTimeStore d = datesOfIncidents [x];
			toAdd += incidents [x] + ";" + incidentPositions [x].ToString () + ";" + d.min + ";" + d.hour + ";" + d.day + ";" + d.month + ";" + d.year;
			retVal.Add (toAdd);
		}
		return retVal;
	}

	public void createIncidents(string name,Vector3 position,int min,int hour,int day,int month,int year)
	{
		foreach (Incident i in incidentExamples) {
			if (i.incidentName == name) {
				i.createIncidentReaction (min, hour, day, month, year, position);
			}
		}
	}
}

public class DateTimeStore
{
	public int min, hour, day, month, year;
}
