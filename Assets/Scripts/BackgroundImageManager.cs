using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackgroundImageManager : MonoBehaviour
{

    private string imageLocation = "";
    public List<Sprite> backgroundImage = new List<Sprite>();
    private List<int> imageIndexPair = new List<int>();
    private int currImageIndex = 0;
    private string activeScene;

    void Start()
    {
        imageLocation = Path.Combine(FileManager.gamepath, "Background Images");
        activeScene = SceneManager.GetActiveScene().name;

        if(!Directory.Exists(imageLocation))
        {
            //Directory.CreateDirectory(imageLocation);
        }
        LoadMenuBackground();
    }

    private string CheckSystemUser()
    {
        string systemUser = "IMX"; // "The Shooting Range"  // "default"

        systemUser = FileManager.ReadSetting("System User");

        if (systemUser == "0")
        {
            systemUser = "IMX";
            FileManager.SaveSetting("System User", "IMX");
        }

        return systemUser;
    }


    private void LoadMenuBackground()
    {
        // Setup background Index
        if (CheckSystemUser() == "IMX")
        {
            if (activeScene.ToLower().Contains("login"))
            {
                currImageIndex = 0;
            }
            else
            {
                currImageIndex = 1;
            }
        }
        else if(CheckSystemUser() == "The Shooting Range")
        {
            if (activeScene.ToLower().Contains("login"))
            {
                currImageIndex = 2;
            }
            else
            {
                currImageIndex = 3;
            }
        }


        // Set background Image
        this.GetComponent<Image>().sprite = backgroundImage[currImageIndex];
    }

}
