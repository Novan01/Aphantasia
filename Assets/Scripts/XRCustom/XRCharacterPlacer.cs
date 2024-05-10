using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;


public class XRCharacterPlacer : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private SwipeManager swipeManager;
    private ARRaycastManager raycastManager;
    private bool isPlaced = false;
    public bool placingEnemy = false;

    private GameObject playerObject;
    private Rigidbody playerRb;
    private Animator playerAnimator;
    private Transform playerGroundCheck;
    private SwordController swordController;

    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        SwipeManager swipeManager = FindObjectOfType<SwipeManager>();
    }

        // Update is called once per frame
    void Update()
    {
        // if (!isPlaced) {
        //     //if someone touched the screen
        //     if(Input.touchCount > 0 && !IsPointerOverUIObject()) {
        //         if(!isPlaced)
        //         {
        //             Touch touch = Input.GetTouch(0); //get the object of the first touch
        //             Vector2 touchPosition = touch.position; //get the first touch position and pass it as a vector
        //             isPlaced = true; // Prevent further spawns
        //             ARraycasting(touchPosition);
        //         } 
        //     }
        // } 
        
    }

    public void PlacePlayer() {
        if(!isPlaced) {
            Vector3 spawnPoint = CalculateSpawnPoint();
            Quaternion spawnRotation = Quaternion.Euler(0,0,0);

            InstantiatePlayer(spawnPoint, spawnRotation);
        }
    }

    //function 
    void InstantiatePlayer(Vector3 pos, Quaternion rot, bool spawnPlayer = true) {
        if(spawnPlayer) {
            GameObject playerObject = Instantiate(player, pos, rot);
            Rigidbody playerRb = playerObject.GetComponent<Rigidbody>();
            Animator playerAnimator = playerObject.GetComponent<Animator>();
            Transform playerGroundCheck = playerObject.transform.Find("groundCheck"); // Make sure the player prefab has a child with this name
            swordController = FindSwordControllerInChildren(playerObject.transform);
            isPlaced = true;
            swipeManager.PlayerSetup(playerRb, playerAnimator, swordController, playerGroundCheck);
        } 
    }

    private SwordController FindSwordControllerInChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // First, check if this child is the one we're looking for
            if (child.CompareTag("Weapon"))
            {
                SwordController sword = child.GetComponent<SwordController>();
                if (sword != null)
                {
                    return sword;
                }
            }
            
            // If not, or if it doesn't have a SwordController, search its children
            SwordController swordInChildren = FindSwordControllerInChildren(child);
            if (swordInChildren != null)
            {
                return swordInChildren;
            }
        }
        
        // Return null if no matching child is found at any depth
        return null;
    }

    private Vector3 CalculateSpawnPoint() {
        Vector3 sumPositions = Vector3.zero;
        int count = 0;

        foreach (var plane in planeManager.trackables)
        {
            // Filter to only include horizontal planes if desired
            if (plane.alignment == PlaneAlignment.HorizontalUp)
            {
                sumPositions += plane.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            return sumPositions / count; // Return the average position
        }

        return Vector3.zero; // Default position if no planes are found
    }
}
