using System;
using System.Collections.Generic;
using UnityEngine;

using Riptide;
using Riptide.Transports.Tcp;
using Riptide.Utils;
using LogType = UnityEngine.LogType;
using Object = UnityEngine.Object;

namespace Visus.Robotics.RosBridge
{
    [System.Serializable]
    public class ROSConnection
    {
        private const int DEFAULT_PUBLISHER_QUEUE = 10;
        private const bool DEFAULT_PUBLISHER_LATCH = false;
        
        #region Singleton

        private static ROSConnection __instance = null;
        
        public static ROSConnection GetOrCreateInstance()
        {
            if (__instance == null)
            {
                //config = Resources.Load<ROSConfiguration>("_ros_configuration");
                //ROSConfiguration config = go.GetComponent<ROSConfiguration>();
                __instance = new ROSConnection(Resources.Load<ROSConfiguration>("_ros_configuration"));
                __instance.startUpdater();
            }

            return __instance;
        }

        #endregion
        
        private ROSConfiguration config;

        private ROSNetClient tcpClient;

        private GameObject updater;

        #region Connect Waiting

        private List<Message> message_queue = new List<Message>();

        #endregion
        
        #region Subscriptions & Publishers

        private Dictionary<string, List<Action<ROSMessage>>> subscriptions;
        private Dictionary<string, Type> subscriptionTypes;
        private Dictionary<string, int> subscriberCount;

        private Dictionary<string, string> publishers;
        private Dictionary<string, int> publisherCount;

        #endregion
        
        #region Logging

        private Dictionary<UnityEngine.LogType, int> loglevels = new Dictionary<LogType, int>()
        {
            { LogType.Log, 2 },
            { LogType.Warning, 4},
            { LogType.Error, 8},
            { LogType.Exception, 8},
            { LogType.Assert, 16},
        };

        #endregion
        
        
        #region Startup & Initialisation
        private ROSConnection(ROSConfiguration config)
        {
            this.config = config;

            this.subscriptions = new Dictionary<string, List<Action<ROSMessage>>>();
            this.subscriptionTypes = new Dictionary<string, Type>();
            this.subscriberCount = new Dictionary<string, int>();
            
            this.publishers = new Dictionary<string, string>();
            this.publisherCount = new Dictionary<string, int>();
            
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            
            TcpClient tcpClientTransport = new TcpClient();
            
            tcpClient = new ROSNetClient(tcpClientTransport);
            
            if (config.connectOnStartup)
            {
                Connect();
            }

            if (config.autoForwardLogs)
            {
                Application.logMessageReceived += LogCallback;
            }

        }

        private void startUpdater()
        {
            ROSUpdater[] updaters = Object.FindObjectsOfType<ROSUpdater>();

            if (updaters.Length > 0)
            {
                foreach (ROSUpdater up in updaters)
                {
                    Object.Destroy(up);
                }
            }

            updater = new GameObject("ROS Client Updater");
            updater.AddComponent<ROSUpdater>();
            if (Application.IsPlaying(updater))
            {
                Object.DontDestroyOnLoad(updater);
            }
        }
        
        private void LogCallback(string condition, string stackTrace, UnityEngine.LogType type)
        {
            

            // ignore normal logs
            //if (logTypeApp != LogTypeApp.Normal)
            //{
            //    LogData(type + " - condition: " + condition + ", stacktrace: " + stackTrace, logTypeApp);
            //}
        }
        
        #endregion
        
        #region Connect & Disconnect

        public void Connect()
        {
            if (config.useTCP)
            {
                tcpClient.MessageReceived += delegate(object sender, MessageReceivedEventArgs args)
                {
                    handleMessage(args.FromConnection, args.Message, args.MessageId);
                };

                config.TCPClientState = 1;
                
                tcpClient.Connected += delegate(object sender, EventArgs args)
                {
                    config.TCPClientState = 2;

                    foreach (Message msg in message_queue)
                    {
                        tcpClient.Send(msg);
                    }
                    
                };
                
                tcpClient.Disconnected += delegate(object sender, DisconnectedEventArgs args)
                {
                    config.TCPClientState = 0;
                };

                tcpClient.Connect(String.Format("{0}:{1}", config.IPAddress, config.TCPPort));
            }
        }
        
        public void Disconnect()
        {
            if (config.useTCP)
            {
                if (tcpClient != null && tcpClient.IsConnected)
                {
                    tcpClient.Disconnect();
                }
            }
        }

        #endregion

        #region Tools

        public void log(string origin, byte lvl,
            string name, string msg_content, string file, string function, int line, string[] topics)
        {
            int secs = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            int nsecs = (DateTimeOffset.Now.Millisecond * 1000);
            
            _sendRawMessage(BridgeMessageFactory.unifiedLogMessage(secs, nsecs, origin, lvl, name, msg_content, file, 
                function, line, topics));
        }

        #endregion
        
        #region Message Handling

