using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public void Move(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
