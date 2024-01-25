global using DInvoke.DynamicInvoke;

namespace DInvoke.Tests;

public sealed class DynamicInvokeTests
{
    private const long KEY = 0xdeadbeef;
    
    [Fact]
    public void GetLibraryAddressByName()
    {
        var hFunction = Generic.GetLibraryAddress(
            "kernel32.dll",
            "OpenProcess");
        
        Assert.False(hFunction == IntPtr.Zero);
    }

    [Fact]
    public void GetLibraryAddressByOrdinal()
    {
        var ordinal = IntPtr.Size == 8
            ? 0x42F
            : 0x42D;
        
        var hFunction = Generic.GetLibraryAddress(
            "kernel32.dll",
            (short)ordinal);
        
        Assert.False(hFunction == IntPtr.Zero);
    }

    [Fact]
    public void GetLibraryAddressByHash()
    {
        var hash = Utilities.GetApiHash("OpenProcess", KEY);
        var hFunction = Generic.GetLibraryAddress(
            "kernel32.dll",
            hash,
            KEY);
        
        Assert.False(hFunction == IntPtr.Zero);
    }

    [Fact]
    public void GetLoadedModuleAddressByHash()
    {
        var hash = Utilities.GetApiHash("kernel32.dll", KEY);
        var hLibrary = Generic.GetLoadedModuleAddress(
            hash,
            KEY);
        
        Assert.False(hLibrary == IntPtr.Zero);
    }

    [Fact]
    public void GetPebAddress()
    {
        var hPeb = Generic.GetPebAddress();
        Assert.False(hPeb == IntPtr.Zero);
    }

    [Fact]
    public void GetPebLdrModuleEntry()
    {
        var hModule = Generic.GetPebLdrModuleEntry("kernel32.dll");
        Assert.False(hModule == IntPtr.Zero);
    }

    [Fact]
    public void GetSyscallStubByName()
    {
        byte[] expected =
        [
            0x49, 0x89, 0xCA,                // mov r10, rcx
            0xB8, 0x26, 0x00, 0x00, 0x00,    // mov eax, ssn
            0x0F, 0x05,                      // syscall
            0xC3                             // ret
        ];

        var pPeb = Generic.GetPebAddress();
        var stub = Generic.GetSyscallStub(pPeb, "NtOpenProcess");
        
        Assert.Equal(expected, stub);
    }

    [Fact]
    public void GetSyscallStubByHash()
    {
        byte[] expected =
        [
            0x49, 0x89, 0xCA,                // mov r10, rcx
            0xB8, 0x26, 0x00, 0x00, 0x00,    // mov eax, ssn
            0x0F, 0x05,                      // syscall
            0xC3                             // ret
        ];

        var hash = Utilities.GetApiHash("NtOpenProcess", KEY);
        var pPeb = Generic.GetPebAddress();
        var stub = Generic.GetSyscallStub(pPeb, hash, KEY);
        
        Assert.Equal(expected, stub);
    }

    [Fact]
    public void GetNativeExportAddress()
    {
        var hModule = Generic.GetLoadedModuleAddress("kernel32.dll");
        var hExport = Generic.GetNativeExportAddress(hModule, "OpenProcess");
        
        Assert.False(hExport == IntPtr.Zero);
    }
}