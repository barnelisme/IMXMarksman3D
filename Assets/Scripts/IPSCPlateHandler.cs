using UnityEngine;
using UnityEngine.SceneManagement;

public class IPSCPlateHandler : MonoBehaviour
{

    private string activeScene = "";
    public bool isPlateHit = false;
    private int objectLive = 1;
    private int numHits = 0;
    private GameObject player;
    float destroyTimer = 5;

    // Load the color of the material
    float r;
    float g;
    float b;
    float a;

    // Texas Wheel 
    public GameObject targetHolder;
    public GameObject movingCentralHub;
    public GameObject slider;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!this.transform.name.ToLower().Contains("board")) loadTargetColor();
    }

    // Update is called once per frame
    void Update()
    {
        // centralHub.GetComponent<TexasWheelManager>()...

        if (isPlateHit)
        {
            destroyTimer -= Time.deltaTime * 1;
            if (destroyTimer <= 0f)
            {
                // StaticVariableManager.plateDestroyed = true;
                Destroy(this.gameObject);
            }
        }
    }

    private void ApplyDamage(string tagged)
    {
        activeScene = SceneManager.GetActiveScene().name;
        // Debug.Log("I was hit:" + transform.name + " apply Damage sent:" + tagged);

        if (activeScene.ToLower().Contains("ipscextended"))
        {
            objectLive--;
            numHits++;



            if (objectLive <= 0f)
            {
                player.GetComponent<ReplayCounter>().SetCurrReplayTarget(("next"), this.gameObject, this.gameObject); // Set current target to the currently activated target.
                // this.transform.gameObject.GetComponent<MeshCollider>().enabled = false;
                string targetSide = "";


                // Tell the hub which plate was hit
                if (targetHolder != null)
                {
                    // print("Point reached");
                    if (movingCentralHub!= null)
                    {
                        if(movingCentralHub.transform.name == "Texas Wheel Cental Hub")
                        {
                            TexasWheelManager hub = movingCentralHub.GetComponent<TexasWheelManager>();
                            IPSCSliderManager slider_manager = slider.GetComponent<IPSCSliderManager>();

                            slider_manager.isPlateHit = true;

                            if (hub != null)
                            {
                                if (tagged.Contains("TopLeft")) targetSide = "TopLeft";
                                else if (tagged.Contains("TopRight")) targetSide = "TopRight";
                                else if (tagged.Contains("BottomLeft")) targetSide = "BottomLeft";
                                else if (tagged.Contains("BottomRight")) targetSide = "BottomRight";
                                else if (tagged.Contains("Bottom")) targetSide = "Bottom";

                                hub.OnPlateHit(targetSide);
                            }

                            if(targetHolder.transform.name.Contains("Target 1"))
                            {
                                if (numHits >= StaticVariableManager.numTarget1ShotsSetting / 5)
                                    CloneAndRelease(this.transform.gameObject);
                            }
                            else if (targetHolder.transform.name.Contains("Target 2"))
                            {
                                if (numHits >= StaticVariableManager.numTarget2ShotsSetting / 5)
                                    CloneAndRelease(this.transform.gameObject);
                            }
                            else if (targetHolder.transform.name.Contains("Target 3"))
                            {
                                if (numHits >= StaticVariableManager.numTarget3ShotsSetting / 5)
                                    CloneAndRelease(this.transform.gameObject);
                            }

                        }
                        else if (movingCentralHub.transform.name == "IPSC Slider 1 Arm")
                        {
                            IPSCSliderManager hub = movingCentralHub.GetComponent<IPSCSliderManager>();

                            if (targetHolder.transform.name.Contains("Target 1"))
                            {
                                if (numHits >= StaticVariableManager.numTarget1ShotsSetting)
                                {
                                    //CloneAndRelease(this.transform.gameObject);
                                    hub.isPlateHit = true;
                                }
                                    
                            }
                            else if (targetHolder.transform.name.Contains("Target 2"))
                            {
                                if (numHits >= StaticVariableManager.numTarget2ShotsSetting)
                                {
                                    //CloneAndRelease(this.transform.gameObject);
                                    hub.isPlateHit = true;
                                }
                            }
                            else if (targetHolder.transform.name.Contains("Target 3"))
                            {
                                if (numHits >= StaticVariableManager.numTarget3ShotsSetting)
                                {
                                    //CloneAndRelease(this.transform.gameObject);
                                    hub.isPlateHit = true;
                                }
                            }
                        }
                        else if (movingCentralHub.transform.name == "Pendulum Texas Wheel Cental Hub")
                        {
                            TexasWheelManager hub = movingCentralHub.GetComponent<TexasWheelManager>();
                            IPSCSliderManager sliderManager = slider.GetComponent<IPSCSliderManager>();

                            if (hub != null)
                            {
                                if (tagged.Contains("BottomLeft")) targetSide = "BottomLeft";
                                else if (tagged.Contains("BottomRight")) targetSide = "BottomRight";
                                else if (tagged.Contains("Top")) targetSide = "Top";

                                hub.OnPlateHit(targetSide);
                            }
                            sliderManager.isPlateHit = true;

                            CloneAndRelease(this.transform.gameObject);
                            if (targetHolder.transform.name.Contains("Target 1"))
                            {
                                //if (numHits >= StaticVariableManager.numTarget1ShotsSetting / 5)
                                //    CloneAndRelease(this.transform.gameObject);
                            }
                        }
                    }
                    else if (targetHolder.transform.name == "IPSC Pole 1")
                    {
                        if(numHits >= StaticVariableManager.numIPSCPole1Shots)
                        {

                        }
                    }
                    else if (targetHolder.transform.name == "IPSC Pole 2")
                    {

                    }
                    else if (targetHolder.transform.name == "IPSC Pole 3")
                    {

                    }
                    else if (targetHolder.transform.name == "IPSC Slider 1")
                    {

                    }
                    else if (targetHolder.transform.name == "IPSC Slider 2")
                    {

                    }
                    else if(targetHolder.transform.name.Contains("Steel plate stand"))
                    {
                        SteelPlateManager plateManager = slider.GetComponent<SteelPlateManager>();
                        plateManager.MovePlate();
                    }
                    else if (targetHolder.transform.name.Contains("Pepper Popper"))
                    {
                        this.GetComponent<MeshCollider>().enabled = false;
                        PepperPopperManager  plateManager = slider.GetComponent<PepperPopperManager>();
                        plateManager.MovePlate();
                    }

                }

                // next_Target.SetActive(true);

            }
        }


    }

    private void loadTargetColor()
    {
        if (Scoring.ammo_setting.ToLower().Contains("live") || Scoring.ammo_setting.ToLower().Contains("infrared"))
        {
            Renderer renderer = this.GetComponent<Renderer>();
            //StaticVariableManager.targetColorSetting = "black";
            switch (StaticVariableManager.targetColorSetting.ToLower())
            {
                case "black":
                    r = 42f / 255f;
                    g = 42f / 255f;
                    b = 42f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "white":
                    r = 212f / 255f;
                    g = 212f / 255f;
                    b = 212f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "yellow":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 217f / 255f;
                    g = 200f / 255f;
                    b = 0f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);

                    break;
                case "red":
                    r = 234f / 255f;
                    g = 54f / 255f;
                    b = 54f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;
                case "blue":
                    //fpsCam.backgroundColor = Color.blue;
                    r = 59f / 255f;
                    g = 59f / 255f;
                    b = 236f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;

                case "green":
                    r = 54f / 255f;
                    g = 181f / 255f;
                    b = 54f / 255f;
                    a = 1f;
                    renderer.material.color = new Color(r, g, b, a);
                    break;
            }
        }
    }

    private void CloneAndRelease(GameObject plate)
    {
        // STEP 1: Create a blank GameObject in world space
        GameObject clone = new GameObject("DetachedPlateClone");

        // STEP 2: Copy world transform from the plate
        clone.transform.position = plate.transform.position;
        clone.transform.rotation = plate.transform.rotation;
        clone.transform.localScale = plate.transform.lossyScale;

        // STEP 3: Copy the MeshFilter and MeshRenderer
        MeshFilter originalFilter = plate.GetComponent<MeshFilter>();
        MeshRenderer originalRenderer = plate.GetComponent<MeshRenderer>();

        if (originalFilter != null)
        {
            MeshFilter cloneFilter = clone.AddComponent<MeshFilter>();
            cloneFilter.sharedMesh = originalFilter.sharedMesh;
        }

        if (originalRenderer != null)
        {
            MeshRenderer cloneRenderer = clone.AddComponent<MeshRenderer>();
            cloneRenderer.sharedMaterials = originalRenderer.sharedMaterials;
        }

        // STEP 4: Copy collider (optional: BoxCollider or MeshCollider)
        Collider originalCollider = plate.GetComponent<Collider>();
        if (originalCollider != null)
        {
            if (originalCollider is BoxCollider originalBox)
            {
                BoxCollider cloneBox = clone.AddComponent<BoxCollider>();
                cloneBox.center = originalBox.center;
                cloneBox.size = originalBox.size;
                cloneBox.isTrigger = originalBox.isTrigger;
                cloneBox.material = originalBox.material;
            }
            else if (originalCollider is SphereCollider originalSphere)
            {
                SphereCollider cloneSphere = clone.AddComponent<SphereCollider>();
                cloneSphere.center = originalSphere.center;
                cloneSphere.radius = originalSphere.radius;
                cloneSphere.isTrigger = originalSphere.isTrigger;
                cloneSphere.material = originalSphere.material;
            }
            else if (originalCollider is MeshCollider originalMesh)
            {
                MeshCollider cloneMesh = clone.AddComponent<MeshCollider>();
                cloneMesh.sharedMesh = originalMesh.sharedMesh;
                cloneMesh.convex = originalMesh.convex;
                cloneMesh.isTrigger = originalMesh.isTrigger;
                cloneMesh.material = originalMesh.material;
            }
            // Add more collider types here as needed
        }


        // STEP 5: Add Rigidbody
        Rigidbody rb = clone.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;

        // STEP 6: Optional – Copy custom script value
        IPSCPlateHandler receiver = clone.AddComponent<IPSCPlateHandler>();
        receiver.isPlateHit = true;

        // STEP 7: Disable the original nested plate
        plate.SetActive(false);
    }

}
