using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

    [Header("Player Settings")]
    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    public Vector3 center = Vector3.zero;  
    public float maxSlope = .6f;
    public float ladderSpeed = 5;
    [Header("Player Results")]
    public float height;

    public bool canLadder = false;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private BoxCollider2D box;
    // Use this for initialization
    void Awake () 
    {
        spriteRenderer = GetComponent<SpriteRenderer> ();    
        animator = GetComponent<Animator> ();
        box = GetComponent<BoxCollider2D>();
        float radius = box.edgeRadius;
        size = box.size+new Vector2(2*radius,2*radius);
        height = box.size.y;
    }

    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis ("Horizontal");
        onLadder = canLadder ? onLadder : false;
        if (canLadder) {
            if(onLadder) {
                velocity.y = Input.GetAxis ("Vertical") * ladderSpeed;
                if(isGrounded && Input.GetAxis("Vertical") < 0) {
                    onLadder = false;
                }
            }
            else {
                if (Input.GetAxis ("Vertical") > 0) {
                    onLadder = true;
                    velocity.y = ladderSpeed;
                }
            }
        }
        if (Input.GetButtonDown ("Jump") && isGrounded && !onLadder) {
            velocity.y = jumpTakeOffSpeed;
        } else if (Input.GetButtonUp ("Jump")) 
        {
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.5f;
            }
        }

        //bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        
        //if (flipSprite) 
        //{
            //spriteRenderer.flipX = move.x >= 0;
            //facing = move.x >= 0;
        //}
        if (move.x > .01f) {
            spriteRenderer.flipX = false;
            //facing = true;
        }
        else if (move.x < -.01f) {
            spriteRenderer.flipX = true;
            //facing = false;
        }
        if(animator) {
            animator.SetBool ("grounded", isGrounded);
            animator.SetFloat ("velocityX", Mathf.Abs (velocity.x) / maxSpeed);
        }
        targetVelocity = move * maxSpeed;// * (1 - (1/maxSlope)*(slope));
        /*if (slope >= maxSlope) {
            targetVelocity = Vector2.zero;
        }
        else {
            targetVelocity = move * maxSpeed * (1-2*slope*slope);
        }
        Debug.Log(targetVelocity);*/
        //Debug.Log(Mathf.Round(10*(Mathf.Abs(10-  (  -((slope/maxSlope)*12-10)  +  Mathf.Sqrt(  ((slope/maxSlope)*12-10)*((slope/maxSlope)*12-10)  +  4  )  )/2  )))/10);
    }
}