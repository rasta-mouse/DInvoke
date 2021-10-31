# D/Invoke

Fork of [D/Invoke by TheWover](https://github.com/TheWover/DInvoke), but refactored to .NET Standard 2.0 and split into individual [NuGet packages](https://nuget.rastamouse.me/?q=DInvoke).

## Why?

The aim of this project is to provide D/Invoke in a more minimalist form.  It only contains the core DynamicInvoke and ManualMap functionality, without all the additional helper methods and delegates.  This help keeps the packages small and lowers the detection surface for AV.  The [dinvoke.net Wiki](https://dinvoke.net) contains a library of useful delegates that you can add to your projects.