﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public enum Endpoint { get_statevector, do_measurement }

public class CircuitGridClient : MonoBehaviour
{

    private const string API_URL = "http://127.0.0.1:8008/";
    private const string API_VERSION = "api/run/";
    private string circuitDimensionString;

    // Start is called before the first frame update
    public bool getStatevectorFlag;
    public bool doMeasurementFlag;
    public int qubitNumber;
    public int circuitDepth;
    public int stateNumber;
    private string gateString => string.Join(",", GameObject.Find("CircuitGrid").GetComponent<CircuitGridControl>().gateArray);

    public GameObject[] paddleArray;
    GameObject circuitGrid;
    CircuitGridControl circuitGridControlScript;

    void Start()
    {
        circuitGrid = GameObject.Find("CircuitGrid");
        circuitGridControlScript = circuitGrid.GetComponent<CircuitGridControl>();

        qubitNumber = circuitGridControlScript.qubitNumber;
        circuitDepth = circuitGridControlScript.circuitDepth;
        stateNumber = (int) Math.Pow(2, qubitNumber);
        circuitDimensionString = string.Join(",", qubitNumber, circuitDepth);
        paddleArray = circuitGridControlScript.paddleArray;
        GetStateVector(gateString);
    }

    //TODO: find out if there is ever an instance of both flags being true for one update, and do we need that to happen? can we optimize to just one?
    void Update()
    {
        if (getStatevectorFlag) {
            getStatevectorFlag = false;
            GetStateVector(gateString);
        }

        if (doMeasurementFlag) {
            doMeasurementFlag = false;
            DoMeasurement(gateString);
        }
    }

    private void GetStateVector(string gateString)
    {
        string urlString = API_URL + API_VERSION + Endpoint.get_statevector;
        StartCoroutine(PostRequest(urlString, circuitDimensionString, gateString, (results) => {

            // Deserialize stateVector from JSON

            RootObject obj = (RootObject)JsonUtility.FromJson(results, typeof(RootObject));
            Complex[] stateVector = new Complex[stateNumber];
            double[] stateProbability = new double[stateNumber];
            for (int i = 0; i < stateNumber; i++)
            {
                stateVector[i] = new Complex(obj.__ndarray__[i].__complex__[0], obj.__ndarray__[i].__complex__[1]);
                stateProbability[i] = Complex.Pow(stateVector[i], 2).Magnitude;
                paddleArray[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, (float)stateProbability[i]);
            }
           // Debug.Log("State Probability: [" + string.Join(", ", stateProbability) + "]");
        }));
    }

    private void DoMeasurement(string gateString)
    {
        Debug.Log("Send Gate Array: "+ gateString);
        string urlString = API_URL + API_VERSION + Endpoint.do_measurement;
        StartCoroutine(PostRequest(urlString, circuitDimensionString, gateString, (results) =>
        {
            for (int i = 0; i < 8; i++)
            {
                // make all states invisible and disable colliders
                paddleArray[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                paddleArray[i].GetComponent<BoxCollider2D>().enabled = false;
            }
            int stateInDecimal = Int32.Parse(results);
            // make the measured state visible and enable collider
            paddleArray[stateInDecimal].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            paddleArray[stateInDecimal].GetComponent<BoxCollider2D>().enabled = true;
        }));

    }

    public IEnumerator PostRequest(string url, string circuitDimensionString, string gateString, Action<string> completionHandler)
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("circuit_dimension", circuitDimensionString));
        formData.Add(new MultipartFormDataSection("gate_array", gateString));
        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, formData))
        {
            print(url);
            // Request and wait for return
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                // Some Sort of error to be handled
                // Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                completionHandler(webRequest.downloadHandler.text);

            }
        }
    }

    [Serializable]
    public class DataObject{
        public double[] __complex__;
    }

    [Serializable]
    public class RootObject{
        public DataObject[] __ndarray__;
        public string dtype;
        public int[] shape;
    }


}
