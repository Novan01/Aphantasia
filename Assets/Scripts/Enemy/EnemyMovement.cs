using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    private EnemySpawner enemySpawner;
    private float speed = 0.17f;
    private Rigidbody rb;
    private float strafeOffset; // to make strafing more random
    private float strafeSpeed;




    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        GameObject enemySpawnerObject = GameObject.FindGameObjectWithTag("EnemySpawner"); // Replace "EnemySpawner" with the tag of the GameObject that has the EnemySpawner component
        if (enemySpawnerObject != null)
        {
            enemySpawner = enemySpawnerObject.GetComponent<EnemySpawner>();
        }
        else
        {
            Debug.LogError("EnemySpawner game object not found in the scene.");
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player game object not found in the scene.");
        }
        strafeOffset = Random.Range(0f, 2f * Mathf.PI);
        strafeSpeed = Random.Range(0.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            return; // Early exit if player reference is lost or not set
        }

        if(transform.position.y < -10) {
            enemySpawner.numEnemies--;
            Destroy(gameObject);
            UIScript.instance.updateEnemyMessage();
        }

        // Calculate the direction towards the player
        Vector3 direction = player.position - transform.position;
        direction.y = 0; // Keep the enemy on the same plane

        // Normalize the direction vector
        direction.Normalize();

        // Add a sine wave function to the direction for strafing
        direction += transform.right * Mathf.Sin((Time.time + strafeOffset) * strafeSpeed);

        // Calculate the new position
        Vector3 newPosition = transform.position + direction * speed * Time.deltaTime;

        // Check if the new position is within the plane boundaries
        float boundaryBuffer = 1.5f; // Adjust this value as needed
        if (newPosition.x < -5 + boundaryBuffer || newPosition.x > 5 - boundaryBuffer || newPosition.z < -5 + boundaryBuffer || newPosition.z > 5 - boundaryBuffer)
        {
            return; // If the new position is outside the plane boundaries, don't move the enemy
        }

        // Move the enemy towards the player
        transform.position = newPosition;

        // Make the enemy look at the player. Only the y-axis rotation is adjusted.
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 direction = transform.position - player.position;
            direction.y = 0; // Keep the enemy on the same plane
            direction.Normalize();

            rb.AddForce(direction * 100); // Adjust the force as needed
        }
    }
}
