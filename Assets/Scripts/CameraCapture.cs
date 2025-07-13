using UnityEngine;
using System.IO;

public class CameraCapture : MonoBehaviour
{
    public Camera captureCamera;  // Camera from which to capture the image
    public string fileName = "captured_image.png"; // The name of the saved image

    // Control variable for cropping direction: "left", "right", or "center"
    private string cropDirection = "center";

    // Percentage to crop from the width (e.g., 0.05 = 5%)
    private float cropPercentage = 0.5f;

    //string oneStepBackDirectory = "";

    void Start()
    {
        // Optionally call the capture function
        // CaptureAndSaveImage("1");

        string oneStepBackDirectory = Directory.GetParent((Application.dataPath)).FullName;

    }

    public void CaptureAndSaveImage(int camera_number, string image_side, float crop_percentage)
    {
        cropDirection = image_side;
        cropPercentage = crop_percentage;
        fileName = "Trainee_" + camera_number + ".png";


        // Ensure the camera is set
        if (captureCamera == null)
        {
            Debug.LogError("No capture camera found! Using Camera.main by default.");
            captureCamera = Camera.main; // Default to the main camera if none is set
        }

        // Create a RenderTexture to capture the camera's view
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        captureCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        // Render the camera's view into the RenderTexture
        captureCamera.Render();

        cropPercentage = Mathf.Clamp(cropPercentage, 0f, 1f);

        // Calculate cropped width safely
        int croppedWidth = Mathf.Max(1, (int)(Screen.width * (1 - cropPercentage)));

        // Calculate the cropped width based on the percentage
        //int croppedWidth = (int)(Screen.width * (1 - cropPercentage));  // Commented original
        int croppedX = 0;  // Default for left or right (we'll adjust this based on crop direction)

        // Create a new Texture2D to hold the cropped image
        Texture2D texture = new Texture2D(croppedWidth, Screen.height, TextureFormat.RGB24, false);

        // Determine how to crop the image based on the direction
        switch (cropDirection.ToLower())
        {
            case "right":
                // Crop from the right side
                croppedX = (int)(Screen.width * cropPercentage);
                break;

            case "left":
                // Crop from the left side
                croppedX = 0;
                break;

            case "center":
                // Crop from both sides towards the center
                croppedX = (int)(Screen.width * cropPercentage / 2);
                break;

            default:
                Debug.LogWarning("Invalid cropDirection. Using 'center' as default.");
                croppedX = (int)(Screen.width * cropPercentage / 2);
                break;
        }

        // Read the pixels from the RenderTexture into the cropped area of the Texture2D
        texture.ReadPixels(new Rect(croppedX, 0, croppedWidth, Screen.height), 0, 0);
        texture.Apply();

        ////////////////////////// Now, we need to ensure the image is a perfect square/////////////////////////
        int squareSize = Mathf.Max(texture.width, texture.height); // Make the square side the max dimension (height or width)

        // Create a new Texture2D for the square image with black padding
        Texture2D squareTexture = new Texture2D(squareSize, squareSize, TextureFormat.RGB24, false);

        // Set the background color to black (or any other color if preferred)
        Color[] colorPixels = new Color[squareSize * squareSize];
        for (int i = 0; i < colorPixels.Length; i++)
        {
            colorPixels[i] = Color.black;  // Black padding
        }
        squareTexture.SetPixels(colorPixels);

        // Now we calculate the position where the cropped image should be placed in the square texture
        int paddingX = (squareSize - texture.width) / 2;
        int paddingY = (squareSize - texture.height) / 2;

        // Place the cropped image in the center of the square texture
        squareTexture.SetPixels(paddingX, paddingY, texture.width, texture.height, texture.GetPixels());


        //////////////////// Apply the changes to the square texture//////////////////////////////
        squareTexture.Apply();

        // Convert black padding areas to grey (Color.grey or Color(0.5f, 0.5f, 0.5f))
        Color[] squarePixels = squareTexture.GetPixels();
        for (int i = 0; i < squarePixels.Length; i++)
        {
            if (squarePixels[i] == Color.black)  // If the pixel is black, convert it to grey
            {
                squarePixels[i] = new Color(0.614f, 0.614f, 0.614f);  // Or use Color(0.5f, 0.5f, 0.5f) for a custom grey tone
            }
        }

        // Set the modified pixels back to the texture
        squareTexture.SetPixels(squarePixels);
        squareTexture.Apply();

        //////////////////////////// Convert the Texture2D into a PNG byte array//////////////////////////////
        byte[] imageBytes = squareTexture.EncodeToPNG();

        // Determine the path to save the image under Resources/Score Images
        //string directoryPath = Path.Combine(Application.dataPath, "Score Images");

        string oneStepBackDirectory = Directory.GetParent((Application.dataPath)).FullName;
        string AssetsDirectory = Path.Combine(oneStepBackDirectory, "Assets");
        string resourcesDirectory = Path.Combine(AssetsDirectory, "Resources");
        string scoreImagesDirectory = Path.Combine(resourcesDirectory, "Score Images");

        if (!Directory.Exists(scoreImagesDirectory))
        {
            // If Sim Data does not exist, create the folders and copy files from source folder
            Directory.CreateDirectory(scoreImagesDirectory);
            Debug.Log("Created folder: " + scoreImagesDirectory);
        }

        //string directoryPath = Path.Combine(Application.dataPath, "");

        // Save the image as a PNG file in the specified folder
        string filePath = Path.Combine(scoreImagesDirectory, fileName);
        File.WriteAllBytes(filePath, imageBytes);

        // Clean up by releasing the RenderTexture
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        //Debug.Log("Camera " + camera_number + " Image saved to: " + filePath);
    }


