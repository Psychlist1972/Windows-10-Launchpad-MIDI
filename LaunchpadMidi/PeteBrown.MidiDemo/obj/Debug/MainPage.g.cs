﻿#pragma checksum "c:\users\iottools\documents\visual studio 2015\Projects\LaunchpadMidi\LaunchpadMidi\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "98F34EAFDF95095C1DA738E0C425C645"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PeteBrown.MidiDemo
{
    partial class MainPage : 
        global::Windows.UI.Xaml.Controls.Page, 
        global::Windows.UI.Xaml.Markup.IComponentConnector,
        global::Windows.UI.Xaml.Markup.IComponentConnector2
    {
        /// <summary>
        /// Connect()
        /// </summary>
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                {
                    this.ConnectToLaunchpad = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 88 "..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.ConnectToLaunchpad).Click += this.Connect_Click;
                    #line default
                }
                break;
            case 2:
                {
                    this.ResetLaunchpad = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 89 "..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.ResetLaunchpad).Click += this.Reset_Click;
                    #line default
                }
                break;
            case 3:
                {
                    this.CoolStuff = (global::Windows.UI.Xaml.Controls.Button)(target);
                    #line 90 "..\..\MainPage.xaml"
                    ((global::Windows.UI.Xaml.Controls.Button)this.CoolStuff).Click += this.CoolStuff_Click;
                    #line default
                }
                break;
            case 4:
                {
                    this.OutputDevices = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                }
                break;
            case 5:
                {
                    this.InputDevices = (global::Windows.UI.Xaml.Controls.ListBox)(target);
                }
                break;
            default:
                break;
            }
            this._contentLoaded = true;
        }

        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 14.0.0.0")]
        //[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::Windows.UI.Xaml.Markup.IComponentConnector GetBindingConnector(int connectionId, object target)
        {
            global::Windows.UI.Xaml.Markup.IComponentConnector returnValue = null;
            return returnValue;
        }
    }
}
