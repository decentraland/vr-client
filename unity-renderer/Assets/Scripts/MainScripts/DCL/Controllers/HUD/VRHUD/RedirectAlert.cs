using UnityEngine;
using System.Collections;

public class RedirectAlert : MonoBehaviour
{
    private GameObject alertObject;
    public GameObject AlertObject
    {
        get
        {
            if (alertObject == null)
            {
                alertObject = Instantiate(Resources.Load("RedirectAlert") as GameObject);
            }
            return alertObject;
        }
    }
    
    public void ShowAlert()
    {
        //AlertObject.SetActive(true);
    }
}
