using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DOTNET.Common.WindowsRegistry
{
    public class StartupHelper
    {
        #region Properties

        /// <summary>
        /// Contains name ( without  extension )
        /// </summary>
        public string AppName { get; private set; }

        /// <summary>
        /// Contains address, name , extension
        /// </summary>
        public string AppAddress { get; private set; }

        private const string REGISTRY_STARTUP_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        #endregion

        #region Constructors

        public StartupHelper()
        {

        }

        public StartupHelper(Assembly assembly)
        {
            InitialAppInfo(assembly);
        }

        public StartupHelper(Type assemblyType)
        {
            InitialAppInfo(assemblyType);
        }

        public StartupHelper(string assemblyAddress)
        {
            InitialAppInfo(assemblyAddress);
        }

        #endregion

        #region Public methods

        public object Get()
        {
            //string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, true))
            {
                object result = key.GetValue(AppName);
                key.Dispose();
                return result;
            }
        }

        public void Delete()
        {
            using (Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, true))
            {
                key.DeleteValue(AppName);
            }
        }

        public void Set(string[] args)
        {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, true);

            var _args_ = String.Join(" ", args);

            key.SetValue(AppName, AppAddress + " " + _args_);
        }

        public bool IsSet()
        {
            return Get() != null;
        }

        public string[] GetArguments()
        {
            if (!IsSet())
                return [];

            string? fullnameAndArgs = Get()?.ToString();

            if (string.IsNullOrEmpty(fullnameAndArgs) || string.IsNullOrWhiteSpace(fullnameAndArgs))
                return [];
            else
            {

                // First one is name of application like:
                // C:\...\myapp.exe
                string[] Args = fullnameAndArgs.Split(' ');

                if (Args.Length > 1)
                {
                    var arrrayArgs = Args.Skip(1).Take(Args.Length - 1).ToArray(); // Skip name of file

                    return arrrayArgs.Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)).ToArray(); // Remove any empty space
                }
                else
                {
                    return [];
                }
            }
        }

        #endregion

        #region Helpers

        public void InitialAppInfo(Assembly assembly)
        {
            AppName = System.IO.Path.GetFileNameWithoutExtension(assembly.Location);
            AppAddress = assembly.Location;

        }

        public void InitialAppInfo(Type assemblyType)
        {
            InitialAppInfo(Assembly.GetAssembly(assemblyType));
        }

        public void InitialAppInfo(string assemblyAddress)
        {
            InitialAppInfo(Assembly.LoadFrom(assemblyAddress));
        }

        #endregion
    }
}