    public void CaptureAndSaveImage_1(int camera_number, string image_side, float crop_percentage)
    {
        cropDirection = image_side;
        cropPercentage = crop_percentage;
        fileName = "Trainee_" + camera_number + ".png";

        // Ensure the camera is set
        if (captureCamera == null)
        {
            captureCamera = Camera.main; // Default to the main camera if none is set
        }

        // Create a RenderTexture to capture the camera's view
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        captureCamera.targetTexture = renderTexture;
        RenderTexture.active = renderTexture;

        // Render the camera's view into the RenderTexture
        captureCamera.Render();

        // Calculate the cropped width based on the percentage
        int croppedWidth = (int)(Screen.width * (1 - cropPercentage));
        int croppedX = 0;  // Default for left or right (we'll adjust this based on crop direction)

        // Create a new Texture2D to hold the cropped image
        Texture2D texture = new Texture2D(croppedWidth, Screen.height, TextureFormat.RGB24, false);

        // Determine how to crop the image based on the direction
        switch (cropDirection.ToLower())
        {
            case "right":
                // Crop from the right side
                croppedX = (int)(Screen.width * cropPercentage);
                break;

            case "left":
                // Crop from the left side
                croppedX = 0;
                break;

            case "center":
                // Crop from both sides towards the center
                croppedX = (int)(Screen.width * cropPercentage / 2);
                break;

            default:
                Debug.LogWarning("Invalid cropDirection. Using 'center' as default.");
                croppedX = (int)(Screen.width * cropPercentage / 2);
                break;
        }

        // Read the pixels from the RenderTexture into the cropped area of the Texture2D
        texture.ReadPixels(new Rect(croppedX, 0, croppedWidth, Screen.height), 0, 0);
        texture.Apply();

        // Convert the Texture2D into a PNG byte array
        byte[] imageBytes = texture.EncodeToPNG();

        // Determine the path to save the image under Resources/Score Images
        string directoryPath = Path.Combine(Application.dataPath, "Score Images");

        // Create the directory if it doesn't exist
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Save the image as a PNG file in the specified folder
        string filePath = Path.Combine(directoryPath, fileName);
        File.WriteAllBytes(filePath, imageBytes);

        // Clean up by releasing the RenderTexture
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        Debug.Log("Camera " + camera_number + " Image saved to: " + filePath);
    }
}
