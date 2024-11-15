using UnityEngine;

namespace Visus.Robotics.RosBridge
{
    public class ROSUpdater: MonoBehaviour
    {
        private ROSConnection _connection;
        
        private void Awake()
        {
            _connection = ROSConnection.GetOrCreateInstance();
        }

        private void FixedUpdate()
        {
            _connection.Update();
        }

        private void OnApplicationQuit()
        {
            _connection.Disconnect();
        }
    }
}