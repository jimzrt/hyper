using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace hyper.Inputs
{
    public class InputManager
    {
        // private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        private List<IInput> inputs = new List<IInput>();

        public event ConsoleCancelEventHandler CancelKeyPress;

        private BlockingCollection<string> ownQueue = new BlockingCollection<string>();

        //private static Queue<string> ownQueue = new Queue<string>();
        private readonly object _syncObj = new object();

        public void Interrupt()
        {
            inputs.ForEach(Input => Input.Interrupt());
        }

        public void AddInput(IInput input)
        {
            // Input.SetResetEvent(resetEvent);
            input.SetQueue(ownQueue);
            inputs.Add(input);

            // Input.RegisterEventHandler(CancelKeyPress);
            input.CancelKeyPress += (sender, args) => CancelKeyPress?.Invoke(sender, args);
        }

        public string ReadAny()
        {
            //var message = "";

            //foreach (var Input in Inputs)
            //{
            //    if (Input.Available())
            //    {
            //        message = Input.Read();
            //        return message;
            //    }
            //}
            var message = ownQueue.Take();
            //while (ownQueue.Count == 0)
            //    resetEvent.WaitOne();

            //if (ownQueue.Count > 0)
            //{
            //    lock (_syncObj)
            //    {
            //        message = ownQueue.Dequeue();
            //    }
            //    resetEvent.Reset();
            //    return message;
            //}

            ////foreach (var Input in Inputs)
            ////{
            ////    if (Input.Available())
            ////    {
            ////        message = Input.Read();
            ////        break;
            ////    }
            ////}

            ////   Inputs.ForEach(Input => Input.CanRead = false);
            //resetEvent.Reset();
            return message;
        }

        internal void InjectCommand(string command)
        {
            lock (_syncObj)
                ownQueue.Add(command);
            //  resetEvent.Set();
        }
    }
}