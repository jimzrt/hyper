using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace hyper.Inputs
{
    internal class InputManager
    {
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        private static List<IInput> Inputs = new List<IInput>();

        public static event ConsoleCancelEventHandler CancelKeyPress;

        public static void Interrupt()
        {
            Inputs.ForEach(Input => Input.Interrupt());
        }

        public static void AddInput(IInput Input)
        {
            Input.SetResetEvent(resetEvent);
            Inputs.Add(Input);

            // Input.RegisterEventHandler(CancelKeyPress);
            Input.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender, args);
        }

        public static string ReadAny()
        {
            var message = "";

            //foreach (var Input in Inputs)
            //{
            //    if (Input.Available())
            //    {
            //        message = Input.Read();
            //        return message;
            //    }
            //}

            while (!Inputs.Any(i => i.Available()))
                resetEvent.WaitOne();

            foreach (var Input in Inputs)
            {
                if (Input.Available())
                {
                    message = Input.Read();
                    break;
                }
            }

            //   Inputs.ForEach(Input => Input.CanRead = false);
            resetEvent.Reset();
            return message;
        }
    }
}