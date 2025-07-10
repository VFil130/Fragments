using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Animator anim;
    public float speed ;
    Rigidbody2D rb;
    SpriteRenderer SR;
    private float HorizontalMove;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SR = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        HorizontalMove = Input.GetAxis("Horizontal") * speed;

        Vector2 move = new Vector2(HorizontalMove, 0);
        if (move.x != 0)
        {
            anim.SetBool("Stop", false);
        }
        else
        {
            anim.SetBool("Stop", true);
        }
        rb.velocity = move;

        if (HorizontalMove > 0)
        {
            SR.flipX = false;
        }

        if (HorizontalMove < 0)
        {
            SR.flipX = true;
        }

    }
}
