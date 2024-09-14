using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    private Collider2D coll;
    private SpriteRenderer render;
    void Awake()
    {
        coll = GetComponent<Collider2D>();
        render = GetComponent<SpriteRenderer>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("nicw");
        StartCoroutine(change());
    }

    private IEnumerator change()
    {
        render.color = Color.red;
        yield return new WaitForSeconds(2f);
        render.color = Color.white;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
}
