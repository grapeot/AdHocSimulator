using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
        /// Gets or sets the simulator associated with the device.
        /// </summary>
        /// <value>The simulator.</value>
        public Simulator Simulator { get; set; }

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
        public int[] NearbyDevices { get { return Simulator.GetNearbyDevices(this.ID); } }

        /// <summary>
        /// Sends the specified data to the target device.
        /// </summary>
        /// <param name="target">The target device.</param>
        /// <param name="data">The data.</param>
        /// <param name="callback">The callback which will be invoked when data sending is finished.</param>
        public void Send(Device target, byte[] data, bool isBlocked = false, Action callback = null)
        {
            if (isBlocked)
            {
                var e = new AutoResetEvent(false);
                Simulator.Send(this, target, data, () => { if (callback != null) callback(); e.Set(); }, TriggerDataReceivedEvent);
                e.WaitOne();
            }
            else
            {
                Simulator.Send(this, target, data, callback, TriggerDataReceivedEvent);
            }
        }

        /// <summary>
        /// Triggers the data received event, for the simulator callback use.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="grapeot.AdHocSimulator.DataReceivedEventArgs"/> instance containing the event data.</param>
        void TriggerDataReceivedEvent(object sender, DataReceivedEventArgs e)
        {
            if (DataReceived != null)
                DataReceived(sender, e);
        }
        #endregion
    }

    /// <summary>
    /// Event arguments for Device.DataReceived.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the device from which the data is sent.
        /// </summary>
        /// <value>From device.</value>
        public Device SourceDevice { get; set; }
    }
}
