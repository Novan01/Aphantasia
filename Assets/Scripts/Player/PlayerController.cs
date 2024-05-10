using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    //MOVEMENT VARIABLES
    [SerializeField] private float moveSpeed = 1f;

    //JOYSTICK OBJECT
    [SerializeField] private Joystick joystick;

    //ANIMATOR and CONDITIONS
    [SerializeField] private Animator animator;
    private const string isWalking = "isWalking";
    public bool canJump = false;

    //PLAYER BOOL
    public bool isDead = false;
    [SerializeField] private bool walking = false;

    //ATTACK VARIABLES
    public float attackCooldown = .09f; // Cooldown duration in seconds
    private bool isAttackOnCooldown = false;
    private SwordController sword;

    bool placingEnemy;

    XRCharacterPlacer characterPlacer;
    GameObject xrOrigin;

    //TAP VS SWIPE DETECTION
    private Vector2 touchStartPos;
    private float touchStartTime;
    public float tapMaxMovement = 50f;
    public float tapMaxDuration = 0.2f;
    public bool allowSwipeDetection = true;
    public float swipeDetectionDelay = 0.5f; // Delay in seconds

    //RIGIDBODY
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private bool isGrounded = true;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundDistance = 0.2f;

    //PLAYER HEALTH
    [SerializeField] public float health = 10;
    [SerializeField] public Image healthBar;

    private UIScript uiScript;
    private void Awake()
    {
        if(instance == null) {
            instance = this;
        }
        GameObject hudObject = GameObject.Find("HUD");
        if (hudObject != null)
        {
            uiScript = hudObject.GetComponent<UIScript>();
        }
        else
        {
            Debug.LogError("HUD game object not found in the scene.");
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        //dynamically get the joystick object
        GameObject joystickObj = GameObject.FindGameObjectWithTag("Joystick");
        sword = GetComponentInChildren<SwordController>();
        joystick = joystickObj?.GetComponent<Joystick>();

        //StartCoroutine(PlayerMovement());

        xrOrigin = GameObject.Find("XR Origin");
        characterPlacer = xrOrigin.GetComponent<XRCharacterPlacer>();
        placingEnemy = characterPlacer.placingEnemy; //Point to XRCharacterPlacer's placingEnemy bool

        health = 10;

    }

    // Update is called once per frame
    void Update()
    {
        placingEnemy = characterPlacer.placingEnemy; //Copy XRCharacterPlacer's placingEnemy bool
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        
        if(!isDead) {
            HandleMovement();

            if(allowSwipeDetection) {
                HandleInput();
            }   
        }
        
    }

    void HandleMovement() {
        Vector3 moveDirection  = new Vector3(joystick.Horizontal, 0f, joystick.Vertical).normalized;
        if(moveDirection.magnitude > 0.1f) {
                if(!walking)
                {
                    walking = true;
                    animator.SetBool("isWalking", true);
                }

                transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

                Quaternion lookRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5.0f);
        }
        else {
                walking = false;
                animator.SetBool("isWalking", false);
        }
    }

    void HandleInput() {
        Attack();
    }

    void Attack() {
        // // Check for touch input
        if (!placingEnemy && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began) {
                touchStartPos = touch.position;
                touchStartTime = Time.time;
            }
            
            else if(touch.phase == TouchPhase.Ended) {
                
                float touchDuration = Time.time - touchStartTime;
                float touchDistance = (touch.position - touchStartPos).magnitude;

                // Check if the touch is on the right side of the screen
                if (touchDuration < tapMaxDuration && touchDistance < tapMaxMovement) {
                    if(touch.position.x > Screen.width / 2 && !isAttackOnCooldown) {
                        StartCoroutine(AttackCooldown());
                        StartCoroutine(AttackAnimation());
                        StartCoroutine(DelaySwipeDetection());
                    }
                    
                }
            }  
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Enemy") {
            health -= 1;
            Debug.Log(health);
            healthBar.fillAmount = health / 10;
            if(health == 0) {
                isDead = true;
                animator.SetBool("isDead", true);
                //UIScript.endPanel.SetActive(true);
                UIScript.instance.EndGame();
            }
        }
    }

    //ienumerator to controll the attack animation
    private IEnumerator AttackAnimation() {
        sword.EnableCollider();
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.9f); 
        sword.DisableCollider();
    }

    //set cooldown for the attack
    private IEnumerator AttackCooldown() {
        isAttackOnCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        isAttackOnCooldown = false;
    }

    private IEnumerator DelaySwipeDetection() {
        allowSwipeDetection = false;
        yield return new WaitForSeconds(swipeDetectionDelay);
        allowSwipeDetection = true;
    }

    public void ResetCharacter() {
        health = 10;
        healthBar.fillAmount = health / 10;
        UIScript.instance.score = 0;
        UIScript.instance.updateScore();
        isDead = false;
        animator.SetBool("isDead", false);
        animator.Rebind();
    }

}
