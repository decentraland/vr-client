using System.Collections;
using UnityEngine;

public class Hide : MonoBehaviour
{
    private static readonly WaitForSeconds waiter = new WaitForSeconds(5);
    
    private void OnEnable()
    {
        StartCoroutine(HideOnDelay());
    }
    
    private IEnumerator HideOnDelay()
    {
        yield return waiter;
        gameObject.SetActive(false);
    }
}
