using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ScenarioData
{
    public List<string> allowed_static_scenarios;
    public List<string> allowed_animated3d_scenarios;
    public List<string> allowed_moving_targets_scenarios;
    public List<string> allowed_static_response_scenarios;
}

public class DropDown : MonoBehaviour
{
    public TMP_Dropdown handler_static;
    public TMP_Dropdown handler_static_response;
    public TMP_Dropdown handler_moving_targets;
    public TMP_Dropdown handler_3D;
    public GameObject infoUiPanel;
    public GameObject IPSCSceneButton;
    public GameObject VideoScenarioSceneButton;
    bool handle1_Updated;
    bool handle2_Updated;

    string Url = "http://192.168.137.1/imagistix/datareq.php";
    string get_scene_endpoint;
    static List<string> static_scenarios = new List<string>();       //static Scenarios
    static List<string> static_response_scenarios = new List<string>();       //3D Scenarios
    static List<string> moving_target_scenarios = new List<string>();
    static List<string> animated_3D_scenarios = new List<string>();
    static List<string> TrainingItems = new List<string>();
    string [] sceneNames;
    public static int sceneValSelected = 0;
    public static string softwareSceneName = "";
    public static string ompSceneName = "Basic Target";
    public static string scene_type = "static"; // Default setting
    public static bool isScenarioListSet = false;

    string sceneNames_SavePath = "Assets/Resources/scenesNames.txt";
    string onlineSceneBackupFile = "scDat01.txt";
    public static string UDP_ClientIP_Address;
    string storeTemp;
    int numStaticScenes = 0;
    int numStaticResponseScenes = 0;
    int numMovingScenes = 0;
    int num3DAnimatedScenes = 0;
    string activeScene = "";
    static string online_scene_data = "";
    private List<string> allowed_admins_list = new List<string> { "admin", "cib.kam@zonezapp.com", "nheuer25@gmail.com" };
    private bool isAllowedAdScenes = false;

    public void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;
        login_Manager.isSuperAdmin = false;
        //CreateFiles();
        //infoUiPanel = GameObject.FindGameObjectWithTag("InfoUI");

        if (login_Manager.isSuperAdmin)
        {
            ReadFile();
        }
        else
        {
            if (activeScene.ToLower().Contains("login"))
            {
                StaticVariableManager.online_scene_data = "";
                isScenarioListSet = false;
            }
            else if(activeScene.ToLower().Contains("mainmenu"))
            {
                isScenarioListSet = false;
                StartCoroutine(getScenes());
            }
            else if (activeScene.ToLower().Contains("scenemanager")) // if(activeScene.ToLower().Contains("scenemanager"))
            {
                ManageAddAdminScenes();
                ManageScenarios(StaticVariableManager.online_scene_data);
            }
        }
        //ReadFile();
    }

    private void ManageAddAdminScenes()
    {
        isAllowedAdScenes = allowed_admins_list.Contains(login_Manager.EmailText.ToLower());

        if (isAllowedAdScenes) // Check if the current admin is 
        {
            IPSCSceneButton.SetActive(true);
            VideoScenarioSceneButton.SetActive(true);
        }
    }

    public bool IsAdmin(string email)
    {
        if(allowed_admins_list.Contains(email.ToLower())) return true;

        return false;
    }

    IEnumerator getScenes()
    {
        WWW www = null;

        string Url = "";
        string get_scene_endpoint = "";
        bool containsData = false;
        bool isInternetAvailable = false;

        // Check internet connection
        // ✅ Wait for the CheckInternetConnection coroutine
        yield return StartCoroutine(CheckInternetConnection((bool isConnected) =>
        {
            isInternetAvailable = isConnected;
        }));

        if (isInternetAvailable) // Only run when internet is available
        {
            // Prepare request inside try block
            try
            {

                Url = configuration.ConfigAllUrls["server_url"];
                get_scene_endpoint = configuration.ConfigAllEndpoints["get_allowed_scenes_endpoint"];

                WWWForm form = new WWWForm();
                form.AddField("email", login_Manager.EmailText);

                www = new WWW(Url + get_scene_endpoint, form);

            }
            catch (Exception e)
            {
                // Debug.LogError("Exception during request setup: " + e.Message);
                // yield break;  // Exit early if setup failed
            }

            // ✅ Wait for download outside the try block
            yield return www;

            if (www != null)
            {
                // StaticVariableManager.online_scene_data = www.text;
                // Debug.Log("The Scene Data: " + www.text);
                containsData = Regex.IsMatch(www.text, @"[a-zA-Z]");

                if(containsData)
                    print("Online Scenario List: " + www.text);
            }
        }

        // Detect errors in the response
        bool isError = false;
        
        if(www != null)
            isError = www.text.ToLower().Contains("invalid email address") || www.text.ToLower().Contains("error");

        if (containsData && !isError) // If response contains valid data and no error
        {
            string onlineResponse = www.text;
            onlineResponse = FixSceneNames(onlineResponse);

            if (onlineResponse == "No scenarios")
            {
                StaticVariableManager.online_scene_data = FileManager.ReadFromFile(onlineSceneBackupFile);
                // Save data
                SaveScenarios();
            }
            else
            {
                StaticVariableManager.online_scene_data = onlineResponse;
                SaveScenarios();
                // print("Online scene info set.");
            }

        }
        else
        {
            StaticVariableManager.online_scene_data = FileManager.ReadFromFile(onlineSceneBackupFile);
            if (StaticVariableManager.online_scene_data != "") 
                SaveScenarios();
            // print("Offline scene info set.");
        }

        if (www != null) www.Dispose();
        isScenarioListSet = true;
    }

    private void SaveScenarios()
    {
        // Incript back
        if (StaticVariableManager.online_scene_data != "" && login_Manager.EmailText == "admin")
            FileManager.WriteDataToFile(onlineSceneBackupFile, StaticVariableManager.online_scene_data);
        else if (StaticVariableManager.online_scene_data != "")
            FileManager.incriptToFile(onlineSceneBackupFile, StaticVariableManager.online_scene_data);

        // print("Saved Data: \n" + StaticVariableManager.online_scene_data);
    }

    public IEnumerator CheckInternetConnection(System.Action<bool> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Head("https://www.google.com"))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            bool isConnected = !(request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError);
