using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerTutorial : MonoBehaviour {

	private GameObject Player;
	private PlayerBehaviour playerBehaviour;

	private GameObject Panel;
	private Text panelHeader;
	private Text panelText;
	private Text wallWalkerText;
	private GameObject bKeyImage;
	private GameObject spaceKeyImage;
	private GameObject arrowKeysImage;
	private GameObject shiftKeyImage;
	private GameObject mouseLeftImage;
	private GameObject oneKeyImage;
	private GameObject twoKeyImage;
	private GameObject threeKeyImage;
	private GameObject fourKeyImage;
	private GameObject fiveKeyImage;
	private GameObject tKeyImage;

	private GameObject trigger1;
	private GameObject trigger2;
	private GameObject trigger3;
	private GameObject trigger4;

	private int step;
	private bool clickedW = false;
	private bool clickedA = false;
	private bool clickedS = false;
	private bool clickedD = false;
	private bool clickedSpace = false;
	private bool enteredTrigger3 = false;

	//private GameObject panelHeader;



	// Use this for initialization
	void Start () {
		Player = GameObject.Find ("Player");
		playerBehaviour = Player.GetComponent<PlayerBehaviour> ();

		panelHeader = GameObject.Find ("PanelHeader").GetComponent<Text> ();
		panelText = GameObject.Find ("PanelText").GetComponent<Text> ();
		Panel = GameObject.Find ("Panel");
		wallWalkerText = GameObject.Find ("walker_a_active").GetComponent<Text> ();
		bKeyImage = GameObject.Find ("buttonB");
		arrowKeysImage = GameObject.Find ("arrowKeys");
		spaceKeyImage = GameObject.Find ("spaceKey");
		shiftKeyImage = GameObject.Find ("shiftKey");
		mouseLeftImage = GameObject.Find ("mouseLeftKey");
		oneKeyImage = GameObject.Find ("oneKey");
		twoKeyImage = GameObject.Find ("twoKey");
		threeKeyImage = GameObject.Find ("threeKey");
		fourKeyImage = GameObject.Find ("fourKey");
		fiveKeyImage = GameObject.Find ("fiveKey");
		tKeyImage = GameObject.Find ("tKey");

		trigger1 = GameObject.Find ("trigger_1");
		trigger2 = GameObject.Find ("trigger_2");
		trigger3 = GameObject.Find ("trigger_3");
		trigger4 = GameObject.Find ("trigger_4");

		Panel.SetActive (false);

		if (!bKeyImage != null) {
			bKeyImage.SetActive (false);
		}
		if (!arrowKeysImage != null) {
			arrowKeysImage.SetActive (false);
		}
		if (!spaceKeyImage != null) {
			spaceKeyImage.SetActive (false);
		}
		if (!shiftKeyImage != null) {
			shiftKeyImage.SetActive (false);
		}
		if (!mouseLeftImage != null) {
			mouseLeftImage.SetActive (false);
		}
		if (!oneKeyImage != null) {
			oneKeyImage.SetActive (false);
		}
		if (!twoKeyImage != null) {
			twoKeyImage.SetActive (false);
		}
		if (!threeKeyImage != null) {
			threeKeyImage.SetActive (false);
		}
		if (!fourKeyImage != null) {
			fourKeyImage.SetActive (false);
		}
		if (!fiveKeyImage != null) {
			fiveKeyImage.SetActive (false);
		}
		if (!tKeyImage != null) {
			tKeyImage.SetActive (false);
		}

		step = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if (Panel != null && panelText != null && panelHeader != null && Application.loadedLevelName == "tutorial room") {
			tutorial ();
		}
	}

	private void tutorial(){
		if (step == 1 && arrowKeysImage != null) {
			arrowKeyTutorial ();
		} else if (step == 2 && spaceKeyImage != null) {
			spaceKeyTutorial ();
		} else if (step == 3 && shiftKeyImage != null) {
			shiftKeyTutorial ();
		} else if (step == 4) {
			climbTutorial ();
		} else if (step == 5) {
			findWeaponTutorial ();
		} else if (step == 6) {
			if (oneKeyImage != null && twoKeyImage != null && threeKeyImage != null
			   && fourKeyImage != null && fiveKeyImage != null) {
				testWeaponTutorial ();
			}
		} else if (step == 7 && tKeyImage != null) {
			timeRecordingTutorial ();
		}


	}

	private void arrowKeyTutorial(){

		Panel.SetActive (true);
		arrowKeysImage.SetActive (true);
		/*
		panelHeader.text = "Me:";
		panelText.text = "'Where am I?'";
		System.Threading.Thread.Sleep(1000);

		panelText.text = "'Where am I?'\n'.";
		System.Threading.Thread.Sleep(1000);

		panelText.text = "'Where am I?'\n'..";
		System.Threading.Thread.Sleep(1000);*/
		panelText.text = "'Where am I?'\n'...'\n'Who am I...?";
		//System.Threading.Thread.Sleep(1000);



		if (hardInput.GetKeyDown("Forward")){
			clickedW = true;
		}
		if (hardInput.GetKeyDown("Backward")){
			clickedS = true;
		}
		if (hardInput.GetKeyDown("Left")){
			clickedA = true;
		}
		if (hardInput.GetKeyDown("Right")){
			clickedD = true;
		}
		if ((clickedW || clickedA || clickedS || clickedD) && playerBehaviour.isInTrigger()) {
			arrowKeysImage.SetActive (false);
			Panel.SetActive (false);
			step = 2;
		}
	}

	private void spaceKeyTutorial(){

		Panel.SetActive (true);
		panelHeader.text = "Me:";
		panelText.text = "'I need to jump over this.'";
		spaceKeyImage.SetActive (true);

		if (hardInput.GetKeyDown ("Jump")) {
			clickedSpace = true;
		}
		if (clickedSpace && !playerBehaviour.isInTrigger()) {
			spaceKeyImage.SetActive (false);
			Panel.SetActive (false);
			trigger1.SetActive (false);
			step = 3;
		}
	}

	private void shiftKeyTutorial(){

		Panel.SetActive (true);
		panelHeader.text = "Me:";
		panelText.text = "'Too slow..'\n'I have to run faster..'\n'What has happened to me'";
		arrowKeysImage.SetActive (true);
		shiftKeyImage.SetActive (true);

		if (playerBehaviour.isInTrigger ()) {
			arrowKeysImage.SetActive (false);
			shiftKeyImage.SetActive (false);
			Panel.SetActive (false);
			step = 4;
		}
	}

	private void climbTutorial(){
		if (trigger2.activeSelf && !playerBehaviour.isInTrigger ()) {
			trigger2.SetActive (false);
		}
		if (!trigger2.activeSelf) {
			Panel.SetActive (true);
			panelHeader.text = "Me:";
			panelText.text = "'I need to get higher'";

			if (playerBehaviour.isInTrigger ()) {
				panelText.text = "'The wall is too high..'\n'But I can climb up this kind of walls.'";
				enteredTrigger3 = true;
			}
			if (enteredTrigger3 && !playerBehaviour.isInTrigger ()) {
				trigger3.SetActive (false);
				Panel.SetActive (false);
				step = 5;
			}
		}
	}	

	private void findWeaponTutorial(){
		Panel.SetActive (true);
		panelHeader.text = "Me:";
		panelText.text = "'WHAT THE HELL IS THIS PLACE?'\n'There is something in the middle of the room..'\n" +
			"'Looks like a sword, I should go and check that.'";

		if (playerBehaviour.isInTrigger ()) {
			Panel.SetActive (false);
			step = 6;
		}
	}

	private void testWeaponTutorial(){
		Panel.SetActive (true);
		panelHeader.text = "Me:";
		panelText.text = "'They are weapons dudu.'\n'..I feel I used them before..'\n" +
			"\n'When...'\n\n'Lets me check if they are still working.'";
		mouseLeftImage.SetActive (true);
		oneKeyImage.SetActive (true);
		twoKeyImage.SetActive (true);
		threeKeyImage.SetActive (true);
		fourKeyImage.SetActive (true);
		fiveKeyImage.SetActive (true);
		
		if (!playerBehaviour.isInTrigger()) {
			mouseLeftImage.SetActive (false);
			oneKeyImage.SetActive (false);
			twoKeyImage.SetActive (false);
			threeKeyImage.SetActive (false);
			fourKeyImage.SetActive (false);
			fiveKeyImage.SetActive (false);
			Panel.SetActive (false);
			trigger4.SetActive (false);
			step = 7;
		}
	}

	private void timeRecordingTutorial(){
	}

	private void wallWalkerTutorial(){
	}
}
