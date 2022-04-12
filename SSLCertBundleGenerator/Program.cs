namespace SSLCertBundleGenerator
{
    using SSLCertBundleGenerator.Commons;
    using SSLCertBundleGenerator.Windows;

    internal static class Program
    {
        private static void InternalMain(string[] args)
        {
            if (!EnvironmentUtils.IsUnix())
            {
                if (EnvironmentUtils.IsAtLeastWindows10())
                    NativeMethods.SetProcessDpiAwareness(NativeMethods.ProcessDpiAwareness.ProcessSystemDpiAware);
                else if (EnvironmentUtils.IsAtLeastWindowsVista()) NativeMethods.SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormSslCertBundleGenerator());
        }

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            InternalMain(args);
        }
    }
}