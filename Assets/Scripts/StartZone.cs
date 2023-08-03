using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartZone : MonoBehaviour
{
    public GameManager manager;
    float time = 6;

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            time -= Time.deltaTime;
            manager.updateNotification(true, (int)time);

            if (time < 0.5)
            {
                manager.StageStart();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            manager.updateNotification(false, 0);
            time = 6;
        }
    }
}
