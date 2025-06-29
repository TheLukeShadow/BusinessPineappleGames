using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vaulting : MonoBehaviour
{
    int vaultLayer;
    [SerializeField] Camera cam;
    [SerializeField] float vaultHeight = 1.6f;
    float playerHeight = 2f;
    float playerRadius = 0.5f;
    [SerializeField] KeyCode JumpKey = KeyCode.Space;
    // Start is called before the first frame update
    void Start()
    {
        vaultLayer = LayerMask.NameToLayer("canVault");
        vaultLayer = ~vaultLayer;
    }

    // Update is called once per frame
    void Update()
    {
        Vault();
    }

    private void Vault()
    {
        if (Input.GetKeyDown(JumpKey))
        {
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out var firstHit, 1f, vaultLayer))
            {
                if(Physics.Raycast(firstHit.point + (cam.transform.forward*  playerRadius) + (Vector3.up * vaultHeight * playerHeight), Vector3.down, out var secondHit, playerHeight))
                {
                    StartCoroutine(LerpVault(secondHit.point, 0.5f));
                }
            }
        }
    }

    IEnumerator LerpVault(Vector3 targetPosition, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration) { 
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        } 
        transform.position = targetPosition;
    }
}
