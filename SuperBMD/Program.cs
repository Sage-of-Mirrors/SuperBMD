using System;

namespace SuperBMD
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0 || args[0] == "-h" || args[0] == "--help")
            {
                DisplayHelp();
                return;
            }

            Arguments cmd_args = new Arguments(args);

            /*
            if (args.Length > 0)
            {
                if (args[0] == "help")
                {
                    DisplayHelp();
                    return;
                }

                Model mod = Model.Load(args[0]);

                if (args[0].EndsWith(".bmd") || args[0].EndsWith(".bdl"))
                    mod.ExportAssImp(args[0], "dae", new ExportSettings());
                else
                    mod.ExportBMD(args[0] + ".bmd");
            }
            else
                DisplayHelp();
            */
        }

        /// <summary>
        /// Prints credits and argument descriptions to the console.
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("SuperBMD: A tool to import and export various 3D model formats into the Binary Model (BMD) format.");
            Console.WriteLine("Written by Sage_of_Mirrors/Gamma (@SageOfMirrors) and Yoshi2/RenolY2.");
            Console.WriteLine("Made possible with help from arookas, LordNed, xDaniel, and many others.");
            Console.WriteLine("Visit https://github.com/Sage-of-Mirrors/SuperBMD/wiki for more information.");
            Console.WriteLine();
            Console.WriteLine("Usage: SuperBMD.exe -i/--input filePath [-o/--output filePath] [-m/materialPresets filePath]\n" +
                              "       [-x/--texHeaders filePath] [-t/--tristrip mode] [-r/--rotate] [-f/--fixNormals]");
            Console.WriteLine();
            Console.WriteLine("Parameters:");
            Console.WriteLine("\t-i/--input              filePath\tPath to the input file, either a BMD/BDL file or a DAE model.");
            Console.WriteLine("\t-o/--output             filePath\tPath to the output file.");
            Console.WriteLine("\t-m/--materialPresets    filePath\tPath to the material presets JSON for DAE to BMD conversion.");
            Console.WriteLine("\t-x/--textureHeaders     filePath\tPath to the texture headers JSON for DAE to BMD conversion.");
            Console.WriteLine("\t-t/--tristrip           mode\t\tMode for tristrip generation.");
            Console.WriteLine("\t\tstatic: Only generate tristrips for static (unrigged) meshes.");
            Console.WriteLine("\t\tall:    Generate tristrips for all meshes.");
            Console.WriteLine("\t\tnone:   Do not generate tristrips.");
            Console.WriteLine();
            Console.WriteLine("\t-r/--rotate\t\t\t\tRotate the model from Z-up to Y-up orientation.");
            Console.WriteLine("\t-f/fixNormals\t\t\t\tModify the normals in the mesh to facilitate proper in-game shading.");
        }
    }
}
