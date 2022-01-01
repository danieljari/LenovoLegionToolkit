﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LenovoLegionToolkit.Lib;
using LenovoLegionToolkit.Lib.Features;
using LenovoLegionToolkit.Lib.Listeners;
using LenovoLegionToolkit.WPF.Utils;

namespace LenovoLegionToolkit.WPF.Controls
{
    public partial class PowerModeControl
    {
        private readonly PowerModeFeature _feature = Container.Resolve<PowerModeFeature>();
        private readonly PowerModeListener _listener = Container.Resolve<PowerModeListener>();

        public PowerModeControl()
        {
            InitializeComponent();

            _listener.Changed += Listener_Changed;
        }

        private void Listener_Changed(object sender, PowerModeState e) => Dispatcher.Invoke(async () =>
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        });

        private async void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!IsVisible)
                return;

            await RefreshAsync();
        }

        private async void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_comboBox.TryGetSelectedItem(out PowerModeState state))
                return;

            if (state != await _feature.GetStateAsync())
                await _feature.SetStateAsync(state);
        }

        private async Task RefreshAsync()
        {
            try
            {
                var items = Enum.GetValues<PowerModeState>();
                var selectedItem = await _feature.GetStateAsync();
                _comboBox.SetItems(items, selectedItem);
                Visibility = Visibility.Visible;
            }
            catch
            {
                Visibility = Visibility.Collapsed;
            }
        }
    }
}