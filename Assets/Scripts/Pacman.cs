using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
//using UnityEditor.Experimental.GraphView;

public class Pacman : NetworkBehaviour
{

    public Movement movement { get; private set; }
    public Joymovement joymovement { get; private set; }

    [Header("Joy Setting")]
    public FixedJoystick joymove;

    private GameObject Canvas;
    private GameObject JoystickManager;
    private GameObject Mycamera;
    bool isTurnRight;

    

    public override void OnNetworkSpawn()
    {
        this.movement = GetComponent<Movement>();
        this.joymovement = GetComponent<Joymovement>();
        this.joymove = GameObject.FindWithTag("JoyStick").GetComponent<FixedJoystick>();
        Mycamera = GameObject.FindWithTag("MainCamera");

        isTurnRight = true;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        Mycamera.transform.position = new Vector3 (gameObject.transform.position.x,gameObject.transform.position.y , -10);

        if (this.movement.enabled && !this.joymovement.enabled)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.movement.SetDirection(Vector2.up);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.movement.SetDirection(Vector2.left);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.movement.SetDirection(Vector2.right);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.movement.SetDirection(Vector2.down);
            }
            else
            {
                return;
            }

        }

        else if (this.joymovement.enabled && !this.movement.enabled)
        {
            this.joymovement.Direction = joymove.Direction;
            RotatePlayer(joymove.Direction);

        }
    }

    void RotatePlayer(Vector2 direction)
    {
        if (direction.x < 0 && isTurnRight)
        {
            transform.Rotate(0, 180, 0);
            isTurnRight = false;
        }
        else if (direction.x > 0 && !isTurnRight)
        {
            transform.Rotate(0, -180, 0);
            isTurnRight = true;
        }
    }
}
