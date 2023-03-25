using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Joymovement : NetworkBehaviour
{

    [Header("Movement")]
    public float Speed = 6f;

    private new Rigidbody2D rigidbody;
    public Vector2 Direction;
    


    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        //movementDirection.x = joymove.Horizontal;
        //movementDirection.y = joymove.Vertical;

        //if (movementDirection != Vector2.zero)
        //    transform.rotation = movementDirection;

    }


    void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + Direction * Speed * Time.fixedDeltaTime);
    }
}


