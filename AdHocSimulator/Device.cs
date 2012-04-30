using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grapeot.AdHocSimulator
{
    /// <summary>
    /// Class for a mobile device in the Ad-Hoc network.
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the optional name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Occurs when there is data received. Note due to the network limitation, the data may be incomplete,
        /// therefore it's Devices' responsibility to do buffer stuffs.
        /// </summary>
        /// <remarks>
        /// Considering the device can only know its neighbor devices, Simulator class shouldn't be invoked in
        /// the event handler.
        /// </remarks>
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        #region Encapsulation of Simulator APIs
        /// <summary>
        /// Gets the nearby devices.
        /// </summary>
        /// <value>The nearby devices.</value>
        public Device[] NearbyDevices { get; }

        /// <summary>
        /// Sends the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="callback">The callback which will be invoked when data sending is finished.</param>
        public void Send(byte[] data, Action callback = null)
        {
            if (callback != null)
                callback();
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for Device.DataReceived.
    /// </summary>
    public class DataReceivedEventArgs: EventArgs
    {
        public byte[] Data { get; set; }
    }
}
