using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUSGov_CSVReWriter
{
    class Program
    {
        private static AUSGov_CSVReWriter.Config.ConfigHelper _configHelper;
        // Command line option Config file location
        public static string _CLIOption_GlobalConfigFileLocation = null;
        // Skip Wiring Processing execution for help/info display type commands
        private static bool _CLIOption_SkipExecution = false;

        static void Main(string[] args)
        {
            Console.WriteLine("AUSGov_CSVReWriter starting... (-h for help)");

            // Command line Options
            int arg_index = 0;
            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "-h":
                        Display_UsageArgs();
                        _CLIOption_SkipExecution = true;
                        break;
                    case "-v":
                        Display_Version();
                        _CLIOption_SkipExecution = true;
                        break;
                    case "-c":
                        _CLIOption_GlobalConfigFileLocation = args[arg_index + 1];
                        break;

                }
                arg_index++;
            }

            // If processing file
            if (!_CLIOption_SkipExecution)
            {
                // Load Config
                _configHelper = AUSGov_CSVReWriter.Config.ConfigHelper.LoadConfig(_CLIOption_GlobalConfigFileLocation);

                if (_configHelper != null)
                {
                    Console.WriteLine($"Main: Config loaded, Trying to process file");
                    ProcessFile();
                    Console.WriteLine($"Main: Finished");
                }
            }
        }

        private static void Display_UsageArgs()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Usage: {System.Reflection.Assembly.GetEntryAssembly().GetName().Name} [options]");
            Console.WriteLine("\nOptions:\n");
            Console.WriteLine("\t-h\t\tDisplay help");
            Console.WriteLine("\t-v\t\tDisplay version infomation");
            Console.WriteLine("\t-c <path>\tConfig.xml path");
        }

        /// <summary>
        /// Outputs application version information to command line
        /// </summary>
        private static void Display_Version()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name} [Version: {System.Reflection.Assembly.GetEntryAssembly().GetName().Version}]");
        }

        static void ProcessFile()
        {
            string _inputFileName = _configHelper.InputFileName;
            string[] lines = System.IO.File.ReadAllLines(_inputFileName);

            string _outFileName = _configHelper.OutputFileName;
            System.IO.StreamWriter _streamWriter = new System.IO.StreamWriter(_outFileName);
            int start = 0;

            // Write Headers
            if (_configHelper.FirstRowIsHeader)
            // If first row is column heading, write headers to CSV
            {
                start++;

                WriteCSVHeaderRow(_streamWriter);
            }


            // Process the Lines
            for (int rowIndex = start; rowIndex < lines.Length; rowIndex++)
            {
                // Remove expression
                string input = lines[rowIndex];
                string final = null;

                int whiteSpacesInserted = 0;
                // Check for any Fixed Length columns and insert delimiter
                foreach (AUSGov_CSVReWriter.Config.Column column in _configHelper.ColumnDefinitions)
                {
                    if (column.Field.IsFixedSize)
                        input = input.Insert(column.Field.DelimitPossition + whiteSpacesInserted * 2, "  ");
                }

                //Regex regex = new Regex(_configHelper.WhiteSpaceExpression);
                //var o = Regex.Matches(input, _configHelper.WhiteSpaceExpression);
                final = Regex.Replace(input, _configHelper.WhiteSpaceExpression, _configHelper.DelimitCharacter);

                // Check if whitespace was delimited at end of line, remove it
                if (final[final.Length - 1] == (char)_configHelper.DelimitCharacter[0])
                    final = final.Remove(final.Length - 1, 1);

                WriteCSVLine(_streamWriter, final);
            }

            Console.WriteLine($"ProcessFile: Output file {_configHelper.OutputFileName} created");
            _streamWriter.Close();

        }

        private static void WriteCSVLine(StreamWriter streamWriter, string line)
        {
            streamWriter.Write($"{line}\n");

            /* int index = 0;
             int pos = 0;
             foreach (AUSGov_CSVReWriter.Config.Column col in _configHelper.ColumnDefinitions)
             {
                 if (++index < _configHelper.ColumnDefinitions.Count)
                     streamWriter.Write($"{line.Substring(pos, col.Field.FixSize)},");
                 else
                     streamWriter.Write($"{line.Substring(pos, col.Field.FixSize)}\n\r");

                 pos += col.Field.FixSize + 1;
             }*/


        }

        private static void WriteCSVHeaderRow(StreamWriter streamWriter)
        {
            int index = 0;
            foreach (AUSGov_CSVReWriter.Config.Column col in _configHelper.ColumnDefinitions)
            {
                if (++index < _configHelper.ColumnDefinitions.Count)
                    streamWriter.Write($"{col.Field.Name},");
                else
                    streamWriter.Write($"{col.Field.Name}\n\r");
            }
        }
    }
}
