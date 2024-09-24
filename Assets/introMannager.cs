using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class introMannager : MonoBehaviour
{

        public GameObject intro1;
    public GameObject intro2;
    public GameObject intro3;
    public GameObject intro4;

    int contador = 0;
    public float[] intervals = { 6.0f, 8.0f, 10.0f, 6.0f,};
    // Start is called before the first frame update
    void Start()
    {
            StartCoroutine(IncrementarContador());
    }

    // Update is called once per frame
  IEnumerator IncrementarContador()
    {
        for (int i = 0; i < intervals.Length; i++)
        {
            yield return new WaitForSeconds(intervals[i]);
            contador++;
        }
 
        // Asegurarse de que la última acción (cambio de escena) ocurra
        if (contador == intervals.Length)
        {
            yield return new WaitForSeconds(7.0f); // Un pequeño retraso antes de cambiar de escena
            SceneManager.LoadScene("Configuracion");
        }
    }
 
    void Update()
    {
      if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("Configuracion");
        }
        if (contador == 1)
        {
            intro1.SetActive(false);
        }
        if (contador == 2)
        {
            intro2.SetActive(false);
        }
        if (contador == 3)
        {
            intro3.SetActive(false);
        }
  
    }
}
