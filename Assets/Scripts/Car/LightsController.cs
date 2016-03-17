﻿using UnityEngine;
using System.Collections;

public class LightsController : MonoBehaviour {

	public GameObject []LowLights;
	public GameObject []HighLights;
	public GameObject []BrakeLights;
	public GameObject []TurnLights;
	public GameObject PanelLight;

	//sonido
	public AudioSource CarSignal;

	//White Lights
	private Light LowLightFrontRight;
	private Light LowLightFrontLeft;
	private Light HighLightFrontRight;
	private Light HighLightFrontLeft;

	//Brake Lights
	private Light LowBrakeLightRight;
	private Light LowBrakeLightLeft;
	private Light HighBrakeLightRight;
	private Light HighBrakeLightLeft;

	//turn Lights
	private Light TurnLightFrontRight;
	private Light TurnLightFrontLeft;
	private Light TurnLightBackRight;
	private Light TurnLightBackLeft;

	//Panel Light
	private Light InnerPanelLight;

	//light private variables
	private int lightState;

	//turn private variables
	private bool turnRight;
	private bool turnLeft;
	private float timeSpend;

	// Use this for initialization
	void Start () {
		//Get the light component from the GameObjects
		//White Lights
		LowLightFrontLeft = LowLights[0].GetComponent<Light>();
		LowLightFrontRight = LowLights[1].GetComponent<Light>();
		HighLightFrontLeft = HighLights[0].GetComponent<Light>();
		HighLightFrontRight = HighLights[1].GetComponent<Light>();

		//Brake Lights
		LowBrakeLightLeft = BrakeLights [0].GetComponent<Light> ();
		LowBrakeLightRight = BrakeLights [1].GetComponent<Light> ();
		HighBrakeLightLeft = BrakeLights [2].GetComponent<Light> ();
		HighBrakeLightRight = BrakeLights [3].GetComponent<Light> ();

		//Turn Lights
		TurnLightFrontLeft = TurnLights[0].GetComponent<Light>();
		TurnLightFrontRight = TurnLights[1].GetComponent<Light>();
		TurnLightBackLeft = TurnLights [2].GetComponent<Light> ();
		TurnLightBackRight = TurnLights [3].GetComponent<Light> ();

		//Panel Light
		InnerPanelLight = PanelLight.GetComponent<Light>();

		//initialize the lights off
		LowLightFrontLeft.enabled = false;
		LowLightFrontRight.enabled = false;
		HighLightFrontLeft.enabled = false;
		HighLightFrontRight.enabled = false;
		LowBrakeLightLeft.enabled = false;
		LowBrakeLightRight.enabled = false;
		HighBrakeLightLeft.enabled = false;
		HighBrakeLightRight.enabled = false;
		TurnLightFrontLeft.enabled = false;
		TurnLightFrontRight.enabled = false;
		TurnLightBackLeft.enabled = false;
		TurnLightBackRight.enabled = false;
		InnerPanelLight.enabled = false;

		//Initialize the state in 0
		lightState = 0;

		//Initialize the turn input
		turnRight = false;
		turnLeft = false;
		timeSpend = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeSpend += Time.deltaTime;
		
		//TurnLeft
		if (turnLeft) {
			if (!CarSignal.isPlaying)
				CarSignal.Play ();
			if (timeSpend > .5) {
				bool state = TurnLightFrontLeft.enabled;
				TurnLightFrontLeft.enabled = !state;
				state = TurnLightBackLeft.enabled;
				TurnLightBackLeft.enabled = !state;
				timeSpend = 0;
			}
		}
		//Turn Right
		else if (turnRight) {
			if (!CarSignal.isPlaying)
				CarSignal.Play ();
			if (timeSpend > .5) {
				bool state = TurnLightFrontRight.enabled;
				TurnLightFrontRight.enabled = !state;
				state = TurnLightBackRight.enabled;
				TurnLightBackRight.enabled = !state;
				timeSpend = 0;
			}
		} 
		else if (CarSignal.isPlaying && !(turnLeft || turnRight))
			CarSignal.Stop ();
	}

	public void WhiteLights(){
		if (lightState == 0) {
			LowLightFrontLeft.enabled = true;
			LowLightFrontRight.enabled = true;
			LowBrakeLightLeft.enabled = true;
			LowBrakeLightRight.enabled = true;
			InnerPanelLight.enabled = true;
			HighLightFrontLeft.enabled = false;
			HighLightFrontRight.enabled = false;
			lightState = 1;
		}
		else if (lightState == 1){
			LowLightFrontLeft.enabled = false;
			LowLightFrontRight.enabled = false;
			HighLightFrontLeft.enabled = true;
			HighLightFrontRight.enabled = true;
			lightState = 2;
		}
		else if (lightState == 2){
			LowLightFrontLeft.enabled = false;
			LowLightFrontRight.enabled = false;
			HighLightFrontLeft.enabled = false;
			HighLightFrontRight.enabled = false;
			LowBrakeLightLeft.enabled = false;
			LowBrakeLightRight.enabled = false;
			InnerPanelLight.enabled = false;
			lightState = 0;
		}
	}

	public void ActivateTurnRigth(){
		if(turnRight) {
			turnRight = false;
			TurnLightFrontRight.enabled = false;
			TurnLightBackRight.enabled = false;
		}
		else {
			turnRight = true;
			turnLeft = false;
			TurnLightFrontLeft.enabled = false;
			TurnLightBackLeft.enabled = false;
		}
	}

	public void ActivateTurnLeft(){
		if(turnLeft) {
			turnLeft = false;
			TurnLightFrontLeft.enabled = false;
			TurnLightBackLeft.enabled = false;
		}
		else {
			turnLeft = true;
			turnRight = false;
			TurnLightFrontRight.enabled = false;
			TurnLightBackRight.enabled = false;
		}
	}

	public void RedLights(float brake){
		if (brake > 0) {
			HighBrakeLightLeft.enabled = true;
			HighBrakeLightRight.enabled = true;
		} else {
			HighBrakeLightLeft.enabled = false;
			HighBrakeLightRight.enabled = false;
		}
	}
}
