﻿#pragma checksum "..\..\..\Windows\InitSettingWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4C191CDD2A1670CE459C32D1FB8D5C86"
//------------------------------------------------------------------------------
// <auto-generated>
//     이 코드는 도구를 사용하여 생성되었습니다.
//     런타임 버전:4.0.30319.42000
//
//     파일 내용을 변경하면 잘못된 동작이 발생할 수 있으며, 코드를 다시 생성하면
//     이러한 변경 내용이 손실됩니다.
// </auto-generated>
//------------------------------------------------------------------------------

using AutoCapturer.UserControls;
using AutoCapturer.Windows;
using System;
using System.Diagnostics;
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


namespace AutoCapturer.Windows {
    
    
    /// <summary>
    /// InitSettingWindow
    /// </summary>
    public partial class InitSettingWindow : AutoCapturer.Windows.ChromeWindow, System.Windows.Markup.IComponentConnector {
        
        
        #line 45 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TabControl Grids;
        
        #line default
        #line hidden
        
        
        #line 65 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button BtnAllSkip;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button NextBtn1;
        
        #line default
        #line hidden
        
        
        #line 101 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PtnNameTB;
        
        #line default
        #line hidden
        
        
        #line 152 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox SaveLocTB;
        
        #line default
        #line hidden
        
        
        #line 244 "..\..\..\Windows\InitSettingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AutoCapturer.UserControls.Switch swStartupProgram;
        
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
            System.Uri resourceLocater = new System.Uri("/AutoCapturer;component/windows/initsettingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Windows\InitSettingWindow.xaml"
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
            this.Grids = ((System.Windows.Controls.TabControl)(target));
            return;
            case 2:
            this.BtnAllSkip = ((System.Windows.Controls.Button)(target));
            
            #line 65 "..\..\..\Windows\InitSettingWindow.xaml"
            this.BtnAllSkip.Click += new System.Windows.RoutedEventHandler(this.BtnAllSkip_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.NextBtn1 = ((System.Windows.Controls.Button)(target));
            
            #line 70 "..\..\..\Windows\InitSettingWindow.xaml"
            this.NextBtn1.Click += new System.Windows.RoutedEventHandler(this.NextBtn_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.PtnNameTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.SaveLocTB = ((System.Windows.Controls.TextBox)(target));
            return;
            case 6:
            
            #line 153 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 206 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.BtnAllSkip_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 211 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PrevBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 216 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NextBtn_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.swStartupProgram = ((AutoCapturer.UserControls.Switch)(target));
            return;
            case 11:
            
            #line 254 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.PrevBtn_Click);
            
            #line default
            #line hidden
            return;
            case 12:
            
            #line 259 "..\..\..\Windows\InitSettingWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.NextBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

