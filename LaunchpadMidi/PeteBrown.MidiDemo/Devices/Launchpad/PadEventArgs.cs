using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

namespace PeteBrown.MidiDemo.Devices.Launchpad
{
    abstract class PadEventArgs : EventArgs
    {
        public byte PadNumber { get; private set; }
        public IMidiMessage Message { get; private set; }

        public PadEventArgs(byte padNumber, IMidiMessage message)
        {
            PadNumber = padNumber;
            Message = message;
        }
    }

    class PadPressedEventArgs : PadEventArgs
    {
        public byte Velocity { get; private set; }

        public PadPressedEventArgs(byte padNumber, byte velocity, IMidiMessage message)
            : base(padNumber, message)
        {
            Velocity = velocity;
        }
    }

    class PadReleasedEventArgs : PadEventArgs
    {
        public PadReleasedEventArgs(byte padNumber, IMidiMessage message)
            : base(padNumber, message)
        {
        }
    }
}
