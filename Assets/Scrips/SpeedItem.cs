using UnityEngine;
using System;

public class SpeedItem : MonoBehaviour , IItem
{
    public static event Action<float> OnSpeedCollected;
    public float speedMultiplier = 1.5f;
    public void Collect()
    {
        //let playermovement know
        OnSpeedCollected.Invoke(speedMultiplier);
        Destroy(gameObject);

    }
}
