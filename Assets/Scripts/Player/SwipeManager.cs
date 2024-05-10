using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeManager : MonoBehaviour
{
    public float minSwipeLength = 100f;
    private Vector2 swipeStartPos;
    private Vector2 swipeEndPos;
    private List<Vector2> points = new List<Vector2>();
    private bool isSwiping = false;
    

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;
    [SerializeField] private float jumpForce = 5f;
    public float groundDistance = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] public PlayerController playerController;
    [SerializeField] private SwordController sword;
    [SerializeField] private AudioClip spinSwordClip;

    void Update()
    {
        if(!playerController.isDead) {
        // Check for touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.position.x > Screen.width / 2) {
                    switch (touch.phase)
                    {
                        case TouchPhase.Began:
                            swipeStartPos = touch.position;
                            points.Clear();
                            isSwiping = true;
                            points.Add(touch.position);
                            break;
                        case TouchPhase.Moved:
                            // Add point to points list if we are swiping
                            if (isSwiping)
                            {
                                points.Add(touch.position);
                            }
                            break;
                        case TouchPhase.Ended:
                            if (isSwiping)
                            {
                                isSwiping = false;
                                swipeEndPos = touch.position;
                                RecognizeSwipeOrGesture();
                            }
                            break;
                    }
                }  
            }
            UpdateGroundedStatus();
        }
    }

    void RecognizeSwipeOrGesture()
    {
        if(!playerController.allowSwipeDetection) return;

        if(playerController.isDead) return;

        Vector2 direction = swipeEndPos - swipeStartPos;
        if (direction.magnitude >= minSwipeLength)
        {
            direction.Normalize();
                if (direction.y > 0) {
                    Debug.Log("Swipe Up");
                    Jump();
                }
        }
        else
        {
            // Not a simple swipe, check for complex gestures
            if (IsZGesture(points)) {
                Debug.Log("Z Gesture");
            }
            else if (IsOGesture(points)) { 
                SpinAttack();
            }
        }
    }

    // Placeholder methods for gesture recognition
    bool IsZGesture(List<Vector2> points) { return false; }
    
    bool IsOGesture(List<Vector2> points) 
    {
        if (points.Count < 20) // Ensure there are enough points to form a circle.
            return false;

        // Calculate centroid
        Vector2 centroid = CalculateCentroid(points);

        // Calculate average distance from centroid
        float avgDistance = CalculateAverageDistance(points, centroid);

        // Check each point's distance from centroid
        bool isCircle = true;
        foreach (var point in points)
        {
            float distance = Vector2.Distance(point, centroid);
            if (Mathf.Abs(distance - avgDistance) > minSwipeLength / 2) // Allow some variance
            {
                isCircle = false;
                break;
            }
        }

        return isCircle;
    }

    Vector2 CalculateCentroid(List<Vector2> points)
    {
        Vector2 centroid = Vector2.zero;
        foreach (var point in points)
        {
            centroid += point;
        }
        centroid /= points.Count;
        return centroid;
    }

    float CalculateAverageDistance(List<Vector2> points, Vector2 centroid)
    {
        float totalDistance = 0f;
        foreach (var point in points)
        {
            totalDistance += Vector2.Distance(point, centroid);
        }
        return totalDistance / points.Count;
    }

     public void Jump() {
        if(isGrounded) {
            StartCoroutine(JumpAnimation());
        }
    }

    void SpinAttack() {
       StartCoroutine(SpinAttackAnimation());
       SoundFXManager.instance.PlaySoundFXClip(spinSwordClip, transform, 1f);
    }
    
    void UpdateGroundedStatus() {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
        }
    }

    private IEnumerator SpinAttackAnimation() {
        sword.EnableCollider();
        animator.SetTrigger("SpinAttack");

        yield return new WaitForSeconds(0.9f); 
        sword.DisableCollider();
    }
    private IEnumerator JumpAnimation() {
        animator.SetBool("isJumping", true);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isJumping", false);
    }

    public void PlayerSetup(Rigidbody rb, Animator anim, SwordController swordCon, Transform groundChk) {
        this.rb = rb;
        this.animator = anim;
        this.groundCheck = groundChk;
        this.sword = swordCon;
        // Update the isGrounded status based on the newly assigned groundCheck
        UpdateGroundedStatus();
    }
}