        private void handleMessage(Connection fromConnection, Message msg, ushort msgID)
        {
            switch (msgID)
            {  
                case 0:
                    // Log Message
                    break;
                case 1:
                    // Parameter List Response
                    break;
                case 2:
                    // Parameter Response
                    break;
                case 4:
                    // Topic List Response
                    break;
                case 5:
                    // Message Structure
                    break;
                case 6:
                    // Subscription Ack
                    string sub_topic;
                    int status;
                    string sub_err_message;
                    
                    BridgeMessageUnpacker.unpackSubscriptionAck(msg, out sub_topic, out status, out sub_err_message);

                    if (status != 0)
                    {
                        handleSubscriptionFailed(sub_topic, sub_err_message);
                    }
                    break;
                case 7:
                    // Unsubscription Ack
                    break;
                case 9:
                    // ROS Message
                    string msg_topic;
                    ROSMessage ros_message;
                    BridgeMessageUnpacker.unpackROSMessage(msg, out msg_topic, out ros_message);
                    
                    callSubscribers(msg_topic, ros_message);
                    break;
                case 11:
                    // Call Service
                    break;
                case 12:
                    // Service Response
                    break;
                case 13:
                    // Text Broadcast
                    break;
                default:
                    Debug.Log("Got unhandled message with ID: " + msgID);
                    break;
            }
            
            msg.Release();
        }

        private void handleSubscriptionFailed(string topic, string error)
        {
            Debug.Log(string.Format("Failed to subscribe to '{0}': {1}", topic, error));
        }

        private void callSubscribers(string topic, ROSMessage message)
        {
            foreach (Action<ROSMessage> action in subscriptions[topic])
            {
                try
                {
                    action(message);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        #endregion
        
        #region Functions

        private void _sendRawMessage(Message message)
        {
            if (config.useTCP)
            {
                if (tcpClient.IsConnected)
                {
                    tcpClient.Send(message, false);
                    message.Release();
                }
                else
                {
                    Debug.Log("Client not yet connected !");
                    message_queue.Add(message);
                }
            }
        }

        private void _sendRawMessageTCP(Message message)
        {
            if (config.useTCP)
            {
                tcpClient.Send(message);
                return;
            }
            Debug.LogWarning("TCP is not enabled !");
        }
        
        private void _sendRawMessageUDP(Message message)
        {
            Debug.LogWarning("UDP is not enabled !");
        }

        #endregion
        
        #region Publisher & Subscriber API

        public void Subscribe<T>(string topic, Action<T> callback) where T: ROSMessage
        {
            
            if (subscriptions.ContainsKey(topic))
            {
                if (typeof(T) != subscriptionTypes[topic])
                {
                    Debug.LogError(String.Format("Attempted to subscribe to topic {0} with message type {1}, " +
                                                 "but topic already exists using type {2}", 
                        topic, typeof(T), subscriptionTypes[topic]));
                    return;
                }
            }
            else
            {
                string type = ROSMessageFactory.mapROSType(typeof(T));
                subscriptions[topic] = new List<Action<ROSMessage>>();
                subscriptionTypes[topic] = typeof(T);
                subscriberCount[topic] = 0;
                _sendRawMessage(BridgeMessageFactory.requestSubscription(topic, type));
            }
            
            subscriptions[topic].Add(delegate(ROSMessage message)
            {
                callback((T)message);
            });
            subscriberCount[topic] += 1;
        }

        public void Unsubscribe(string topic)
        {
            subscriberCount[topic] -= 1;

            if (subscriberCount[topic] <= 0)
            {
                _sendRawMessage(BridgeMessageFactory.requestUnsubscription(topic));
            }
        }
        
        public void UnsubscribeAll(string topic)
        {
            subscriberCount[topic] = 0;
            _sendRawMessage(BridgeMessageFactory.requestUnsubscription(topic));
        }
        
        public void RegisterPublisher<T>(string topic, int? queue = null, bool? latch = null)
        {
            string type = ROSMessageFactory.mapROSType(typeof(T));
            RegisterPublisher(topic, type, queue, latch);
        }

        public void RegisterPublisher(string topic, string ros_type, int? queue = null, bool? latch = null)
        {
            if (publishers.ContainsKey(topic))
            {
                if (publishers[topic] != ros_type)
                {
                    Debug.LogError(String.Format("Attempted to publish to topic {0} with message type {1}, " +
                                                 "but topic already exists using type {2}", 
                        topic, ros_type, subscriptionTypes[topic]));
                }
            }
            else
            {
                publishers[topic] = ros_type;
                publisherCount[topic] = 0;
                _sendRawMessage(BridgeMessageFactory.createPublisher(topic, ros_type, queue.GetValueOrDefault(DEFAULT_PUBLISHER_QUEUE),
                    latch.GetValueOrDefault(DEFAULT_PUBLISHER_LATCH)));
            }
            
            publisherCount[topic] += 1;
            //TODO: Add option for subscribe listener 
        }

        public void Publish(string topic, ROSMessage message)
        {
            _sendRawMessage(BridgeMessageFactory.rosMessage(topic,message));
        }

        #endregion
        
        internal void Update()
        {
            tcpClient.Update();
        }
    }
}


