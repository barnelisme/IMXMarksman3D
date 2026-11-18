using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;
using NUnit.Framework.Internal.Commands;

public class HuntConditionsManager : MonoBehaviour
{

    public TMP_InputField numAnimalsField;
    public TMP_InputField numTargetHitsField;
    private static int backupTAH = 0;

    private void Start()
    {
        if(backupTAH != 0)
        {
            TestConditionsManager.totalAllowedHitShots = backupTAH;
        }
        else if (TestConditionsManager.totalAllowedHitShots > AnimalSpawner.animalPopulation) // Fix setting if outside the bound
        {
            TestConditionsManager.totalAllowedHitShots = AnimalSpawner.animalPopulation;

            if(TestConditionsManager.totalAllowedHitShots >= 5)
            {
                TestConditionsManager.totalAllowedHitShots -= 2;
                backupTAH = TestConditionsManager.totalAllowedHitShots;
            }
        }

        numAnimalsField.text = AnimalSpawner.animalPopulation.ToString();
        numTargetHitsField.text = TestConditionsManager.totalAllowedHitShots.ToString();
    }

    public void IncrementAnimalPopulation()
    {
        if (AnimalSpawner.animalPopulation < AnimalSpawner.maxPopulation)
            AnimalSpawner.animalPopulation++;

        numAnimalsField.text = AnimalSpawner.animalPopulation.ToString();
    }
    public void DecrementAnimalPopulation()
    {
        if (AnimalSpawner.animalPopulation > AnimalSpawner.minPopulation)
            AnimalSpawner.animalPopulation--;

        numAnimalsField.text = AnimalSpawner.animalPopulation.ToString();
    }

    public void IncrementTargetHits()
    {
        if (TestConditionsManager.totalAllowedHitShots < AnimalSpawner.animalPopulation)
            TestConditionsManager.totalAllowedHitShots++;

        backupTAH = TestConditionsManager.totalAllowedHitShots;
        numTargetHitsField.text = TestConditionsManager.totalAllowedHitShots.ToString();
    }
    public void DecrementTargetHits()
    {
        if (TestConditionsManager.totalAllowedHitShots > AnimalSpawner.minPopulation)
            TestConditionsManager.totalAllowedHitShots--;

        backupTAH = TestConditionsManager.totalAllowedHitShots;
        numTargetHitsField.text = TestConditionsManager.totalAllowedHitShots.ToString();
    }
}       
