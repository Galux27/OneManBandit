using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TimeSkipUI : MonoBehaviour {
	public static TimeSkipUI me;

	void Awake()
	{
		me = this;
	}

	public GameObject skipParent;
	public int min, hour, day, month, year;
	public Text t_min, t_hour, t_day, t_month, t_year,currentDate;
	public void enableUI()
	{
		min = TimeScript.me.minute;
		hour = TimeScript.me.hour;
		day = TimeScript.me.day;
		month = TimeScript.me.month;
		year = TimeScript.me.year;
		skipParent.SetActive (true);
	}

	public void disable(){
		skipParent.SetActive (false);
	}

	public void increaseMin()
	{
		if (min < 59) {
			min++;
		} else if (min == 59) {
			increaseHour ();
			min = 0;
		}
	}


	public void decreaseMin()
	{
		if (hour == TimeScript.me.hour && day == TimeScript.me.day && month == TimeScript.me.month && year == TimeScript.me.year) {
			if (min > TimeScript.me.minute) {
				if (min > 0) {
					min--;
				} else {
					min = 59;
					decreaseHour ();
				}
			}

		} else {
			if (min > 0) {
				min--;
			} else {
				min = 59;
				decreaseHour ();
			}
		}
	}

	public void increaseHour()
	{
		if (hour < 23) {
			hour++;
		} else {
			hour = 0;
			increaseDay ();
		}
	}

	public void decreaseHour()
	{
		if (day == TimeScript.me.day && month == TimeScript.me.month && year == TimeScript.me.year) {
			if (hour > TimeScript.me.hour) {
				hour--;
			}
		} else {
			if (hour > 0) {
				hour--;
			} else {
				hour = 23;
				decreaseDay ();
			}
		}
	}

	public void increaseDay()
	{
		if (day < TimeScript.me.daysInMonth [TimeScript.me.month]) {
			day++;
		} else {
			day = 0;
			increaseMonth ();
		}
	}

	public void decreaseDay()
	{
		if (month == TimeScript.me.month && year == TimeScript.me.year) {
			if (day > TimeScript.me.day) {
				day--;
			}
		} else {
			if (day > 0) {
				day--;
			} else {
				decreaseMonth ();
			}
		}
	}

	void increaseMonth()
	{
		if (month < 11) {
			month++;
		} else {
			month = 0;
			increaseYear ();
		}
	}

	void decreaseMonth()
	{
		if (year == TimeScript.me.year) {
			if (month > TimeScript.me.month) {
				month--;
			}
		} else {
			if (month > 0) {

			} else {
				month = 11;
				decreaseYear ();
			}
		}
	}

	void increaseYear()
	{
		year++;
	}

	void decreaseYear()
	{
		if (year > TimeScript.me.year) {
			year--;
		}
	}

	void Update()
	{
		

		if (skipParent.activeInHierarchy == true) {
			validateTimeNumber ();
			setUIText ();
		}
	}

	void validateTimeNumber()
	{
		if (hour == TimeScript.me.hour && day == TimeScript.me.day && month == TimeScript.me.month && year == TimeScript.me.year) {
			if (min < TimeScript.me.minute) {
				min = TimeScript.me.minute;
			}
		} else if (day == TimeScript.me.day && month == TimeScript.me.month && year == TimeScript.me.year) {
			if (hour < TimeScript.me.hour) {
				hour = TimeScript.me.hour;
			}
		} else if (month == TimeScript.me.month && year == TimeScript.me.year) {
			if (day < TimeScript.me.day) {
				day = TimeScript.me.day;
			}
		} else if (year == TimeScript.me.year) {
			if (month < TimeScript.me.month) {
				month = TimeScript.me.month;
			}
		}
	}

	void setUIText()
	{
		t_min.text = min.ToString ();
		t_hour.text = hour.ToString ();
		t_day.text = (day+1).ToString ();
		t_month.text = TimeScript.me.months [month];
		t_year.text = year.ToString();
		currentDate.text = "Current Date: "+TimeScript.me.hour + ":"+TimeScript.me.minute+"  "+TimeScript.me.getTime ();
	}

	public void skipTime()
	{
		skipParent.SetActive (false);
		TimeScript.me.skipAhead (min, hour, day, month, year);
	}
}
