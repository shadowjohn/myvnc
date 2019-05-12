# -*- coding: utf-8 -*-
import socket
from zlib import decompress
import pygame
import numpy as np
import cv2
import php
import base64
import ctypes
import sys


user32 = ctypes.windll.user32
screen_width=user32.GetSystemMetrics(0)
screen_height=user32.GetSystemMetrics(1)
my = php.kit()
is_first_time = 1 
w = WIDTH = screen_width
h = HEIGHT = screen_height
info_data = ""

def recvall(conn, length):
    """ Retreive all pixels. """
    buf = b''
    while len(buf) < length:
        data = conn.recv(length - len(buf))
        if not data:
            return data
        buf += data
    return buf

def from_bytes( b, byteorder='big'):
    # type: (bytes, str) -> Int
    val = bytes_(memoryview(b).tobytes())
    byteorder = byteorder.lower()
    ch = '>' if byteorder.startswith('b') else '<'
    hi = struct.unpack(ch + 'B'*len(val), val)
    xo = 0 + sum(n**(i+1) for i, n in enumerate(hi))
    return xo
def setGet(screenSize):
    global w
    global h
    #print(screenSize)
    w = screenSize[0]
    h = screenSize[1] 
    print(w)
    print(h)
    #w = screenSize.width
    #h = screenSize.height 
    pygame.display.set_mode(screenSize, pygame.HWSURFACE | pygame.DOUBLEBUF | pygame.RESIZABLE)
def main(host='localhost', port=5000):
    global w
    global h
    global info_data
    global is_first_time
    pygame.init()
    screen = pygame.display.set_mode((WIDTH, HEIGHT), pygame.HWSURFACE | pygame.DOUBLEBUF | pygame.RESIZABLE)
    clock = pygame.time.Clock()
    watching = True    

    sock = socket.socket()
    sock.setsockopt(socket.IPPROTO_TCP, socket.TCP_NODELAY, 1)
    sock.connect((host, port))
    
    first_pixel = 0
    pixels = ""
    
    address = ('0.0.0.0', 5001)
    s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    s.bind(address)
    
    try:
        while watching:
            for event in pygame.event.get():
                if event.type == pygame.VIDEORESIZE:
                  setGet(event.size)
                if event.type == pygame.QUIT:
                  watching = False
                  
                elif event.type == pygame.KEYDOWN:
                  print("KEYDOWN %s : %s " % (chr(event.key),event.key))
                  
                elif event.type == pygame.KEYUP:
                  print("KEYUP %s : %s " % (chr(event.key),event.key))
                  
                elif event.type == pygame.MOUSEBUTTONDOWN:
                  pos = pygame.mouse.get_pos()
                  if event.button == 1:
                    print("Mouse Down 1")
                  if event.button == 2:
                    print("Mouse Down 2")
                  if event.button == 3:
                    print("Mouse Down 3")  
                  
                elif event.type == pygame.MOUSEBUTTONUP:
                  pos = pygame.mouse.get_pos()
                  if event.button == 1:
                    print("Mouse Up 1")
                  if event.button == 2:
                    print("Mouse Up 2")
                  if event.button == 3:
                    print("Mouse Up 3")  
                  
                  
            #data, addr = s.recvfrom(2048)
            
            #print("received: from: %s", (addr))
            
            # Retreive the size of the pixels length, the pixels length and pixels
            size_len = int.from_bytes(sock.recv(1), byteorder='big')
            
            size = int.from_bytes(sock.recv(size_len), byteorder='big')
            
            #print(size)
            #my.exit();
            
            data = recvall(sock, size)            
            #data = udp_server
            
            if is_first_time == 1:              
              m = my.explode(b"WTF_____WTF",data)
              info = m[0]            
              #imgs = my.explode(b"WTF|||||WTF",m[1])              
              info_data = my.json_decode(info.decode("utf-8"))
              is_first_time = 0              
              continue
            
            
            if first_pixel == 0:             
              pixels = np.zeros((info_data["info"]["server_height"],info_data["info"]["server_width"],3), np.uint8)
              first_pixel=1
              
            m_send_data = my.explode(b"MERGE|||MERGE",data)
            #print(data)
            for i in range(0,len(m_send_data)):
              cut_img = m_send_data[i]            
            size = width, height = info_data["info"]["server_width"],info_data["info"]["server_height"]
            img = pygame.image.fromstring(cut_img, size, "RGB")
            
            # Display the picture                        
            screen.blit(pygame.transform.scale(img, (w, h)), (0, 0))
            pygame.display.update()
            #pygame.display.flip()
            #clock.tick(5)
    finally:
        sock.close()


if __name__ == '__main__':
    main()