using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _grabDistance;
    [SerializeField] private LayerMask whatIsObject;
    [SerializeField] private float _throwForce;
    private float horizontal;

    private RaycastHit2D other;
    private bool _isGrabbed;
    [SerializeField] Vector3 offset;
    private bool dirRight = true;



    private Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Grab();
        Throw();
    }

    private void Move()
    {
        #region Move
        horizontal = Input.GetAxisRaw("Horizontal");
        Flip();
        _rb.velocity = new Vector2(horizontal * _speed, _rb.velocity.y);

        #endregion


        #region Jump

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("zipladi cian");
            _rb.AddForce(new Vector2(_rb.velocity.x, _jumpForce), ForceMode2D.Impulse);
        }
        #endregion
    }

    private void Grab()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isGrabbed)
            {
                Rigidbody2D objRb = other.collider.GetComponent<Rigidbody2D>();
                other.collider.isTrigger = false;
                other.transform.parent = null;
                objRb.isKinematic = false;
                objRb.constraints = RigidbodyConstraints2D.None;
                if (dirRight)
                    other.transform.localPosition = transform.position + offset;
                else
                    other.transform.localPosition = transform.position + offset + new Vector3(-2, 0, 0);
                _isGrabbed = false;
            }
            else if (!_isGrabbed && CanGrab())
            {
                Rigidbody2D objRb = other.collider.GetComponent<Rigidbody2D>();
                other.collider.isTrigger = true;
                other.transform.parent = transform;
                objRb.isKinematic = true;
                objRb.constraints = RigidbodyConstraints2D.FreezeRotation;
                objRb.velocity = Vector2.zero;
                other.transform.localPosition = new Vector3(.6f, .4f);
                _isGrabbed = true;
            }
        }
    }

    private void Throw()
    {
        //if (_isGrabbed && Input.GetKey(KeyCode.Mouse1))
        //{

        //    //Aim icin 5 adet nesne uret
        //    //Nesneler Mouse Pozisyonuna göre dizilsin
        //}
        if (_isGrabbed && Input.GetKeyDown(KeyCode.Mouse1))
        {
            Rigidbody2D objRb = other.collider.GetComponent<Rigidbody2D>();
            other.transform.parent = null;
            other.collider.isTrigger = false;
            objRb.isKinematic = false;
            objRb.constraints = RigidbodyConstraints2D.None;
            objRb.gravityScale = 0;
            _isGrabbed = false;
            if (dirRight)
                objRb.AddForce(Vector2.right * _throwForce, ForceMode2D.Impulse);
            else
                objRb.AddForce(Vector2.right * _throwForce * -1f, ForceMode2D.Impulse);

        }
    }
    public bool CanGrab()
    {
        RaycastHit2D hit;
        if (dirRight)
            hit = Physics2D.Raycast(transform.position, Vector2.right, _grabDistance, whatIsObject);
        else
            hit = Physics2D.Raycast(transform.position, Vector2.left, _grabDistance, whatIsObject);

        if (hit == true)
            other = hit;
        return hit;
    }

    void Flip()
    {
        if (horizontal != 0)
        {
            if (!dirRight && horizontal > 0)
            {
                transform.Rotate(0, 180, 0);
                dirRight = true;
            }

            else if (dirRight && horizontal < 0)
            {
                transform.Rotate(0, 180, 0);
                dirRight = false;
            }

        }
        else
            return;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (dirRight)
        {
            Gizmos.DrawRay(transform.position, Vector2.right);
        }
        else
        {
            Gizmos.DrawRay(transform.position, Vector2.left);
        }

    }

}
