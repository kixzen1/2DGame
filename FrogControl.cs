using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogControl : Enemy
{
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;

    [SerializeField] private float jumpLength = 10f;
    [SerializeField] private float jumpHeight = 15f;
    [SerializeField] private LayerMask ground;

    private bool facingLeft = true;
    private Collider2D coll;
    
    protected override void Start()
    {
        base.Start();
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // transition from jump to fall
        if(anim.GetBool("Jumping"))
        {
            if(rb.velocity.y < .1)
            {
                anim.SetBool("Falling", true);
                anim.SetBool("Jumping", false);
            }
        }
        // transition from fall to idle
        if (coll.IsTouchingLayers(ground) && anim.GetBool("Falling"))
        {
            anim.SetBool("Falling", false);
        }
    }

    private void Move()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {   // makes sure frog is facing rigtht direction
                if (transform.localScale.x != 1 && coll.IsTouchingLayers(ground))
                {
                    transform.localScale = new Vector3(1, 1);
                }

                // test to see if the frog is on the ground, if so jump.
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {   // makes sure frog is facing rigtht direction
                if (transform.localScale.x != -1 && coll.IsTouchingLayers(ground))
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                // test to see if the frog is on the ground, if so jump.
                if (coll.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLength, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            } 
        }
    }

   

}
