using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace PeteBrown.MidiDemo.Devices
{
    class Midi : IDisposable
    {
        public List<MidiDeviceInformation> ConnectedInputDevices { get; private set; }
        public List<MidiDeviceInformation> ConnectedOutputDevices { get; private set; }

        private DeviceWatcher _inputWatcher;
        private DeviceWatcher _outputWatcher;

        public event EventHandler InputDevicesEnumerated;
        public event EventHandler OutputDevicesEnumerated;

        public event EventHandler InputDevicesChanged;
        public event EventHandler OutputDevicesChanged;


        // using an Initialize method here instead of the constructor in order to
        // prevent a race condition between wiring up the event handlers and
        // finishing enumeration
        public void Initialize()
        {
            ConnectedInputDevices = new List<MidiDeviceInformation>();
            ConnectedOutputDevices = new List<MidiDeviceInformation>();


            // set up watchers so we know when input devices are added or removed
            _inputWatcher = DeviceInformation.CreateWatcher(MidiInPort.GetDeviceSelector());

            _inputWatcher.EnumerationCompleted += InputWatcher_EnumerationCompleted;
            _inputWatcher.Updated += InputWatcher_Updated;
            _inputWatcher.Removed += InputWatcher_Removed;
            _inputWatcher.Added += InputWatcher_Added;

            _inputWatcher.Start();

            // set up watcher so we know when output devices are added or removed
            _outputWatcher = DeviceInformation.CreateWatcher(MidiOutPort.GetDeviceSelector());

            _outputWatcher.EnumerationCompleted += OutputWatcher_EnumerationCompleted;
            _outputWatcher.Updated += OutputWatcher_Updated;
            _outputWatcher.Removed += OutputWatcher_Removed;
            _outputWatcher.Added += OutputWatcher_Added;

            _outputWatcher.Start();
        }



        private void OutputWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // let other classes know enumeration is complete
            if (OutputDevicesEnumerated != null)
                OutputDevicesEnumerated(this, new EventArgs());
        }

        private void OutputWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // this is where you capture changes to a specific ID
            // you could change this to be more specific and pass the changed ID
            if (OutputDevicesChanged != null)
                OutputDevicesChanged(this, new EventArgs());
        }

        private void OutputWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // remove from our collection the item with the specified ID

            var id = args.Id;

            var toRemove = (from MidiDeviceInformation mdi in ConnectedOutputDevices
                            where mdi.Id == id
                            select mdi).FirstOrDefault();

            if (toRemove != null)
            {
                ConnectedOutputDevices.Remove(toRemove);

                // notify clients
                if (OutputDevicesChanged != null)
                    OutputDevicesChanged(this, new EventArgs());
            }
        }

        private void OutputWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var id = args.Id;

            // you could use DeviceInformation directly here, using the
            // CreateFromIdAsync method. However, that is an async method
            // and so adds a bit of delay. I'm using a trimmed down object
            // to hold MIDI information rather than using the DeviceInformation class

#if DEBUG
            // this is so you can see what the properties contain
            //foreach (var p in args.Properties.Keys)
            //{
            //    System.Diagnostics.Debug.WriteLine("Output: " + args.Name + " : " + p + " : " + args.Properties[p]);
            //}
#endif   

            var info = new MidiDeviceInformation();
            info.Id = id;
            info.Name = args.Name;
            info.IsDefault = args.IsDefault;
            info.IsEnabled = args.IsEnabled;

            ConnectedOutputDevices.Add(info);

            // notify clients
            if (OutputDevicesChanged != null)
                OutputDevicesChanged(this, new EventArgs());
        }


        private void InputWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            // let other classes know enumeration is complete
            if (InputDevicesEnumerated != null)
                InputDevicesEnumerated(this, new EventArgs());

        }

        private void InputWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // this is where you capture changes to a specific ID
            // you could change this to be more specific and pass the changed ID
            if (InputDevicesChanged != null)
                InputDevicesChanged(this, new EventArgs());
        }

        private void InputWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // remove from our collection the item with the specified ID

            var id = args.Id;

            var toRemove = (from MidiDeviceInformation mdi in ConnectedInputDevices
                            where mdi.Id == id
                            select mdi).FirstOrDefault();

            if (toRemove != null)
            {
                ConnectedInputDevices.Remove(toRemove);

                // notify clients
                if (InputDevicesChanged != null)
                    InputDevicesChanged(this, new EventArgs());
            }
        }

        private void InputWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var id = args.Id;

            // you could use DeviceInformation directly here, using the
            // CreateFromIdAsync method. However, that is an async method
            // and so adds a bit of delay. I'm using a trimmed down object
            // to hold MIDI information rather than using the DeviceInformation class

#if DEBUG
            // this is so you can see what the properties contain
            //foreach (var p in args.Properties.Keys)
            //{
            //    System.Diagnostics.Debug.WriteLine("Input: " + args.Name + " : " + p + " : " + args.Properties[p]);
            //}
#endif            

            var info = new MidiDeviceInformation();
            info.Id = id;
            info.Name = args.Name;
            info.IsDefault = args.IsDefault;
            info.IsEnabled = args.IsEnabled;

            ConnectedInputDevices.Add(info);

            // notify clients
            if (InputDevicesChanged != null)
                InputDevicesChanged(this, new EventArgs());
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


                    _inputWatcher.EnumerationCompleted -= InputWatcher_EnumerationCompleted;
                    _inputWatcher.Updated -= InputWatcher_Updated;
                    _inputWatcher.Removed -= InputWatcher_Removed;
                    _inputWatcher.Added -= InputWatcher_Added;

                    _outputWatcher.EnumerationCompleted -= OutputWatcher_EnumerationCompleted;
                    _outputWatcher.Updated -= OutputWatcher_Updated;
                    _outputWatcher.Removed -= OutputWatcher_Removed;
                    _outputWatcher.Added -= OutputWatcher_Added;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                _outputWatcher.Stop();
                _inputWatcher.Stop();


                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. 
        ~Midi()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion


    }
}
