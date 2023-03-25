using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : NetworkBehaviour
{
    private new Rigidbody2D rigidbody;

    [Header("Movement")]
    public float speed = 8.0f;
    public float speed_multiplier = 1.0f;

    [Header("Direction and Layer")]
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public Vector2 direction { get; private set; }
    public Vector2 next_direction { get; private set; }
    public Vector3 starting_position { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.starting_position = this.transform.position;
    }

    private void Start()
    {
        Reset();
    }

    public void Reset()
    {
        this.speed_multiplier = 1.0f;
        this.direction = this.initialDirection;
        this.next_direction = Vector2.zero;
        this.transform.position = this.starting_position;
        this.rigidbody.isKinematic = false; //make ghost when they turn home will not collise against the wall
        this.enabled = true;
    }

    private void Update()
    {
        
        if (this.next_direction != Vector2.zero)
        {
            SetDirection(this.next_direction);
        }
        
    }

    private void FixedUpdate()
    {
            Vector2 position = this.rigidbody.position;
            Vector2 translation = this.direction * this.speed * this.speed_multiplier * Time.fixedDeltaTime;
            this.rigidbody.MovePosition(position + translation);
        
            
    }

    public void SetDirection(Vector2 direction)
    {
        if (!Check_WallCollision(direction))
        {
            this.direction = direction;
            this.next_direction = Vector2.zero;
        }
        else
        {
            //this.direction = Vector2.zero;
            this.next_direction = direction;
            
        }
    }

    public bool Check_WallCollision(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, Vector2.one * 0.5f, 0.0f, direction , 1.0f , this.obstacleLayer);
        return hit.collider != null; //.collider = null if in that boxcast is not collise with obstacleLayer
    }
}
