using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperThrow : MonoBehaviour
{
    public float throwStrength = 10.0f;

    private void Update()
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
        }
    }
}