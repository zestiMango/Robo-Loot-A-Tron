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
    public float wallJumpPropel = 2f;
    [Header("Player Results")]
    public float height;
    public bool affected;

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
    public void addVelocity(Vector2 newVel, bool overwriteX, bool overwriteY) {
        Vector2 newVelocity = velocity;
        if(overwriteX) {
            newVelocity.x = newVel.x;
        }
        else {
            newVelocity.x += newVel.x;
        }
        if(overwriteY) {
            newVelocity.y = newVel.y;
        }
        else {
            newVelocity.y += newVel.y;
        }
        targetVelocity = newVelocity;
    }
    void addMoveX(float movex) {
        if (!affected) {
            if (velocity.x <= maxSpeed && velocity.x >= -maxSpeed) {
                addVelocity(new Vector2(maxSpeed * movex,0), true, false);
                return;
            }
        }
        /*
        if(movex > 0) {
            if (velocity.x <= maxSpeed - (2f+.1f) && velocity.x >= -maxSpeed) {
                if ( velocity.x < 0) {
                    addVelocity(new Vector2(0,0), true, false);
                }
                    addVelocity(new Vector2(2f,0), false, false);
            }
            else if (velocity.x > maxSpeed - (2f+.1f) && velocity.x <= maxSpeed) {
                addVelocity(new Vector2(maxSpeed * movex,0), true, false);
            }
            else if (velocity.x < -maxSpeed) {
                addVelocity(new Vector2(.5f,0), false, false);
            }
        }
        else if (movex < 0) {
            if (velocity.x <= maxSpeed && velocity.x >= -maxSpeed + (2f+.1f)) {
                if ( velocity.x > 0) {
                    addVelocity(new Vector2(0,0), true, false);
                }
                addVelocity(new Vector2(-2f,0), false, false);
            }
            else if (velocity.x < -maxSpeed + (2f+.1f) && velocity.x >= -maxSpeed) {
                addVelocity(new Vector2(maxSpeed * movex,0), true, false);
            }
            else if (velocity.x > maxSpeed) {
                addVelocity(new Vector2(-.5f,0), false, false);
            }
        }*/
    }
    protected override void ComputeVelocity()
    {
        Vector2 move = Vector2.zero;

        move.x = Input.GetAxis("Horizontal");
        /*onLadder = canLadder ? onLadder : false;
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
        }*/
        //targetVelocity = move * maxSpeed;
        bool wasWalled = (Time.time - wallTime) < timeSinceWall;


        if (Input.GetButtonDown ("Jump") && isGrounded /*&& !onLadder*/) {
            velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonDown ("Jump") && isWalled) {
            wallTime = Time.time;
            velocity.y = jumpTakeOffSpeed;
            Vector2 takeOff = Vector2.zero;
            takeOff.y = move.y;
            takeOff.x = jumpTakeOffSpeed * wallJumpPropel * (!wallSide ? 1 : -1);
            wallInitial = takeOff.x;
            addVelocity(takeOff, true, true);
            affected = true;
        }
        else if (wasWalled) {
            Vector2 takeOff = Vector2.zero;
            takeOff.y = move.y;
            takeOff.x = wallInitial * (timeSinceWall - (Time.time - wallTime)) / timeSinceWall;
            if (move.x != 0) {
                if (move.x > 0 && wallInitial > 0) {
                    if(takeOff.x < maxSpeed)
                        takeOff.x = maxSpeed;
                }
                else if (move.x < 0 && wallInitial < 0) {
                    if(takeOff.x > -maxSpeed)
                        takeOff.x = -maxSpeed;
                }
            }
            addVelocity(takeOff, true, true);
        }
        else if (!wasWalled) {
            affected = false;
        }
        else if (Input.GetButtonUp ("Jump")) {
            if (velocity.y > 0) {
                addVelocity(new Vector2(0,velocity.y * 0.5f), false, true);
            }
        }
        addMoveX(move.x);



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
    }
}