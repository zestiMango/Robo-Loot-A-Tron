using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    [Header("Physics Settings")]
    public float minGroundNormalY = .65f;
    public float distanceFromGround = 1f;
    public float gravityModifier = 1f;
    public bool debugRay;
    
    [Header("Physics Results")] 
    public bool isGrounded;
    public float slope;
    public float heightFromGround;

    protected bool onLadder;
    protected Vector2 size = Vector2.zero;
    protected bool facing = true;
    protected Vector2 targetVelocity;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb2d;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D> (16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    void OnEnable()
    {
        rb2d = GetComponent<Rigidbody2D> ();
    }

    void Start () 
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask (Physics2D.GetLayerCollisionMask (gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void Update () 
    {
        targetVelocity = Vector2.zero;
        ComputeVelocity ();    
    }

    protected virtual void ComputeVelocity()
    {

    }

    void FixedUpdate()
    {
        if (!onLadder) {
            velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        }
        velocity.x = targetVelocity.x;

        isGrounded = false;

        Vector2 deltaPosition = velocity * Time.deltaTime;

        Vector2 moveAlongGround = new Vector2 (groundNormal.y, -groundNormal.x);

        Vector2 move = moveAlongGround * deltaPosition.x;

        Movement (move, false);

        move = Vector2.up * deltaPosition.y;

        Movement (move, true);

        Vector3 padding = Vector3.zero;
        /*if (facing) {
            padding.x = size.x/2;
        }
        else {
            padding.x = -size.x/2;
        }*/
        padding.x = size.x/2;
        RaycastHit2D hitL = Physics2D.Raycast(transform.position - padding, transform.TransformDirection(Vector2.down), Mathf.Infinity, LayerMask.GetMask("Ground"), -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D hitR = Physics2D.Raycast(transform.position + padding, transform.TransformDirection(Vector2.down), Mathf.Infinity, LayerMask.GetMask("Ground"), -Mathf.Infinity, Mathf.Infinity);
        if (hitR.distance == 0 && hitL.distance == 0){
                isGrounded = false;
                heightFromGround = -1;
                slope = 0;
        }
        else if(hitL.distance == 0 || hitR.distance == 0) {
            if (hitL.distance == 0 && hitR.distance != 0) {
                if (debugRay) {
                    Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.black);
                    Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.blue);
                }
                heightFromGround = hitR.distance;
                if (heightFromGround < distanceFromGround) {
                    isGrounded = true;
                    slope = Mathf.Abs(hitR.normal.x/hitR.normal.y);
                }                
            }
            else if ( hitR.distance == 0 && hitL.distance != 0) {
                if (debugRay) {
                Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.blue);
                Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.black);
                }
                heightFromGround = hitL.distance;
                if (heightFromGround < distanceFromGround) {
                    isGrounded = true;
                    slope = Mathf.Abs(hitL.normal.x/hitL.normal.y);
                }
            }

        }

        else if (hitL.distance < hitR.distance) {
            if (debugRay) {
                Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.blue);
                Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.black);
            }
            heightFromGround = hitL.distance;
            if (heightFromGround < distanceFromGround) {
                isGrounded = true;
                slope = Mathf.Abs(hitL.normal.x/hitL.normal.y);
            }
        }
        else {
            if (debugRay) {
                Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.black);
                Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.blue);
            }
            heightFromGround = hitR.distance;
            if (heightFromGround < distanceFromGround) {
                isGrounded = true;
                slope = Mathf.Abs(hitR.normal.x/hitR.normal.y);
            }
        }
        /*
        if(hitL.distance == 0 && hitR.distance == 0) {
            Debug.Log("both");
            isGrounded = false;
            distanceFromGround = Mathf.Infinity;
            slope = 0;
        }
        if(hitL.distance < hitR.distance || (hitR.distance == 0 && hitL.distance != 0)) {
                        Debug.Log("left");

            if (debugRay) {
                Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.blue);
                Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.black);
            }
            heightFromGround = hitL.distance;
            if (heightFromGround < distanceFromGround) {
                isGrounded = true;
                slope = Mathf.Abs(hitL.normal.x/hitL.normal.y);
            }
        }
        else if (((hitR.distance <= hitL.distance) && hitR.distance != 0) || (hitL.distance == 0 && hitR.distance != 0)) {
                        Debug.Log("right");

            if (debugRay) {
                Debug.DrawRay(transform.position - padding, transform.TransformDirection(Vector2.down) * hitL.distance, Color.black);
                Debug.DrawRay(transform.position + padding, transform.TransformDirection(Vector2.down) * hitR.distance, Color.blue);
            }
            heightFromGround = hitR.distance;
            if (heightFromGround < distanceFromGround) {
                isGrounded = true;
                slope = Mathf.Abs(hitR.normal.x/hitR.normal.y);
            }
        }
        */
        

    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > minMoveDistance) 
        {
            int count = rb2d.Cast (move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear ();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add (hitBuffer [i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) 
            {
                Vector2 currentNormal = hitBufferList [i].normal;
                //Debug.Log(currentNormal);
                if (currentNormal.y > minGroundNormalY) 
                {
                    //grounded = true;
                    if (yMovement) 
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot (velocity, currentNormal);
                if (projection < 0) 
                {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList [i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        rb2d.position = rb2d.position + move.normalized * distance;
    }

}