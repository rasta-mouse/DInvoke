global using DInvoke.ManualMap;

namespace DInvoke.Tests;

public sealed class ManualMapTests
{
    [Fact]
    public void MapModuleFromDiskToSection()
    {
        var map = Map.MapModuleFromDiskToSection(@"C:\Windows\System32\secur32.dll");
        Assert.False(map.ModuleBase == IntPtr.Zero);
    }

    [Fact]
    public void FindDecoyModule()
    {
        var decoy = Overload.FindDecoyModule(666);
        Assert.False(string.IsNullOrWhiteSpace(decoy));
    }

    [Fact]
    public void OverloadModule()
    {
        const string decoy = @"C:\Windows\System32\xpsservices.dll";
        
        var map = Overload.OverloadModule(
            @"C:\Windows\System32\ntdll.dll",
            decoy);
        
        Assert.Equal(decoy, map.DecoyModule);
        Assert.False(map.ModuleBase == IntPtr.Zero);
    }
}