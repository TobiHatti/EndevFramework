using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreSR
{
    public class S2CControllerCH
    {
        private static readonly Socket ClientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private const int PORT = 2226;

        public void Start()
        {
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            int attempts = 0;

            while (!ClientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection attempt " + attempts);
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    ClientSocket.Connect(IPAddress.Loopback, PORT);
                }
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");
        }

        public void ReveiveCycle()
        {
            ReceiveResponse();
            SendString("okcool");
        }

        private void SendString(string text)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(text);
            ClientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);
        }

        private bool ReceiveResponse()
        {
            var buffer = new byte[10000];
            int received = ClientSocket.Receive(buffer, SocketFlags.None);
            if (received == 0) return false;
            var data = new byte[received];
            Array.Copy(buffer, data, received);
            string text = Encoding.ASCII.GetString(data);

            return text == "RldWOk1TNHhJRkl6LElTVjpNUzR3LFBVSzpQRkpUUVV0bGVWWmhiSFZsUGp4TmIyUjFiSFZ6UGpsSFozTkpUbVZMTHpGUlpucHpNR0ZWWnl0WlRFSXdORTlPZWt3cmFXTkpUbFo0Y0ROb1lYWk5WSE5KYlZKclptSkJaemRIZDNwaVZUVXdlV3RqVVZkbVRuWjJPRGRRUm1GbFpXdFlhSFIxYTFweU5FZ3dNSEpyTTA5YVNuRllaMmRUTDFGNmQwMTZWMjFtS3k5NVZVUkVPRnAxVmxSUlVUUTFSV0pySzNCaVMwdG1kMDF0SzB4MlVXazRVVFpRYUVGRFozZHpjMlp3Y1dKamQwNWhiSFF2VEZsUlJIWmlia3hGVlQwOEwwMXZaSFZzZFhNK1BFVjRjRzl1Wlc1MFBrRlJRVUk4TDBWNGNHOXVaVzUwUGp3dlVsTkJTMlY1Vm1Gc2RXVSssVVNSOlZHOWlhV0Z6LFBTVzoxODctOTItMjQwLTEyOC0xNTMtMjQwLTExNi0yLTg2LTkyLTUtMjA4LTEyNi0xMDAtMTM1LTE0Ni0xMzMtNzQtMTUxLTE1OC0xNjQtMTMxLTE3NC02MC0xODYtMjIzLTIwMC02LTAtMjIzLTcxLTIzMC0xNTktODEtOTEtMjEwLTc3LTE2NC05MS0yMjgtMTEtMTMyLTE5LTIzLTk4LTEyNi0yNTEtNzktNTItMjAtODgtMTAyLTE1LTE2MS0yMDUtMjA0LTE0Ny0xNy00NC0yNC0yNi0xOC0yLTExNS01OC0xOTgtODAtOTEtMjE3LTExMS0yNDMtNzUtMjI1LTIzNS0xNTEtMjA2LTI0Ni0yNi0xMDAtNjItMTI3LTIxMi0yMC0yMDctMTgyLTE0LTYzLTI1NC0yMzYtMTI1LTIyMC0xMzQtMTA3LTE1LTc5LTE2My00MS05OC01My0xNDYtMTgzLTE0NS0xMDMtMTI2LTIzMC05LTI1MS01MS0yOC0yMjItMTA0LTExNC0yMjQtMjQ2LTE5Ny02MC0xNTEtMTQzLTE5OS0yNS01Ni01OC0xNjItMjE0LTQxLTM5LTE1NC0zNSxTR1A6TkRVM1pUbGlaR1V0TldVMU15MDBZV0U0TFRrM01EY3RZMlZtTVdGbE5UQXhOR05tLFNHQzoyMTQtMTUzLTEyNi0xNDUtMjA5LTUtMTM5LTE4Ny05OC0xMTEtMjI1LTcwLTEyNi0yMTYtMTMyLTgzLTI0Ni0xMzEtODktMzYtMTcwLTg5LTU1LTEyNy05OS0yMC0xMzgtMTAtMjIyLTE1Ny0yMjAtMTAzLTIyMi0zMy0xOTgtOTctODgtMjMzLTIyOC0xNzctMjI1LTE2Ni0xMzgtMTYtMTIyLTI0NS0xNzQtNzktMTQtMTIxLTE4Mi0xNi0xNzctMTQzLTE4NS0yMzktMjMyLTE1OC0yNTAtNzEtMTk3LTE0My0xNzYtMjAxLTk1LTI4LTExNC0xNTEtMjAwLTE3Ni00OS0zMC0xMDktMjIzLTIwMC0xMDctMjEyLTEwMi0zMS0xMDEtMTgxLTc3LTIzMC0xNDAtMTI5LTE5NS0xMTctMTI3LTE3OC0xMzMtMjQxLTExMS0yLTEzOC03Ni0xMTEtMjM3LTEwMy0xMjAtMTcwLTIwMC01Mi0xNzgtMzYtNDctMjAyLTIwMC0yMjMtMTg3LTE4OC05Mi0zNS03My0xOTktMTcyLTIxMy0xNTItNjEtMjQwLTIyNC0zNC01LTIwNy0xNzAtMTIyLTY5LTEwMC0xMDgsSU5TOlJXNWtaWFpHVjA1M2RFTnZjbVV1U1c1emRISjFZM1JwYjI1TWFXSnlZWEo1UlhOelpXNTBhV0ZzY3l0TmVWTjBZV0pwYkdsMGVWUmxjM1FzSUVWdVpHVjJSbGRPZDNSRGIzSmxMQ0JXWlhKemFXOXVQVEV1TUM0d0xqQXNJRU4xYkhSMWNtVTlibVYxZEhKaGJDd2dVSFZpYkdsalMyVjVWRzlyWlc0OWJuVnNiQT09LA==";
        }
    }
}
