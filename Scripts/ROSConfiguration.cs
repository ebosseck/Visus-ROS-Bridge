using UnityEngine;

namespace Visus.Robotics.RosBridge
{
    public class ROSConfiguration: MonoBehaviour
    {

        #region Network Configuration

        [HideInInspector]
        public string IPAddress = "127.0.0.1";

        [HideInInspector]
        public bool useTCP = true;
        
        [HideInInspector]
        public int TCPPort = 10000;
        
        [HideInInspector]
        public bool connectOnStartup;
        
        #endregion

        #region Logging Configuration

        [HideInInspector] 
        public bool autoForwardLogs = true;

        [HideInInspector] 
        public string defaultComponentName;

        public int minimumForwardLevel = 0;
        
        #endregion
        
        #region Status

        public static string[] client_states = { "Disconnected", "Connecting...", "Connected" };
        public static Color[] client_state_colors = {Color.red, Color.yellow, Color.green};
        
        [HideInInspector]
        public int TCPClientState = 0;

        #endregion
        
        private void Awake()
        {
            // Self-Destruct in case the Configuration is instantiated.
            // This is intentional to avoid inconsistent duplicates.
            // DO NOT REMOVE
            Debug.LogError("DO NOT ADD THE 'ROSConfiguration' DIRECTLY IN YOUR SCENE !. " +
                           "Please configure the ROS Bridge using the Configuration Window.");
            Destroy(this.gameObject);
        }
    }
}