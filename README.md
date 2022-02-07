BN_Astyle
================

An extension for Visual Studio 2022 to format C/C++ documents using Artistic Style engine.

More info on Artistic Style at http://astyle.sourceforge.net/

Supported Visual Studio editions:

    Visual Studio 2022 Community
    Visual Studio 2022 Professional
    Visual Studio 2022 Enterprise

Features:
1. Ability to set astyle arguments `Tools -> Options -> BN_Astyle`
2. Optional automatic format on save `Tools -> Options -> BN_Astyle`
3. Command to format current C/C++ document `Edit -> Format Document (BN_Astyle)`


## Using with Cataclysm Bright Nights project:

### Installing from Releases
1. Download `BN_Astyle.zip` from Releases page
2. Unpack anywhere
3. Double click `.vsix` to install. Visual Studio installer should guide you from there

### Installing from source
1. Check out the repository
2. Open `BN_Astyle.sln`
3. Install any workloads Visual Studio prompts you to (.NET framework 4.8 and Extension SDK)
4. Select `Release` / `Any CPU` configuation
5. Build Solution
6. If there were no issues, resulting `.vsix` should be in `BN_Astyle\bin\Release`. Double click to install

### Configuring
1. Go to `Tools -> Options -> BN_Astyle`
2. Enable automatic `Format on save` (or don't, depends on how you like it)
3. In BN repository, open `msvc-full-features\AStyleExtension-Cataclysm-DDA.cfg` with any text editor, copy contents of `CppCommandLine` entry and paste into `Formatting options`
4. Close options window by pressing `Ok` in bottom-right corner
