# -*- coding: utf-8 -*-
import socket
from threading import Thread
from zlib import compress
import binascii
from mss import mss
from PIL import Image
import numpy as np
#import grab_screen
import numpy
import sys
#import pygame
import win32gui, win32ui, win32con, win32api
import cv2
import time
import php
import ctypes
import hashlib
import copy
import base64
import bson
import json
import io

def md5(data):
  m = hashlib.md5()
  m.update(data)
  h = m.hexdigest()
  return h
def imgEncodeDecode(in_imgs, ch, quality=5):
  # https://qiita.com/ka10ryu1/items/5fed6b4c8f29163d0d65  
  encode_param = [int(cv2.IMWRITE_JPEG_QUALITY), quality]  
  result, encimg = cv2.imencode('.jpg', in_imgs, encode_param)
  if False == result:
      print('could not encode image!')
      exit()
  decimg = cv2.imdecode(encimg, ch)

  return decimg
my = php.kit()

WIDTH = 1024
HEIGHT = 768
hwin = win32gui.GetDesktopWindow()
hwindc = win32gui.GetWindowDC(hwin)
srcdc = win32ui.CreateDCFromHandle(hwindc)
memdc = srcdc.CreateCompatibleDC()
bmp = win32ui.CreateBitmap()
play_one = 0;
grab_width = ""
grab_height = ""
grab_left = ""
grab_top = ""
is_first_time = 1
merge_send = []
max_merge = 3
is_new = True
encode_param = [int(cv2.IMWRITE_PNG_COMPRESSION ), 3]
client_server_addr = ""
address = ""
udp_server = ""
sct = mss()
def grab_screen(region=None):
    global hwin
    global hwindc
    global srcdc
    global memdc
    global bmp
    global play_one
    global grab_width
    global grab_height
    global grab_left
    global grab_top
    global sct
    #if region:
    #        left,top,x2,y2 = region
    #        width = x2 - left + 1
    #        height = y2 - top + 1
    #else:
    
    if play_one==0:
      grab_width = win32api.GetSystemMetrics(win32con.SM_CXVIRTUALSCREEN)
      grab_height = win32api.GetSystemMetrics(win32con.SM_CYVIRTUALSCREEN)
      grab_left = win32api.GetSystemMetrics(win32con.SM_XVIRTUALSCREEN)
      grab_top = win32api.GetSystemMetrics(win32con.SM_YVIRTUALSCREEN)
      bmp.CreateCompatibleBitmap(srcdc, grab_width, grab_height)
      memdc.SelectObject(bmp)
      play_one = 1
    '''
    memdc.BitBlt((0, 0), (grab_width, grab_height), srcdc, (grab_left, grab_top), win32con.SRCCOPY)
    #print(width)
    #print(height)
    signedIntsArray = bmp.GetBitmapBits(True)
    img = np.fromstring(signedIntsArray, dtype='uint8')
    img.shape = (grab_height,grab_width,4)
    #img.shape = (HEIGHT,WIDTH,4)

    #srcdc.DeleteDC()
    #memdc.DeleteDC()
    #win32gui.ReleaseDC(hwin, hwindc)
    #win32gui.DeleteObject(bmp.GetHandle())
    
    '''
    monitor = {"top": grab_top, "left": grab_left, "width": grab_width, "height": grab_height}
    
    sct_img = sct.grab(monitor)
    img = numpy.array(sct_img)
    
    return cv2.cvtColor(img, cv2.COLOR_BGRA2RGB)
    #return img[:,:,::-1] 



def to_bytes(self, length, byteorder='big'):
    try:
        ch = '>' if byteorder.startswith('b') else '<'
        hex_str = '%x' % int(self)
        n = len(hex_str)
        x = binascii.unhexlify(hex_str.zfill(n + (n & 1)))
        val = x[-1 * length:] if ch == '>' else x[:length]
    except OverflowError:
        raise ValueError(self)
    return val
def send_data(conn,bytes_size_len,size_bytes,pixels):
  
  #udp_server.sendto(pixels, address)
  conn.send(bytes_size_len)                  
  conn.send(size_bytes) 
  conn.sendall(pixels)
         
