﻿using System;
using System.Diagnostics;
using System.Collections.Generic;

using Tanji.Pages;
using Tanji.Components;
using Tanji.Pages.About;
using Tanji.Applications;
using Tanji.Pages.Toolbox;
using Tanji.Pages.Injection;
using Tanji.Pages.Extensions;
using Tanji.Pages.Connection;

using Sulakore.Communication;

namespace Tanji
{
    public partial class MainFrm : TanjiForm, IDataManager
    {
        private readonly IList<IDataHandler> _dataHandlers;
        private readonly IList<IDataHandler> _toRemoveList;

        public HConnection Connection { get; }

        public AboutPage AboutPg { get; }
        public ToolboxPage ToolboxPg { get; }
        public InjectionPage InjectionPg { get; }
        public ExtensionsPage ExtensionsPg { get; }
        public ConnectionPage ConnectionPg { get; }

        public PacketLoggerFrm PacketLoggerUI { get; }

        public MainFrm()
        {
            InitializeComponent();

            _toRemoveList = new List<IDataHandler>();
            _dataHandlers = new List<IDataHandler>();

            Connection = new HConnection();
            Connection.DataOutgoing += DataOutgoing;
            Connection.DataIncoming += DataIncoming;

            AboutPg = new AboutPage(this, AboutTab);
            ToolboxPg = new ToolboxPage(this, ToolboxTab);
            InjectionPg = new InjectionPage(this, InjectionTab);
            ExtensionsPg = new ExtensionsPage(this, ExtensionsTab);
            ConnectionPg = new ConnectionPage(this, ConnectionTab);

            PacketLoggerUI = new PacketLoggerFrm(this);

            AttachDataHandlers();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            ConnectionPg.CreateTrustedRootCertificate();
        }
        private void TanjiInfoTxt_Click(object sender, EventArgs e)
        {
            Process.Start("https://GitHub.com/ArachisH/Tanji");
        }
        private void TanjiVersionTxt_Click(object sender, EventArgs e)
        {
            if (AboutPg.TanjiRepo.LatestRelease != null)
                Process.Start(AboutPg.TanjiRepo.LatestRelease.HtmlUrl);
        }

        public void AttachDataHandlers()
        {
            _toRemoveList.Clear();
            _dataHandlers.Clear();

            _dataHandlers.Add(ExtensionsPg);
            _dataHandlers.Add(InjectionPg.FiltersPg);
            // TODO: Implement toolbox functionality, unsure if
            // we'll need block/replacing capabilities.
            //_dataHandlers.Add(ToolboxPg);
            _dataHandlers.Add(ConnectionPg.HandshakeMngr);
            _dataHandlers.Add(PacketLoggerUI);
        }
        protected void ProcessRemoveQueue()
        {
            if (_toRemoveList.Count < 1) return;
            lock (_dataHandlers)
            {
                foreach (IDataHandler dataHandler in _toRemoveList)
                {
                    if (_dataHandlers.Contains(dataHandler))
                        _dataHandlers.Remove(dataHandler);
                }
                _toRemoveList.Clear();
            }
        }

        public void AddDataHandler(IDataHandler dataHandler)
        {
            lock (_dataHandlers)
            {
                if (!_dataHandlers.Contains(dataHandler))
                    _dataHandlers.Add(dataHandler);
            }
        }
        public void RemoveDataHandler(IDataHandler dataHandler)
        {
            lock (_dataHandlers)
            {
                if (_dataHandlers.Contains(dataHandler) &&
                    !_toRemoveList.Contains(dataHandler))
                {
                    _toRemoveList.Add(dataHandler);
                }
            }
        }

        private void DataOutgoing(object sender, InterceptedEventArgs e)
        {
            ProcessRemoveQueue();
            foreach (IDataHandler dataHandler in _dataHandlers)
            {
                if (dataHandler.IsHandlingOutgoing)
                    dataHandler.HandleOutgoing(e);
            }
        }
        private void DataIncoming(object sender, InterceptedEventArgs e)
        {
            ProcessRemoveQueue();
            foreach (IDataHandler dataHandler in _dataHandlers)
            {
                if (dataHandler.IsHandlingIncoming)
                    dataHandler.HandleIncoming(e);
            }
        }
    }
}