using System.Configuration;
using System.Data;
using System.Windows;
using System.IO;
using System.Reflection;
using System;

namespace SimpleMetronome
{
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += LoadFromLibsFolder;
        }

        private Assembly LoadFromLibsFolder(object sender, ResolveEventArgs args)
        {
            string baseFolder = AppDomain.CurrentDomain.BaseDirectory;
            string libsFolder = Path.Combine(baseFolder, "libs");
            string assemblyPath = Path.Combine(libsFolder, new AssemblyName(args.Name).Name + ".dll");

            if (File.Exists(assemblyPath))
            {
                return Assembly.LoadFrom(assemblyPath);
            }
            return null;
        }
    }
}


