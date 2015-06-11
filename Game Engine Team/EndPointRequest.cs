using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game_Engine_Team
{
    /// <summary>
    /// A helper class which makes web requests to the server and returns the text/json from the server.
    /// </summary>
    /// <author>Carl Kuang</author>
    /// <editor>Morgan Wynne</editor>
    internal class EndPointRequest
    {
        /// <summary>
        /// Build the http request with the specified method.
        /// </summary>
        /// <param name="endpoint">The endpoint trying to reach</param>
        /// <param name="method">Request method</param>
        /// <param name="authToken">User's Authentication token</param>
        /// <returns>HTTP request to an endpoint</returns>
        public static HttpWebRequest BuildRequest( string endpoint, string method, string authToken = "" )
        {
            // Creates the initial HTTP request object to the server
            HttpWebRequest httpWebRequest = WebRequest.CreateHttp( endpoint );

            // Sets properties of request
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = method;

            if ( !String.IsNullOrEmpty( authToken ) )
                httpWebRequest.Headers.Add( "X-Auth-Token", authToken );

            return httpWebRequest;
        }

        /// <summary>
        /// Runs the entire generic HTTP Post logic and returns the Json response from the server
        /// </summary>
        /// <param name="endpoint">Server endpoint URL</param>
        /// <param name="json">Json data to write to server</param>
        /// <param name="authToken">User's Authentication token</param>
        /// <returns>Json result string</returns>
        public static string HttpRequest( string endpoint, string json = "", string authToken = "" )
        {
            // Build the HTTP GET request to the server endpoint where POST and GET are dependant on json
            HttpWebRequest httpWebRequest = BuildRequest( endpoint, 
                                                (String.IsNullOrEmpty( json ) ? "GET" : "POST"), 
                                                authToken );

            // If Json string is specified, write Json to the server
            if ( !String.IsNullOrEmpty( json ) )
                using ( var streamWriter = new StreamWriter( httpWebRequest.GetRequestStream() ) )
                    streamWriter.Write( json );

            // Reads back the response
            WebResponse httpResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
            using ( var streamReader = new StreamReader( httpResponse.GetResponseStream() ) )
                return streamReader.ReadToEnd();
        }

    }
}