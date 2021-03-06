﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallControl : MonoBehaviour
{
    public float speed = 30;
    public float startDirection = -1f;
    public float startPosition = 30;
    public float startPositionYOffset = 8;
    private Rigidbody2D rb2d;
    // Sound
    public AudioPlayer audioPlayer;
   

    void GoBall(){
        float rand = Random.Range(-2f, 2f);
        if (startDirection > 0) {
            rb2d.velocity = new Vector2(rand,-1).normalized * speed;
        } else {
            rb2d.velocity = new Vector2(rand,1).normalized * speed;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        var width = Camera.main.orthographicSize * 8.0f * Screen.width / Screen.height; // Width of the screen
        transform.localScale = new Vector2(width/40.0f, width/40.0f);
        audioPlayer = GetComponent<AudioPlayer>();
        RestartRound(startDirection);
    }

    public void ResetBall(float startSide){
        rb2d.velocity = Vector2.zero;
        if (startSide > 0){
            transform.position = new Vector2(0, startPosition + startPositionYOffset);
        }
        else if (startSide < 0){
            transform.position = new Vector2(0, -startPosition + startPositionYOffset);
        }
        
    }

    public void RestartRound(float startSide){
        startDirection = startSide;
        ResetBall(startDirection);
        Invoke("GoBall", 1);
    }

    void OnCollisionEnter2D(Collision2D col) {
        // Hit the classical paddle?
        if (col.gameObject.CompareTag("ClassicalPaddle")) {
            audioPlayer.PlaySound(Sound.bouncePaddle);
           
            // Calculate hit Factor
            float x = hitFactor(transform.position,
                            col.transform.position,
                            col.collider.bounds.size.x);

            // Calculate direction, make length=1 via .normalized
            Vector2 dir = new Vector2(x, -1).normalized;

            // Set Velocity with dir * speed
            rb2d.velocity = dir * rb2d.velocity.magnitude * 1.1f;
            Debug.Log("Hit Classical Paddle");
        }

        // Hit the quantum paddle?
        if (col.gameObject.CompareTag("QuantumPaddle")) {
        
            audioPlayer.PlaySound(Sound.bouncePaddle);
            // Calculate hit Factor
            float x = hitFactor(transform.position,
                            col.transform.position,
                            col.collider.bounds.size.x);

            // Calculate direction, make length=1 via .normalized
            Vector2 dir = new Vector2(x, 1).normalized;

            // Set Velocity with dir * speed
            rb2d.velocity = dir * rb2d.velocity.magnitude * 1.1f;
            Debug.Log("Hit Quantum Paddle");
        }

        if (col.gameObject.CompareTag("Wall")) {
            audioPlayer.PlaySound(Sound.bounceWall);
          
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        if (col.gameObject.CompareTag("SideWalls")) {
            audioPlayer.PlaySound(Sound.lostSound);
        }
    }

    float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth) {
        // ascii art:
        // ||  1 <- at the top of the racket
        // ||
        // ||  0 <- at the middle of the racket
        // ||
        // || -1 <- at the bottom of the racket
        return (ballPos.x - racketPos.x) / racketWidth;
    }
}
