using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;    // for byte[].AsBuffer()
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace PeteBrown.MidiDemo.Devices.Launchpad
{

    /// <summary>
    /// NOTE: Works with Launchpad S, not original Launchpad
    /// TBD if works with Launchpad Mini. Not tested.
    /// </summary>
    class LaunchpadInterface : IDisposable
    {
        public event EventHandler<PadPressedEventArgs> PadPressed;
        public event EventHandler<PadReleasedEventArgs> PadReleased;
        public event EventHandler TextScrollingComplete;

        private const byte InputMidiChannel = 0;
        private const byte OutputMidiChannel = 0;

        private MidiInPort _midiIn;
        private MidiOutPort _midiOut;

        private PadMappingMode _currentMappingMode;

        public void InitializeMidi(MidiDeviceInformation midiInToPC, MidiDeviceInformation midiOutToLaunchpad)
        {
            InitializeMidi(midiInToPC.Id, midiOutToLaunchpad.Id);
        }

        public void InitializeMidi(DeviceInformation midiInToPC, DeviceInformation midiOutToLaunchpad)
        {
            InitializeMidi(midiInToPC.Id, midiOutToLaunchpad.Id);
        }

        public async void InitializeMidi(string midiInToPCDeviceId, string midiOutToLaunchpadDeviceId)
        {
            // acquire the MIDI ports

            // TODO: Exception handling

            _midiIn = await MidiInPort.FromIdAsync(midiInToPCDeviceId);
            _midiIn.MessageReceived += OnMidiInMessageReceived;

            _midiOut = (MidiOutPort)await MidiOutPort.FromIdAsync(midiOutToLaunchpadDeviceId);

            SetPadMappingMode(PadMappingMode.XY);
        }

        private void OnMidiInMessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
        {
            // handle incoming messages
            // these are USB single-device connections, so we're not going to do any filtering

            if (args.Message is MidiNoteOnMessage)
            {

                var msg = args.Message as MidiNoteOnMessage;

                if (msg.Velocity == 0)
                {
                    // note off
                    if (PadReleased != null)
                        PadReleased(this, new PadReleasedEventArgs(msg.Note, (IMidiMessage)msg));
                }
                else
                {
                    // velocity is always 127 on the novation, but still passing it along here
                    // in case they add touch sensitivity in the future

                    // note on
                    if (PadPressed != null)
                        PadPressed(this, new PadPressedEventArgs(msg.Note, msg.Velocity, (IMidiMessage)msg));
                }
            }
            else if (args.Message is MidiControlChangeMessage)
            {
                var msg = args.Message as MidiControlChangeMessage;

                if (msg.Controller == 0 && msg.ControlValue == 3)
                {
                    // this is the notification that text has stopped scrolling
                    if (TextScrollingComplete != null)
                        TextScrollingComplete(this, EventArgs.Empty);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Unhandled MIDI-IN control change message controller: " + msg.Controller + ", value: " + msg.ControlValue);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Unhandled MIDI-IN message " + args.Message.GetType().ToString());
            }

        }

        public void ReleaseMidi()
        {
            if (_midiIn != null)
            {
                _midiIn.MessageReceived -= OnMidiInMessageReceived;
                _midiIn.Dispose();
            }

            if (_midiOut != null)
            {
                _midiOut.Dispose();
            }
        }


        // also need to handle the double-buffering Novation supports
        // http://d19ulaff0trnck.cloudfront.net/sites/default/files/novation/downloads/4700/launchpad-s-prm.pdf

        /// <summary>
        /// Assumes launchpad is in XY layout mode
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="color"></param>
        public void TurnOnPad(int row, int column, byte color)
        {
            TurnOnPad((byte)(row * 16 + column), color);
        }

        public void TurnOnPad(int row, int column, KnownPadColors color)
        {
            TurnOnPad((byte)(row * 16 + column), color);
        }

        public void TurnOnPad(byte padNumber, byte color)
        {
            if (MidiOutPortValid())
            {
                _midiOut.SendMessage(new MidiNoteOnMessage(OutputMidiChannel, padNumber, color));
            }
        }

        public void TurnOnPad(byte padNumber, KnownPadColors color)
        {
            TurnOnPad(padNumber, (byte)color);
        }


        public static byte RedGreenToColorByte(byte red, byte green, byte flags = 0x0C)
        {
            byte color = (byte)(0x10 * (green & 0x03) + (red & 0x03) + flags);

            //System.Diagnostics.Debug.WriteLine("Red: 0x{0:x2}, Green: 0x{1:x2}, Color: 0x{2:x2}", red, green, color);

            return color;
        }

        public void TurnOffPad(byte row, byte column)
        {
            TurnOnPad(row, column, KnownPadColors.Off);
        }

        public void TurnOffPad(byte padNumber)
        {
            TurnOnPad(padNumber, KnownPadColors.Off);
        }


        public void TurnOnTopRowPad(byte padNumber)
        {

        }

        public void TurnOffTopRowPad(byte padNumber)
        {

        }

        #region Text Scrolling

        public void ScrollText(string text, KnownPadColors color, TextScrollingSpeed speed = TextScrollingSpeed.Normal, bool loop = false)
        {
            ScrollText(text, (byte)color, speed, loop);
        }

        public void ScrollText(string text, byte color, TextScrollingSpeed speed = TextScrollingSpeed.Normal, bool loop = false)
        {
            if (MidiOutPortValid())
            {
                var encoding = Encoding.GetEncoding("us-ascii");
                var characters = encoding.GetBytes(text);

                if (loop)
                    color += 64;        // set bit 4 to set looping

                // 
                var header = new byte[] { 0xF0, 0x00, 0x20, 0x29, 0x09, color, (byte)speed };
                var fullData = new byte[characters.Length + header.Length];

                header.CopyTo(fullData, 0);                     // header info, including color
                characters.CopyTo(fullData, header.Length);     // actual text
                fullData[fullData.Length - 1] = 0xF7;           // sysex terminator

                _midiOut.SendMessage(new MidiSystemExclusiveMessage(fullData.AsBuffer()));
            }
        }

        public void StopScrollingText()
        {
            var data = new byte[] { 0xF0, 0x00, 0x20, 0x29, 0x09, 0x00, 0xF7 };

            _midiOut.SendMessage(new MidiSystemExclusiveMessage(data.AsBuffer()));

        }

        #endregion


        public void Reset()
        {
            // NOTE: this also changes the mapping mode to the default power-on value
            // We'll have a real problem keeping that in sync

            if (MidiOutPortValid())
            {
                _midiOut.SendMessage(new MidiControlChangeMessage(OutputMidiChannel, 0x00, 0x00));

                _currentMappingMode = PadMappingMode.XY;
            }
        }

        public void TurnOnAllPads(LedIntensity intensity)
        {
            if (MidiOutPortValid())
            {
                _midiOut.SendMessage(new MidiControlChangeMessage(OutputMidiChannel, 0x00, (byte)intensity));
            }
        }

        ///// <summary>
        ///// denominator must between 1 and 18. The effect is numerator/denominator brightness
        ///// numerator must be between 1 and 
        ///// </summary>
        ///// <param name="denominator"></param>
        //public void SetOverallBrightness(int numerator, int denominator)
        //{
        //    if (MidiOutPortValid())
        //    {
        //        _midiOut.SendMessage(new MidiControlChangeMessage(_outputMidiChannel, 0x00, (byte)mode));

        //    }
        //}


        public void SetPadMappingMode(PadMappingMode mode)
        {
            if (MidiOutPortValid())
            {
                _midiOut.SendMessage(new MidiControlChangeMessage(OutputMidiChannel, 0x00, (byte)mode));

                _currentMappingMode = mode;
            }
        }

        public void SetBufferingMode(BufferingMode mode)
        {
            if (MidiOutPortValid())
            {
                _midiOut.SendMessage(new MidiControlChangeMessage(OutputMidiChannel, 0x00, (byte)mode));
            }
        }



        private bool MidiOutPortValid()
        {
            return _midiOut != null;
        }

        private bool MidiInPortValid()
        {
            return _midiIn != null;
        }


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).     
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                ReleaseMidi();

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        // ~LaunchpadInterface() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion



    }
}
