import cv2
import numpy as np
import socket
import threading
import time
import udpReceive

# UDP settings
UDP_IP = '127.0.0.1'  # Replace with the IP address of your UDP receiver
UDP_PORT = 22222
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
IGNORE_RADIUS = 2

framewidth = 640
frameheight = 480

#UDP Receive Variables
server_socket = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_socket.setblocking(0)
server_socket.settimeout(0.0)
server_socket.bind(('', 55552))


# Function to send coordinates via UDP
def send_coordinates(coordinates):
    #message = f"{coordinates[0]},{coordinates[1]}"
    #sock.sendto(message.encode(), (UDP_IP, UDP_PORT))

    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
    # print("Broadcasting")

    sock.sendto(bytes(str(coordinates), "utf-8"), ("255.255.255.255", 22222))

def receive_UDP():
    # UDP RECEIVE CHECK POINT
    global udpData, activeScene, videoCapVal ,cameraOption, functionEndpoint
    udpData = " "
    activeScene = " "
    cameraOption = " "
    functionEndpoint = 1
    videoCapVal = 0

    while True:
        temp = udpReceive.ReceiveV2(server_socket)
        if "NONE" not in temp:
            if temp == "Calibration" or temp == "Reset" or temp == "Simulate" or temp == "Calibrate":
                udpData = temp
            if temp == "Camera 0" or temp == "Camera 1":
                cameraOption = temp
            else:
                activeScene = temp

                # temp = "NULL"
        #print (cameraOption)
        if cameraOption == "Camera 0":
            videoCapVal = 0
            if functionEndpoint != 0:      #Condition for running the function once
                generateCameraThread()
                functionEndpoint = 0

        elif cameraOption == "Camera 1":
            videoCapVal = 1
            if functionEndpoint != 0:      #Condition for running the function once
                generateCameraThread()
                functionEndpoint = 0

def process_stream():
    global frameInit
    # Initialize the USB camera
    print("Camera Value is :" + str(videoCapVal))
    cap = cv2.VideoCapture(videoCapVal)
    cap.set(3, framewidth)

    cap.set(4, frameheight)

    # Create a window with a slider bar to adjust the threshold value
    window_name = 'Threshold'
    cv2.namedWindow(window_name)
    cv2.createTrackbar('Value', window_name, 250 , 255, lambda x: None) #Mall
    current_value = cv2.getTrackbarPos('Value', window_name)
    global trackBarSwitch
    trackBarSwitch = 0

    # Initialize a list to store all previously detected points
    previous_points = []


    while True:

        try:
            if "Calibrate" in udpData:
                trackBarSwitch = 1

            if trackBarSwitch == 1:
                if "Calibrate" in udpData:
                    cv2.setTrackbarPos('Value', window_name, 240)
                if "Calibrate" not in udpData:
                    cv2.setTrackbarPos('Value', window_name, current_value)
                    trackBarSwitch = 0
            if trackBarSwitch == 0:
                current_value = cv2.getTrackbarPos('Value', window_name)
                #if "NONE" not in temp:
                if "Indoor" in activeScene:
                    cv2.setTrackbarPos('Value', window_name, current_value)
                if "Basic" in activeScene:
                    cv2.setTrackbarPos('Value', window_name, current_value)


            #print(trackBarSwitch)
            #print(udpData)

            # Read a frame from the camera
            ret, frame = cap.read()
            frameInit = frame

            # Convert the frame to grayscale
            gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

            # Get the current threshold value from the slider bar
            threshold_value = cv2.getTrackbarPos('Value', window_name)

            # Threshold the grayscale image to get all points above the current threshold value
            ret, thresh = cv2.threshold(gray, threshold_value, 255, cv2.THRESH_BINARY)

            # Find the contours of the thresholded image
            contours, hierarchy = cv2.findContours(thresh, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

            areas = [cv2.contourArea(c) for c in contours]  ##Branch of the UDP send X variables

            # if (current_value is not cv2.getTrackbarPos('Value', window_name)) or "Calibration" in udpData:
            # Add the new point to the list of previous points

            # Draw a blue circle around each contour and send its coordinates via UDP
            for contour in contours:
                (x, y), radius = cv2.minEnclosingCircle(contour)
                center = (int(x), int(y))
                radius = int(radius)

                # Variables for UDP Send Cordinates
                max_index = np.argmax(areas)
                cnt = contours[max_index]
                M = cv2.moments(cnt)

                # Check if the new point is within 2 pixels of any previous point
                point_exists = False
                ''' if(previous_points.__contains__([center])):
                    point_exists = True
                    print("Valid")
                else:
                    point_exists = False
                    print("Skip")'''

                for previous_point in previous_points:
                    try:
                        if np.linalg.norm(np.array(previous_point) - np.array(center)) < IGNORE_RADIUS:
                            point_exists = True
                            break
                    except:
                        pass

                if not point_exists:
                    # Draw a blue circle around the new point
                    cv2.circle(frame, center, radius, (255, 0, 0), 2)

                    # Send the new point's coordinates via UDP
                    #cx = int(M['m10'] / M['m00'])
                    #cy = int(M['m01'] / M['m00'])
                    #print(str(int(x)) + ":" + str(frameheight - int(y)))
                    center_UDP = str(int(x)) + ":" + str(frameheight - int(y))
                    send_coordinates(center_UDP)
                    #print(current_value)


                    previous_points.append(center)
                else:
                    print("Point already exist.")

                # Remove each point from the list after 5 seconds
                try:
                    current_time = time.time()
                    previous_points = [previous_point for previous_point in previous_points if
                                       current_time - previous_point[2] <= 5]
                except:

                    pass

            # Add the current time to each point in the list of previous points
            previous_points = [(previous_point[0], previous_point[1], time.time()) for previous_point in
                               previous_points]

            # Display the frame and slider bar
            cv2.imshow(window_name, thresh)
            cv2.imshow('frame', frame)

            # Check for key press and exit if 'q' is pressed
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break

        except Exception as err:
            #print("Exception:" + str(err))
            pass



    # Release the camera and close the windows
    cap.release()
    cv2.destroyAllWindows()


# Define the function to generate the video stream for Flask
def generate():
    global frameInit
    #while True:
    if frameInit is not None:
            ret, jpeg = cv2.imencode('.jpg', frameInit)
            frameInit = None
            yield (b'--frame\r\n'
                   b'Content-Type: image/jpeg\r\n\r\n' + jpeg.tobytes() + b'\r\n')

def generateCameraThread():
    print("Starting Camera")
    threading.Thread(target=process_stream).start()

if __name__ == '__main__':
    # Start the video stream processing in a separate thread

    # Start the Flask server in a separate thread
    #threading.Thread(target=app.run, kwargs={'host': '0.0.0.0', 'port': 5000, 'debug': False}).start()
    threading.Thread(target=receive_UDP()).start()
