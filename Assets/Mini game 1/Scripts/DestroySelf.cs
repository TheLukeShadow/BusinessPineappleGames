using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.canThrow == false)
        {
            StartCoroutine(Kill());
        }
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);

    }
}
