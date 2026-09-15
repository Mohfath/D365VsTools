using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// MscrmTools.Xrm.Connection (Add Connection dialog) needs ADAL 5.2.9, but Microsoft.CrmSdk.XrmTooling.CoreAssembly
// was compiled against ADAL 3.19.8.16603. NuGet unifies the deployed DLL to 5.2.9.0, so without this redirect the
// CLR refuses to load it for the 3.19.8.16603 request: "Could not load file or assembly
// 'Microsoft.IdentityModel.Clients.ActiveDirectory, Version=3.19.8.16603...'".
[assembly: ProvideBindingRedirection(
    AssemblyName = "Microsoft.IdentityModel.Clients.ActiveDirectory",
    PublicKeyToken = "31bf3856ad364e35",
    OldVersionLowerBound = "0.0.0.0",
    OldVersionUpperBound = "5.2.9.0",
    NewVersion = "5.2.9.0")]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Dynamics 365 Tools for VisualStudio")]
[assembly: AssemblyDescription("Dynamics 365 Tools for VisualStudio")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("CSharpAngels")]
[assembly: AssemblyProduct("Dynamics 365 Tools for VisualStudio")]
[assembly: AssemblyCopyright("Copyright © CSharpAngels 2022")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.25063.1139")]
[assembly: AssemblyFileVersion("1.0.2025.0304")]
