var app = new PIXI.Application(800, 600, { backgroundColor: 0x1099bb });
$("#js-game").get(0).appendChild(app.view);

var graphics = new PIXI.Graphics();

// set a fill and line style
graphics.beginFill(0xFF3300);
graphics.lineStyle(4, 0xffd900, 1);

// draw a circle, set the lineStyle to zero so the circle doesn't have an outline
graphics.lineStyle(0);
graphics.beginFill(0xFFFF0B, 0.5);
graphics.drawCircle(470, 90, 60);
graphics.endFill();

app.stage.addChild(graphics);

//var socket = io.connect(window.location.host); //подключаем сокеты

//socket.on("restore", function (data) {
    //data = JSON.parse(data);
  //  graphics.position.x = 0;
//});

var connectId;

$(document).on('keypress', function (e) {
    
    if (e.which === 97)
        graphics.position.x = graphics.position.x - 10;
    else if (e.which === 100)
        graphics.position.x = graphics.position.x + 10;
    else if (e.which === 119)
        graphics.position.y = graphics.position.y - 10;
    else if (e.which === 115)
        graphics.position.y = graphics.position.y + 10;

    if (!socket || socket.readyState !== WebSocket.OPEN) {
        alert("socket not connected");
    }
    var context = { "header": { "connectionId": connectId}, value: "test"};
    var str = JSON.stringify(context);
    socket.send(str);
});




var socket;
var scheme = document.location.protocol === "https:" ? "wss" : "ws";
var port = document.location.port ? (":" + document.location.port) : "";
var url = scheme + "://" + document.location.hostname + port + "/ws";
function updateState() {
    function disable() {
        alert('disable');
    }
    function enable() {
        alert('enable');
    }
    //connectionUrl.disabled = true;
    //connectButton.disabled = true;
    
}


    socket = new WebSocket(url);
    socket.onopen = function (event) {
        //alert('open');
    };
    socket.onclose = function (event) {
        //alert('close');
    };
    socket.onerror = function (event) {
        //alert('error');
    };
    socket.onmessage = function (event) {
        //alert(event.data);
        var data = JSON.parse(event.data);
        if (data.Command === 4)
            connectId = data.Value;
        
    };
