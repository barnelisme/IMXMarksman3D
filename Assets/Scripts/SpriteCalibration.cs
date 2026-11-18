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
    System.Drawing.Point CalibratePoint6 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint7 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint8 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint9 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint10 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint11 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint12 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint13 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint14 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint15 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint16 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint17 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint18 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint19 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint20 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint21 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint22 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint23 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint24 = new System.Drawing.Point();
    System.Drawing.Point CalibratePoint25 = new System.Drawing.Point();
    string calibrationPath = "Assets/Resources/Calibration.txt";

    float xPos = 0f;
    float yPos = 0f;

    public GameObject topLeftImage;
    public GameObject topRightImage;
    public GameObject bottomRightImage;
    public GameObject bottomLeftImage;
    public GameObject centerImage;
    public GameObject autoCalibSprite;
    public GameObject whiteSpace;

    private bool isDirectCalibration = false, isFourPointsRequest = false;
    private List<int> calibrationPoints = new List<int>();
    public GameObject DisplayPanel;

    // Start is called before the first frame update
    void Start()
    {
        MultipleScreens();
        udpClient = new UdpClient(portnum);
        udpClient.Client.Blocking = false; // 🔑 non-blocking

        isDirectCalibration = true;
        isFourPointsRequest = false;

        if (isDirectCalibration) whiteSpace.SetActive(true);
    }
    void Update()
    {
        if (isDirectCalibration)
        {
            getPointsSignal();
        }
        else
        {
            // Pass
        }
    }

    private void SaveCalibration()
    {
        // Debug.Log("Saving:" + calibrationPath + " points:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString() + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString());
        System.IO.File.WriteAllText(@calibrationPath, "Newpoints:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString()
            + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString() + ":" + CalibratePoint6.X.ToString() + "," + CalibratePoint6.Y.ToString() + ":" + CalibratePoint7.X.ToString() + "," + CalibratePoint7.Y.ToString() + ":" + CalibratePoint8.X.ToString() + "," + CalibratePoint8.Y.ToString() + ":" + CalibratePoint9.X.ToString() + "," + CalibratePoint9.Y.ToString()
            + ":" + CalibratePoint10.X.ToString() + "," + CalibratePoint10.Y.ToString() + ":" + CalibratePoint11.X.ToString() + "," + CalibratePoint11.Y.ToString() + ":" + CalibratePoint12.X.ToString() + "," + CalibratePoint12.Y.ToString() + ":" + CalibratePoint13.X.ToString() + "," + CalibratePoint13.Y.ToString() + ":" + CalibratePoint14.X.ToString() + "," + CalibratePoint14.Y.ToString()
            + ":" + CalibratePoint15.X.ToString() + "," + CalibratePoint15.Y.ToString() + ":" + CalibratePoint16.X.ToString() + "," + CalibratePoint16.Y.ToString() + ":" + CalibratePoint17.X.ToString() + "," + CalibratePoint17.Y.ToString() + ":" + CalibratePoint18.X.ToString() + "," + CalibratePoint18.Y.ToString() + ":" + CalibratePoint19.X.ToString() + "," + CalibratePoint19.Y.ToString()
            + ":" + CalibratePoint20.X.ToString() + "," + CalibratePoint20.Y.ToString() + ":" + CalibratePoint21.X.ToString() + "," + CalibratePoint21.Y.ToString() + ":" + CalibratePoint22.X.ToString() + "," + CalibratePoint22.Y.ToString() + ":" + CalibratePoint23.X.ToString() + "," + CalibratePoint23.Y.ToString() + ":" + CalibratePoint24.X.ToString() + "," + CalibratePoint24.Y.ToString()
            + ":" + CalibratePoint25.X.ToString() + "," + CalibratePoint25.Y.ToString());
    }

    void getPointsSignal()
    {
        // print("Point Reached...");
        try
        {
            IPEndPoint remoteEP = null;

            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                print(message + " from " + remoteEP.Address.ToString());

                if (message.ToLower().Contains("point") || message.ToLower().Contains("calibrate"))
                {
                    if (message.ToLower().Contains("recalibrate"))
                    {
                        whiteSpace.transform.gameObject.SetActive(true);
                        autoCalibSprite.transform.gameObject.SetActive(false);
                    }
                    else if (message.ToLower().Contains("point "))
                    {
                        whiteSpace.transform.gameObject.SetActive(false);
                        autoCalibSprite.transform.gameObject.SetActive(true);

                        string[] pointNumber = message.Split(" ");
                        SetCalibrationSprite(pointNumber[1]);
                    }
                    else if (message.ToLower().Contains("calibration points"))
                    {
                        // Load calibration
                        string[] points = message.Split(':');

                        ProcessReceivedMessage(message);
                        SaveCalibration();


                        CalibrationUdpSend udp = new CalibrationUdpSend();
                        udp.sendEndless("IMX Calibration Set");
                        
                        ExitToMenu();
                    }

                }
                else if (message.ToLower().Contains("calibration complete"))
                {
                    ExitToMenu();
                }
            }
        }
        catch (Exception e)
        {
            Scoring.logs += "\n" + e.Message + ":" + e.StackTrace;
            Debug.Log("Exception:" + e.StackTrace + " " + e.Message);
        }

    }

    private void SetCalibrationSprite(string pointNumber)
    {
        RectTransform panelRect = DisplayPanel.GetComponent<RectTransform>();
        RectTransform spriteRect = autoCalibSprite.GetComponent<RectTransform>();

        float width = panelRect.rect.width;
        float height = panelRect.rect.height;

        // Use percentage-based margins (e.g. 1%)
        float marginX = width * 0.01f;
        float marginY = height * 0.01f;

        // Work in panel local space (center = 0,0)
        float minX = -width / 2f + marginX;
        float maxX = width / 2f - marginX;
        float minY = -height / 2f + marginY;
        float maxY = height / 2f - marginY;

        float centerX = (minX + maxX) / 2f;
        float centerY = (minY + maxY) / 2f;

        float midTopY = (centerY + maxY) / 2f;
        float midBottomY = (centerY + minY) / 2f;
        float midLeftX = (minX + centerX) / 2f;
        float midRightX = (centerX + maxX) / 2f;

        Vector2 targetPos = Vector2.zero;

        switch (pointNumber)
        {
            // Primary Corners
            case "1": targetPos = new Vector2(minX, maxY); break; // Top-left
            case "2": targetPos = new Vector2(maxX, maxY); break; // Top-right
            case "3": targetPos = new Vector2(maxX, minY); break; // Bottom-right
            case "4": targetPos = new Vector2(minX, minY); break; // Bottom-left
            case "5": targetPos = new Vector2(centerX, centerY); break; // Center

            // Midpoints of main quadrants
            case "6": targetPos = new Vector2(midLeftX, midTopY); break;
            case "7": targetPos = new Vector2(midRightX, midTopY); break;
            case "8": targetPos = new Vector2(midRightX, midBottomY); break;
            case "9": targetPos = new Vector2(midLeftX, midBottomY); break;

            // Subdivided quadrants
            case "10": targetPos = new Vector2((minX + midLeftX) / 2, (maxY + midTopY) / 2); break;
            case "11": targetPos = new Vector2((midLeftX + centerX) / 2, (maxY + midTopY) / 2); break;
            case "12": targetPos = new Vector2((centerX + midRightX) / 2, (maxY + midTopY) / 2); break;
            case "13": targetPos = new Vector2((midRightX + maxX) / 2, (maxY + midTopY) / 2); break;

            case "14": targetPos = new Vector2((minX + midLeftX) / 2, (midTopY + centerY) / 2); break;
            case "15": targetPos = new Vector2((midLeftX + centerX) / 2, (midTopY + centerY) / 2); break;
            case "16": targetPos = new Vector2((centerX + midRightX) / 2, (midTopY + centerY) / 2); break;
            case "17": targetPos = new Vector2((midRightX + maxX) / 2, (midTopY + centerY) / 2); break;

            case "18": targetPos = new Vector2((minX + midLeftX) / 2, (midBottomY + centerY) / 2); break;
            case "19": targetPos = new Vector2((midLeftX + centerX) / 2, (midBottomY + centerY) / 2); break;
            case "20": targetPos = new Vector2((centerX + midRightX) / 2, (midBottomY + centerY) / 2); break;
            case "21": targetPos = new Vector2((midRightX + maxX) / 2, (midBottomY + centerY) / 2); break;

            case "22": targetPos = new Vector2((minX + midLeftX) / 2, (minY + midBottomY) / 2); break;
            case "23": targetPos = new Vector2((midLeftX + centerX) / 2, (minY + midBottomY) / 2); break;
            case "24": targetPos = new Vector2((centerX + midRightX) / 2, (minY + midBottomY) / 2); break;
            case "25": targetPos = new Vector2((midRightX + maxX) / 2, (minY + midBottomY) / 2); break;
        }

        // 🔑 Force sprite to follow panel strictly
        spriteRect.SetParent(panelRect, false); // lock into panel’s local space
        spriteRect.localPosition = targetPos;   // use localPosition, not anchoredPosition
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

                LoadPoints(x, y, corner);
            }


        }
        else
        {
            Debug.LogWarning("No valid points found in message!");
        }
    }

    private void LoadPoints(int x, int y, int corner)
    {
        switch (corner)
        {

            case 1:
                //print("Point 1: " + x.ToString() + "," + y.ToString());
                CalibratePoint1.X = (int)x;
                CalibratePoint1.Y = (int)y;
                break;

            case 2:
                //print("Point 2: " + x.ToString() + "," + y.ToString());
                CalibratePoint2.X = (int)x;
                CalibratePoint2.Y = (int)y;
                break;

            case 3:
                //print("Point 3: " + x.ToString() + "," + y.ToString());
                CalibratePoint3.X = (int)x;
                CalibratePoint3.Y = (int)y;
                break;

            case 4:
                //print("Point 4: " + x.ToString() + "," + y.ToString());
                CalibratePoint4.X = (int)x;
                CalibratePoint4.Y = (int)y;
                break;

            case 5:
                //print("Point 5: " + x.ToString() + "," + y.ToString());
                CalibratePoint5.X = (int)x;
                CalibratePoint5.Y = (int)y;
                break;

            case 6:
                //print("Point 6: " + x.ToString() + "," + y.ToString());
                CalibratePoint6.X = (int)x;
                CalibratePoint6.Y = (int)y;
                break;

            case 7:
                //print("Point 7: " + x.ToString() + "," + y.ToString());
                CalibratePoint7.X = (int)x;
                CalibratePoint7.Y = (int)y;
                break;

            case 8:
                //print("Point 8: " + x.ToString() + "," + y.ToString());
                CalibratePoint8.X = (int)x;
                CalibratePoint8.Y = (int)y;
                break;

            case 9:
                //print("Point 9: " + x.ToString() + "," + y.ToString());
                CalibratePoint9.X = (int)x;
                CalibratePoint9.Y = (int)y;
                break;

            case 10:
                //print("Point 10: " + x.ToString() + "," + y.ToString());
                CalibratePoint10.X = (int)x;
                CalibratePoint10.Y = (int)y;
                break;
            case 11:
                //print("Point 11: " + x.ToString() + "," + y.ToString());
                CalibratePoint11.X = (int)x;
                CalibratePoint11.Y = (int)y;
                break;

            case 12:
                //print("Point 12: " + x.ToString() + "," + y.ToString());
                CalibratePoint12.X = (int)x;
                CalibratePoint12.Y = (int)y;
                break;

            case 13:
                //print("Point 13: " + x.ToString() + "," + y.ToString());
                CalibratePoint13.X = (int)x;
                CalibratePoint13.Y = (int)y;
                break;

            case 14:
                //print("Point 14: " + x.ToString() + "," + y.ToString());
                CalibratePoint14.X = (int)x;
                CalibratePoint14.Y = (int)y;
                break;

            case 15:
                //print("Point 15: " + x.ToString() + "," + y.ToString());
                CalibratePoint15.X = (int)x;
                CalibratePoint15.Y = (int)y;
                break;

            case 16:
                //print("Point 16: " + x.ToString() + "," + y.ToString());
                CalibratePoint16.X = (int)x;
                CalibratePoint16.Y = (int)y;
                break;

            case 17:
                //print("Point 17: " + x.ToString() + "," + y.ToString());
                CalibratePoint17.X = (int)x;
                CalibratePoint17.Y = (int)y;
                break;

            case 18:
                //print("Point 18: " + x.ToString() + "," + y.ToString());
                CalibratePoint18.X = (int)x;
                CalibratePoint18.Y = (int)y;
                break;

            case 19:
                //print("Point 19: " + x.ToString() + "," + y.ToString());
                CalibratePoint19.X = (int)x;
                CalibratePoint19.Y = (int)y;
                break;

            case 20:
                //print("Point 20: " + x.ToString() + "," + y.ToString());
                CalibratePoint20.X = (int)x;
                CalibratePoint20.Y = (int)y;
                break;

            case 21:
                //print("Point 21: " + x.ToString() + "," + y.ToString());
                CalibratePoint21.X = (int)x;
                CalibratePoint21.Y = (int)y;
                break;

            case 22:
                //print("Point 22: " + x.ToString() + "," + y.ToString());
                CalibratePoint22.X = (int)x;
                CalibratePoint22.Y = (int)y;
                break;

            case 23:
                //print("Point 23: " + x.ToString() + "," + y.ToString());
                CalibratePoint23.X = (int)x;
                CalibratePoint23.Y = (int)y;
                break;

            case 24:
                //print("Point 24: " + x.ToString() + "," + y.ToString());
                CalibratePoint24.X = (int)x;
                CalibratePoint24.Y = (int)y;
                break;

            case 25:
                //print("Point 25: " + x.ToString() + "," + y.ToString());
                CalibratePoint25.X = (int)x;
                CalibratePoint25.Y = (int)y;
                break;
        }
    }

    private void ExitToMenu()
    {
        SceneManager.LoadScene("Calib_Acc_Page");
    }

    private void MultipleScreens()
    {
        // Debug.Log(Display.displays.Length + " is/are connected");

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

    }
}
