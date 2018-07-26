using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class AnimationCreator : EditorWindow {


	static EditorWindow myWindow;
	static List<Sprite> animSprites = new List<Sprite> ();

	[MenuItem("Window/Unit02Games/Animation Creator")]
	public static void OnEnable()
	{
		myWindow = EditorWindow.GetWindow (typeof(AnimationCreator));
		myWindow.minSize = new Vector2 (300, 600);



	}

	void OnGUI(){
		GUILayout.Label ("Animation Creator");

		GUILayout.BeginHorizontal ();
		//spritesInAnim = EditorGUILayout.PropertyField (newAnim.FindProperty("mySprites"),false);
		//spritesInAnimation = (List<Sprite>)EditorGUILayout.ObjectField (spritesInAnimation, typeof(List<Sprite>), false);

		GUILayout.EndHorizontal ();
	}

}

public class seriAnimation : ScriptableObject{
	public Sprite[] mySprites;
	public string animName,animID;
	public float timePerFrame;
}
