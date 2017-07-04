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


$(document).on('keypress', function (e) {
    
    if (e.which === 97)
        graphics.position.x = graphics.position.x - 10;
    else if (e.which === 100)
        graphics.position.x = graphics.position.x + 10;
    else if (e.which === 119)
        graphics.position.y = graphics.position.y - 10;
    else if (e.which === 115)
        graphics.position.y = graphics.position.y + 10;
     
});
