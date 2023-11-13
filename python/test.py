import copy
import base64
import cv2
import numpy as np
import pygame
import time
import json
import php
import cv2
my = php.kit()

''''
img = cv2.imread("image.jpg")


x = 100
y = 100

w = 250
h = 150

crop_img = img[10:120, 0:50] # h w
pixels = np.zeros((h,w,3),np.uint8)
pixels[0:110,0:50]=crop_img

cv2.imshow("cropped", pixels)
cv2.waitKey(0)
my.exit()
'''
pygame.init()
clock = pygame.time.Clock()
watching = True
WIDTH=450
HEIGHT=450
w=450
h=422
screen = pygame.display.set_mode((WIDTH, HEIGHT), pygame.HWSURFACE | pygame.DOUBLEBUF | pygame.RESIZABLE)

img = cv2.imread('image.jpg')




img_str = img 
#.tostring()

'''
img_str = base64.b64encode(img_str).decode("ascii")
c = {"a":img_str}
c = json.dumps(c)
c = json.loads(c)
img_str = base64.b64decode(c["a"])
'''

#img_str = str(img_str)
#img_str = bin(img_str) 

img_np = np.fromstring(img_str, dtype='uint8').reshape( h, w, 3 )
#img_np = cv2.imdecode(img_np, cv2.IMREAD_COLOR )

pixels = np.zeros((h,w,3),np.uint8)
#print(dir(img_np))
#height, width, cha = img_np.shape
#my.exit()
#pixels[0:h,0:w]=img_np
#img_np = cv2.cvtColor(img_np, cv2.COLOR_RGB2BGR)
#height, width, channels = img_np.shape
#print("height: %d" ,(height))
#print("width: %d" ,(width))

#pixels = np.zeros((h,w,3),np.uint8)



pixels[0:422,0:450] = img_np

size = width, height = w,h
img = pygame.image.fromstring(img_np.tobytes(), size, "RGB")
screen.blit(pygame.transform.scale(img, (w, h)), (0, 0))


while watching:
  pygame.display.flip()
  time.sleep(0.5)