using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionTracking : MonoBehaviour
{
    private Vector3 lastPosition;
    public float threshold = -10f;
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > threshold) {
            lastPosition = transform.position;
        }
        else {
            ResetToLastSafePosition();
        }
    }

    void ResetToLastSafePosition() {
        Vector3 resetPoint = new Vector3(lastPosition.x,lastPosition.y-threshold,lastPosition.z);
        transform.position = resetPoint;
        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null) {
            rb.velocity = Vector3.zero;
        }
    }
}
