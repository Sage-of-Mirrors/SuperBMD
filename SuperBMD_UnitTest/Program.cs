using System;
using System.IO;
using SuperBMD;

namespace SuperBMD_UnitTest
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

            string[] processed_args = ProcessArgs(args);
            ValidateArgs(processed_args);

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
        /// Deconstructs the input from the user to a standardized array of settings.
        /// </summary>
        /// <param name="args">The raw input from the user</param>
        /// <returns>An array of settings parsed from the user's input</returns>
        private static string[] ProcessArgs(string[] args)
        {
            // This array holds the paths or other data (bools, enum types) parsed from the arguments
            string[] processed_args = new string[] { "", "", "", "", "static", "false", "true" };

            for (int i = 0; i < args.Length; i++)
            {
                if (i + 1 >= args.Length)
                    throw new Exception("The parameters were malformed.");

                switch(args[i])
                {
                    case "-i":
                    case "--input":
                        processed_args[0] = args[i + 1];
                        break;
                    case "-o":
                    case "--output":
                        processed_args[1] = args[i + 1];
                        break;
                    case "-m":
                    case "--materialPresets":
                        processed_args[2] = args[i + 1];
                        break;
                    case "-x":
                    case "--texHeaders":
                        processed_args[3] = args[i + 1];
                        break;
                    case "-t":
                    case "--tristrip":
                        processed_args[4] = args[i + 1].ToLower();
                        break;
                    case "-r":
                    case "--rotate":
                        processed_args[5] = "true";
                        break;
                    case "-f":
                    case "--fixNormals":
                        processed_args[6] = "false";
                        break;
                    default:
                        throw new Exception($"Unknown parameter \"{ args[i] }\"");
                }

                // Increment the counter to skip to the next parameter
                i++;
            }

            return processed_args;
        }

        /// <summary>
        /// Ensures that all the settings parsed from the user's input are valid.
        /// </summary>
        /// <param name="args">Array of settings parsed from the user's input</param>
        private static void ValidateArgs(string[] args)
        {
            // Input
            if (args[0] == "")
                throw new Exception("No input file was specified.");
            if (!File.Exists(args[0]))
                throw new Exception($"Input file \"{ args[0] }\" does not exist.");

            // Output
            if (args[1] == "")
            {
                string input_without_ext = Path.GetFileNameWithoutExtension(args[0]);

                if (args[0].EndsWith(".bmd") || args[0].EndsWith(".bdl"))
                    args[1] = input_without_ext + ".dae";
                else
                    args[1] = input_without_ext + ".bmd";
            }

            // Material presets
            if (args[2] != "")
            {
                if (!File.Exists(args[2]))
                    throw new Exception($"Material presets file \"{ args[2] }\" does not exist.");
            }

            // Texture headers
            if (args[3] != "")
            {
                if (!File.Exists(args[3]))
                    throw new Exception($"Texture headers file \"{ args[3] }\" does not exist.");
            }

            // Tristrip options
            if (args[4] != "static" && args[4] != "all" && args[4] != "none")
                throw new Exception($"Unknown tristrip option \"{ args[4] }\".");
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
