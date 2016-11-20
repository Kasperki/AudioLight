var http = require('http');

//Initialize pins
var Gpio = require('pigpio').Gpio,
  ledR = new Gpio(4, {mode: Gpio.OUTPUT}),
  ledG = new Gpio(17, {mode: Gpio.OUTPUT}),
  ledB = new Gpio(27, {mode: Gpio.OUTPUT});

//For default cycle leds
var idleInterval = setInterval(function(){ cycleLeds(); }, 100);

//Websocket connection ***************************************************************

//InitWebsocketServer
var server = http.createServer(function(request, response) {
    console.log((new Date()) + ' Received request for ' + request.url);
    response.writeHead(404);
    response.end();
});
server.listen(8080, function() {
    console.log((new Date()) + ' Server is listening on port 8080');
});

var io = require('socket.io')(server);

//OnConnection
io.on('connection', function(socket) {
    console.log("someone connected! :)"); //TODO check that it is our program, if not kick it.

    //STOP IDLE INTERVAL
    idleInterval.stop();

    socket.on('colorData', function(data) {
        var color = JSON.parse(data); //TODO make method
        console.log("r:" + color.r + " g:" + color.g + " b:" + color.b);

        ledR.pwmWrite(color.r);
        ledG.pwmWrite(color.g);
        ledB.pwmWrite(color.b);
    });
});

//OnDisconnected
io.on('disconnected', function(socket) {
    io.of('/').clients(function(error, clients){
        if (error) throw error;
        if (clients.length == 0) {
            idleInterval = setInterval(function(){ cycleLeds(); }, 100);
        }
    });
});

//SerialPort connection ***************************************************************************

//SerialOnConnection
//SerialOnDisconnection

var r = 0, g = 0, b = 0, startTime;

var cycleLeds = function()
{
    var date = new Date();
    time = date.getTime() / 1000 - startTime;

    if (time < 6)
    {
        r = easeInQuad(time, 0, 255, 5);
    }
    else if (time < 12)
    {
        r = 255 - easeInQuad(time - 6, 0, 255, 4);
        b = easeInQuad(time - 6, 0, 255, 6);
    }
    else if (time < 18)
    {
        b = 255 - easeInQuad(time - 12, 0, 255, 4);
        g = easeInQuad(time - 12, 0, 255, 6);
    }
    else if (time < 24)
    {
        g = 255 - easeInQuad(time - 18, 0, 255, 2);
        r = easeInQuad(time - 18, 0, 255, 3);
    }
    else if (time < 30)
    {
        startTime = date.getTime() / 1000 - 5;
    }
    else
    {
        startTime = date.getTime() / 1000;
    }

    console.log("time:" + time + " r:" + r, " g:" + g + " b:" + b);

    r = parseInt(r); r = Math.max(0, r); r = Math.min(255, r);
    g = parseInt(g); g = Math.max(0, g); g = Math.min(255, g);
    b = parseInt(b); b = Math.max(0, b); b = Math.min(255, b);

    ledR.pwmWrite(r);
    ledG.pwmWrite(g);
    ledB.pwmWrite(b);
}


var easeInQuad = function (time, startVal, endVal, duration) {
    return endVal * (time /= duration) * time + startVal;
}