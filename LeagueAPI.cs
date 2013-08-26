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
    class LeagueAPI : IDisposable
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
                string ip = EloCheck.Properties.Settings.Default.IP;
                int port = EloCheck.Properties.Settings.Default.Port;
                IPAddress address = IPAddress.Parse(ip);
                IPEndPoint remoteEP = new IPEndPoint(address, port);

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
                string pswdPrompt = sReader.ReadLine();
                if(!pswdPrompt.Equals("Connected. Enter Password."))
                    return false;
                // Send password to endpoint.
                string pswd = EloCheck.Properties.Settings.Default.Password;
                sWriter.WriteLine(pswd);
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

                if (result != null && result.Equals(""))
                    throw new BaseGameLookupException("{\"error\": \"No such game found.\"}");
                else if (result.Contains("connection offline"))
                    throw new ConnectionOfflineException(result);
                else if (result.Contains("not in game or active game"))
                    throw new BaseGameLookupException(result);
                else if (result.Contains("Summoner") && result.Contains("not found"))
                    throw new BaseGameLookupException(result);
                else
                    return result;
            }
            catch (BaseGameLookupException)
            {
                // Rethrow exception to be handled in UI
                throw;
            }
            catch (Exception e)
            {
                isConnected = false;
                throw new ClientDisconnectedException(e.Message, e);
            }
        }

        public virtual void Dispose()
        {
            sWriter.Dispose();
            sReader.Dispose();
            clientSocket.Close();
        }
    }

    /// <summary>
    /// A class of exceptions indicating the server status
    /// is offline and therefore no meaningful data is returned
    /// </summary>
    [Serializable]
    public class ConnectionOfflineException : BaseGameLookupException
    {
        public ConnectionOfflineException()
        { }
        public ConnectionOfflineException(string message)
            : base(message)
        {}
        public ConnectionOfflineException(string message, Exception inner)
            : base(message, inner)
        {}
    }

    [Serializable]
    public class ClientDisconnectedException : BaseGameLookupException
    {
        public ClientDisconnectedException()
        { }
        public ClientDisconnectedException(string message)
            : base(message)
        { }
        public ClientDisconnectedException(string message, Exception inner)
        { }
    }
}