#else
            bool isConnected = !(request.isNetworkError || request.isHttpError);
#endif

            callback(isConnected);
        }
    }

    private string FixSceneNames(string json)
    {
        string safetyChar = "*";
        // Headers to skip (protect them)
        Dictionary<string, string> headersToProtect = new Dictionary<string, string>
    {
        { "allowed_static_scenarios", $"allowed{safetyChar}static{safetyChar}scenarios" },
        { "allowed_moving_targets_scenarios", $"allowed{safetyChar}moving{safetyChar}targets{safetyChar}scenarios" },
        { "allowed_static_response_scenarios", $"allowed{safetyChar}static{safetyChar}response{safetyChar}scenarios" },
        { "allowed_animated3d_scenarios", $"allowed{safetyChar}animated3d{safetyChar}scenarios" }
    };

        bool atLeastOneHeaderFound = false;

        // Check if at least one required header exists in json
        foreach (var header in headersToProtect.Keys)
        {
            if (json.Contains(header))
            {
                // print(header);
                atLeastOneHeaderFound = true;
                break; // We found one, no need to keep checking the rest
            }
        }

        if (!atLeastOneHeaderFound)
        {
            return "No scenarios";
        }

        // Replace protected headers with placeholders
        foreach (var kvp in headersToProtect)
        {
            json = json.Replace(kvp.Key, kvp.Value);
        }

        // Replace all underscores with spaces
        string fixedJson = json.Replace("_", " ");

        // Restore the protected headers
        foreach (var kvp in headersToProtect)
        {
            fixedJson = fixedJson.Replace(kvp.Value, kvp.Key);
        }

        return fixedJson;
    }




    private void ManageScenarios(string data)
    {
        try
        {
            if (!data.ToLower().Contains("invalid email address") && !data.ToLower().Contains("error") && data != "")
            {
                ScenarioData Response_data = JsonConvert.DeserializeObject<ScenarioData>(data);
                ScenarioData myLists = new ScenarioData();

                //Reset the value to 0 and clear the options
                ResetDropDowns();

                handler_static.options.Add(new TMP_Dropdown.OptionData() { text = "Static" });
                handler_static_response.options.Add(new TMP_Dropdown.OptionData() { text = "Static Response" });
                handler_moving_targets.options.Add(new TMP_Dropdown.OptionData() { text = "Moving Targets" });
                handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = "Animated 3D" });

                // Static scenario processing
                if (Response_data.allowed_static_scenarios != null)
                {
                    foreach (string scenario in Response_data.allowed_static_scenarios) 
                        AddStaticScene(scenario);
                }
                // Static Response scenario processing
                if (Response_data.allowed_static_response_scenarios != null)
                {
                    foreach (string scenario in Response_data.allowed_static_response_scenarios) 
                        AddStaticResponseScene(scenario);
                }
                // Moving targets scenario response
                if (Response_data.allowed_moving_targets_scenarios != null)
                {
                    foreach (string scenario in Response_data.allowed_moving_targets_scenarios) 
                        AddMovingTargetScene(scenario);
                }
                // 3D Scenario processing
                if (Response_data.allowed_animated3d_scenarios != null)
                {
                    foreach (string scenario in Response_data.allowed_animated3d_scenarios) 
                        AddAnimated3DScene(scenario);
                }

                // Fallback Logic
                if (handler_3D.options.Count <= 1) handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = "No scenarios." });
                if (handler_static.options.Count <= 1) handler_static.options.Add(new TMP_Dropdown.OptionData() { text = "No scenarios." });
                if (handler_static_response.options.Count <= 1) handler_static_response.options.Add(new TMP_Dropdown.OptionData() { text = "No scenarios." });
                if (handler_moving_targets.options.Count <= 1) handler_moving_targets.options.Add(new TMP_Dropdown.OptionData() { text = "No scenarios." });

                // Force Unity to update the visual UI with the brand new data structures
                handler_static.RefreshShownValue();
                handler_static_response.RefreshShownValue();
                handler_moving_targets.RefreshShownValue();
                handler_3D.RefreshShownValue();
            }
        }
        catch (Exception e)
        {
            print(e.StackTrace);
        }
    }

    private void ResetDropDowns()
    {
        // Reset drop downs
        handler_static.value = 0;
        handler_static.options.Clear();

        handler_static_response.value = 0;
        handler_static_response.options.Clear();

        handler_moving_targets.value = 0;
        handler_moving_targets.options.Clear();

        handler_3D.value = 0;
        handler_3D.options.Clear();

        // Reset scenario variables
        static_scenarios.Clear();
        static_response_scenarios.Clear();
        moving_target_scenarios.Clear();
        animated_3D_scenarios.Clear();
    }

    public void populateDropDown()
    {
       
        try
        {
            string temp;
            //print("Point reached...");

            if (!activeScene.ToLower().Contains("mainmenu"))
            {
                handler_3D.options.Clear();
                handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = "Animated 3D" });
                handler_static.options.Clear();
                handler_static.options.Add(new TMP_Dropdown.OptionData() { text = "Static" });
                handler_static_response.options.Clear();
                handler_static_response.options.Add(new TMP_Dropdown.OptionData() { text = "Static Response" });
                handler_moving_targets.options.Clear();
                handler_moving_targets.options.Add(new TMP_Dropdown.OptionData() { text = "Moving Targets" });


                foreach (string item in sceneNames)
                {
                    temp = "";
                    foreach (char c in item)
                    {
                        if (!char.IsWhiteSpace(c))
                        {
                            temp += c;
                        }
                    }


                    if (temp.ToLower().Contains("static"))
                    {
                        if (temp.ToLower().Contains("circular"))
                        {
                            if (handle1_Updated == false)
                            {
                                //handler_static.options.Clear();
                                //handler_static.options.Add(new TMP_Dropdown.OptionData() { text = "__[Select Training Option]__" });
                                handle1_Updated = true;
                            }
                            AddStaticScene("Bullseye Challenge");
                        }
                        else if (temp.ToLower().Contains("manhostage.1"))
                        {
                            AddStaticScene("Hostage Situation");
                        }
                        else if (temp.ToLower().Contains("threatman.1"))
                        {
                            AddStaticScene("Man Threat 2");
                        }
                        else if (temp.ToLower().Contains("threatman.2"))
                        {
                            AddStaticScene("Man Threat 8");
                        }
                        else if (temp.ToLower().Contains("mantarget.1"))
                        {
                            AddStaticScene("IPSC Man 4");
                        }
                        else if (temp.ToLower().Contains("mantarget.2"))
                        {
                            AddStaticScene("IPSC Man 10");
                        }
                        else if (temp.ToLower().Contains("bullseye.5"))
                        {
                            AddStaticScene("Diverse Bullseye");
                        }
                        else if (temp.ToLower().Contains("lightpole"))
                        {
                            AddStaticScene("Pole Alignment");
                        }
                        else if (temp.ToLower().Contains("numbersequenceadd"))
                        {
                            AddStaticScene("Seqnum Addition");
                        }
                        else if (temp.ToLower().Contains("numbersequence"))
                        {
                            AddStaticScene("Numseq");
                        }
                        else if (temp.ToLower().Contains("colorsequence"))
                        {
                            AddStaticScene("ROYGBIV");
                        }
                        else if (temp.ToLower().Contains("Indoor Range"))
                        {
                            AddStaticScene("Indoor range 1 Lane");
                        }
                        else if (temp.ToLower().Contains("distancesimulator"))
                        {
                            AddStaticScene("Distance Simulator");
                        }
                        else if (temp.ToLower().Contains("animalIPSC"))
                        {
                            AddStaticScene("Animal Target");
                        }
                        else if (temp.ToLower().Contains("expressionshoot"))
                        {
                            AddStaticScene("Expression Shoot");
                        }
                    }
                    else if (temp.ToLower().Contains("basic") || temp.ToLower().Contains("rising"))
                    {
                        if (handle2_Updated == false)
                        {
                            //handler_3D.options.Clear();
                            //handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = "__[Select Training Option]__" });
                            handle2_Updated = true;
                        }

                        if (temp.ToLower().Contains("fallingplat"))
                        {
                            AddStaticResponseScene("Falling Plates");
                        }
                        else if (temp.ToLower().Contains("freeshoot"))
                        {
                            AddStaticResponseScene("Basic IPSC Board");
                        }
                        else if (temp.ToLower().Contains("IPSCboard"))
                        {
                            AddStaticResponseScene("Shifting IPSC Targets");
                        }
                        else if (temp.ToLower().Contains("manhostage.1"))
                        {
                            AddStaticResponseScene("Hostage Situation");
                        }
                        else if (temp.ToLower().Contains("threatman.1"))
                        {
                            AddStaticResponseScene("Man Threat 2");
                        }
                        else if (temp.ToLower().Contains("threatman.2"))
                        {
                            AddStaticResponseScene("Man Threat 8");
                        }
                        else if (temp.ToLower().Contains("mantarget.1"))
                        {
                            AddStaticResponseScene("IPSC Man 4");
                        }
                        else if (temp.ToLower().Contains("mantarget.2"))
                        {
                            AddStaticResponseScene("IPSC Man 10");
                        }
                        else if (temp.ToLower().Contains("bullseye.5"))
                        {
                            AddStaticResponseScene("Diverse Bullseye");
                        }
                        else if (temp.ToLower().Contains("rising.lane"))
                        {
                            AddStaticResponseScene("Rising Plates");
                        }
                        else if (temp.ToLower().Contains("dueling"))
                        {
                            AddStaticResponseScene("Dueling Tree");
                        }
                        else if (temp.ToLower().Contains("suspect"))
                        {
                            AddStaticResponseScene("Suspect Shoot");
                        }
                        else if (temp.ToLower().Contains("block"))
                        {
                            AddStaticResponseScene("Block Target");
                        }
                        else if (temp.ToLower().Contains("dice"))
                        {
                            AddStaticResponseScene("Dice Shoot");
                        }
                        else if (temp.ToLower().Contains("shell"))
                        {
                            AddStaticResponseScene("Shell Game");
                        }
                        else if (temp.ToLower().Contains("animalipec"))
                        {
                            AddStaticResponseScene("Animal Target");
                        }

                        //Moving
                        else if (temp.ToLower().Contains("risingshape"))
                        {
                            AddMovingTargetScene("Rising Shape");
                        }
                        else if (temp.ToLower().Contains("cargame"))
                        {
                            AddMovingTargetScene("Racetrack Target");
                        }
                        else if (temp.ToLower().Contains("claypigeon"))
                        {
                            AddMovingTargetScene("Clay Pigeon");
                        }
                        else if (temp.ToLower().Contains("hidden"))
                        {
                            AddMovingTargetScene("Hidden Shape");
                        }
                    }
                    else if (temp.ToLower().Contains("baloon"))
                    {
                        if (temp.ToLower().Contains("baloon"))
                        {
                            if (temp.ToLower().Contains("up"))
                            {
                                AddMovingTargetScene("Ascending Balloons");
                            }
                            else
                            {
                                //AddMovingTargetScene("Descending Balloons");
                            }

                        }
                    }
                    else
                    {
                        AddAnimated3DScene(temp);
                    }

                }

                string all_scenarios = "";
                
                foreach(string name in static_scenarios)
                {
                    all_scenarios += " * " + name;
                }
                foreach (string name in static_response_scenarios)
                {
                    all_scenarios += " * " + name; ;
                }
                foreach (string name in moving_target_scenarios)
                {
                    all_scenarios += " * " + name; ;
                }

                //("Scenarios : " + all_scenarios);
            }

        }
        catch (Exception e)
        {
            print(e.StackTrace);
        }

    }

    private void AddStaticScene(string name)
    {
        numStaticScenes++;
        static_scenarios.Add(name);
        handler_static.options.Add(new TMP_Dropdown.OptionData() { text = numStaticScenes + ". " + name });
    }
    private void AddStaticResponseScene(string name)
    {
        numStaticResponseScenes++;
        handler_static_response.options.Add(new TMP_Dropdown.OptionData() { text = numStaticResponseScenes + ". " + name });
        static_response_scenarios.Add(name);
    }
    private void AddMovingTargetScene(string name)
    {
        numMovingScenes++;
        handler_moving_targets.options.Add(new TMP_Dropdown.OptionData() { text = numMovingScenes + ". " + name });
        moving_target_scenarios.Add(name);
    }
    private void AddAnimated3DScene(string name)
    {
        num3DAnimatedScenes++;

        if(name.ToLower().Contains("hunting"))
        {
            // handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = num3DAnimatedScenes + ". Ground Hunting" });
            // handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = num3DAnimatedScenes + ". Outdoor Hunting" });
            handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = num3DAnimatedScenes + ". Wildlife Hunting" });
            // animated_3D_scenarios.Add("Ground Hunting");
            // animated_3D_scenarios.Add("Outdoor Hunting");
            animated_3D_scenarios.Add("Wildlife Hunting");
        }
        else
        {
            handler_3D.options.Add(new TMP_Dropdown.OptionData() { text = num3DAnimatedScenes + ". " + name });
            animated_3D_scenarios.Add(name);
        }
    }


    public void HandleStaticInputData(int val)
    {
        //infoUiPanel.GetComponent<InfoManager>().closeInfo();
        sceneValSelected = val - 1;
        softwareSceneName = static_scenarios[val - 1];
        softwareSceneName = regenerateName(softwareSceneName, "static");

        LoadNextScenario();
    }
    public void HandleStaticResponseInputData(int val)
    {
        //infoUiPanel.GetComponent<InfoManager>().closeInfo();
        sceneValSelected = val - 1;
        softwareSceneName = static_response_scenarios[sceneValSelected];
        

        softwareSceneName = regenerateName(softwareSceneName, "static response");
        //print("Scene Selected is: " + softwareSceneName);

        LoadNextScenario();
    }
    public void HandleMovingTargetInputData(int val)
    {
        //infoUiPanel.GetComponent<InfoManager>().closeInfo();
        sceneValSelected = val - 1;
        softwareSceneName = moving_target_scenarios[val - 1];

        softwareSceneName = regenerateName(softwareSceneName, "moving");

        LoadNextScenario();
    }
    public void Handle3DInputData(int val)
    {
        //infoUiPanel.GetComponent<InfoManager>().closeInfo();
        sceneValSelected = val - 1;
        softwareSceneName = animated_3D_scenarios[val - 1];

        softwareSceneName = regenerateName(softwareSceneName, "3D");
        //print(softwareSceneName);

        LoadNextScenario();
    }


    private void LoadNextScenario()
    {
        if (softwareSceneName.ToLower().Contains("basic") || softwareSceneName.ToLower().Contains("lane")
            || softwareSceneName.ToLower().Contains("indoor range"))
        {
            if(activeScene.ToLower().Contains("scenemanager"))
            {
                SceneManager.LoadScene("TestConditionSetting");
            }
            else
            {
                //Do nothing, all scenario data has been assigned to global variables.
            }
        }
        else if (softwareSceneName.ToLower().Contains("hunting"))
        {
            // SceneManager.LoadScene("HuntingConditionSetting");
            SceneManager.LoadScene("TestConditionSetting");
        }
        else
        {
            SceneManager.LoadScene(softwareSceneName);
        }
    }
    public void OpenIPSCOptions()
    {
        softwareSceneName = "Basic1LaneIPSCExtended";
        SceneManager.LoadScene("IPSCExtendedSceneOptions");
    }
    public void OpenVideoOptions()
    {
        SceneManager.LoadScene("VideoSceneOptions");
    }

    private string regenerateName(string name, string sceneType)
    {
        ompSceneName = name; //Assign dropdown name for scene display
        scene_type = sceneType;

        if(sceneType == "static")
        {
            if (name == "Man Threat 8")
            {
                name = "Basic1Lane8PointManTargetStatic";
            }
            else if (name == "Hostage Situation")
            {
                name = "Basic1LaneThreateningHostageStatic";
            }
            else if (name == "IPSC Man 4")
            {
                name = "Basic1Lane4PointManTargetStatic";
            }
            else if (name == "IPSC Man 10")
            {
                name = "Basic1Lane10PointManTargetStatic";
            }
            else if (name == "Diverse Bullseye")
            {
                name = "Basic1Lane5PointBullseyeTargetStatic";
            }
            else if (name == "Bullseye Challenge")
            {
                name = "Circular.LaneTargets";
            }
            else if (name == "Man Threat 2")
            {
                name = "Basic1LaneThreateningManStatic";
            }
            else if (name == "Pole Alignment")
            {
                name = "Basic1LaneRifflePoleShooting";
            }
            else if (name == "Numseq")
            {
                name = "Basic1LaneSequenceNumShoot";
            }
            else if (name == "Seqnum Addition")
            {
                name = "Basic1LaneSequenceNumAddShoot";
            }
            else if (name == "ROYGBIV")
            {
                name = "Basic1LaneColorSequenceShoot";
            }
            else if (name == "Indoor Range")
            {
                name = "Indoor Range Lane";
            }
            else if (name == "Indoor range 2 Lane")
            {
                name = "Indoor_range_2Targets";
            }
            else if (name == "Indoor range 3 Lane")
            {
                name = "Indoor_range_3Targets";
            }
            else if (name == "Distance Simulator")
            {
                name = "Basic1LaneDistanceSimulator";
            }
            else if (name == "Animal Target")
            {
                name = "Basic1LaneAnimalTargetStatic";
            }
            else if (name == "Ipsc Board")
            {
                name = "Basic1LaneIPSCBoardTargetStatic";
            }

        }
        else if(sceneType == "3D")
        {
            if (name == "Open Plain")
            {
                name = "OpenPlain";
            }
            else if (name == "Outdoor Containers")
            {
                name = "Outdoor_Containers";
            }
            else if (name == "Outdoor Forest")
            {
                name = "Outdoor_Forest";
            }
            else if (name == "Underground Parking")
            {
                name = "Underground_Parking";
            }
            else if (name == "Indoor Restaurant")
            {
                name = "Indoor_Restaurant";
            }
            else if (name == "Mall Shooting")
            {
                name = "Mall";
            }
            else if (name == "Suspect Shoot")
            {
                name = "CloseUpSuspectShoot";
            }
            else if (name == "Suspect Shoot")
            {
                name = "CloseUpSuspectShoot";
            }
        }
        else
        {
            if (name == "Hidden Shape")
            {
                name = "Basic.LaneHiddenTarget";
            }
            else if (name == "Man Threat 2")
            {
                name = "Basic1LaneThreateningMan";
            }
            else if (name == "Falling Plates")
            {
                name = "Basic1LaneFallingPlat";
            }
            else if (name == "Balloon Shooting")
            {
                name = "Upwards.Lane_Baloons";
            }
            else if (name == "Descending�Balloons")
            {
                name = "Downwards.Lane_Baloons";
            }
            else if (name == "Basic IPSC Board")
            {
                name = "Basic1LaneTargetPopUpFreeShoot";
            }
            else if (name == "Shifting IPSC Targets")
            {
                name = "Basic.LaneIPECBoard";
            }
            else if (name == "Rising Plates")
            {
                name = "Rising.LanePlates";
            }
            else if (name == "Dueling Tree")
            {
                name = "Basic.Lane_DuelingTree";
            }
            else if (name == "Suspect Shoot")
            {
                name = "Basic.Lane_Suspect";
            }
            else if (name == "Block Target")
            {
                name = "Basic1LaneBlockShooting";
            }
            else if (name == "Pole Alignment")
            {
                name = "Basic1LaneRifflePoleShooting";
            }
            else if (name == "Bullseye Targets.2")
            {
                name = "Basic1Lane5TargetShoot";
            }
            else if (name == "Man Threat 2")
            {
                name = "Basic1LaneThreateningMan";
            }
            else if (name == "Man Threat 8")
            {
                name = "Basic1Lane8PointManTarget";
            }
            else if (name == "Hostage Situation")
            {
                name = "Basic1LaneThreateningHostage";
            }
            else if (name == "IPSC Man 4")
            {
                name = "Basic1Lane4PointManTarget";
            }
            else if (name == "IPSC Man 10")
            {
                name = "Basic1Lane10PointManTarget";
            }
            else if (name == "Diverse Bullseye")
            {
                name = "Basic1Lane5PointBullseyeTarget";
            }//p
            else if (name == "Dice Shoot")
            {
                name = "Basic1LaneDiceFlipping";
            }
            else if (name == "Shell Game")
            {
                name = "Basic1LaneShellGame";
            }
            else if (name == "Racetrack Target")
            {
                name = "Basic2LaneCarGame";
            }
            else if (name == "Clay Pigeon")
            {
                name = "BasicClayPigeonPlat1LaneShoot";
            }
            else if (name == "3D Clay Pigeon")
            {
                name = "Basic1Lane3DClayPigeon";
            } // 3D Bird Hunting
            else if (name == "3D Bird Hunting")
            {
                name = "Basic1LaneBirdHunting";
            } // 
            else if (name == "Rising Target")
            {
                name = "BasicRisingShapePlat1LaneShoot";
            }
            else if (name == "Animal Target")
            {
                name = "Basic1LaneAnimalTarget";
            }
            else if (name == "Expression Shoot")
            {
                name = "Basic1LaneColorExpressionShoot";
            }
            else if (name == "Ipsc Board")
            {
                name = "Basic1LaneIPSCBoardTarget";
            }
            else if (name == "Pepper Popper Plate")
            {
                name = "Basic1LanePepperPopperTarget";
            }
            else if (name == "Target Recognition")
            {
                name = "Basic1LaneTargetRecognition";
            }
            else if (name == "Decision Point")
            {
                name = "Basic1LaneDecisionPTShoot";
            }
        }

        //print("RRE: test scene name is..." + name);

        return name;
    }

    public void CreateFiles()
    {
        if (!File.Exists(onlineSceneBackupFile))
        {
            File.Create(onlineSceneBackupFile).Close();
        }
    }
    private void ReadFile()
    {

        using (StreamReader reader = new StreamReader(sceneNames_SavePath))
        {
            
            sceneNames = File.ReadAllLines(sceneNames_SavePath);

            foreach (string r in static_scenarios)
            {
                //print(r);
                TrainingItems.Add(r);
            }
            populateDropDown();

        }


    }
    private void WriteFile(string val)
    {
        using (StreamWriter writer = new StreamWriter(sceneNames_SavePath))
        {
            writer.WriteLine(val); //Load the LocalIp Address to the UDP IP Text File
        }
    }

    void incriptLoginDetails(string data)
    {

        Encryption encrypt = new Encryption();
        data = login_Manager.EmailText + "," + data ;        //Add Mac Address as first item is the Data

        string base64 = encrypt.AESEncryption(data);//encrypt username and password
        FileManager.CreateFile("scDat01.txt");
        FileManager.WriteDataToFile("scDat01.txt", base64);
        print("File created and written succesfully...");

    }
    private string OfflineLogin()
    {
        try
        {
            
            string base64 = FileManager.ReadFromFile("scDat01.txt");
            Encryption encrypt = new Encryption();
            return encrypt.AESDecryption(base64);                    //decryption code
        }
        catch (Exception e)
        {
            print("Error retrieving zero file:" + e);
            throw new Exception("First login");
        }

    }

}