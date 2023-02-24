using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehavior : MonoBehaviour
{
    public float platformSpeed = 3f;
    public float onscreenDelay = 20;
    
    private float platformAngle;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, onscreenDelay);
        platformAngle = this.transform.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(Vector3.forward * platformSpeed * Time.deltaTime);
    }

}
