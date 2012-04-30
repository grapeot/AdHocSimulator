using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grapeot.AdHocSimulator
{
    class Test
    {
        static void Main(string[] args)
        {
            var simulator = new Simulator() { MaxNetworkSpeed = 5 << 10, SimulationInterval = 500 };

            // initialize two devices and register them to the network
            var d1 = new Device() { Name = "D1" };
            var d2 = new Device() { Name = "D2" };
            simulator.Register(d1, null);
            simulator.Register(d2, new int[] { d1.ID });

            // define their responses to the received data.
            var defaultReceivedHandler = new EventHandler<DataReceivedEventArgs>((sender, e) =>
            {
                var device = sender as Device;
                Console.WriteLine("Data received from {0} to {1}.", e.FromDevice.Name, device.Name);
            });
            d1.DataReceived += defaultReceivedHandler;
            d2.DataReceived += defaultReceivedHandler;

            // test send a large data
            d1.Send(d2, new byte[20 << 10], () => { Console.WriteLine("Data sent complete from D1 to D2"); });
        }
    }
}
