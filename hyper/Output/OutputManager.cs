using System;
using System.Collections.Generic;
using System.Text;

namespace hyper.Output
{
    class OutputManager
    {
        public static List<IOutput> Outputs = new List<IOutput>();

        public static void AddOutput(IOutput output)
        {
            Outputs.Add(output);
        }


        public static void HandleCommand(object command, byte srcNodeId, byte destNodeId)
        {
            foreach (var output in Outputs)
            {
                output.HandleCommand(command, srcNodeId, destNodeId);
            }
        }
    }
}