def retreive_screenshot(conn):
    #with mss() as sct:
        # The region to capture
        global is_first_time
        global is_new
        global encode_param
        
        user32 = ctypes.windll.user32
        screen_width=user32.GetSystemMetrics(0)
        screen_height=user32.GetSystemMetrics(1)
        #screen_width=1024
        #screen_height=768
        cut_width_counts = 1 
        cut_height_counts = 2
        one_width = int(screen_width / cut_width_counts)
        one_height = int(screen_height / cut_height_counts) 
        #rect = {'top': 0, 'left': 0, 'width': WIDTH, 'height': HEIGHT}
        
        orin_img_md5 = []            
        data = {
          "info":{            
            "server_width":screen_width,
            "server_height":screen_height,
            "cuts_x":cut_width_counts,
            "cuts_y":cut_height_counts,            
            "one_width":one_width,
            "one_height":one_height
          }        
        }
        #print(one_width)
        #print(one_height)
        for i in range(0,max_merge):
          merge_send.append("")
        for i in range(0,cut_width_counts*cut_height_counts):
          
          #data["img_data"].append("")
          orin_img_md5.append("")
        merge_step = 0
        while 'recording':
            # Capture the screen
            #sct_img = sct.grab(sct.monitors[0])
            # Tweak the compression level here (0-9)
            #img = img.resize((320,240),Image.ANTIALIAS)
            #img.rgb.resize((320,240),Image.ANTIALIAS)
            
            #img = Image.frombytes('RGB', sct_img.size, sct_img.rgb)
            #img = img.resize((WIDTH, HEIGHT), resample=Image.LANCZOS)
            #img = ""
            #try:
            img = grab_screen()
            #img = cv2.imencode('.jpeg', img,encode_param)[1]
            #nparr = np.fromstring(cut_img, np.uint8)
            #img = cv2.imdecode(img, cv2.IMREAD_COLOR)
            #img = imgEncodeDecode(img,cv2.IMREAD_COLOR, 85)
            #except:
            #  continue        
            #img = img.resize((WIDTH, HEIGHT), resample=Image.LANCZOS)
            #img = cv2.resize(img, (data["info"]["new_w"],data["info"]["new_h"]),cv2.INTER_NEAREST )
            
            #is_new = False
            #if len(orin_img_data) == 0:
            #   is_new = True
            step=0
            
            if is_first_time == 1:
              binary_stream = io.BytesIO()            
              binary_stream.write(json.dumps(data).encode('ascii'))
  
              binary_stream.write("WTF_____WTF".encode("ascii"))
              binary_stream.seek(0)
              pixels = binary_stream.read()
              
                            
              size = len(pixels)
              size_len = (size.bit_length() + 7) // 8
              bytes_size_len = bytes([size_len])
              size_bytes = to_bytes(size,size_len,'big') 
              send_data(conn,bytes_size_len,size_bytes,pixels)                            
              is_first_time = 0
              continue
            
            
            step = 0
            for x in range(0,cut_width_counts):
              for y in range(0,cut_height_counts):
                
                crop_img = img[one_height*y:one_height*(y+1), one_width*x:one_width*(x+1)]                
                #crop_img = cv2.resize(crop_img, (one_width,one_height),cv2.INTER_NEAREST )
                crop_imgb = crop_img.tobytes()                
                #IMREAD_COLOR                                
                #crop_imgb = cv2.imencode('.png', crop_img,encode_param)[1].tostring() #, encode_param)[1].tostring()                
                if is_new == True:           
                  #print("OK3")       
                  #data["img_data"][step]=crop_img
                  #pixels=pixels+str(crop_imgb)                                  
                  #binary_stream.write(  crop_img.tostring() )
                  #binary_stream.write("WTF|||||WTF".encode("ascii"))
                  
                  orin_img_md5[step]=crop_imgb[:-10]
                  
                  xy = "%d,%d" % (x , y)
                  merge_send[merge_step] = xy.encode("ascii")
                  merge_send[merge_step] = merge_send[merge_step] + "WTF|||||WTF".encode("ascii")
                  merge_send[merge_step] = merge_send[merge_step] + crop_imgb
                  merge_step=merge_step+1                 
                  #binary_stream.seek(0)
                  #binary_stream.write(xy.encode("ascii"))
                  #binary_stream.write("WTF|||||WTF".encode("ascii"))
                  #binary_stream.write(crop_imgb)
                  #binary_stream.seek(0)                  
                  #pixels = binary_stream.read()
                  #size = len(pixels)
                  #size_len = (size.bit_length() + 7) // 8
                  #bytes_size_len = bytes([size_len])
                        
                  #size_bytes = to_bytes(size,size_len,'big')
                  #conn.send(bytes([size_len]))                  
                  #conn.send(size_bytes) 
                  #conn.sendall(pixels)   
                  #send_data(conn,bytes_size_len,size_bytes,pixels)
                else:
                  if crop_imgb[:-10] != orin_img_md5[step]:
                    #print("OK2")
                    #data["img_data"][step]=crop_img
                    #pixels=pixels+str(crop_imgb)
                    #binary_stream.write( crop_img.tostring() )
                    #binary_stream.write("WTF|||||WTF".encode("ascii"))
                    
                    #orin_img_md5[step]=md5(crop_imgb)
                    xy = "%d,%d" % (x , y)
                    merge_send[merge_step] = xy.encode("ascii")
                    merge_send[merge_step] = merge_send[merge_step] + "WTF|||||WTF".encode("ascii")
                    merge_send[merge_step] = merge_send[merge_step] + crop_imgb      
                    merge_step = merge_step + 1
                    orin_img_md5[step]=crop_imgb[:-10]
                    #pixels = binary_stream.read()
                    #size = len(pixels)
                    #size_len = (size.bit_length() + 7) // 8
                    #bytes_size_len = bytes([size_len])
                          
                    #size_bytes = to_bytes(size,size_len,'big')
                    #conn.send(bytes([size_len]))                  
                    #conn.send(size_bytes) 
                    #conn.sendall(pixels)   
                    #send_data(conn,bytes_size_len,size_bytes,pixels)
                    
                  else:
                    #print("OK1")
                    xy = "%d,%d" % (x , y)
                    merge_send[merge_step] = xy.encode("ascii")
                    merge_send[merge_step] = merge_send[merge_step] + "WTF|||||WTF".encode("ascii")
                    merge_step = merge_step + 1
                    #data["img_data"][step]=""
                    #pixels=pixels+""  
                    #binary_stream.write("WTF|||||WTF".encode("ascii"))          
                  step=step+1
                if merge_step >= max_merge:
                  merge_step=0
                  binary_stream = io.BytesIO()
                  for i in range(0,max_merge):                    
                    binary_stream.write( merge_send[i]  )
                    if i != max_merge-1:
                      binary_stream.write( b"MERGE|||MERGE"  )
                  binary_stream.seek(0)                  
                  pixels = binary_stream.read()
                  size = len(pixels)
                  size_len = (size.bit_length() + 7) // 8
                  bytes_size_len = bytes([size_len])                        
                  size_bytes = to_bytes(size,size_len,'big')
                  #conn.send(bytes([size_len]))                  
                  #conn.send(size_bytes) 
                  #conn.sendall(pixels)   
                  send_data(conn,bytes_size_len,size_bytes,pixels)
            is_new = False  
                
        
