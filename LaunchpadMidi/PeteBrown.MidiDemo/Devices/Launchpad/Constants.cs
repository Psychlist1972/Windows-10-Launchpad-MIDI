using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeteBrown.MidiDemo.Devices.Launchpad
{


    enum PadMappingMode
    {
        XY = 0x00,
        DrumRack = 0x01
    }

    enum BufferingMode
    {
        Simple = 0x20,
        Buffered0 = 0x24,
        Buffered1 = 0x21,
        Buffered0PlusCopy = 0x34,
        Buffered1PlusCopy = 0x31,
        Flash = 0x28
    }

    enum LedIntensity
    {
        Dim = 0x7D,
        Normal = 0x7E,
        Brightest = 0x7F
    }

    enum KnownPadColors
    {
        Off = 0x0C,
        DimRed = 0x0D,
        MediumRed = 0x0E,
        FullRed = 0x0F,
        DimAmber = 0x1D,
        Yellow = 0x3E,
        FullAmber = 0x3F,
        DimGreen = 0x1C,
        MediumGreen = 0x2C,
        FullGreen = 0x3C,
    }

    // yes, I just mangled the English language for these
    enum TextScrollingSpeed
    {
        Slowest = 0x01,
        Slower = 0x02,
        Slow = 0x03,
        Normal = 0x04,
        Fast = 0x05,
        Faster = 0x06,
        Fastest = 0x07
    }

}
