# TriggersTools.SteinsGate

A .NET Standard library for generating divergence meter displays. It may contain other Steins;Gate features in the future. Drawing is done using the .NET Standard library `System.Drawing.Common`, which is available as a Nuget package.

***

<<<<<<< HEAD
# Divergence ![AppIcon](https://i.imgur.com/D2U0KkP.png)
=======
# Divergence ![AppIcon](https://i.imgur.com/Ia9zq9O.png)

[![NuGet Version](https://img.shields.io/nuget/v/TriggersTools.SteinsGate.Divergence.svg?style=flat)](https://www.nuget.org/packages/TriggersTools.SteinsGate.Divergence/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/TriggersTools.SteinsGate.Divergence.svg?style=flat)](https://www.nuget.org/packages/TriggersTools.SteinsGate.Divergence/)
[![Creation Date](https://img.shields.io/badge/created-june%202018-A642FF.svg?style=flat)](https://github.com/trigger-death/TerrariaMidiPlayer/commit/2a6570de78f8c2fd8816b8ba9380614e1badec0f)

>>>>>>> 75940969318f3e6ce47814334d81e1a913641e17
The only existing feature in the library at the moment. Allows you to programatically draw [Divergence Meter](http://steins-gate.wikia.com/wiki/Divergence_Meter) nixie tubes from the visual novel & anime: [Steins;Gate](https://vndb.org/v2002).

`Divergence` uses graphics taken from the Steins;Gate visual novel for drawing the tubes, digits, and decimal point. The rest of the available characters are drawn with the [Oslo II font, by Antonio Rodrigues Jr](http://www.1001fonts.com/oslo-ii-font.html). This font was chosen as it had a similar style as well as perfect aspect ratio for each character.

## Basic Examples

Below is example code used to draw nixie tubes using the `Divergence` static class. At the moment, nixie tubes can be drawn at 3 pre-scaled sizes: `Large` <sup>(132x428px per tube)</sup>, `Medium` <sup>(66x214px per tube)</sup>, or `Small` <sup>(33x107px per tube)</sup>. The library currently doesn't support drawing at other scales, but that could be added later.

```cs
using TriggersTools.SteinsGate;

// Draw Figure A
var args = new DivergenceArgs {
    Scale = DivergenceScale.Medium,
    Spacing = new DivergenceSpacing(8, 8),
    Background = Color.FromArgb(224, 224, 224),
};
string text = "Oslo II";
using (Bitmap bmp = Divergence.Draw(text, args))
    bmp.Save("OsloII.png");

// Draw Figure B
args.Scale = DivergenceScale.Small;
args.Spacing = new DivergenceSpacing(5, 5);
DateTime date = DateTime.Now;
text = $"{date:MM\\/dd\\/yy}\n{date.TimeOfDay:hh\\:mm\\:ss}";
using (Bitmap bmp = Divergence.Draw(text, args))
    bmp.Save("DateTime.png");
```

![Figure A + B](https://i.imgur.com/sA8IIKq.png)

*(Figure B: Displaying the date and time)*
*(Figure A: Displaying the name of the font used for all other characters)*

## Backgrounds

As you can see in the code below, `DivergenceArgs.Background` accepts `string` as well. `DivergenceArgs.Background` is actually a struct called `DivergenceBackground` that can draw a background color, and/or a bitmap that is optionally scaled to fit the output image size.

```cs
// Draw Figure C
args = new DivergenceArgs {
    Scale = DivergenceScale.Small,
    Background = "EV_Z02A.PNG", // The CG background
};
text = "1.130426";
Divergence.CalculateSpacingFor(1920 / 2, 1080 / 2, text, ref args, left: 5, top: 2);
using (Bitmap bmp = Divergence.Draw(text, args))
    bmp.Save("Original Worldline.png");
```

![Figure C](https://i.imgur.com/MuZIeIz.png)

*(Figure C: Drawing the divergence meter onto a CG to display the worldline)*

## Escaping

In many scenarios, the user may not be able to pass actual new line characters if not done programmatically. `DivergenceArgs.Escape` uses the `DivergenceEscape` enum to allow 3 choices: `None` <sup>(Don't escape anything)</sup>, `NewLines` <sup>(Only escape \r and \n)</sup>, and `All` <sup>(Escape any character after '\\')</sup>.

An example would be in the command line: `divergence.exe "#1\n#2"`. (Note: No command line program exists for this library at the moment.)

![Figure D](https://i.imgur.com/tEnTLVQ.png)

*(Figure D: The supposed output from the command line example above)*

## Example use of this library

`Divergence` isn't extremely useful or anything, but it can be fun to use as a bot command. Some of the features were specifically designed for use with an automated system.

```cs
// Automatically throw an ArgumentException if a formatted line
// is longer than 24 characters or there are more than 3 lines.
Divergence.EnabledLimits = true;
Divergence.MaxLength = 24;
Divergence.MaxLines = 3;
```

![Figure E](https://i.imgur.com/Ls2YRMl.png)

*(Figure E: The '\\' before the space is used to escape it so that it is included in the command)*
