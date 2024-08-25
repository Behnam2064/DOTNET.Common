using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;


namespace DOTNET.Common.WindowsServices
{
    public class ServiceHelper
    {
        public static ServiceControllerStatus GetStatus(string ServiceName)
        {
            using (ServiceController sc = new ServiceController(ServiceName))
                return sc.Status;
        }

        public static bool IsInstalled(string ServiceName)
        {
            try
            {
                using (ServiceController sc = new ServiceController(ServiceName))
                {
                    var testIsNull = sc.Status;
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool IsRunning(string ServiceName)
        {
            using (ServiceController controller =
                new ServiceController(ServiceName))
            {
                if (!IsInstalled(ServiceName)) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        public static void Install(string ExePath)
        {
            string[] vs = new string[] { ExePath };
            ManagedInstallerClass.InstallHelper(vs);
        }

        public static void Uninstall(string ExePath)
        {
            ManagedInstallerClass.InstallHelper(new string[] { "/u", ExePath });
        }

        public static void Start(string ServiceName, bool WaitChangeStatus = true)
        {
            using (ServiceController sc = new ServiceController(ServiceName))
            {

                if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(3));

                }
                else if (sc.Status.Equals(ServiceControllerStatus.StopPending))
                {
                    if (WaitChangeStatus)
                    {
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        sc.Start();

                    }

                }
                else if (sc.Status.Equals(ServiceControllerStatus.Paused))
                {
                    sc.Continue();

                }
                else if (sc.Status.Equals(ServiceControllerStatus.PausePending))
                {
                    if (WaitChangeStatus)
                    {
                        sc.WaitForStatus(ServiceControllerStatus.Paused);
                        sc.Continue();
                    }
                }


            }
        }

        public static void Stop(string ServiceName, bool WaitChangeStatus = true)
        {
            using (ServiceController sc = new ServiceController(ServiceName))
            {

                if (sc.Status.Equals(ServiceControllerStatus.Running))
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));

                }
                else if (sc.Status.Equals(ServiceControllerStatus.ContinuePending))
                {
                    if (WaitChangeStatus)
                    {
                        sc.WaitForStatus(ServiceControllerStatus.Running);
                        sc.Stop();
                    }

                }
                else if (sc.Status.Equals(ServiceControllerStatus.Paused))
                {

                    sc.Stop();

                }
                else if (sc.Status.Equals(ServiceControllerStatus.PausePending))
                {
                    if (WaitChangeStatus)
                    {
                        sc.WaitForStatus(ServiceControllerStatus.Paused);
                        sc.Stop();
                    }
                }

            }

        }
    }
}
