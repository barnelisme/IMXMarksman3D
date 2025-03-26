using System;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class SpriteCalibration : MonoBehaviour
{
    public Canvas mainCanvas;
    float trackTime;
    UdpClient udpClient;
    int portnum = 22222;
    System.Drawing.Point CalibratePoint1 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint2 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint3 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint4 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint5 = new System.Drawing.Point();
    string calibrationPath = "Assets/Resources/Calibration.txt";

    float xPos = 0f; 
    float yPos = 0f;

    public GameObject topLeftImage;
    public GameObject topRightImage;
    public GameObject bottomRightImage;
    public GameObject bottomLeftImage;
    public GameObject centerImage;

    private bool isDirectCalibration = false, isFourPointsRequest = false;
    private List<int> calibrationPoints = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        MultipleScreens();
        udpClient = new UdpClient(portnum);

        isDirectCalibration = false;
        isFourPointsRequest = true;
    }
    void Update()
    {
        if(isDirectCalibration)
        {
            if(isFourPointsRequest)
            {
                CalibratePoint5.X = 0;
                CalibratePoint5.Y = 0;

                getCalibration();
            }
            else
            {
                // More points setup
            }

        }
        else
        {
            trackTime += Time.deltaTime;
            if (trackTime < 5)
            {
                //TopLeft();
                topLeftImage.SetActive(true);
                topRightImage.SetActive(false);
                bottomRightImage.SetActive(false);
                bottomLeftImage.SetActive(false);
                take1stPoint();
            }
            else if (trackTime > 5 && trackTime < 10)
            {
                //BottomRight();
                topLeftImage.SetActive(false);
                topRightImage.SetActive(true);
                bottomRightImage.SetActive(false);
                bottomLeftImage.SetActive(false);
                take2ndPoint();
            }
            else if (trackTime > 10 && trackTime < 15)
            {
                //BottomRight();
                topLeftImage.SetActive(false);
                topRightImage.SetActive(false);
                bottomRightImage.SetActive(true);
                bottomLeftImage.SetActive(false);
                take3rdPoint();
            }
            else if (trackTime > 15 && trackTime < 20)
            {
                //BottomRight();
                topLeftImage.SetActive(false);
                topRightImage.SetActive(false);
                bottomRightImage.SetActive(false);
                bottomLeftImage.SetActive(true);
                take4rthPoint();
            }
            else if (trackTime > 20 && trackTime < 20)
            {
                //BottomRight();
                topLeftImage.SetActive(false);
                topRightImage.SetActive(false);
                bottomRightImage.SetActive(false);
                bottomLeftImage.SetActive(false);
                centerImage.SetActive(true);
                take5fthPoint();
            }
            else
            {
                SaveCalibration();
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
    void TopLeft()
    {
        float minX = (mainCanvas.GetComponent<RectTransform>().position.x + mainCanvas.GetComponent<RectTransform>().rect.xMin) + 10f;
        float maxY = mainCanvas.GetComponent<RectTransform>().position.y + mainCanvas.GetComponent<RectTransform>().rect.yMax - 8f;
        float z = mainCanvas.GetComponent<RectTransform>().position.z;
        Color newColor = new Vector4(0.3f, 0.4f, 0.6f);
        Vector3 topLeft = new Vector3(minX, maxY, z);
        transform.position = topLeft;
    }
    void BottomRight()
    {
        float maxX = mainCanvas.GetComponent<RectTransform>().position.x + mainCanvas.GetComponent<RectTransform>().rect.xMax - 8f;
        float minY = mainCanvas.GetComponent<RectTransform>().position.y + mainCanvas.GetComponent<RectTransform>().rect.yMin + 10f;
        float z = mainCanvas.GetComponent<RectTransform>().position.z;

        Vector3 bottomRight = new Vector3(maxX, minY, z);
        transform.position = bottomRight;
    }
    private void SaveCalibration()
    {
        Debug.Log("Saving:" + calibrationPath + " points:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString() + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString());
        System.IO.File.WriteAllText(@calibrationPath, "Newpoints:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString() + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString());
    }
    void take1stPoint()
    {
        getXnYpos();
        //Debug.Log(" New X:" + xPos + " and New Y:" + yPos);
        CalibratePoint1.X = (int)xPos;
        CalibratePoint1.Y = (int)yPos;
    }
    void take2ndPoint()
    {
        getXnYpos();
        //Debug.Log(" New X2:" + xPos + " and New Y2:" + yPos);
        CalibratePoint2.X = (int)xPos;
        CalibratePoint2.Y = (int)yPos;
    }
    void take3rdPoint()
    {
        getXnYpos();
        //Debug.Log(" New X:" + xPos + " and New Y:" + yPos);
        CalibratePoint3.X = (int)xPos;
        CalibratePoint3.Y = (int)yPos;
    }
    void take4rthPoint()
    {
        getXnYpos();
        //Debug.Log(" New X2:" + xPos + " and New Y2:" + yPos);
        CalibratePoint4.X = (int)xPos;
        CalibratePoint4.Y = (int)yPos;
    }
    void take5fthPoint()
    {
        getXnYpos();
        //Debug.Log(" New X2:" + xPos + " and New Y2:" + yPos);
        CalibratePoint5.X = (int)xPos;
        CalibratePoint5.Y = (int)yPos;
    }
    void getXnYpos()
    {
        try
        {
            IPEndPoint remoteEP = null;
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log(message + " from " + remoteEP.Address.ToString());

                xPos = float.Parse(message.Split(':')[0]);
                yPos = float.Parse(message.Split(':')[1]);
            }
        }
        catch (Exception e)
        {
            Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
            Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
        }

    }

    void getCalibration()
    {
        try
        {
            IPEndPoint remoteEP = null;
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                Debug.Log(message + " from " + remoteEP.Address.ToString());
                if(message.ToLower().Contains("calibration point"))
                {
                    // Load calibration
                    print(message);
                    string [] points = message.Split(':');

                    ProcessReceivedMessage(message);
                    SaveCalibration();
                    SceneManager.LoadScene("MainMenu");
                }
            }
        }
        catch (Exception e)
        {
            Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
            Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
        }

    }

    public void ProcessReceivedMessage(string message)
    {
        // Example message: "Calibration Points: [[131, 96], [690, 83], [635, 560], [43, 453]]"
        var match = Regex.Match(message, @"\[\[.*\]\]");
        if (match.Success)
        {
            string pointsStr = match.Value.Trim('[', ']'); // Remove outer brackets
            string[] pairs = pointsStr.Split(new string[] { "], [" }, System.StringSplitOptions.None);

            calibrationPoints.Clear();

            int corner = 0;
            foreach (string pair in pairs)
            {
                corner++; // Screen points increment

                string[] nums = pair.Split(',');
                int x = int.Parse(nums[0].Trim());
                int y = int.Parse(nums[1].Trim());

                LoadPoints(x,y,corner);
            }


        }
        else
        {
            Debug.LogWarning("No valid points found in message!");
        }
    }

    private void LoadPoints(int x, int y, int corner)
    {
        switch(corner)
        {

            case 1:
                print("Point 1: " + x.ToString() + "," + y.ToString());
                CalibratePoint1.X = (int)x;
                CalibratePoint1.Y = (int)y;
                break;

            case 2:
                print("Point 2: " + x.ToString() + "," + y.ToString());
                CalibratePoint2.X = (int)x;
                CalibratePoint2.Y = (int)y;
                break;

            case 3:
                print("Point 3: " + x.ToString() + "," + y.ToString());
                CalibratePoint3.X = (int)x;
                CalibratePoint3.Y = (int)y;
                break;

            case 4:
                print("Point 4: " + x.ToString() + "," + y.ToString());
                CalibratePoint4.X = (int)x;
                CalibratePoint4.Y = (int)y;
                break;

        }
    }

    private void MultipleScreens()
    {
        Debug.Log(Display.displays.Length + " is/are connected");

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

    }
}
