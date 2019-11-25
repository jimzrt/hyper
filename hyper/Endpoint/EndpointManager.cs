using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hyper.Endpoints
{
    class EndpointManager
    {
        static List<IEndpoint> endpoints = new List<IEndpoint>();

        public static event ConsoleCancelEventHandler CancelKeyPress;

        public static void AddEndpoint(IEndpoint endpoint)
        {
            endpoints.Add(endpoint);
            // endpoint.RegisterEventHandler(CancelKeyPress);
            endpoint.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender,args);
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

    }
}
