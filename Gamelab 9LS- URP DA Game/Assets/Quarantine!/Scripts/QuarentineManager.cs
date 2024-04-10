using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;


public enum animalTypes
{
    dog,
    crow,
    parrot,
    Empty,
    closed 
}


public enum sickState
{
    healthy,
    sickening,
    sick
}


public class AnimalWeight
{
    public animalTypes AnimalType { get; set; }
    public int Weight { get; set; }
}


public class QuarentineManager : MonoBehaviour
{
    
    public static QuarentineManager Instance { get; private set; }

    [Header("colours")]
    public Material dog;
    public Material native,exotic,empty,closed;

    [Header("cage Set-up")]
    [SerializeField] private GameObject CagePrefab;
    [SerializeField] private int rowCount, rowAmount;
    [SerializeField] private float cageOffset;
    [SerializeField] private GameObject[] Cages;
    [SerializeField] private int dogWeight, crowWeight, parrotWeight, emptyWeight, closedWeight, healthyWeight;

    private List<AnimalWeight> animalWeights;

    [Header("Game Rules")]
    public float spreadSpeed;


    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this);}  
        else {Instance = this;}
        GetAnimalWeights();
        SpawnCages();
    }

    private void GetAnimalWeights()
    {
        animalWeights = new List<AnimalWeight>
        {
            new AnimalWeight {AnimalType = animalTypes.dog, Weight = dogWeight},
            new AnimalWeight {AnimalType = animalTypes.crow, Weight = crowWeight},
            new AnimalWeight {AnimalType = animalTypes.parrot, Weight = parrotWeight},

            new AnimalWeight {AnimalType = animalTypes.Empty, Weight = emptyWeight},
            new AnimalWeight {AnimalType = animalTypes.closed, Weight = closedWeight},
        };
    }

    private void SpawnCages()
    {
        Cages = new GameObject[rowCount*rowAmount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < rowAmount; j++)
            {
                Vector3 pos = new Vector3(j * cageOffset,0, i*cageOffset);


                GameObject newCage = Instantiate(CagePrefab, pos, Quaternion.identity, this.transform);
                Cages[i * rowAmount + j] = newCage;

                CageBehaviour cage = newCage.GetComponent<CageBehaviour>();

                cage.ChangeOccupation(GetWeightedRandomAnimal());
                cage.ChangeSickstate(GetWeightedRandomState());
            }
        }
    }

    private animalTypes GetWeightedRandomAnimal()
    {
        int totalWeight = 0; 
        foreach (AnimalWeight weight in animalWeights) 
        { 
            totalWeight += weight.Weight;
            Debug.Log(totalWeight);
        }

        int randomWeight = UnityEngine.Random.Range(0, totalWeight);

        for (int i = 0; i < animalWeights.Count; ++i)
        {
            randomWeight -= animalWeights[i].Weight;
            if (randomWeight < 0)
            {
                return animalWeights[i].AnimalType;
            }
        }
        return animalTypes.Empty;
    }

    private sickState GetWeightedRandomState()
    {
        int totalWeight = 1 + healthyWeight;
        int randomWeight = UnityEngine.Random.Range(0, totalWeight);
        randomWeight -= healthyWeight;
        if (randomWeight < 0)
        {
            return sickState.healthy;
        }
        return sickState.sick;

    }

}