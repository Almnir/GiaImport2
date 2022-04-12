using DevExpress.Skins;
using DevExpress.UserSkins;
using DevExpress.XtraEditors;
using GiaImport2.Services;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Threading;
using System.Windows.Forms;

namespace GiaImport2
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(false, "GiaImportRunning");
            try
            {
                if (mutex.WaitOne(0, false))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    BonusSkins.Register();
                    SkinManager.EnableFormSkins();

                    var container = Bootstrap();

                    Application.Run(container.GetInstance<MainForm>());
                    //Application.Run(new MainForm());
                }
                else
                {
                    MessageBox.Show("Приложение GiaImport уже запущено!");
                }
            }
            finally
            {
                if (mutex != null)
                {
                    mutex.Close();
                    mutex = null;
                }
            }
        }

        private static Container Bootstrap()
        {
            // Create the container as usual.
            var container = new Container();

            // Register your types, for instance:
            container.Register<ICommonRepository, CommonRepository>(Lifestyle.Transient);

            AutoRegisterWindowsForms(container);

            container.Verify();

            return container;
        }
        private static void AutoRegisterWindowsForms(Container container)
        {
            var types = container.GetTypesToRegister<XtraForm>(typeof(Program).Assembly);

            foreach (var type in types)
            {
                var registration =
                    Lifestyle.Transient.CreateRegistration(type, container);

                registration.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "Forms should be disposed by app code; not by the container.");

                container.AddRegistration(type, registration);
            }
        }
    }
}
