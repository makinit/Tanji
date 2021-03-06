﻿using System;
using System.Windows.Forms;

using Sulakore.Protocol;
using Sulakore.Components;

using Tanji.Manipulators;

namespace Tanji.Pages.Injection
{
    public class SchedulerPage : TanjiSubPage<InjectionPage>, IHaltable
    {
        private bool _suppressUIUpdating;

        private ushort _interval = 100;
        public ushort Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                RaiseOnPropertyChanged(nameof(Interval));
            }
        }

        private int _cycles = 0;
        public int Cycles
        {
            get { return _cycles; }
            set
            {
                _cycles = value;
                RaiseOnPropertyChanged(nameof(Cycles));
            }
        }

        private bool _autoStart = true;
        public bool AutoStart
        {
            get { return _autoStart; }
            set
            {
                _autoStart = value;
                RaiseOnPropertyChanged(nameof(AutoStart));
            }
        }

        private HDestination _destination = HDestination.Server;
        public HDestination Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                RaiseOnPropertyChanged(nameof(Destination));
            }
        }

        private SKoreHotkey.KeyCombination _hotkey = null;
        public SKoreHotkey.KeyCombination Hotkey
        {
            get { return _hotkey; }
            set
            {
                _hotkey = value;
                RaiseOnPropertyChanged(nameof(Hotkey));
            }
        }

        public SchedulerPage(InjectionPage parent, TabPage tab)
            : base(parent, tab)
        {
            UI.STDestinationTxt.DataSource = Enum.GetValues(typeof(HDestination));
            UI.STDestinationTxt.DataBindings.Add("SelectedItem", this,
                nameof(Destination), false, DataSourceUpdateMode.OnPropertyChanged);

            UI.STCyclesTxt.DataBindings.Add("Value", this,
                nameof(Cycles), false, DataSourceUpdateMode.OnPropertyChanged);

            UI.STIntervalTxt.DataBindings.Add("Value", this,
                nameof(Interval), false, DataSourceUpdateMode.OnPropertyChanged);

            UI.STAutoStartChckbx.DataBindings.Add("Checked", this,
                nameof(AutoStart), false, DataSourceUpdateMode.OnPropertyChanged);

            UI.STHotkeyTxt.DataBindings.Add("Value", this,
                nameof(Hotkey), true, DataSourceUpdateMode.OnPropertyChanged);

            UI.STClearBtn.Click += STClearBtn_Click;
            UI.STRemoveBtn.Click += STRemoveBtn_Click;
            UI.STCreateBtn.Click += STCreateBtn_Click;
            UI.STUpdateBtn.Click += STUpdateBtn_Click;

            UI.STSchedulerVw.ItemChecked += STSchedulerVw_ItemChecked;
            UI.STSchedulerVw.ScheduleTick += STSchedulerVw_ScheduleTick;
            UI.STSchedulerVw.ItemSelectionStateChanged += STSchedulerVw_ItemSelectionStateChanged;
        }

        private void STClearBtn_Click(object sender, EventArgs e)
        {
            UI.STSchedulerVw.ClearItems();
            UpdateUI();
        }
        private void STUpdateBtn_Click(object sender, EventArgs e)
        {
            HMessage packet = GetPacket();
            if (Parent.IsInjectionAuthorized(packet))
            {
                UI.STSchedulerVw.SelectedCycles = Cycles;
                UI.STSchedulerVw.SelectedInterval = Interval;
                UI.STSchedulerVw.SelectedDestination = Destination;
                UI.STSchedulerVw.SelectedHotkey = Hotkey;
                UI.STSchedulerVw.SelectedPacket = GetPacket().ToString();
            }
        }
        private void STRemoveBtn_Click(object sender, EventArgs e)
        {
            UI.STSchedulerVw.RemoveSelectedItem();
            UpdateUI();
        }
        private void STCreateBtn_Click(object sender, EventArgs e)
        {
            HMessage packet = GetPacket();
            if (!Parent.IsInjectionAuthorized(packet)) return;

            packet.Destination = Destination;
            UI.STSchedulerVw.AddSchedule(packet, Interval, Cycles, Hotkey, AutoStart);
        }

        private void STSchedulerVw_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            UpdateUI();
        }
        private void STSchedulerVw_ScheduleTick(object sender, ScheduleTickEventArgs e)
        {
            if (Parent.SendAsync(e.Packet).Result < 6)
                e.Cancel = true;
        }
        private void STSchedulerVw_ItemSelectionStateChanged(object sender, EventArgs e)
        {
            UI.STUpdateBtn.Enabled =
                UI.STRemoveBtn.Enabled = UI.STSchedulerVw.HasSelectedItem;
        }

        public void Halt()
        {
            try
            {
                _suppressUIUpdating = true;
                foreach (ListViewItem item in UI.STSchedulerVw.Items)
                {
                    item.Checked = false;
                }
            }
            finally
            {
                _suppressUIUpdating = false;
                UpdateUI();
            }
        }
        public HMessage GetPacket()
        {
            return new HMessage(
                UI.STPacketTxt.Text, Destination);
        }

        private void UpdateUI()
        {
            if (!_suppressUIUpdating)
            {
                UI.SchedulesTxt.Text =
                    $"Schedules: {UI.STSchedulerVw.CheckedItems.Count}/{UI.STSchedulerVw.Items.Count}";
            }
        }

        protected override void OnTabSelecting(TabControlCancelEventArgs e)
        {
            UI.InjectionMenu.InputBox = UI.STPacketTxt;
            base.OnTabSelecting(e);
        }
    }
}