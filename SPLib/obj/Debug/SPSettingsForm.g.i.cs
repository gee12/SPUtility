﻿#pragma checksum "..\..\SPSettingsForm.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "0263877A006C0B5ED23838E7A5F29E46"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using SPLib;
using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace SPLib {
    
    
    /// <summary>
    /// SPSettingsForm
    /// </summary>
    public partial class SPSettingsForm : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SPLib.SPSettingsForm _this;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gMain;
        
        #line default
        #line hidden
        
        
        #line 62 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbPort;
        
        #line default
        #line hidden
        
        
        #line 64 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbBaudRate;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbDataBits;
        
        #line default
        #line hidden
        
        
        #line 68 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbStopBits;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cbParity;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bApply;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bCancel;
        
        #line default
        #line hidden
        
        
        #line 81 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button bTest;
        
        #line default
        #line hidden
        
        
        #line 86 "..\..\SPSettingsForm.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal SPLib.CircularProgressBar pbCircular;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SPLib;component/spsettingsform.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\SPSettingsForm.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this._this = ((SPLib.SPSettingsForm)(target));
            return;
            case 2:
            
            #line 30 "..\..\SPSettingsForm.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CommandBinding_Executed);
            
            #line default
            #line hidden
            return;
            case 3:
            this.gMain = ((System.Windows.Controls.Grid)(target));
            return;
            case 4:
            this.cbPort = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.cbBaudRate = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 6:
            this.cbDataBits = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 7:
            this.cbStopBits = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 8:
            this.cbParity = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 9:
            this.bApply = ((System.Windows.Controls.Button)(target));
            return;
            case 10:
            this.bCancel = ((System.Windows.Controls.Button)(target));
            return;
            case 11:
            this.bTest = ((System.Windows.Controls.Button)(target));
            return;
            case 12:
            this.pbCircular = ((SPLib.CircularProgressBar)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

