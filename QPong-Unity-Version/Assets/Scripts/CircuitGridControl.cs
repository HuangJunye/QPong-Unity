﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class CircuitGridControl : MonoBehaviour
{
    public GameObject emptyGate;
    public GameObject paddle;
    public GameObject[] paddleArray;
    public int circuitDepth = 3;
    public int qubitNumber = 3;
    public int columnHeight = 5;
    public int rowHeight = 5;
    public float xOffset = -51f;
    public float yOffset = -35f;

    // Variables for gate array
    public string[] gateArray; // array of string representing gates
    private GameObject[] gateObjectArray;  //1D array of gate
    public GameObject selectedGate;
    public int selectedColNum;
    public int selectedRowNum;
    public int selectedIndex;
    public GameObject cursor;
    public bool updateCircuit;

    // Variables for gate sprites
    public Sprite XGateSprite;
    public Sprite YGateSprite;
    public Sprite ZGateSprite;
    public Sprite HGateSprite;
    public Sprite emptyGateSprite;

    // Variables for inputs
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode addXGate = KeyCode.X;
    public KeyCode addYGate = KeyCode.Y;
    public KeyCode addZGate = KeyCode.Z;
    public KeyCode addHGate = KeyCode.H;
    public KeyCode deleteGate = KeyCode.Space;
    public KeyCode measure = KeyCode.Return;

    // Start is called before the first frame update
    void Start()
    {
        gateArray = new string[qubitNumber * circuitDepth];
        gateObjectArray = new GameObject[qubitNumber * circuitDepth];
        
        for (int i = 0; i < qubitNumber; i++)
        {
            for (int j = 0; j < circuitDepth; j++)
            {
                int index = i * circuitDepth + j;
                gateArray[index] = "I";
                gateObjectArray[index] = (GameObject)Instantiate(emptyGate, new Vector2(xOffset + j * columnHeight, yOffset + -i * rowHeight), 
                    Quaternion.identity);
                gateObjectArray[index].name = "gate["+i+"]["+j+"]";
            }
        }
        selectedGate = GameObject.Find("gate[0][0]");
        cursor = GameObject.Find("Cursor");

        int numberOfState = (int) Math.Pow(2, qubitNumber);
        paddleArray = new GameObject[numberOfState];
        for (int i = 0; i < numberOfState; i++)
        {
            Vector3 paddlePosition = Camera.main.ScreenToWorldPoint(new Vector3((i+0.5f)*Screen.width/numberOfState, Screen.height*0.18f,0));
            paddlePosition.z = 0f;
            paddleArray[i] = (GameObject)Instantiate(paddle, paddlePosition, Quaternion.identity);
            paddleArray[i].name = "paddle1["+i+"]";

        }
    }

    // Update is called once per frame
    void Update()
    {
        // Extract column number and row number from name
        var index = Regex.Matches(selectedGate.name, @"\d+").OfType<Match>().Select(m => int.Parse(m.Value)).ToArray();
        selectedRowNum = index[0];
        selectedColNum = index[1];

        // Handle moving cursor
        if (Input.GetKeyDown(moveDown)) {
            selectedRowNum ++;
        } else if (Input.GetKeyDown(moveUp)) {
            selectedRowNum --;
        } else if (Input.GetKeyDown(moveRight)) {
            selectedColNum ++;
        } else if (Input.GetKeyDown(moveLeft)) {
            selectedColNum --;
        }

        if (selectedColNum >= circuitDepth) {
            selectedColNum = circuitDepth - 1;
        } else if (selectedColNum < 0) {
            selectedColNum = 0;
        }

        if (selectedRowNum >= qubitNumber) {
            selectedRowNum = qubitNumber - 1;
        } else if (selectedRowNum < 0) {
            selectedRowNum = 0;
        }

        selectedIndex = selectedRowNum * circuitDepth + selectedColNum;
        selectedGate = GameObject.Find("gate["+selectedRowNum+"]["+selectedColNum+"]");
        cursor.transform.position = selectedGate.transform.position;

        // Handle adding and removing gates
        if (Input.GetKeyDown(addXGate)) {
            updateCircuit = true;
            if (gateArray[selectedIndex] == "X") {
                gateArray[selectedIndex] = "I";
            } else {
                gateArray[selectedIndex] = "X";
            }
        } else if (Input.GetKeyDown(addYGate)) {
            updateCircuit = true;
            if (gateArray[selectedIndex] == "Y") {
                gateArray[selectedIndex] = "I";
            } else {
                gateArray[selectedIndex] = "Y";
            }
        } else if (Input.GetKeyDown(addZGate)) {
            updateCircuit = true;
            if (gateArray[selectedIndex] == "Z") {
                gateArray[selectedIndex] = "I";
            } else {
                gateArray[selectedIndex] = "Z";
            }
        } else if (Input.GetKeyDown(addHGate)) {
            updateCircuit = true;
            if (gateArray[selectedIndex] == "H") {
                gateArray[selectedIndex] = "I";
            } else {
                gateArray[selectedIndex] = "H";
            }
        } else if (Input.GetKeyDown(deleteGate)) {
            updateCircuit = true;
            gateArray[selectedIndex] = "I";
        } else if (Input.GetKeyDown(measure)) {
            GameObject.Find("CircuitGrid").GetComponent<CircuitGridClient>().doMeasurementFlag = true;
        }


        // Update gateObjectArray based on changes in the gateArray, if any
        if (updateCircuit) {
            updateCircuit = false;
            GameObject.Find("BottomMeasurementWall").GetComponent<MeasureWalls>().updateCircuit = true;
            GameObject.Find("CircuitGrid").GetComponent<CircuitGridClient>().getStatevectorFlag = true;
            for (int i = 0; i < qubitNumber; i++)
            {
                for (int j = 0; j < circuitDepth; j++)
                {
                    int gate_index = i * circuitDepth + j;
                    if (gateArray[gate_index] == "I"){
                        gateObjectArray[gate_index].GetComponent<Gate>().SetGateIcon(emptyGateSprite);
                    } else if (gateArray[gate_index] == "X") {
                        gateObjectArray[gate_index].GetComponent<Gate>().SetGateIcon(XGateSprite);
                    } else if (gateArray[gate_index] == "Y") {
                        gateObjectArray[gate_index].GetComponent<Gate>().SetGateIcon(YGateSprite);
                    } else if (gateArray[gate_index] == "Z") {
                        gateObjectArray[gate_index].GetComponent<Gate>().SetGateIcon(ZGateSprite);
                    } else if (gateArray[gate_index] == "H") {
                        gateObjectArray[gate_index].GetComponent<Gate>().SetGateIcon(HGateSprite);
                    }
                }
            }
        }
    }
    void ResetCircuit()
    {
        for (int i = 0; i < qubitNumber; i++)
        {
            for (int j = 0; j < circuitDepth; j++)
            {
                int index = i * circuitDepth + j;
                gateArray[index] = "I";
            }
        }
        updateCircuit = true;
    }
}