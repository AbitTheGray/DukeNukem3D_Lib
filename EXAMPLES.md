# Examples

## Export all textures

For those examples, data of Duke Nukem 3D are in `Game` directory and saved into `duke3d.grp.d` directory (images go into `ART` subdirectory). 

### Without animations

```c#
string inputGrpFile = Path.Combine("Game", "duke3d.grp");
string outputGrpDir = "duke3d.grp.d";

var grp = Grp.Parse(inputGrpFile);

if(!Directory.Exists(outputGrpDir))
    Directory.CreateDirectory(outputGrpDir);

foreach (var gFile in grp.Files)
    if(string.Equals(Path.GetExtension(gFile.Filename), ".ART", StringComparison.InvariantCultureIgnoreCase))
        using (BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(outputGrpDir, gFile.Filename), FileMode.Create, FileAccess.Write)))
            writer.Write(gFile.FileRawData);

PaletteFile palette = PaletteFile.Parse(Path.Combine(outputGrpDir,"PALETTE.DAT"));


string artDirname = Path.Combine(outputGrpDir, "ART");
if(!Directory.Exists(artDirname))
    Directory.CreateDirectory(artDirname);

var artFiles = ArtFile.ParseAllFilesInDirectory(outputGrpDir);
foreach (ArtFile aFile in artFiles)
{
    foreach (Texture texture in aFile.Textures)
    {
        if(texture == null)
            continue;

        texture.Palette = palette;
        
        texture.AsBitmap.Save(Path.Combine(artDirname, $"{texture.TextureID}.png"));
    }
}
```

### With Animations

Requires [AnimatedGif](https://www.nuget.org/packages/AnimatedGif/) library.

```c#
string inputGrpFile = Path.Combine("Game", "duke3d.grp");
string outputGrpDir = "duke3d.grp.d";

var grp = Grp.Parse(inputGrpFile);

if(!Directory.Exists(outputGrpDir))
    Directory.CreateDirectory(outputGrpDir);

foreach (var gFile in grp.Files)
    if(string.Equals(Path.GetExtension(gFile.Filename), ".ART", StringComparison.InvariantCultureIgnoreCase))
        using (BinaryWriter writer = new BinaryWriter(File.Open(Path.Combine(outputGrpDir, gFile.Filename), FileMode.Create, FileAccess.Write)))
            writer.Write(gFile.FileRawData);

PaletteFile palette = PaletteFile.Parse(Path.Combine(outputGrpDir,"PALETTE.DAT"));

string artDirname = Path.Combine(outputGrpDir, "ART");
if(!Directory.Exists(artDirname))
    Directory.CreateDirectory(artDirname);
        
var artFiles = ArtFile.ParseAllFilesInDirectory(outputGrpDir);
foreach (ArtFile aFile in artFiles)
{
    for(int i = 0; i < aFile.TexturesCount; i++)
    {
        var texture = aFile.Textures[i];
        if(texture == null)
            continue;

        texture.Palette = palette;

        if (texture.Animation.Type == Animation.AnimationType.NoAnimation || texture.Animation.NumberOfFrames == 0)
            texture.AsBitmap.Save(Path.Combine(artDirname, $"{texture.TextureID}.png"));
        else
        {
            using (var gif = AnimatedGif.AnimatedGif.Create(Path.Combine(artDirname, $"{texture.TextureID}.gif"), 10 * (texture.Animation.AnimationSpeed + 1)))
            {
                for (int frame = 0; frame <= texture.Animation.NumberOfFrames; frame++, i++)
                {
                    var animTexture = aFile.Textures[texture.TextureIDInFile + frame];
                    if(animTexture == null)
                        continue;
                    animTexture.Palette = palette;
                    
                    gif.AddFrame(animTexture.AsBitmap, -1, GifQuality.Bit8);
                }

                i--;
            }
        }
    }
}
```
