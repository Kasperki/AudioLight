# AudioLight
Captures audio from windows audio input and turns into light to rasperry pi.

# Installation prequistes

You need rasperry pi, with node & npm.
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
npm install
```

Red led to pin 4,
Green led to pin 17,
Blue led to pin 27;

run node Server.js

## windows sound input
You might need to modify the connection ip address: CONNECTION_URL;
Just run the program and it will send the data to your rasperry pi.