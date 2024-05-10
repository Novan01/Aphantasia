using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR Foundation namespace
using UnityEngine.XR.ARSubsystems; // Required for trackables

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }
    // Start is called before the first frame update
 
    public void StartGame()
    {
        // Start spawning enemies
        EnemySpawner.instance.StartSpawning();
    }

    public void RestartGame() {
        EnemySpawner.instance.StopAllCoroutines();
        EnemySpawner.instance.ResetSpawner();
        PlayerController.instance.ResetCharacter();
        // UIScript.instance.InitializeGamePanel();
        UIScript.instance.StartGame();

    }

}