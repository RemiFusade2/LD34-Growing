using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MoveCameraBehaviour : MonoBehaviour 
{
	public bool mainMenuState;
	public bool createDecorationState;
	public bool christmasTreeState;

	public List<GameObject> thingsToHide;

	public void ShowThings()
	{
		foreach(GameObject obj in thingsToHide)
		{
			obj.SetActive(true);
		}
	}
	public void HideThings()
	{
		foreach(GameObject obj in thingsToHide)
		{
			obj.SetActive(false);
		}
	}

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void SwitchToMainMenu()
	{
		if (!mainMenuState)
		{
			mainMenuState = true;
			christmasTreeState = false;
			createDecorationState = false;
			this.GetComponent<Animator>().SetTrigger("MainMenu");
		}
	}

	public void SwitchToCreateOrnament()
	{
		if (!createDecorationState)
		{
			createDecorationState = true;
			christmasTreeState = false;
			mainMenuState = false;
			this.GetComponent<Animator>().SetTrigger("CreateOrnament");
		}
	}
	
	public void SwitchToChristmasTree()
	{
		if (!christmasTreeState)
		{
			christmasTreeState = true;
			createDecorationState = false;
			mainMenuState = false;
			this.GetComponent<Animator>().SetTrigger("ChristmasTree");
		}
	}

	public GameObject inGameUI;
	public GameObject christmasTreeUI;
	public GameObject christmasTreeSubmitDecorationUI;
	public GameObject mainMenuUI;
	
	public void HideAllUI()
	{
		mainMenuUI.SetActive (false);
		christmasTreeUI.SetActive (false);
		inGameUI.SetActive (false);
	}
	public void ShowMainMenuUI()
	{
		mainMenuUI.SetActive (true);
	}
	public void ShowInGameUI()
	{
		inGameUI.SetActive (true);
	}
	public void ShowChristmasTreeUI()
	{
		christmasTreeUI.SetActive (true);
	}

	public GameObject currentDecoration;
	public MainBallBehaviour mainBall;

	public ChristmasTreeBehaviour christmasTree;

	public void SaveCurrentDecoration()
	{
		currentDecoration.SetActive (true);
		currentDecoration.GetComponent<OrnamentSphereBehaviour> ().SetScale (mainBall.transform.localScale.x / 5.0f);
		currentDecoration.GetComponent<OrnamentSphereBehaviour> ().SetTexture (mainBall.ballTexture);
		christmasTree.readyToPutOrnament = true;
		christmasTreeSubmitDecorationUI.SetActive (true);
	}


}
