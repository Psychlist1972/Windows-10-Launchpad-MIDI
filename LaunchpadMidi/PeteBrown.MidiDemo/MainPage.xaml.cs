using PeteBrown.MidiDemo.Devices;
using PeteBrown.MidiDemo.Devices.Launchpad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PeteBrown.MidiDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Midi _midi;

        public MainPage()
        {
            Loaded += MainPage_Loaded;

            this.InitializeComponent();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _midi = new Midi();

            _midi.OutputDevicesEnumerated += _midi_OutputDevicesEnumerated;
            _midi.InputDevicesEnumerated += _midi_InputDevicesEnumerated;

            _midi.Initialize();
        }

        private async void _midi_InputDevicesEnumerated(object sender, EventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    InputDevices.ItemsSource = _midi.ConnectedInputDevices;

                    // only wire up device changed event after enumeration has completed
                    _midi.InputDevicesChanged += _midi_InputDevicesChanged;

                    // Pre-selec the launchpad if there's one
                    InputDevices.SelectedItem = (from d in _midi.ConnectedInputDevices
                                                 where d.Name.IndexOf("launchpad", StringComparison.CurrentCultureIgnoreCase) >= 0
                                                 select d).FirstOrDefault();
                });
            }
        }

        private async void _midi_InputDevicesChanged(object sender, EventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    InputDevices.ItemsSource = null;
                    InputDevices.ItemsSource = _midi.ConnectedInputDevices;

                    // Pre-selec the launchpad if there's one
                    InputDevices.SelectedItem = (from d in _midi.ConnectedInputDevices
                                                 where d.Name.IndexOf("launchpad", StringComparison.CurrentCultureIgnoreCase) >= 0
                                                 select d).FirstOrDefault();
                    
                    //_midi.ConnectedInputDevices.FirstOrDefault(() x => x.)
                });
            }
        }

        private async void _midi_OutputDevicesEnumerated(object sender, EventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    OutputDevices.ItemsSource = _midi.ConnectedOutputDevices;

                    // only wire up device changed event after enumeration has completed
                    _midi.OutputDevicesChanged += _midi_OutputDevicesChanged;

                    // Pre-selec the launchpad if there's one
                    OutputDevices.SelectedItem = (from d in _midi.ConnectedOutputDevices
                                                 where d.Name.IndexOf("launchpad", StringComparison.CurrentCultureIgnoreCase) >= 0
                                                 select d).FirstOrDefault();
                });

            }
        }

        private async void _midi_OutputDevicesChanged(object sender, EventArgs e)
        {
            if (!Dispatcher.HasThreadAccess)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    OutputDevices.ItemsSource = null;
                    OutputDevices.ItemsSource = _midi.ConnectedOutputDevices;

                    // Pre-selec the launchpad if there's one
                    InputDevices.SelectedItem = (from d in _midi.ConnectedInputDevices
                                                 where d.Name.IndexOf("launchpad", StringComparison.CurrentCultureIgnoreCase) >= 0
                                                 select d).FirstOrDefault();
                });

            }
        }




        private LaunchpadInterface _launchpad;

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
            if (OutputDevices.SelectedItem != null && InputDevices.SelectedItem != null)
            {
                if (_launchpad != null)
                {
                    _launchpad.Dispose();
                    _launchpad = null;
                }

                _launchpad = new LaunchpadInterface();

                _launchpad.PadPressed += _launchpad_PadPressed;
                _launchpad.PadReleased += _launchpad_PadReleased;
                _launchpad.TextScrollingComplete += _launchpad_TextScrollingComplete;

                _launchpad.InitializeMidi(
                    (MidiDeviceInformation)InputDevices.SelectedItem,
                    (MidiDeviceInformation)OutputDevices.SelectedItem);
            }
            else
            {
                var msg = new MessageDialog("Please select the appropriate input and output MIDI interfaces.");
                msg.ShowAsync();
            }

        }

        private void _launchpad_TextScrollingComplete(object sender, EventArgs e)
        {
            for (int row = 0; row < 8; row++)
                for (int col = 0; col < 8; col++)
                    _launchpad.TurnOnPad((byte)(row * 16 + col),
                        LaunchpadInterface.RedGreenToColorByte((byte)(row % 4), (byte)(col % 4)));
        }

        private void _launchpad_PadReleased(object sender, PadReleasedEventArgs e)
        {
            _launchpad.TurnOffPad(e.PadNumber);
        }

        private void _launchpad_PadPressed(object sender, PadPressedEventArgs e)
        {
            _launchpad.TurnOnPad(e.PadNumber, KnownPadColors.FullRed);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            if (_launchpad != null)
            {
                _launchpad.Reset();
            }
        }

        private void CoolStuff_Click(object sender, RoutedEventArgs e)
        {
            if (_launchpad != null)
            {
                _launchpad.SetBufferingMode(BufferingMode.Simple);

                _launchpad.ScrollText("Hello Windows 10!", KnownPadColors.FullRed, TextScrollingSpeed.Normal, false);
            }
        }
    }
}
