﻿//#define UI_DEBUG

using System;
using System.Diagnostics;
using System.Windows.Forms;

using Tanji.Dialogs;
using Tanji.Managers;
using Tanji.Applications;

using Sulakore.Habbo;
using Sulakore.Habbo.Web;
using Sulakore.Communication;

namespace Tanji
{
    public partial class MainFrm : Form
    {
        public bool IsRetro { get; set; }

        public HFlash Game { get; set; }
        public HGameData GameData { get; set; }

        public HConnection Connection { get; }
        public EncoderManager EncoderMngr { get; }
        public HandshakeManager HandshakeMngr { get; }
        public ExtensionManager ExtensionMngr { get; }
        public InjectionManager InjectionMngr { get; }

        public UpdateFrm UpdateUI { get; }
        public ConnectFrm ConnectUI { get; }
        public PacketLoggerFrm PacketLoggerUI { get; }

        public MainFrm()
        {
            InitializeComponent();

            Connection = new HConnection();
            UpdateUI = new UpdateFrm(this);
            ConnectUI = new ConnectFrm(this);

            Shown += MainFrm_Shown;
#if !UI_DEBUG
            Load += MainFrm_Load;

            Connection.Connected += Connected;
            Connection.Disconnected += Disconnected;
#endif

            // Data Receive Order - #1 | Notify Extensions
            ExtensionMngr = new ExtensionManager(this);
            // Data Receive Order - #2 | Process Handshake
            HandshakeMngr = new HandshakeManager(this);
            // Data Receive Order - #3 | Display Data
            PacketLoggerUI = new PacketLoggerFrm(this);

            EncoderMngr = new EncoderManager(this);
            InjectionMngr = new InjectionManager(this);
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            ConnectUI.ShowDialog();

            if (!Connection.IsConnected) Close();
            else Text = $"Tanji ~ Connected[{Connection.Host}:{Connection.Port}]";
        }
        private void MainFrm_Shown(object sender, EventArgs e)
        {
            PacketLoggerUI.Show();
            BringToFront();
        }

        private void Connected(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(ConnectUI.Close));
        }
        private void Disconnected(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(Close));
        }

        private void TanjiInfoTxt_Click(object sender, EventArgs e)
        {
            Process.Start("https://GitHub.com/ArachisH/Tanji");
        }
        private void TanjiVersionTxt_Click(object sender, EventArgs e)
        {
            Process.Start(UpdateUI.CurrentRelease.HtmlUrl);
        }
    }
}