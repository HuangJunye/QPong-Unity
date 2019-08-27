﻿using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameHUD gameHUD;
    public int winScore = 7;
    public int showGameOverTime = 5;
    public int numberOfState = 8;

    GameObject theBall;
    public SuperposedBallControl ballControlScript;
    public GameObject[] ballArray;
    GameObject theCircuitGrid;
    CircuitGridControl circuitGridControlScript;
    GameObject theClassicalPaddle;
    ComputerControls classicalPaddleControlScript;
    Player player;
    ArcadeButtonInput arcadeButtonInput;


    // Start is called before the first frame update
    void Start()
    {
        ballArray = new GameObject[numberOfState];
        for (int i = 0; i < numberOfState; i++)
        {
            Vector3 ballPosition = Camera.main.ScreenToWorldPoint(new Vector3((i+0.5f)*Screen.width/numberOfState, Screen.height*0.5f,0));
            ballPosition.z = 0f;
            //ballArray[i] = (GameObject)Instantiate(ball, ballPosition, Quaternion.identity);
            ballArray[i] = GameObject.Find("ball"+i);
            //ballArray[i].name = "ball"+i;
            // hide the balls
            ballArray[i].GetComponent<SuperposedBallControl>().ballType = "HiddenBall";
            ballArray[i].GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 0.2f, 0.3f);
            ballArray[i].GetComponent<BoxCollider2D>().enabled = false;
            
            // show ball0 and enable collider
            if (i == 0) {
                ballArray[i].GetComponent<SuperposedBallControl>().ballType = "ClassicalBall";
                ballArray[i].GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 1f, 1f);
                ballArray[i].GetComponent<BoxCollider2D>().enabled = true;
            }

        }
        theBall = ballArray[0];
        ballControlScript = theBall.GetComponent<SuperposedBallControl>();
        // enable collider
        //theBall.GetComponent<SpriteRenderer>().color = new Color(10, 1, 1, 0.3f);
        //theBall.GetComponent<BoxCollider2D>().enabled = true;

        theCircuitGrid = GameObject.FindGameObjectWithTag("CircuitGrid");
        circuitGridControlScript = theCircuitGrid.GetComponent<CircuitGridControl>();

        theClassicalPaddle = GameObject.FindGameObjectWithTag("ClassicalPaddle");
        classicalPaddleControlScript = theClassicalPaddle.GetComponent<ComputerControls>();

        arcadeButtonInput = gameObject.GetComponent<ArcadeButtonInput>();
        player = GameController.Instance.player;

        RestartGame();
    }

    public void Score(string wallID){
        if (wallID == "TopWall"){
            player.AddPointsToPlayer();
        } else {
            player.AddPointsToComputer();
        }
        gameHUD.UpdateScores();
        Debug.Log("Update Score");
    }

    void Update()
    {
        if (player == null) {
            player = GameController.Instance.player;
        }

        if (player.playerScore >= winScore){
            Debug.Log("Quantum computer wins");
            gameHUD.showPlayerWinMessage();
            StartCoroutine(GameOver());
        } else if (player.computerScore >= winScore){
            Debug.Log("Classical computer wins");
            gameHUD.showComputerWinMessage();
            StartCoroutine(GameOver());
        }
        // print("Update while game is playing");

        // Check for Arcade controls
        PollForButtonInput();
    }

    IEnumerator GameOver() {
        ballControlScript.ResetBall(-1f);
        yield return new WaitForSeconds(showGameOverTime);

        // Check high scores before to move to main menu
        player.timeScore = Time.timeSinceLevelLoad;

        if (player.CheckHighScore()) {
            GameController.Instance.ShowHighscore();
        } else {
            GameController.Instance.LoadMainMenu();
        }
    }

    public void RestartGame()
    {
        print("player " + player);
        player.ResetScores();
        gameHUD.UpdateScores();
        ballControlScript.RestartRound(-1f);
        classicalPaddleControlScript.ResetPaddle();
    }

    #region Board Input
    private void PollForButtonInput()
    {

        if (Input.GetButtonDown("Start"))
        {
                //TODO: this is where we can go to reset the game or maybe even close it out and go back to the app selection screen

        }
        if (Input.GetKeyDown(JoystickButtonMaps.left.ToString()) || Input.GetKeyDown(JoystickButtonMaps.a.ToString()))
        {
            //TODO: set up the move cursor to the left
            print("BACK");
            circuitGridControlScript.MoveCursor(JoystickButtonMaps.left);
        }
        if (Input.GetKeyDown(JoystickButtonMaps.right.ToString()) || Input.GetKeyDown(JoystickButtonMaps.d.ToString()))
        {
            //TODO: setup the move cursor to the right
            print("FORWARD");
            circuitGridControlScript.MoveCursor(JoystickButtonMaps.right);
        }

        ArcadeButtonGates gateButtonPressed = arcadeButtonInput.isButtonPressed();
        if (gateButtonPressed != ArcadeButtonGates.None)
        {
            // print("what is gate buton " + gateButtonPressed);
            PressedGate(gateButtonPressed);
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

    }


    public void PressedGate(ArcadeButtonGates gateName)
    {

        //NOTE: my next step is to implement this
        //arcadeButtonController.ButtonPressed(gateName);
        switch (gateName)
        {
            case ArcadeButtonGates.xi:
                print("XI");
                circuitGridControlScript.AddGate(gateName);
                break;
            case ArcadeButtonGates.hi:
                print("HI");
                circuitGridControlScript.AddGate(gateName);
                break;
            default:
                print("we no use these yet");
                break;
        }
    }
    #endregion
}
