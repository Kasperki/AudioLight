var http = require('http');

var Gpio = require('pigpio').Gpio,
  ledR = new Gpio(4, {mode: Gpio.OUTPUT}),
  ledG = new Gpio(17, {mode: Gpio.OUTPUT}),
  ledB = new Gpio(27, {mode: Gpio.OUTPUT});

var server = http.createServer(function(request, response) {
    console.log((new Date()) + ' Received request for ' + request.url);
    response.writeHead(404);
    response.end();
});
server.listen(8080, function() {
    console.log((new Date()) + ' Server is listening on port 8080');
});

var io = require('socket.io')(server);

io.on('connection', function(socket) {
    console.log("someone connected!");

    socket.on('colorData', function(data) {
        var color = JSON.parse(data);
        console.log("r:" + color.r + " g:" + color.g + " b:" + color.b);
        ledR.pwmWrite(color.r);
        ledG.pwmWrite(color.g);
        ledB.pwmWrite(color.b);
    });
});
