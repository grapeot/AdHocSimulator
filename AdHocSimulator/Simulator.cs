﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace grapeot.AdHocSimulator
{
    public class Simulator
    {
        /// <summary>
        /// Gets or sets the max network speed (per SimulationInterval).
        /// </summary>
        /// <value>The max network speed.</value>
        public long MaxNetworkSpeed { get; set; }

        /// <summary>
        /// Gets or sets the simulation interval, in the unit of millisecond.
        /// </summary>
        /// <value>The simulation interval.</value>
        public long SimulationInterval { get; set; }

        /// <summary>
        /// Indicate the next available ID.
        /// </summary>
        int nextId = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Simulator"/> class
        /// with the default MaxNetworkSpeed.
        /// </summary>
        public Simulator()
        {
            MaxNetworkSpeed = 10 << 10;
            SimulationInterval = 500;
        }

        #region Adjacency APIs
        List<int> activeIds = new List<int>();
        Dictionary<int, List<int>> adjacentList = new Dictionary<int, List<int>>();

        /// <summary>
        /// Gets the adjacent list, which shows how the devices can reach each other.
        /// </summary>
        /// <value>The adjacent list.</value>
        public Dictionary<int, List<int>> AdjacentList { get { return adjacentList; } }

        /// <summary>
        /// Connects the specified devices. Note we assume all the connections are symmetric.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Connect(int id1, int id2)
        {
            AdjacentList[id1].Add(id2);
            AdjacentList[id2].Add(id1);
        }

        /// <summary>
        /// Connects the specified devices. We assume all the connections are symmetric.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Connect(int[] ids1, int[] ids2)
        {
            if (ids1.Length != ids2.Length)
                throw new Exception("The two id arrays are expected to have the same length.");
            for (int i = 0; i < ids2.Length; i++)
                Connect(ids1[i], ids2[i]);
        }

        /// <summary>
        /// Disconnects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Disconnect(int id1, int id2)
        {
            if (AdjacentList[id1].Contains(id2) && AdjacentList[id2].Contains(id1))
            {
                AdjacentList[id1].Remove(id2);
                AdjacentList[id2].Remove(id1);
            }
            else
            {
                throw new Exception("The specified devices are not connected.");
            }
        }

        /// <summary>
        /// Disconnects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Disconnect(int[] ids1, int[] ids2)
        {
            if (ids1.Length != ids2.Length)
                throw new Exception("The two id arrays are expected to have the same length.");
            for (int i = 0; i < ids2.Length; i++)
                Disconnect(ids1[i], ids2[i]);
        }
        #endregion

        #region Device APIs
        List<Device> devices = new List<Device>();

        /// <summary>
        /// Gets the device list.
        /// </summary>
        /// <value>The devices.</value>
        public List<Device> Devices { get { return devices; } }

        /// <summary>
        /// Registers the specified device in the network.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="connectedIds">The connected ids.</param>
        /// <returns>Assigned ID to the device</returns>
        public void Register(Device d, int[] connectedIds)
        {
            devices.Add(d);
            activeIds.Add(nextId);
            d.ID = nextId;
            d.Simulator = this;
            AdjacentList.Add(nextId, new List<int>());
            if (connectedIds != null)
                foreach (var id in connectedIds)
                    Connect(nextId, id);
            ++nextId;
        }

        /// <summary>
        /// Make specified device leave the network.
        /// </summary>
        /// <param name="d">The d.</param>
        public void Leave(Device d)
        {
            devices.Remove(d);
            activeIds.Remove(d.ID);
            AdjacentList.Remove(d.ID);  //Directly disconnect all the neighbors.
            // note the connections are symmetric. Do another round.
            foreach (var key in AdjacentList.Keys)
                if (AdjacentList[key].Contains(d.ID))
                    AdjacentList[key].Remove(d.ID);
        }

        /// <summary>
        /// Gets the nearby devices.
        /// </summary>
        /// <returns></returns>
        public int[] GetNearbyDevices(int id)
        {
            return AdjacentList[id].ToArray();
        }
        #endregion

        #region Data APIs
        Dictionary<int, Queue<Queue<byte>>> dataToSend = new Dictionary<int, Queue<Queue<byte>>>();
        Dictionary<int, Queue<Action>> callbacks = new Dictionary<int, Queue<Action>>();
        Dictionary<int, Queue<Device>> sourceDevices = new Dictionary<int, Queue<Device>>();

        /// <summary>
        /// A set for the devices receiving data (i.e. having timer running).
        /// </summary>
        HashSet<int> sendingIds = new HashSet<int>();

        /// <summary>
        /// Sends the specified data from a device to another.
        /// </summary>
        /// <param name="from">The device from which the data is sent.</param>
        /// <param name="to">The target device</param>
        /// <param name="data">The data.</param>
        /// <remarks>
        /// This function also force the network speed limit. Currently the limit is put
        /// in the capability of receiving data for every device.
        /// </remarks>
        public void Send(Device from, Device to, byte[] data, Action callback, EventHandler<DataReceivedEventArgs> dataReceivedTrigger)
        {
            // check whether the two devices are connected.
            if (!AdjacentList[from.ID].Contains(to.ID))
                throw new Exception("The two devices are not connected.");

            // prepare for the buffer,callback queue, and sourceDevice queue
            if (!dataToSend.ContainsKey(to.ID))
                dataToSend.Add(to.ID, new Queue<Queue<byte>>());
            dataToSend[to.ID].Enqueue(new Queue<byte>(data));
            if (!callbacks.ContainsKey(to.ID))
                callbacks.Add(to.ID, new Queue<Action>());
            callbacks[to.ID].Enqueue(callback);
            if (!sourceDevices.ContainsKey(to.ID))
                sourceDevices.Add(to.ID, new Queue<Device>());
            sourceDevices[to.ID].Enqueue(from);

            // Set a time to trigger the data received events.
            if (sendingIds.Contains(to.ID)) return;
            sendingIds.Add(to.ID);
            var timer = new System.Timers.Timer(this.SimulationInterval) { AutoReset = true };
            timer.Elapsed += (sender, e) =>
            {
                // use closure to access the target device
                var queue = dataToSend[to.ID].Peek();
                var buffer = new byte[Math.Min(this.MaxNetworkSpeed, queue.Count)];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = queue.Dequeue();
                // trigger the event
                var sourceDevice = sourceDevices[to.ID].Peek();
                dataReceivedTrigger(to, new DataReceivedEventArgs() { SourceDevice = sourceDevice, Data = buffer });
                // dispose the timer when all the data is sent.
                if (queue.Count == 0)
                {
                    if (dataToSend[to.ID].Count == 0)
                    {
                        timer.Dispose();
                        sendingIds.Remove(to.ID);
                    }
                    sourceDevices[to.ID].Dequeue();
                    var toCallBack = callbacks[to.ID].Dequeue();
                    if (toCallBack != null) toCallBack();
                    dataToSend[to.ID].Dequeue();
                }
            };
            timer.Enabled = true;
        }
        #endregion
    }
}
