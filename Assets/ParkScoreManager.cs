using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ParkScoreManager : MonoBehaviour
{
    [System.Serializable]
    public struct CarParkScore
    {
        public float parkScoreTolarated;
        public string appreciation;
    }

    public GameObject parkContainerUI;
    public TextMeshProUGUI parkAppreciationTMP;

    // Here we are forced to do mutliple arrays instead of a list of array because of unity's serialization problem...
    public CarParkScore[] easyCarParkScores;
    public CarParkScore[] mediumCarParkScores;
    public CarParkScore[] hardCarParkScores;

    private CarParkScore[] carParkScores;
    private ParkingSpace[] parkingSpaces;

    private void Awake()
    {
        parkingSpaces = FindObjectsOfType<ParkingSpace>();
        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            parkingSpace.onCarPark += DefineParkAppreciation;
        }
    }

    private void DefineParkAppreciation(float parkScore)
    {
        // Determine park appreciation based on park score
        CarParkScore carParkScoreReceived = carParkScores[0];
        foreach (CarParkScore carParkScore in carParkScores)
        {
            if (parkScore > carParkScore.parkScoreTolarated) continue;

            carParkScoreReceived = carParkScore;
        }

        // Show park appreciation text
        parkContainerUI.SetActive(true);
        parkAppreciationTMP.text = carParkScoreReceived.appreciation;
    }

    public void PickParkDifficulty(int difficulty)
    {
        switch (difficulty)
        {
            case 0:
                carParkScores = easyCarParkScores;
                break;
            case 1:
                carParkScores = mediumCarParkScores;
                break;
            case 2:
                carParkScores = hardCarParkScores;
                break;
            default:
                carParkScores = easyCarParkScores;
                break;
        }
    }

    private void OnDestroy()
    {
        foreach (ParkingSpace parkingSpace in parkingSpaces)
        {
            parkingSpace.onCarPark -= DefineParkAppreciation;
        }
    }
}
