using System;
using System.IO;
using WolvenKit.Modkit.RED4.Serialization;
using WolvenKit.RED4.TweakDB;

namespace TweakCompiler
{
    class TweakCompiler
    {
        static void Main(string[] args)
        {
            try
            {
                if (Directory.Exists("tweakdbs"))
                {
                    Directory.Delete("tweakdbs", true);
                }
                Directory.CreateDirectory("tweakdbs");
            }
            catch (Exception e)
            {
               Console.WriteLine(e);
            }

            var tweakFiles = Directory.GetFiles("tweaks", "*.tweak", SearchOption.AllDirectories);
            foreach (var f in tweakFiles)
            {
                var text = File.ReadAllText(f);
                var filename = Path.GetFileNameWithoutExtension(f) + ".bin";
                var outPath = Path.Combine("tweakdbs", filename);

                try
                {
                    if (!Serialization.Deserialize(text, out var dict))
                    {
                        continue;
                    }
                    var db = new TweakDB();
                    //flats
                    foreach (var (key, value) in dict.Flats)
                    {
                        db.Add(key, value);
                    }
                    //groups
                    foreach (var (key, value) in dict.Groups)
                    {
                        db.Add(key, value);
                    }

                    db.Save(outPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    continue;
                }
            }
        }
    }
}
