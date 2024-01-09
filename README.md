# D/Invoke

Fork of [D/Invoke by TheWover](https://github.com/TheWover/DInvoke), but refactored to .NET Standard 2.0 and split into individual NuGet packages.

## Why?

The aim of this project is to provide D/Invoke in a more minimalist form.  It only contains the core DynamicInvoke and ManualMap functionality, without all the additional helper methods and delegates.  This help keeps the packages small and lowers the detection surface for AV.

## Examples

### DynamicApiInvoke

Define the delegates and any strucs/enums that you need.  Some common ones are provided in the `DInvoke.Data` namespace.

```c#
[UnmanagedFunctionPointer(CallingConvention.StdCall)]
private delegate Data.Native.NTSTATUS NtOpenProcessDelegate(
    out SafeProcessHandle processHandle,
    Data.Win32.WinNT.ACCESS_MASK desiredAccess,
    Data.Native.OBJECT_ATTRIBUTES objectAttributes,
    CLIENT_ID clientId);

private struct CLIENT_ID
{
    public IntPtr UniqueProcess;
    public IntPtr UniqueThread;
}
```

Initialise the parameters and bundle them into an `object[]`.

```c#
var handle = new SafeProcessHandle();
var oa = new Data.Native.OBJECT_ATTRIBUTES();
var cid = new CLIENT_ID { UniqueProcess = 1234 };

object[] parameters = [ handle, Data.Win32.WinNT.ACCESS_MASK.MAXIMUM_ALLOWED, oa, cid ];
```

Call `DynamicApiInvoke` specifying the DLL name, the API name, the delegate, and the function parameters.  The output data type is specified as a generic, `T`.  Most NT APIs return a type of `NTSTATUS`, which is a `uint`.

```c#
var status = DynamicInvoke.Generic.DynamicApiInvoke<uint>(
    "ntdll.dll",
    "NtOpenProcess",
    typeof(NtOpenProcessDelegate),
    ref parameters);
```

Parameters marked as `out` need to be read out of the `parameters` array and cast back onto the original variable.

```c#
handle = (SafeProcessHandle)parameters[0];
```

### Function Hashing

Use a tool such as [CSharpRepl](https://github.com/waf/CSharpRepl) to import `DInvoke.DynamicInvoke.dll`.  Call `Utilities.GetApiHash` to generate a hash for the desired DLL name and API.

```
> #r "C:\Tools\DInvoke\DInvoke.DynamicInvoke\bin\Debug\netstandard2.0\DInvoke.DynamicInvoke.dll"

> DInvoke.DynamicInvoke.Utilities.GetApiHash("ntdll.dll", 0x123456789)
"9BC00C9AC691986FE3CEEDA6E12F9FB0"

> DInvoke.DynamicInvoke.Utilities.GetApiHash("NtOpenProcess", 0x123456789)
"9713035EC6AE7BB32303F84822AB80AA"
```

Use `GetLoadedModuleAddress` to resolve the address of the DLL and `GetExportAddress` to resolve the address of the target function.

```c#
var hModule = DynamicInvoke.Generic.GetLoadedModuleAddress(
    "9BC00C9AC691986FE3CEEDA6E12F9FB0", // ntdll.dll
    key);

var hPointer = DynamicInvoke.Generic.GetExportAddress(
    hModule,
    "9713035EC6AE7BB32303F84822AB80AA", // NtOpenProcess
    key);
```

The rest of the steps are the same as above, but we call `DynamicFunctionInvoke` instead.

```c#
var status = DynamicInvoke.Generic.DynamicFunctionInvoke<uint>(
    hPointer,
    typeof(NtOpenProcessDelegate),
    ref parameters);
```