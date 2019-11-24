using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hyper.Endpoints
{
    class EndpointManager
    {
        static List<IEndpoint> endpoints = new List<IEndpoint>();

        public static void AddEndpoint(IEndpoint endpoint)
        {
            endpoints.Add(endpoint);
        }

        public static string ReadAny()
        {
            var message = "";
            while (true)
            {
                var found = false;
                // activate
                endpoints.ForEach(endpoint => endpoint.CanRead = true);

                foreach (var endpoint in endpoints)
                {
                    if (endpoint.Available())
                    {
                        found = true;
                        message = endpoint.Read();
                        break;
                    }
                }
                if (found)
                {
                    endpoints.ForEach(endpoint => endpoint.CanRead = false);
                    return message;
                }
            }
        }

        internal static void AddCancelEventHandler(Action<object, ConsoleCancelEventArgs> cancelHandler)
        {
            foreach (var endpoint in endpoints)
            {
                endpoint.AddCancelEventHandler(cancelHandler);
            }
        }
    }
}
