using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Start() Variables - control things directly in Unity!
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;
    
    // Game States
    private enum State {idle, running, jumping, falling, hurt}
    private State state = State.idle;
    
    // Inspector Variables, enables control of jump speed, jump force with ease in Unity editor.
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jump_force = 25f;
    [SerializeField] private int gems = 0;
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private float hurtForce = 7f;
    [SerializeField] private AudioSource gem;
    [SerializeField] private AudioSource footstep;
    [SerializeField] private int health;
    [SerializeField] private Text healthAmount;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();   // getting the component, basically a global but not really...
        anim = GetComponent<Animator>();    // ""                                                       ""
        coll = GetComponent<Collider2D>();
        healthAmount.text = health.ToString();
        
    }

    private void Update()
    {
        if (state != State.hurt)
        {
            MovementHandle();
        }

        AnimationState();
        anim.SetInteger("state", (int)state);
    } // collapsed function - click the + button

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Collectable")
        {
            gem.Play();
            Destroy(collision.gameObject);
            gems += 1;
            gemText.text = gems.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if(state == State.falling)
            {
                enemy.JumpedOn();
                Jump();
            }
            else
            {
                state = State.hurt;
                HandleHealth();
                if (other.gameObject.transform.position.x > transform.position.x)
                {
                    // Enemy is to right, therefore player should be damaged and move left.
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    // Enemy is to left, therefore player should be damaged and moved right.
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }

            }
        }
    }

    private void HandleHealth() // Deals with Health, Updates UI and handles resetting levels.
    {
        health -= 1;
        healthAmount.text = health.ToString();
        if (health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void MovementHandle()
    {
        float Hdirection = Input.GetAxis("Horizontal"); // getting the float of the character position

        if (Hdirection < 0) //less than zero would indicate a negative direction
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);


        }

        else if (Hdirection > 0) // greater than zero would indicate a positive direction
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);

        }
        else // not running at all
        {

        }

        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();
        }
    }


    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jump_force);
        state = State.jumping;
    }
    private void AnimationState()
    {




        if (state == State.jumping)
        {
            if(rb.velocity.y < 0.3f)
            {
                state = State.falling;
            }
        }
        else if(state == State.falling)
        {
            if(coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if (Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 1f) // if the absolute value of the x of rb is bigger than 
        {                                      // 2f(a really really really small number) the character moves.
            state = State.running; // moving
        }
        else
        {
            state = State.idle;
        }

    }

    private void FootStep()
    {   
        if(coll.IsTouchingLayers(ground))
        {
            footstep.Play();
        }
        
    }
}