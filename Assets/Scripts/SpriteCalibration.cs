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

    // Start is called before the first frame update
    void Start()
    {
        MultipleScreens();
        udpClient = new UdpClient(portnum);

        isDirectCalibration = true;
        isFourPointsRequest = false;

        if (isDirectCalibration) whiteSpace.SetActive(true);
    }
    void Update()
    {
        if(isDirectCalibration)
        {
            /*if(isFourPointsRequest)
            {
                CalibratePoint5.X = 0;
                CalibratePoint5.Y = 0;

                getCalibration();
            }
            else
            {
                // More points setup
            }*/

            //CalibratePoint5.X = 0;
            //CalibratePoint5.Y = 0;

            //getCalibration();

            getPointsSignal();

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
                ExitToMenu();
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
        // Debug.Log("Saving:" + calibrationPath + " points:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString() + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString());
        System.IO.File.WriteAllText(@calibrationPath, "Newpoints:" + CalibratePoint1.X.ToString() + "," + CalibratePoint1.Y.ToString() + ":" + CalibratePoint2.X.ToString() + "," + CalibratePoint2.Y.ToString() + ":" + CalibratePoint3.X.ToString() + "," + CalibratePoint3.Y.ToString() + ":" + CalibratePoint4.X.ToString() + "," + CalibratePoint4.Y.ToString() 
            + ":" + CalibratePoint5.X.ToString() + "," + CalibratePoint5.Y.ToString() + ":" + CalibratePoint6.X.ToString() + "," + CalibratePoint6.Y.ToString() + ":" + CalibratePoint7.X.ToString() + "," + CalibratePoint7.Y.ToString() + ":" + CalibratePoint8.X.ToString() + "," + CalibratePoint8.Y.ToString() + ":" + CalibratePoint9.X.ToString() + "," + CalibratePoint9.Y.ToString()
            + ":" + CalibratePoint10.X.ToString() + "," + CalibratePoint10.Y.ToString() + ":" + CalibratePoint11.X.ToString() + "," + CalibratePoint11.Y.ToString() + ":" + CalibratePoint12.X.ToString() + "," + CalibratePoint12.Y.ToString() + ":" + CalibratePoint13.X.ToString() + "," + CalibratePoint13.Y.ToString() + ":" + CalibratePoint14.X.ToString() + "," + CalibratePoint14.Y.ToString()
            + ":" + CalibratePoint15.X.ToString() + "," + CalibratePoint15.Y.ToString() + ":" + CalibratePoint16.X.ToString() + "," + CalibratePoint16.Y.ToString() + ":" + CalibratePoint17.X.ToString() + "," + CalibratePoint17.Y.ToString() + ":" + CalibratePoint18.X.ToString() + "," + CalibratePoint18.Y.ToString() + ":" + CalibratePoint19.X.ToString() + "," + CalibratePoint19.Y.ToString()
            + ":" + CalibratePoint20.X.ToString() + "," + CalibratePoint20.Y.ToString() + ":" + CalibratePoint21.X.ToString() + "," + CalibratePoint21.Y.ToString() + ":" + CalibratePoint22.X.ToString() + "," + CalibratePoint22.Y.ToString() + ":" + CalibratePoint23.X.ToString() + "," + CalibratePoint23.Y.ToString() + ":" + CalibratePoint24.X.ToString() + "," + CalibratePoint24.Y.ToString()
            + ":" + CalibratePoint25.X.ToString() + "," + CalibratePoint25.Y.ToString());
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

    void getPointsSignal()
    {
        try
        {
            IPEndPoint remoteEP = null;
            if (udpClient.Available > 0)
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string message = Encoding.ASCII.GetString(data);
                // Debug.Log(message + " from " + remoteEP.Address.ToString());
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
                        print(message);
                        string[] points = message.Split(':');

                        ProcessReceivedMessage(message);
                        SaveCalibration();
                        ExitToMenu();
                    }

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
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        float minX = canvasRect.position.x + canvasRect.rect.xMin + 10f;
        float maxX = canvasRect.position.x + canvasRect.rect.xMax - 10f;
        float minY = canvasRect.position.y + canvasRect.rect.yMin + 8f;
        float maxY = canvasRect.position.y + canvasRect.rect.yMax - 8f;
        float centerX = (minX + maxX) / 2;
        float centerY = (minY + maxY) / 2;
        float z = canvasRect.position.z;

        // Define key midpoints
        float midTopX = (minX + maxX) / 2;
        float midTopY = (centerY + maxY) / 2;
        float midBottomY = (centerY + minY) / 2;
        float midLeftX = (minX + centerX) / 2;
        float midRightX = (centerX + maxX) / 2;

        Vector3 targetPosition = Vector3.zero;

        print("Point Number: " + pointNumber);
        switch (pointNumber)
        {
            // Primary Corners
            case "1": targetPosition = new Vector3(minX, maxY, z); break; // Top-left
            case "2": targetPosition = new Vector3(maxX, maxY, z); break; // Top-right
            case "3": targetPosition = new Vector3(maxX, minY, z); break; // Bottom-right
            case "4": targetPosition = new Vector3(minX, minY, z); break; // Bottom-left
            case "5": targetPosition = new Vector3(centerX, centerY, z); break; // Center

            // Midpoints of main quadrants
            case "6": targetPosition = new Vector3(midLeftX, midTopY, z); break; // Center of top-left quadrant
            case "7": targetPosition = new Vector3(midRightX, midTopY, z); break; // Center of top-right quadrant
            case "8": targetPosition = new Vector3(midRightX, midBottomY, z); break; // Center of bottom-right quadrant
            case "9": targetPosition = new Vector3(midLeftX, midBottomY, z); break; // Center of bottom-left quadrant

            // Subdivided quadrants
            case "10": targetPosition = new Vector3((minX + midLeftX) / 2, (maxY + midTopY) / 2, z); break;
            case "11": targetPosition = new Vector3((midLeftX + centerX) / 2, (maxY + midTopY) / 2, z); break;
            case "12": targetPosition = new Vector3((centerX + midRightX) / 2, (maxY + midTopY) / 2, z); break;
            case "13": targetPosition = new Vector3((midRightX + maxX) / 2, (maxY + midTopY) / 2, z); break;

            case "14": targetPosition = new Vector3((minX + midLeftX) / 2, (midTopY + centerY) / 2, z); break;
            case "15": targetPosition = new Vector3((midLeftX + centerX) / 2, (midTopY + centerY) / 2, z); break;
            case "16": targetPosition = new Vector3((centerX + midRightX) / 2, (midTopY + centerY) / 2, z); break;
            case "17": targetPosition = new Vector3((midRightX + maxX) / 2, (midTopY + centerY) / 2, z); break;

            case "18": targetPosition = new Vector3((minX + midLeftX) / 2, (midBottomY + centerY) / 2, z); break;
            case "19": targetPosition = new Vector3((midLeftX + centerX) / 2, (midBottomY + centerY) / 2, z); break;
            case "20": targetPosition = new Vector3((centerX + midRightX) / 2, (midBottomY + centerY) / 2, z); break;
            case "21": targetPosition = new Vector3((midRightX + maxX) / 2, (midBottomY + centerY) / 2, z); break;

            case "22": targetPosition = new Vector3((minX + midLeftX) / 2, (minY + midBottomY) / 2, z); break;
            case "23": targetPosition = new Vector3((midLeftX + centerX) / 2, (minY + midBottomY) / 2, z); break;
            case "24": targetPosition = new Vector3((centerX + midRightX) / 2, (minY + midBottomY) / 2, z); break;
            case "25": targetPosition = new Vector3((midRightX + maxX) / 2, (minY + midBottomY) / 2, z); break;
        }

        autoCalibSprite.transform.position = targetPosition;
    }


    private void SetCalibrationSprite_BackUp(string pointNumber)
    {
        RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();
        float minX = canvasRect.position.x + canvasRect.rect.xMin + 10f;
        float maxX = canvasRect.position.x + canvasRect.rect.xMax - 10f;
        float minY = canvasRect.position.y + canvasRect.rect.yMin + 8f;
        float maxY = canvasRect.position.y + canvasRect.rect.yMax - 8f;
        float centerX = (minX + maxX) / 2;
        float centerY = (minY + maxY) / 2;
        float z = canvasRect.position.z;

        Vector3 targetPosition = Vector3.zero;

        print("Point Number: " + pointNumber);
        switch (pointNumber)
        {
            case "1":
                targetPosition = new Vector3(minX, maxY, z); // Top-left
                break;
            case "2":
                targetPosition = new Vector3(maxX, maxY, z); // Top-right
                break;
            case "3":
                targetPosition = new Vector3(maxX, minY, z); // Bottom-right
                break;
            case "4":
                targetPosition = new Vector3(minX, minY, z); // Bottom-left
                break;
            case "5":
                targetPosition = new Vector3(centerX, centerY, z); // Center
                break;
            case "6":
                targetPosition = new Vector3((centerX + minX) / 2, (centerY + maxY) / 2, z); // Midway between center and top-left
                break;
            case "7":
                targetPosition = new Vector3((centerX + maxX) / 2, (centerY + maxY) / 2, z); // Midway between center and top-right
                break;
            case "8":
                targetPosition = new Vector3((centerX + maxX) / 2, (centerY + minY) / 2, z); // Midway between center and bottom-right
                break;
            case "9":
                targetPosition = new Vector3((centerX + minX) / 2, (centerY + minY) / 2, z); // Midway between center and bottom-left
                break;
        }

        autoCalibSprite.transform.position = targetPosition;
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

            case 5:
                print("Point 5: " + x.ToString() + "," + y.ToString());
                CalibratePoint5.X = (int)x;
                CalibratePoint5.Y = (int)y;
                break;

            case 6:
                print("Point 6: " + x.ToString() + "," + y.ToString());
                CalibratePoint6.X = (int)x;
                CalibratePoint6.Y = (int)y;
                break;

            case 7:
                print("Point 7: " + x.ToString() + "," + y.ToString());
                CalibratePoint7.X = (int)x;
                CalibratePoint7.Y = (int)y;
                break;

            case 8:
                print("Point 8: " + x.ToString() + "," + y.ToString());
                CalibratePoint8.X = (int)x;
                CalibratePoint8.Y = (int)y;
                break;

            case 9:
                print("Point 9: " + x.ToString() + "," + y.ToString());
                CalibratePoint9.X = (int)x;
                CalibratePoint9.Y = (int)y;
                break;

            case 10:
                print("Point 10: " + x.ToString() + "," + y.ToString());
                CalibratePoint10.X = (int)x;
                CalibratePoint10.Y = (int)y;
                break;
            case 11:
                print("Point 11: " + x.ToString() + "," + y.ToString());
                CalibratePoint11.X = (int)x;
                CalibratePoint11.Y = (int)y;
                break;

            case 12:
                print("Point 12: " + x.ToString() + "," + y.ToString());
                CalibratePoint12.X = (int)x;
                CalibratePoint12.Y = (int)y;
                break;

            case 13:
                print("Point 13: " + x.ToString() + "," + y.ToString());
                CalibratePoint13.X = (int)x;
                CalibratePoint13.Y = (int)y;
                break;

            case 14:
                print("Point 14: " + x.ToString() + "," + y.ToString());
                CalibratePoint14.X = (int)x;
                CalibratePoint14.Y = (int)y;
                break;

            case 15:
                print("Point 15: " + x.ToString() + "," + y.ToString());
                CalibratePoint15.X = (int)x;
                CalibratePoint15.Y = (int)y;
                break;

            case 16:
                print("Point 16: " + x.ToString() + "," + y.ToString());
                CalibratePoint16.X = (int)x;
                CalibratePoint16.Y = (int)y;
                break;

            case 17:
                print("Point 17: " + x.ToString() + "," + y.ToString());
                CalibratePoint17.X = (int)x;
                CalibratePoint17.Y = (int)y;
                break;

            case 18:
                print("Point 18: " + x.ToString() + "," + y.ToString());
                CalibratePoint18.X = (int)x;
                CalibratePoint18.Y = (int)y;
                break;

            case 19:
                print("Point 19: " + x.ToString() + "," + y.ToString());
                CalibratePoint19.X = (int)x;
                CalibratePoint19.Y = (int)y;
                break;

            case 20:
                print("Point 20: " + x.ToString() + "," + y.ToString());
                CalibratePoint20.X = (int)x;
                CalibratePoint20.Y = (int)y;
                break;

            case 21:
                print("Point 21: " + x.ToString() + "," + y.ToString());
                CalibratePoint21.X = (int)x;
                CalibratePoint21.Y = (int)y;
                break;

            case 22:
                print("Point 22: " + x.ToString() + "," + y.ToString());
                CalibratePoint22.X = (int)x;
                CalibratePoint22.Y = (int)y;
                break;

            case 23:
                print("Point 23: " + x.ToString() + "," + y.ToString());
                CalibratePoint23.X = (int)x;
                CalibratePoint23.Y = (int)y;
                break;

            case 24:
                print("Point 24: " + x.ToString() + "," + y.ToString());
                CalibratePoint24.X = (int)x;
                CalibratePoint24.Y = (int)y;
                break;

            case 25:
                print("Point 25: " + x.ToString() + "," + y.ToString());
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
        Debug.Log(Display.displays.Length + " is/are connected");

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

    }
}
