using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restarting : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            /*
            int aktuelleSzeneIndex = SceneManager.GetActiveScene().buildIndex;
            if (aktuelleSzeneIndex > 0)
            {
                int vorherigeSzeneIndex = aktuelleSzeneIndex - 1;
                SceneManager.LoadScene(vorherigeSzeneIndex);
            }
            */
        }
    }
}