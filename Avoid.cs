using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Avoid : MonoBehaviour
{
    public Rigidbody rb;
    //public LayerMask unwalkableMask;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        int numberofRays = 15;
        float angle = 150;
        for (int i = 0; i < numberofRays; ++i)
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((float)(2 * angle / (numberofRays - 1) * i), new Vector3(0, 1, 0));
            var direction = rotation * rotationMod * new Vector3(1, 0, 0);

            if (Physics.Raycast(this.transform.position, direction, out RaycastHit hit, 3))
            {
                if(hit.distance < 2f)
                {
                    rb.transform.position -= direction / hit.distance * Time.deltaTime * 3;
                    //rb.AddForce(-direction);
                }
                else
                {
                    rb.transform.position -= direction / (hit.distance + 1) * Time.deltaTime;
                }
                
                //rb.AddForce(-direction * 1 / hit.distance); 
                //rb.transform.forward -= direction / hit.distance * Time.deltaTime * 10;
            }
        }
    }
}
