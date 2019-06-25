﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;

public class CircuitGridControl : MonoBehaviour
{
    public GameObject emptyGate;
    public int columnMax = 15;
    public int rowMax = 3;
    public int columnHeight = 5;
    public int rowHeight = 5;
    public float xOffset = -51f;
    public float yOffset = -35f;
    private GameObject[] gateArray;  //1D array of gate
    public GameObject selectedGate;
    public int selectedColNum;
    public int selectedRowNum;
    public GameObject cursor;

    public Sprite XGateSprite;
    public Sprite YGateSprite;
    public Sprite ZGateSprite;
    public Sprite HGateSprite;
    public Sprite emptyGateSprite;

    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode addXGate = KeyCode.X;
    public KeyCode addYGate = KeyCode.Y;
    public KeyCode addZGate = KeyCode.Z;
    public KeyCode addHGate = KeyCode.H;
    public KeyCode deleteGate = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        gateArray = new GameObject[columnMax * rowMax];
        for (int i = 0; i < columnMax; i++)
        {
            for (int j = 0; j < rowMax; j++)
            {
                int index = i + j * rowMax;
                gateArray[index] = (GameObject)Instantiate(emptyGate, new Vector2(xOffset + j * rowHeight, yOffset + i * columnHeight), 
                    Quaternion.Euler(new Vector3(0f, 0f, 90f)));  // initiate gates with 90 degrees rotation
                gateArray[index].name = "gate["+i+"]["+j+"]";
            }
        }
        selectedGate = GameObject.Find("gate[0][0]");
        cursor = GameObject.Find("Cursor");
    }

    // Update is called once per frame
    void Update()
    {
        // extract column number and row number from name
        var index = Regex.Matches(selectedGate.name, @"\d+").OfType<Match>().Select(m => int.Parse(m.Value)).ToArray();
        selectedColNum = index[0];
        selectedRowNum = index[1];

        if (Input.GetKeyDown(moveDown)) {
            selectedRowNum ++;
        } else if (Input.GetKeyDown(moveUp)) {
            selectedRowNum --;
        } else if (Input.GetKeyDown(moveRight)) {
            selectedColNum ++;
        } else if (Input.GetKeyDown(moveLeft)) {
            selectedColNum --;
        }

        if (selectedColNum >= columnMax) {
            selectedColNum = columnMax - 1;
        } else if (selectedColNum < 0) {
            selectedColNum = 0;
        }

        if (selectedRowNum >= rowMax) {
            selectedRowNum = rowMax - 1;
        } else if (selectedRowNum < 0) {
            selectedRowNum = 0;
        }

        if (Input.GetKeyDown(addXGate)) {
            if (selectedGate.GetComponent<SpriteRenderer>().sprite == XGateSprite) {
                selectedGate.GetComponent<SpriteRenderer>().sprite = emptyGateSprite;
            } else {
                selectedGate.GetComponent<SpriteRenderer>().sprite = XGateSprite;
            }
        } else if (Input.GetKeyDown(addYGate)) {
            if (selectedGate.GetComponent<SpriteRenderer>().sprite == YGateSprite) {
                selectedGate.GetComponent<SpriteRenderer>().sprite = emptyGateSprite;
            } else {
                selectedGate.GetComponent<SpriteRenderer>().sprite = YGateSprite;
            }
        } else if (Input.GetKeyDown(addZGate)) {
            if (selectedGate.GetComponent<SpriteRenderer>().sprite == ZGateSprite) {
                selectedGate.GetComponent<SpriteRenderer>().sprite = emptyGateSprite;
            } else {
                selectedGate.GetComponent<SpriteRenderer>().sprite = ZGateSprite;
            }
        } else if (Input.GetKeyDown(addHGate)) {
            if (selectedGate.GetComponent<SpriteRenderer>().sprite == HGateSprite) {
                selectedGate.GetComponent<SpriteRenderer>().sprite = emptyGateSprite;
            } else {
                selectedGate.GetComponent<SpriteRenderer>().sprite = HGateSprite;
            }
        } else if (Input.GetKeyDown(deleteGate)) {
            selectedGate.GetComponent<SpriteRenderer>().sprite = emptyGateSprite;
        }

        selectedGate = GameObject.Find("gate["+selectedColNum+"]["+selectedRowNum+"]");
        cursor.transform.position = selectedGate.transform.position;
    }
}
