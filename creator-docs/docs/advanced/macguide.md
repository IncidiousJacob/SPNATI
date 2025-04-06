# Using SPNatI Dev Tools on Mac OS

Run the SPNatI Character Editor on Mac via Whisky — A free application!

---

The SPNatI Character Editor is essential for making a modern SPNatI character, since it takes care of the otherwise complicated .xml coding. Unfortunately there is no mac-native version of the Character Editor for Mac OS, nor would it be feasible to develop one. Using a version of [Wine](https://www.winehq.org/) to run the CE has been the go-to of Mac-primary devs for years, but it hasn't been without its issues. All 32-bit Wine wrappers were broken when Apple removed 32-bit support in Catalina, and modern equivalents like Codeweavers' Crossover require you to buy a license. 

Thankfully, you can easily and painlessly develop SPNatI characters on MAC OS using [Whisky, a free and open-source compatibility software!](https://getwhisky.app/) Just follow these steps below to build a working bottle. [You'll need to have already downloaded the git repository for SPNatI,](../basics/githubdesktop.html) so do that first.

!!! note
	Whisky requires macOS Sonoma 14.0 or later.

---

## Installing and Running the Character Editor

### Step 1

[Download Whisky and open it.](https://getwhisky.app/)

### Step 2

Click the + icon (in the top right if you've already installed another program) to make a new bottle. A "bottle" is essentially a virtual computer with only the programs you need installed. Because different programs have different dependencies (and the CE has a few), it's better to keep different programs installed into different bottles. For that reason, running the CE via command-line Wine isn't preferred.

You can give the bottle any name that makes sense. Windows version **must** be Windows 10 or 11, earlier versions do not support the Character Editor. Leave the bottle path the default unless you have a reason to change it.

Hit create. The program will take a few moments to build a new bottle for you.

![The new bottle options window](../img/macguide/macguide_01.png)

### Step 3

When the bottle is ready, click the Open C: Drive button in the bottom left, and navigate to the `Program Files (x86)` folder inside. Since the Character Editor doesn't have an installer, you'll need to manually copy it over. In a separate tab or window, open the git repository and navigate to `/spnati/tools/character_editor/`. Unzip and copy over the latest version of the CE. **Make sure you copy over the entire folder you unzipped.**

![The location of the Open C: Drive button](../img/macguide/macguide_02.png)

### Step 4

In order for the Character Editor to run at all, you will need to install several dependencies into your bottle, known as **Winetricks**. Thankfully, Whisky makes this easy! To start, click the Winetricks button in the bottom right of your window.

![The Winetricks button](../img/macguide/macguide_03.png)

![The Winetricks window](../img/macguide/macguide_04.png)

In the Winetricks window that pops up, click on the DLLs tab. **There are three specific DLLs you will need to install.**

| Command | Description | Notes |
| ----------- | ----------- | ----------- |
| dotnet472 | Microsoft .NET Framework 4.7.2 | This will take a million years as you have to install every previous version of .NET first. You'll have to accept the license agreement on each install as well. Whatever other options pop up don't matter. |
| vcrun2017 | Visual C++ 2017 libraries | You will need to accept the licenses. | 
| msxml6 | MS XML Core Services 6.0 | | 

Be patient as all the necessary dependencies are installed.

### Step 5

Once all Winetricks have been installed, click the `Run…` button and navigate to `/drive_c/Program Files (x86)/Character Editor/SPNATI Character Editor.exe`. Hit "Open".

![Run](../img/macguide/macguide_05.png)

If all goes well, the SPNatI character editor should launch!

!!! note
	The Character Editor may crash the first time you run it. If that happens, just try running it again. If the problem persists, try re-installing the necessary Winetricks. Wine may also ask you to install additional dependencies when starting up the program for the first time, such as Gecko.

### Step 6

For ease of access, and so you don't have to use the Run command every time you want to use the Character Editor, you can pin a program to the bottle window. Just click the Pin Program icon, give it a name, and browse to `/drive_c/Program Files (x86)/Character Editor/SPNATI Character Editor.exe`. You can also pin programs in the bottle's "Installed Programs" menu (you may need to run it once before it shows up).

---

## Installing KKL

It's not recommended to work in Kisekae through Whisky— it's way too laggy. Instead, use the Mac-native KKL.app included in the `/spnati/tools/kkl/` folder in the repository.

However, the Mac version of KKL can't interface with the Character Editor run through Whisky. If you want to export poses automatically through the CE's Pose Matrix, you'll need to install kkl.exe into the same bottle as the Character Editor.

To install, go to `/spnati/tools/kkl/` in the repository and unzip the latest version of KKL. Grab the folder you just unzipped, open the C: Drive in your bottle, and copy it to the `Program Files (x86)` folder next to the Character Editor folder.

Then, simply click `Run…` and navigate to kkl.exe. You can also pin it for easy access. KKL should open without issue.