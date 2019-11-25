using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace hyper.Inputs
{
    class InputManager
    {
        static List<IInput> Inputs = new List<IInput>();

        public static event ConsoleCancelEventHandler CancelKeyPress;

        public static void AddInput(IInput Input)
        {
            Inputs.Add(Input);
            // Input.RegisterEventHandler(CancelKeyPress);
            Input.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender,args);
        }

        public static string ReadAny()
        {
            var message = "";
            while (true)
            {
                var found = false;
                // activate
                Inputs.ForEach(Input => Input.CanRead = true);

                foreach (var Input in Inputs)
                {
                    if (Input.Available())
                    {
                        found = true;
                        message = Input.Read();
                        break;
                    }
                }
                if (found)
                {
                    Inputs.ForEach(Input => Input.CanRead = false);
                    return message;
                }
            }
        }

    }
}
