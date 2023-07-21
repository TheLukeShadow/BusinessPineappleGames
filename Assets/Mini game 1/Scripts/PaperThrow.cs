using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperThrow : MonoBehaviour
{
    [SerializeField] float throwStrength = 10.0f;
   
   


    private void Update()
    {

        
        if (GameManager.instance.canThrow)
        {       
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                GetComponent<Rigidbody>().AddForce(direction * throwStrength, ForceMode.Impulse);
            }
                GameManager.instance.canThrow = false;
                Destroy(GetComponent<PaperThrow>());
        }
        }
    }
}