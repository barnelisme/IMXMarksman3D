import cv2

vid = cv2.VideoCapture(0)
fourcc = cv2.VideoWriter_fourcc(*'xVID')
out = cv2.VideoWriter('ResultVideo'+ str(1)+'.avi' , fourcc, 20.0, (640,480))

while True:

    ret , frame = vid.read()
    cv2.imshow('frame', frame)

    out.write(frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

vid.release()
cv2.destroyAllWindows()