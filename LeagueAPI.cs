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

                clientSocket = new TcpClient();
                clientSocket.Connect(remoteEP);
                NetworkStream stream = clientSocket.GetStream();

                sWriter = new StreamWriter(stream);
                sReader = new StreamReader(stream);

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
                if (!isConnected)
                    InitConnection();

                // "Wake Up" connection by writing empty string
                // TODO why is this necessary?
                sWriter.Write("");
                sWriter.Flush();
                string pswd = sReader.ReadLine();
                if(!pswd.Equals("Connected. Enter Password."))
                    return false;
                // Send password to endpoint.
                sWriter.WriteLine("PotatoLatkesAreTheBest");
                sWriter.Flush();

                // Read response
                string response = sReader.ReadLine();
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
                string result = "";
                while (result.Equals("") && result != null)
                {
                    result = sReader.ReadLine();
                }

                if (result.Equals(""))
                    return null;
                else if (result.Contains("connection offline"))
                {
                    throw new ConnectionOfflineException(result);
                }
                else
                    return result;
            }
            catch (ConnectionOfflineException coe)
            {
                // Rethrow exception to be handled in UI
                throw;
            }
            catch (Exception e)
            {
                isConnected = false;
                return null;
            }
        }
    }

    /// <summary>
    /// A class of exceptions indicating the server status
    /// is offline and therefore no meaningful data is returned
    /// </summary>
    public class ConnectionOfflineException : Exception
    {
        public ConnectionOfflineException()
        {

        }

        public ConnectionOfflineException(string message)
            : base(message)
        {
        }

        public ConnectionOfflineException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
