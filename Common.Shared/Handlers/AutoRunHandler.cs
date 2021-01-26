using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

namespace Common.Handlers
{
    public partial class AutoRunHandler
    {
#pragma warning disable CS0649 // 从未对字段“AutoRunHandler._autoRunStatusChangedEventHandler”赋值，字段将一直保持其默认值 null
        private Action<bool> _autoRunStatusChangedEventHandler;
#pragma warning restore CS0649 // 从未对字段“AutoRunHandler._autoRunStatusChangedEventHandler”赋值，字段将一直保持其默认值 null
#pragma warning disable CS0649 // 从未对字段“AutoRunHandler._statusUpdateEventHandler”赋值，字段将一直保持其默认值 null
        private Action _statusUpdateEventHandler;
#pragma warning restore CS0649 // 从未对字段“AutoRunHandler._statusUpdateEventHandler”赋值，字段将一直保持其默认值 null
#pragma warning disable CS0414 // 字段“AutoRunHandler._applicationName”已被赋值，但从未使用过它的值
        private string _applicationName = "Clipboard.Desktop";
#pragma warning restore CS0414 // 字段“AutoRunHandler._applicationName”已被赋值，但从未使用过它的值
#pragma warning disable CS0414 // 字段“AutoRunHandler._applicationId”已被赋值，但从未使用过它的值
        private string _applicationId = "9E00BF43D4184246BD0C45D35BFA1C29";
#pragma warning restore CS0414 // 字段“AutoRunHandler._applicationId”已被赋值，但从未使用过它的值
        private bool _isAutoRun = false;

        public bool IsAutoRun
        {
            get => _isAutoRun;
            set
            {
                _autoRunStatusChangedEventHandler?.Invoke(value);
                OnPropertyChagned(ref _isAutoRun, value);
            }
        }

        public void StatusUpdate()
        {
            _statusUpdateEventHandler?.Invoke();
            OnPropertyChagned(nameof(IsAutoRun));
        }
    }

    public partial class AutoRunHandler
    {
#if UWP
        private Windows.ApplicationModel.StartupTask _startupTask;
        public AutoRunHandler()
        {
            _autoRunStatusChangedEventHandler = AutoRunStatusChangedEventHandler;
            _statusUpdateEventHandler = () => { _isAutoRun = _startupTask.State.Equals(Windows.ApplicationModel.StartupTaskState.Enabled); };
            AutoRunInitialization();
        }

        private async void AutoRunInitialization()
        {
            _startupTask = await Windows.ApplicationModel.StartupTask.GetAsync(_applicationId);
            StatusUpdate();
        }

        private async void AutoRunStatusChangedEventHandler(bool status)
        {
            if (status)
            {
                await _startupTask?.RequestEnableAsync();
            }
            else
            {
                _startupTask?.Disable();
            }
            StatusUpdate();
        }
#endif
    }

    public partial class AutoRunHandler
    {
#if StartupFile
        string startupPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), $"{_applicationName}.lnk");
        public AutoRunHandler()
        {
            _autoRunStatusChangedEventHandler = AutoRunStatusChangedEventHandler;
            _statusUpdateEventHandler = () => { _isAutoRun = File.Exists(startupPath); };
        }

        private void AutoRunStatusChangedEventHandler(bool status)
        {
            if (status)
            {
                if (File.Exists(startupPath))
                {
                    return;
                }
                var shellType = Type.GetTypeFromProgID("WScript.Shell");
                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(startupPath);
                shortcut.TargetPath = Assembly.GetEntryAssembly().Location;
                //shortcut.Arguments = args;
                shortcut.WorkingDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                shortcut.Save();
            }
            else
            {
                if (File.Exists(startupPath))
                {
                    File.Delete(startupPath);
                }
            }
        }
#endif
    }

    public partial class AutoRunHandler
    {
#if Regedit

        private string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.SetupInformation.ApplicationName);

        public AutoRunHandler()
        {
            _autoRunStatusChangedEventHandler = AutoRunStatusChangedEventHandler;
            IsAutoRun = IsKeyInRegistry();
        }

        private void AutoRunStatusChangedEventHandler(bool status)
        {
            try
            {
                RegistryKey local = Registry.CurrentUser;
                RegistryKey key = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (key == null)
                {
                    local.CreateSubKey("SOFTWARE//Microsoft//Windows//CurrentVersion//Run");
                }
                if (status)//若开机自启动则添加键值对
                {
                    key.SetValue(_applicationName, _path);
                    key.Close();
                }
                else//否则删除键值对
                {
                    string[] keyNames = key.GetValueNames();
                    foreach (string keyName in keyNames)
                    {
                        if (keyName.ToUpper() == _applicationName.ToUpper())
                        {
                            key.DeleteValue(_applicationName);
                            key.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                IsAutoRun = false;
                //throw;
            }
        }

        private bool IsKeyInRegistry()
        {
            try
            {
                bool _exist = false;
                RegistryKey local = Registry.CurrentUser;
                RegistryKey runs = local.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
                if (runs == null)
                {
                    RegistryKey key2 = local.CreateSubKey("SOFTWARE");
                    RegistryKey key3 = key2.CreateSubKey("Microsoft");
                    RegistryKey key4 = key3.CreateSubKey("Windows");
                    RegistryKey key5 = key4.CreateSubKey("CurrentVersion");
                    RegistryKey key6 = key5.CreateSubKey("Run");
                    runs = key6;
                }
                string[] runsName = runs.GetValueNames();
                foreach (string strName in runsName)
                {
                    if (strName.ToUpper() == _applicationName.ToUpper())
                    {
                        _exist = true;
                        return _exist;
                    }
                }
                return _exist;

            }
            catch
            {
                return false;
            }
        }
#endif
    }


    public partial class AutoRunHandler : INotifyPropertyChanged
    {
        private static AutoRunHandler _instance = null;
        public static AutoRunHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (Application.Current.Resources.Contains("AutoRun"))
                    {
                        _instance = (AutoRunHandler)Application.Current.Resources["AutoRun"];
                    }
                    else
                    {
                        _instance = new AutoRunHandler();
                    }
                }
                return _instance;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChagned([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        internal void OnPropertyChagned<T>(ref T source, T value, [CallerMemberName] string propertyName = null)
        {
            source = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
