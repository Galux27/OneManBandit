using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeScript : MonoBehaviour {
	/// <summary>
	/// Script that controls the time ingame & the lighting conditions of the sun (color & position). 
	/// </summary>


	public static TimeScript me;
	public GameObject sunObj;
	public Light sunLight;
	//public Vector3[] sunPositions;
	//public Color[] sunColours;

	public Vector3 sunrise,morning,midday,afternoon,evening,sunset,midnight,earlyMorning;
	public Vector3 r_sunrise,r_morning,r_midday,r_afternoon,r_evening,r_sunset,r_midnight,r_earlyMorning;

	public Color c_sunrise, c_morning, c_midday, c_afternoon, c_evening, c_sunset, c_midnight, c_earlyMorning;
	public int hour=7,minute=0;
	public float secondTimer = 1.0f;
	public float timeMod = 1.0f;
	bool initialisedSun=false;


	public int month=0,day=1,year=2010,dayWeekNum=-1;
	public int[] daysInMonth = new int[] {31,28,31,30,31,30,31,31,30,31,30,31};
	public string[] months = new string[]{"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"};
	string[] days = new string[]{"Mon","Tue","Wed","Thu","Fri","Sat","Sun"};
	string dayName="";
	public bool initialisedDay = false;
	void Awake()
	{
		me = this;
	}

	// Use this for initialization
	void Start () {
		sunLight = sunObj.GetComponent<Light> ();
		if (initialisedSun == false) {
			initialiseSunPosition ();
		}
        sunLight.intensity = 0.2f;

		if (initialisedDay == false) {
			initialiseDateTime ();
			initialisedDay = true;
		}
	}

	void initialiseDateTime()
	{
		month = 3;
		day = 10;
		setDayOfWeek ();
	}

	public void increaseDay()
	{
		day++;
		if (day >= daysInMonth [month]) {
			month++;
			day = 0;
		}

		if (month > 11) {
			month = 0;
			year++;
		}

	}

	public void setDayOfWeek(){
		int dayOfWeek = 5;
		int curmonth = 0;
		int curday = 0;
		int curyear = 2010;

		if (dayOfWeek == -1) {
			while ((curday == day && curmonth == month && curyear == year) == false) {
				if (curday == day && curmonth == month && curyear == year) {
				} else {
					curday++;
					dayOfWeek++;
					if (curday > daysInMonth [curmonth]) {
						curmonth++;
						curday = 0;
					}

					if (curmonth > 11) {
						curmonth = 0;
						curyear++;
					}

					if (dayOfWeek > 7) {
						dayOfWeek = 1;
					}
				}
			}
		} else {
			dayWeekNum++;
			if (dayWeekNum > 7) {
				dayWeekNum = 1;
			}
		}
		dayName = days [dayOfWeek-1];
	}
	
	// Update is called once per frame
	void Update () {
		timerIncrease ();
		setSun ();

		//TODO:
		//have the light tilemap fade in gradually? (might not do because it would affect indor lights (maybe have outdoor lights on a seperate layer?)
		//have the mesh material darkeners be altered when its night (or get rid of entirely)
		//fix the issues with the slerp times not working correctly

		//LightSource.sun.setTilesToSun ();
		if (hour >= 5 && hour <= 22) {
			if (LightSource.sun.lightOn == false) {
				LightSource.sun.lightOn = true;
				//LightSource.sun.setTilesToLit ();
			}

		} else {
			if (LightSource.sun.lightOn == true) {
				LightSource.sun.lightOn = false;
				//LightSource.sun.setTilesToDark ();
			}
		}
	}
	public float timer = 0;
	bool resetTimer = false;

	public void initialiseSunPosition()
	{
		if (sunLight == null) {
			sunLight = sunObj.GetComponent<Light> ();
		}
		if (hour >= 5 && hour < 6) {
			//night to sunrise
			sunLight.color =c_sunrise;
			//Debug.Log ("Sun color is sunrise");
		//	sunObj.transform.position = sunrise;
		//	sunObj.transform.rotation = Quaternion.Euler (r_sunrise);

			//sunObj.transform.Translate(
		} else if (hour >= 6 && hour < 11) {
			sunLight.color =c_morning;
			//Debug.Log ("Sun color is morning");

			//sunObj.transform.position = morning;
			//sunObj.transform.rotation = Quaternion.Euler (r_morning);

			//morning
		} else if (hour >= 11 && hour < 13) {
			sunLight.color =c_midday;
			//Debug.Log ("Sun color is midday");

			//sunObj.transform.position = midday;
			//sunObj.transform.rotation = Quaternion.Euler (r_midday);

			//midday
		} else if (hour >= 13 && hour < 19) {
			sunLight.color =c_afternoon;
			//Debug.Log ("Sun color is afternoon");

			//sunObj.transform.position = afternoon;
			//sunObj.transform.rotation = Quaternion.Euler (r_afternoon);
			//afternoon
		} else if (hour >= 19 && hour < 22) {
			sunLight.color =c_evening;
			//Debug.Log ("Sun color is evening");

			//sunObj.transform.position = evening;
			//sunObj.transform.rotation = Quaternion.Euler (r_evening);

			//evening
		} else {
			sunLight.color = c_midnight;
			//Debug.Log ("Sun color is midnight");

			//sunObj.transform.position = midnight;
			//sunObj.transform.rotation = Quaternion.Euler (r_midnight);
			//night
		}
		initialisedSun = true;
		setSunPosOnStart ();
	}

	void setSun()
	{
       // RenderSettings.ambientSkyColor = getSunColor();
		LightSource.sun.setSun ();
		setSunPosition ();
		if (hour >= 0 && hour < 5) {
			//night to sunrise
			if (resetTimer == true) {
				timer = 0.0f;
				resetTimer = false;
			}
			if (Vector3.Distance (sunObj.transform.position, sunrise) > 1.0f) {
				timer += Time.deltaTime / (60) * timeMod;
				//timer+=Time.deltaTime;

				sunLight.color = getColorForSun (colorToVector (c_midnight), colorToVector (c_midnight), timer);
				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, sunrise,timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_sunrise.y, timer), 0); //Quaternion.Euler(  Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_sunrise,timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}
		}
		else if (hour >= 5 && hour < 6) {
			//night to sunrise
			if (resetTimer == true) {
				timer = 0.0f;
				resetTimer = false;
			}
			if (Vector3.Distance (sunObj.transform.position, sunrise) > 1.0f) {
				timer += Time.deltaTime/(60) * timeMod;
				//timer+=Time.deltaTime;

				sunLight.color = getColorForSun (colorToVector (c_midnight), colorToVector (c_sunrise), timer);
				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, sunrise,timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_sunrise.y, timer), 0); //Quaternion.Euler(  Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_sunrise,timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//sunObj.transform.Translate(
		} else if (hour >= 6 && hour < 11) {
			if (Vector3.Distance (sunObj.transform.position,  morning) > 1.0f) {
				timer  += Time.deltaTime/(60*6)*timeMod;
			//	timer+=Time.deltaTime;
				sunLight.color = getColorForSun (colorToVector (c_sunrise), colorToVector (c_morning), timer);

				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, morning, timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_morning.y, timer), 0); // Quaternion.Euler(  Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_morning, timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//morning
		} else if (hour >= 11 && hour < 13) {
			if (resetTimer == true) {
				timer = 0.0f;
				resetTimer = false;
			}
			if (Vector3.Distance (sunObj.transform.position,midday) > 1.0f) {
				timer  += Time.deltaTime/(60*2)*timeMod;
				//timer+=Time.deltaTime;
				sunLight.color = getColorForSun (colorToVector (c_morning), colorToVector (c_midday), timer);

				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, midday, timer);
				//sunObj.transform.rotation =  Quaternion.Euler (0, Mathf.Lerp (sunObj.transform.rotation.eulerAngles.y, r_midday.y, timer), 0); //Quaternion.Euler( Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_midday, timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//midday
		} else if (hour >= 13 && hour < 19) {
			if (resetTimer == true) {
				timer = 0.0f;
				resetTimer = false;
			}
			if (Vector3.Distance (sunObj.transform.position, afternoon) > 1.0f) {
				timer += Time.deltaTime/(60*6)*timeMod;
				//timer+=Time.deltaTime;
				sunLight.color = getColorForSun (colorToVector (c_midday), colorToVector (c_afternoon), timer);

				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, afternoon, timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_afternoon.y, timer), 0); // Quaternion.Euler( Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_afternoon, -1*timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//afternoon
		} else if (hour >= 19 && hour < 22) {
			if (resetTimer == true) {
				timer = 0.0f;
				resetTimer = false;
			}
			if (Vector3.Distance (sunObj.transform.position, evening) > 1.0f) {
				timer  += Time.deltaTime/(60*3)*timeMod;
				//timer+=Time.deltaTime;
				sunLight.color = getColorForSun (colorToVector (c_afternoon), colorToVector (c_evening), timer);

				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, evening, timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_evening.y, timer), 0); // Quaternion.Euler(  Vector3.Lerp (sunObj.transform.rotation.eulerAngles, r_evening, -1*timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//evening
		}else{

			if (resetTimer == true) {
				timer = 1.0f;
				resetTimer = false;
			}

			if (Vector3.Distance (sunObj.transform.position, earlyMorning) >1.0f) {
				timer  += Time.deltaTime/(60*2)*timeMod;
				//timer+=Time.deltaTime;
				sunLight.color = getColorForSun (colorToVector (c_evening), colorToVector (c_midnight), timer);

				//sunObj.transform.position = Vector3.Lerp (sunObj.transform.position, earlyMorning, timer);
				//sunObj.transform.rotation = Quaternion.Euler (0, Mathf.LerpAngle (sunObj.transform.rotation.eulerAngles.y, r_earlyMorning.y, timer), 0); //Quaternion.Euler( Vector3.Lerp (sunObj.transform.rotation.eulerAngles,r_earlyMorning,-1* timer));
			} else {
				//resetTimer = true;
				timer = 1.0f;

			}

			//night
		}
	}
	public float angle = 0.0f;

	void setSunPosOnStart()
	{
		float maxVal = 60 * 24;
		angle = (((float)(hour * 60) + minute) / maxVal) * 6.288f;
		////Debug.Log("Sun angle = " + (Mathf.Rad2Deg * angle).ToString());
		////Debug.Log ("Setting sun position" + angle.ToString ());
		Vector3 offset = new Vector3 (Mathf.Sin (angle*-1), 0,Mathf.Cos (angle*-1))*150;
		sunObj.transform.position = this.transform.position + offset;
		sunObj.transform.LookAt (this.transform.position);
	}

	void setSunPosition()
	{
		////Debug.Log("Current sun " + ((float)(hour * 60) + minute).ToString() + " Total " + (60*24).ToString());
		float maxVal = 60 * 24;
		angle = (((float)(hour * 60) + minute) / maxVal) * 6.288f;
		////Debug.Log("Sun angle = " + (Mathf.Rad2Deg * angle).ToString());
		////Debug.Log ("Setting sun position" + angle.ToString ());
		Vector3 offset = new Vector3 (Mathf.Sin (angle*-1), 0,Mathf.Cos (angle*-1))*150;
		sunObj.transform.position = Vector3.Slerp(sunObj.transform.position, this.transform.position + offset,Time.deltaTime);
		sunObj.transform.LookAt (this.transform.position);
	}


	void timerIncrease()
	{
		secondTimer -= Time.deltaTime * timeMod;
		if (secondTimer <= 0) {
			if (minute < 59) {
				minute++;
				if (minute == 58) {
					LightSource.UpdateLightMeshes ();
				}
			} else {
				if (hour < 23) {
					hour++;
				} else {
					hour = 0;
					increaseDay ();
				}
				minute = 0;
			}

			if (minute == 0) {
				if (hour == 0 || hour == 6 || hour == 11 || hour == 13 || hour == 19 || hour == 22) {
					timer = 0.0f;

					resetTimer = true;
				}
			}

			secondTimer = 1.0f;
		}
	}

	Vector3 colorToVector(Color c)
	{
		return new Vector3 (c.r, c.g, c.b);
	}

	Color getColorForSun(Vector3 s,Vector3 f,float t)
	{
		Vector3 col = Vector3.Slerp (s, f, t);
		////Debug.Log("Sun Color Slerp is " + col.ToString());
		return new Color (col.x, col.y, col.z, 1);
	}

	public Color getSunColor()
	{
		if (hour >= 22 || hour < 4) {
			
			return new Color (sunLight.color.r, sunLight.color.g, sunLight.color.b, 0.75f);

		} else if (hour >= 4 && hour <= 8 || hour >= 19 && hour < 21) {
			return new Color (sunLight.color.r, sunLight.color.g, sunLight.color.b, 0.25f);
	
		} else {
			return new Color (sunLight.color.r, sunLight.color.g, sunLight.color.b, 0.0f);

		}
	}

	public string getTime()
	{
		string retVal = "";
		retVal += days [dayWeekNum ] + " " + day.ToString() + " " + months[month] + " " + year.ToString() + " ";
		if (hour <= 9) {
			retVal += "0" + hour.ToString ();
		} else {
			retVal += hour.ToString ();
		}
		retVal += ":";
		if (minute <= 9) {
			retVal += "0" + minute.ToString ();
		} else {
			retVal += minute.ToString ();
		}

		return retVal;
	}

    public int howManyHoursHavePassed(int hour,int day,int month,int year)
    {
        System.DateTime curDate = new System.DateTime(this.year, this.month, this.day, this.hour, this.minute, 0);
        System.DateTime CompDate = new System.DateTime(year, month, day, hour, 0, 0);
        return Mathf.RoundToInt((float)(curDate - CompDate).TotalHours);
        return (workOutNumberOfDaysSince(0, hour, day, month, year) * 24)+this.hour;
    }

    int workOutNumberOfDaysSince(int min,int hour,int day,int month,int year)
    {

        System.DateTime curDate = new System.DateTime(this.year, this.month, this.day, this.hour, this.minute, 0);
        System.DateTime CompDate = new System.DateTime(year, month, day, hour, 0, 0);
        int numDays = Mathf.RoundToInt((float)(curDate - CompDate).TotalDays);
        //Debug.Log("Number of days since effect started is " + numDays);
        return numDays;
        /*DateTimeStore date = new DateTimeStore();
        date.day = day;
        date.month = month;
        date.year = year;
        date.min = 0;
        date.hour = hour;
        int numberOfDays = 0;
        //int month = date.month;
       // int day = date.day;


        if (date.month > this.month)
        {
            if (date.year < this.year)
            {
                numberOfDays += (date.month + 1) * 30;
            }
        }

        if (date.month < month)
        {
            while (month != this.month)
            {
                numberOfDays += this.daysInMonth[month];
                day = 0;
                month++;
            }
        }


        if (date.month == this.month)
        {
            if (day < this.day)
            {
                while (day < this.day)
                {
                    numberOfDays++;
                    day++;
                }
            }
        }
        //Debug.Log("Number of days since effect started is" + numberOfDays);
        return numberOfDays;*/
    }

    public void bloodBagSkip()
	{
		skipAhead (TimeScript.me.minute, TimeScript.me.hour, TimeScript.me.day + 1, TimeScript.me.month, TimeScript.me.year);
	}

	public void skipAhead(int min,int hour,int day, int month, int year)
	{
		minute = min;
		this.hour = hour;
		this.day = day;
		this.month = month;
		this.year = year;
		LoadScreen.loadScreen.loadGivenScene (SceneManager.GetActiveScene ().name);
	}


}
