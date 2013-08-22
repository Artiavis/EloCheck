using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace EloCheck
{
    /// <summary>
    ///   This class provides a socket connection to the
    ///   remote server.
    /// </summary
    class LeagueAPI
    {
        private TcpClient clientSocket;
        private StreamWriter sWriter;
        private StreamReader sReader;

        private bool isConnected;
        private String identity;

        IOException exception = null;

        public LeagueAPI(String identity)
        {
            this.identity = identity;
            isConnected = false;
        }

        /// <summary>
        ///     Open API connection and authenticate.
        /// </summary>
        private void InitConnection()
        {
            try
            {
                // TODO move the binding information over into a config file
                IPAddress address = IPAddress.Parse("192.227.135.167");
                IPEndPoint remoteEP = new IPEndPoint(address, 10807);

                clientSocket = new TcpClient(remoteEP);
                NetworkStream stream = clientSocket.GetStream();

                sWriter = new StreamWriter(stream);
                sReader = new StreamReader(stream);

                isConnected = Connect();
            }
            catch (IOException e)
            {
                exception = e;
            }
        }

        /// <summary>
        ///     Check to see if the client is connected.
        ///     Will return false if not.
        /// </summary>
        public bool Connect()
        {
            try
            {
                // Open connections 
                InitConnection();
                // Send token to endpoint.
                sWriter.Write("PotatoLatkesAreTheBest");
                //  streams are bidirectional so don't
                // forget to flush.
                sWriter.Flush();

                // Read response
                string response = sReader.ReadToEnd();

                if (response.Equals("Authenticated"))
                    return true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
            return false;
        }

        /// <summary>
        ///     Sends game information request.
        /// </summary>
        /// <param name="func">A string describing an operation</param>
        /// <param name="name">A string representing a name</param>
        /// <param name="region">A string representing the region</param>
        public string ApiRequest(string func, string name, string region)
        {
            try
            {
                sWriter.WriteLine("lol " + func + " " + region + " " + name);
                sWriter.Flush();
                string result = sReader.ReadToEnd();
                if (result.Equals(""))
                    return null;
                else
                    return result;
            }
            catch (Exception e)
            {
                isConnected = false;
                return null;
            }
        }
    }
}
