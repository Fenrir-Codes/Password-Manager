using PasswordManager.Data;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // A konstruktorban beállítjuk a DLL keresést a 'libs' mappában
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "libs", assemblyName);

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                return null;
            };
        }

        // Az OnStartup-ban pedig elindítjuk az adatbázist (miután a DLL-eket már megtalálja a program)
        protected override void OnStartup(StartupEventArgs e)
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            base.OnStartup(e);
        }
    }

}
