import cv2
import keyboard

webCam = cv2.VideoCapture(0)
currentframe = 0
status = 1;

def saveFrameAsImage():
    success, frame = webCam.read()

    cv2.imshow("Output", frame)
    cv2.imwrite('frame' + str(currentframe) + '.jpg', frame)
    currentframe +=1

    print("Image Saved")
    status = 0


webCam.release()
cv2.destroyAllWindows()

