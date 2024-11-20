using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    public void PositionLane()
    {
        // As lanes sao -1, 0, 1
        int randomLane = Random.Range(-1, 2);
        // Faz o objeto aparecer na lane aleatoria
        transform.position = new Vector3(randomLane, transform.position.y, transform.position.z);
    }
}
