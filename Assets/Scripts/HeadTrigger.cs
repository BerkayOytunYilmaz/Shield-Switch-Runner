using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTrigger : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="RailObstacle")
        {
            GameManager.Instance.RailObstacle = true;
            Debug.Log("Obstacle girdi");
        }
        if (other.gameObject.tag == "AirObstacle")
        {
            GameManager.Instance.AirObstacle = true;
            Debug.Log("AirObstacle girdi");
        }
    }
}
