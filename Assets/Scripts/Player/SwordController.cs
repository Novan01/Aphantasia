using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SwordController : MonoBehaviour
{

    public static SwordController instance;
    private Collider swordCollider;
    private EnemySpawner enemySpawner;
    private GameObject enemySpawnerObject;

    private UIScript uiScript;
    private GameObject hudObject;

    [SerializeField] private AudioClip slimeDamageClip;
    [SerializeField] private AudioClip[] swordSwingClips;
    [SerializeField] public int score;


    private void Awake()
    {
        if(instance == null) {
            instance = null;
        }
        // Get the collider component attached to the sword
        swordCollider = GetComponent<Collider>();
        // Initially disable the collider so it doesn't collide unintentionally
        swordCollider.isTrigger = true;
        swordCollider.enabled = false;

        // Find the EnemySpawner game object and get the EnemySpawner script
        GameObject enemySpawnerObject = GameObject.Find("EnemySpawner");
        if (enemySpawnerObject != null)
        {
            enemySpawner = enemySpawnerObject.GetComponent<EnemySpawner>();
        }
        else
        {
            Debug.LogError("EnemySpawner game object not found.");
        }

        // Find the HUD game object and get the UIScript script
        hudObject = GameObject.Find("HUD");
        if (hudObject != null)
        {
            uiScript = hudObject.GetComponent<UIScript>();
        }
        else
        {
            Debug.LogError("HUD game object not found in the scene.");
        }
        score = 0;
    }

    // Method to enable the sword's collider
    public void EnableCollider()
    {
        swordCollider.enabled = true;
        SoundFXManager.instance.PlayRandomSoundFXClip(swordSwingClips, transform, 1f);
    }

    // Method to disable the sword's collider
    public void DisableCollider()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.tag == "Enemy") {
            SoundFXManager.instance.PlaySoundFXClip(slimeDamageClip, transform, 1f);
            other.gameObject.SetActive(false);
            UIScript.instance.score += 10;
            UIScript.instance.updateScore();
            enemySpawner.numEnemies--;
            uiScript.updateEnemyMessage();
            StartCoroutine(CleanUp(other));
        }
    }


    private IEnumerator CleanUp(Collider other) {
        yield return new WaitForSeconds(2.0f); 
        Destroy(other.gameObject);
    }
}
