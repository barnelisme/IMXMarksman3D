using TMPro;
using UnityEngine;

public class DuelingTreeTargetsManager : MonoBehaviour
{

    public GameObject[] targetHolders;
    public TextMeshProUGUI numTargetText;

    void Start()
    {
        SetCurrentHolders();
    }

    public void IncrementTargets()
    {
        if(countDownStart.start_training == false)
        {
            if (StaticVariableManager.numDuelingTreeTargets < 4) StaticVariableManager.numDuelingTreeTargets++;
            SetCurrentHolders();
        }
    }

    public void DecrementTargets()
    {
        if (countDownStart.start_training == false)
        {
            if (StaticVariableManager.numDuelingTreeTargets > 2) StaticVariableManager.numDuelingTreeTargets--;
            SetCurrentHolders();
        }
    }

    private void SetCurrentHolders()
    {
        DeactivateAllHolders();
        targetHolders[StaticVariableManager.numDuelingTreeTargets - 2].SetActive(true);
        numTargetText.text = "Targets: " + StaticVariableManager.numDuelingTreeTargets;
    }

    private void DeactivateAllHolders()
    {

        foreach (GameObject holder in targetHolders)
        {
            holder.SetActive(false);
        }
    }
}
