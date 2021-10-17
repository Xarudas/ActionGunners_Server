﻿using UnityEngine;
using System.Collections;
using System;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.Threading;
using DarkRift.Server;
using Zenject;
using DarkRift.Server.Unity;

namespace MeatInc.ActionGunnersServer.Network.Components.ServerManagment
{
    public class ServerManager : MonoBehaviour
    {
        public event Action CreatedServer;
        public event Action ClosedServer;

#pragma warning disable IDE0044 // Add readonly modifier, Unity can't serialize readonly fields
        [SerializeField]
        [Tooltip("The configuration file to use.")]
        private TextAsset configuration;

        [SerializeField]
        [Tooltip("Indicates whether the server will be created in the OnEnable method.")]
        private bool createOnEnable = true;

        [SerializeField]
        [Tooltip("Indicates whether the server events will be routed through the dispatcher or just invoked.")]
        private bool eventsFromDispatcher = true;
#pragma warning restore IDE0044 // Add readonly modifier, Unity can't serialize readonly fields

        private ServerInfo _serverInfo;

        [Inject]
        public void Construct(ServerInfo serverInfo)
        {
            _serverInfo = serverInfo;
        }

        private void OnEnable()
        {
            //If createOnEnable is selected create a server
            if (createOnEnable)
                Create();
        }

        private void Update()
        {
            //Execute all queued dispatcher tasks
            if (_serverInfo.Server != null)
                _serverInfo.Server.ExecuteDispatcherTasks();
        }

        /// <summary>
        ///     Creates the server.
        /// </summary>
        public void Create()
        {
            Create(new NameValueCollection());
        }

        /// <summary>
        ///     Creates the server.
        /// </summary>
        public void Create(NameValueCollection variables)
        {
            if (_serverInfo.Server != null)
                throw new InvalidOperationException("The server has already been created! (Is CreateOnEnable enabled?)");
            
            if (configuration != null)
            {
                // Create spawn data from config
                ServerSpawnData spawnData = ServerSpawnData.CreateFromXml(XDocument.Parse(configuration.text), variables);

                // Allow only this thread to execute dispatcher tasks to enable deadlock protection
                spawnData.DispatcherExecutorThreadID = Thread.CurrentThread.ManagedThreadId;

                // Inaccessible from XML, set from inspector
                spawnData.EventsFromDispatcher = eventsFromDispatcher;

                // Unity is broken, work around it...
                // This is an obsolete property but is still used if the user is using obsolete <server> tag properties
#pragma warning disable 0618
                spawnData.Server.UseFallbackNetworking = true;
#pragma warning restore 0618

                // Add types
                spawnData.PluginSearch.PluginTypes.AddRange(UnityServerHelper.SearchForPlugins());
                spawnData.PluginSearch.PluginTypes.Add(typeof(UnityConsoleWriter));

                // Create server
                var server = new DarkRiftServer(spawnData);
                server.Start();
                _serverInfo.ReplaceServer(server);
                CreatedServer?.Invoke();
            }
            else
                Debug.LogError("No configuration file specified!");
        }

        private void OnDisable()
        {
            Close();
        }

        private void OnApplicationQuit()
        {
            Close();
        }

        /// <summary>
        ///     Closes the server.
        /// </summary>
        public void Close()
        {
            if (_serverInfo.Server != null)
            {
                _serverInfo.Server.Dispose();
                ClosedServer?.Invoke();
            }
                
        }
    }
}
