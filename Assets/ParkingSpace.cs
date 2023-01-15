using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingSpace : MonoBehaviour
{
    public delegate void OnCarPark(float parkScore);
    public OnCarPark onCarPark;

    private CarController carParking;

    void Update()
    {
        if (carParking == null) return;
        Park();
    }

    private void Park()
    {
        if (!carParking.IsStopped) return;

        // Calculate park score
        float angleCardParked = Vector3.Angle(transform.forward, carParking.transform.forward);
        float distanceFromPlace = (carParking.transform.position - transform.position).magnitude;
        float parkScore = angleCardParked + distanceFromPlace;

        carParking.Park();
        onCarPark?.Invoke(parkScore);
    }

    private void OnTriggerEnter(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            carParking = car;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CarController car = other.GetComponent<CarController>();
        if (car != null)
        {
            carParking = null;
        }
    }
}
