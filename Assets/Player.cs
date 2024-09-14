using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxHp = 0;
    public float hp = 0;

    public float speed = 0;
    public float maxSpeed = 0;

    public float maxStamina = 0;
    public float stamina = 0;
    public float staminaRegen = 0;

    public float acceleration = 2000;
    public float decceleration = 2000;

    public float velPower = 1;
    
    private Vector2 inputVector = Vector2.zero;
    private Collider2D coll;
    private  Rigidbody2D rb;
    
    public bool isRunning = false;
    private bool isMoving;

    private Queue<Vector2> queue = new Queue<Vector2>();
    public float tpCoolDown = 5;
    public float tpTime = 3;
    private float coolDown = 0;

    public GameObject shadowPrefab;
    private GameObject shadow;
    private bool haveShodow = false;
    
    public GameObject daggerPrefab;
    private List<GameObject> daggers = new List<GameObject>();
    public int daggerCount = 3;
    public int daggerSpeed = 4;
    private bool canDestroy = false;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    private void FixedUpdate()
    {
        
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        

        
        isRunning = Input.GetKey(KeyCode.LeftShift);
        isMoving = inputVector != Vector2.zero;
        
        
        if (coolDown < 0 && !haveShodow)
        {
            shadow = Instantiate(shadowPrefab, queue.Peek(), quaternion.identity);
            haveShodow = true;
        }

        if (Input.GetKey(KeyCode.Space) && coolDown < 0)
        {
            
            //transform.position = new Vector3(0, 0, 0);
            transform.position = queue.Peek();
           // Debug.Log(queue.Peek());
            coolDown = tpCoolDown;
            haveShodow = false;
            Destroy(shadow);
        }

        if (isMoving)
        {
            if (isRunning && stamina > 0)
            {
                Run(inputVector);
                stamina -= Time.fixedDeltaTime;
            }
            else
            {
                Move(inputVector);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            if (stamina != maxStamina)
            {
                if (stamina < maxStamina)
                {
                    stamina += Time.fixedDeltaTime * staminaRegen;
                }
                else if (stamina > maxStamina)
                {
                    stamina = maxStamina;
                }
            }
        }
        
        inputVector = Vector2.zero;
        coolDown -= Time.fixedDeltaTime;
        
        if (canDestroy)
        {
            List<int> del = new List<int>();
            for (int i = 0; i < daggers.Count; i++)
            {
                Vector2 dir;
                dir = new Vector2(daggers[i].transform.position.x-transform.position.x,daggers[i].transform.position.y-transform.position.y);
                if (!(dir.magnitude < 0.3)) continue;
                Debug.Log(dir);
                del.Add(i);
            }

            if (del.Count != 0)
            {
                Destroy(daggers[del[0]]); 
                daggers.Remove(daggers[del[0]]);
                
            }



            foreach (var dagger in daggers)
            {
                Vector2 dir;
                dir = new Vector2(transform.position.x - dagger.transform.position.x,transform.position.y- dagger.transform.position.y);
                dir = dir.normalized;
                dagger.transform.position = new Vector3(dagger.transform.position.x + dir.x * daggerSpeed * Time.fixedDeltaTime,
                    dagger.transform.position.y + dir.y * daggerSpeed * Time.fixedDeltaTime);
                
            }
        }
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private void Move(Vector2 dir)
    {
        
        dir = dir.normalized;
        Vector2 targetVecSpeed = dir * speed;
        Vector2 speedDif = targetVecSpeed - rb.velocity;
        Vector2 acceleRate;
        acceleRate.x = (Mathf.Abs(targetVecSpeed.x) > 0.01f) ? acceleration : decceleration;
        acceleRate.y = (Mathf.Abs(targetVecSpeed.y) > 0.01f) ? acceleration : decceleration;
        Vector2 movement;
        movement.x = speedDif.x * acceleRate.x;
        movement.y = speedDif.y * acceleRate.y;
        rb.AddForce(movement * Vector2.one);


     
    }

    private void Run(Vector2 dir)
    {
        dir = dir.normalized;
        Vector2 dirSpeed = dir * maxSpeed;
        rb.velocity = dirSpeed;
    }

    private void Start()
    {
        for (int i = 0; i < tpTime * 10; i++)
        {
            queue.Enqueue(item: transform.position);
        }

        StartCoroutine(Shadow());


    }

    private IEnumerator Shadow()
    {
        while (true)
        {
            queue.Dequeue();
            queue.Enqueue(transform.position);
            
            yield return new WaitForSeconds(0.1f);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
        
       
        
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            if (canDestroy && daggers.Count == 0)
            {
                canDestroy = false;
            }
            
            if (daggers.Count >= daggerCount)
            {
                canDestroy = true;
               
                // for (int i = 0; i < daggerCount; i++)
                // {
                //     //Debug.Log("delete");
                //     Destroy(daggers[0]);
                //     daggers.Remove(daggers[0]);
                //
                // }
            }
            else
            {
                //Debug.Log("create");
                daggers.Add( Instantiate(daggerPrefab, transform.position, quaternion.identity));
            }
        }
        
        
        
        if (haveShodow)
        {
            shadow.transform.position = queue.Peek();
        }
    }
}
