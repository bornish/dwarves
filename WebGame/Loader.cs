﻿
using Bridge.Html5;
using WebGame.PixiJs;

namespace WebGame
{
    public class Loader
    {

        public static void Main(string[] args)
        {
            //Window.Alert("version 20");
            var game = new JsGame();
            game.InitAll();
        }
    }
}
