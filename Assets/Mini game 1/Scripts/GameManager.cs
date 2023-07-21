using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
   public bool canThrow = true;
    [SerializeField] List<GameObject> throwableObjects;
    public bool hasWon = false;
    [SerializeField] GameObject winText;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        Instantiate(throwableObjects[Random.Range(0, throwableObjects.Count)], this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (!canThrow)
        {
            Instantiate(throwableObjects[Random.Range(0, throwableObjects.Count)], this.transform);
            canThrow = true;
            Debug.Log("Worked");
        }

        if(hasWon)
        {
            Win();
        }
    }

    private void Win()
    {

        Time.timeScale = 0;
        winText.SetActive(true);
    }
}
