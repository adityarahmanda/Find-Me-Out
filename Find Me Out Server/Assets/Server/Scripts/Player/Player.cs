using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    private Roles role = Roles.None;
    public Roles Role
    {
        get { return role; }
        set { 
            if(value == Roles.None)
            {
                role = value;

                this.gameObject.tag = "Untagged";
            } 
            else
            { 
                role = value;
                this.gameObject.tag = (value == Roles.Seeker) ? "Seeker" : "Hider";
            }
        }
    }

    [Header("Lobby State")]
    public bool isReady;

    [Header("In-Game State")]
    public bool isCaught;
    private bool isWalk;

    [Header("Player Movement")]
    public float speed;
    public float rotationSpeed;

    private float inputX;
    private float inputZ;

    [Header("Player Physics")]
    public LayerMask playerMask;
    private CapsuleCollider _collider;
    public int MaterialId;

    private void Awake() 
    {
        _collider = GetComponent<CapsuleCollider>();    
    }

    private void Start()
    {
        isReady = false;
        isCaught = false;
        isWalk = false;
        MaterialId = 0;

        inputX = 0;
        inputZ = 0;
    }

    private void FixedUpdate()
    {        
        PlayerCollision();
        PlayerMovement();
    }

    public void Initialize(int _id)
    {
        id = _id;
        username = Server.clients[_id].user.DisplayName;
    }

    public void SetIsReady(bool _value)
    {
        isReady = _value;
        ServerSend.SendPlayerIsReady(id, _value);
    }

    public void SetInput(float _inputX, float _inputZ)
    {
        if (!isCaught)
        {
            inputX = _inputX;
            inputZ = _inputZ;
        }
    }

    public void PlayerCollision()
    {
        if(Role == Roles.Hider)
        {
            float distanceToPoints = _collider.height / 2 - _collider.radius;

            Vector3 point1 = transform.position + _collider.center + Vector3.up * distanceToPoints;
            Vector3 point2 = transform.position + _collider.center - Vector3.up * distanceToPoints;

            RaycastHit[] hiders = Physics.CapsuleCastAll(point1, point2, _collider.radius, transform.forward, 0, playerMask);
            
            for(int i = 0; i < hiders.Length; i++)
            {
                Transform hider = hiders[i].transform;

                if(hider != this.transform)
                {
                    if(PlayerManager.instance.IsPlayerCaught(hider))
                    {
                        PlayerManager.instance.ReleasePlayer(hider);
                    }
                }
            }
        }
    }

    public void PlayerMovement()
    {
        Vector3 movementDirection = new Vector3(inputX, 0, inputZ);
        movementDirection.Normalize();

        if (inputX == 0 && inputZ == 0)
        {
            if(isWalk)
            {
                ServerSend.SendPlayerAnimation(id, "isWalk", false);
                isWalk = false;
            }
        }
        else
        {
            transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

            if (movementDirection != Vector3.zero)
            {
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }

            if(!isWalk)
            {
                ServerSend.SendPlayerAnimation(id, "isWalk", true);
                isWalk = true;
            }
        }

        ServerSend.SendPlayerPosition(this);
        ServerSend.SendPlayerRotation(this);
    }

    public void SetPlayerIsCaught(bool _value)
    {
        inputX = 0;
        inputZ = 0;
        isCaught = _value;
        ServerSend.SendPlayerIsCaught(id, _value);
    }

    public void SetMaterialId(int _materialId)
    {
        MaterialId = _materialId;
    }
}