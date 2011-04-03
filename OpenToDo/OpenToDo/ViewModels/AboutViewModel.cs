using System;
using System.Reflection;
using System.Windows.Interop;
using OpenToDo;

public class AboutViewModel
{
    public AboutViewModel()
    {
        string name = Assembly.GetExecutingAssembly().FullName;
        AssemblyName assemblyName = new AssemblyName(name);

        Version = assemblyName.Version.ToString();
    }

    public string Version { get; set; }
}