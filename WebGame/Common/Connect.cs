using WebGame.Common.Connection;

namespace WebGame.Common
{
    abstract class Connect
    {
        public Game reception;
        protected bool isRegister;
        protected string login = "test";

        public abstract void SendData(RequestPlayerAction action, string param1, string param2);


    }
}