#             binary_stream.seek(0)
#             pixels = binary_stream.read() #pixels.encode('utf-8')
#             # Send the size of the pixels length
#             size = len(pixels)
#             size_len = (size.bit_length() + 7) // 8
#             conn.send(bytes([size_len]))
# 
#             # Send the actual pixels length
#             #size_bytes = size.to_bytes(size_len, 'big')
#             size_bytes = to_bytes(size,size_len,'big')
#             #print(size_bytes)
#             conn.send(size_bytes)
# 
#             # Send pixels
#             conn.sendall(pixels)
#            time.sleep(0.017)


def main(host='0.0.0.0', port=5000):
    global is_first_time
    global client_server_addr
    global address
    global udp_server
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    sock.setsockopt(socket.SOL_TCP, socket.TCP_NODELAY, 1)    
    sock.bind((host, port))
    #address = ('0.0.0.0', 5000)
    #s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    #s.bind(address)
    
    try:
      sock.listen(50)
      print('Server started.')        
      while 'connected':
        conn, addr = sock.accept()
        is_first_time = 1
        print('Client connected IP:', addr)
        #client_server_addr = addr
        thread = Thread(target=retreive_screenshot, args=(conn,))
        thread.start()
        #if address =="":
        #  address = (client_server_addr[0], 5001)
        #  udp_server = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)        
        #udp_server.sendto("WTF".encode("ascii"), address)
        
    finally:
        
      sock.close()


if __name__ == '__main__':
    main(host='0.0.0.0',port=5000)