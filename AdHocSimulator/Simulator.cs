using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace grapeot.AdHocSimulator
{
    public class Simulator
    {
        /// <summary>
        /// Gets or sets the max network speed, in the unit of byte per second.
        /// </summary>
        /// <value>The max network speed.</value>
        public long MaxNetworkSpeed { get; set; }

        /// <summary>
        /// Gets or sets the simulation interval, in the unit of millisecond.
        /// </summary>
        /// <value>The simulation interval.</value>
        public long SimulationInterval { get; set; }

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
        List<Tuple<int, int>> adjacentList = new List<Tuple<int, int>>();

        /// <summary>
        /// Gets the adjacent list, which shows how the devices can reach each other.
        /// </summary>
        /// <value>The adjacent list.</value>
        public List<Tuple<int, int>> AdjacentList { get { return adjacentList; } }

        /// <summary>
        /// Connects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Connect(int id1, int id2)
        { }

        /// <summary>
        /// Connects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Connect(int[] ids1, int[] ids2)
        { }

        /// <summary>
        /// Disconnects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Disconnect(int id1, int id2)
        { }

        /// <summary>
        /// Disconnects the specified devices.
        /// </summary>
        /// <param name="id1">The id1.</param>
        /// <param name="id2">The id2.</param>
        public void Disconnect(int[] ids1, int[] ids2)
        { }
        #endregion

        #region Device APIs
        List<Device> devices;
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
        public int Register(Device d, int[] connectedIds)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Make specified device leave the network.
        /// </summary>
        /// <param name="d">The d.</param>
        public void Leave(Device d)
        { }
        #endregion
    }
}
