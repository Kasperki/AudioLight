# AudioLight

Turns Audio to light.

## How it works.

C# Client uses NAudio (Windows Audio Session API wrapper) it captures audio from your soundcard as stereomix and turns into json formatted rgb light with clumsy algorithm :).
Node Server listens SerialPort and Websocket for connection.
When NodeServer has no connection it will cycle leds in idle state.
If it has connection it will show the colors based on the json data.
AudioLight C# client first tries to connect with USB, if it's unable then tries to connect with Websocket.

# Installation prequistes

You need rasperry pi, with NodeJS & npm.
Windows vista or later.

## rasperry

PIGPIO library if your rasperry don't have it and pigpio npm installation fails. 
```
wget abyz.co.uk/rpi/pigpio/pigpio.zip
unzip pigpio.zip
cd PIGPIO
make
sudo make install
```

```
cd AudioLightServer
npm install
```

Red led to pin GPIO 4,
Green led to pin GPIO 17,
Blue led to pin GPIO 27;

run node Server.js

## windows
If you don't use SerialPort (USB) then you most propably need to modify the IP address of you pi.
Just run the program and it will try to send the data to your rasperry pi.

Commandline run arguments:

-p: SerialPort (Default:COM3)

-i: IP address (Default:http://myiplocalnetworkipv4:8080)

