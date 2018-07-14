using System;
using SuperBMDLib;

namespace SuperBMDLib
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

            Model mod = Model.Load(cmd_args);

            if (cmd_args.input_path.EndsWith(".bmd") || cmd_args.input_path.EndsWith(".bdl"))
                mod.ExportAssImp(cmd_args.output_path, "dae", new ExportSettings());
            else
                mod.ExportBMD(cmd_args.output_path);
        }

        /// <summary>
        /// Prints credits and argument descriptions to the console.
        /// </summary>
        private static void DisplayHelp()
        {
            Console.WriteLine();
            Console.WriteLine("SuperBMD: A tool to import and export various 3D model formats into the Binary Model (BMD or BDL) format.");
            Console.WriteLine("Written by Sage_of_Mirrors/Gamma (@SageOfMirrors) and Yoshi2/RenolY2.");
            Console.WriteLine("Made possible with help from arookas, LordNed, xDaniel, and many others.");
            Console.WriteLine("Visit https://github.com/Sage-of-Mirrors/SuperBMD/wiki for more information.");
            Console.WriteLine();
            Console.WriteLine("Usage: SuperBMD.exe -i/--input filePath [-o/--output filePath] [-m/materialPresets filePath]\n" +
                              "       [-x/--texHeaders filePath] [-t/--tristrip mode] [-r/--rotate] [-b/--bdl]");
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
            Console.WriteLine("\t-b/--bdl\t\t\t\tGenerate a BDL instead of a BMD.");
        }
    }
}
