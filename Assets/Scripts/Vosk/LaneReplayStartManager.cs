using UnityEngine;

public class LaneReplayStartPManager : MonoBehaviour
{
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        string name = this.transform.name;

        // print(name);

        if (name.Contains("1"))
        {
            if (!StaticVariableManager.isLane1StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
        }
        if (name.Contains("2"))
        {
            if (!StaticVariableManager.isLane2StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
        }
        if (name.Contains("3"))
        {
            if (!StaticVariableManager.isLane3StartTargetSet) player.GetComponent<ReplayCounter>().SetCurrReplayTarget("start", this.gameObject, this.gameObject);
        }
    }
}
