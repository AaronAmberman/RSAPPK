using System;
using System.Diagnostics;

namespace RSAPPK
{
    /// <summary>Handles the creation, deletion, importing and exporting of RSA PPK's.</summary>
    public static class RsaPpkManagementService
    {
        #region Outputs

        /*
         * This is what this class will output for each method...
         * 
         * -------------------------------------------------------------------------------
         * Create Results
         * -------------------------------------------------------------------------------
         * Creating RSA Key container...
         * Succeeded!
         * =====================================================================
         * Creating RSA Key container...
         * The RSA key container already exists.
         * Failed!
         * -------------------------------------------------------------------------------
         * 
         * 
         * -------------------------------------------------------------------------------
         * Delete Results
         * -------------------------------------------------------------------------------
         * Deleting RSA Key container...
         * Succeeded!
         * =====================================================================
         * Deleting RSA Key container...
         * The RSA key container was not found.
         * Failed!
         * -------------------------------------------------------------------------------
         * 
         * 
         * -------------------------------------------------------------------------------
         * Import Results
         * -------------------------------------------------------------------------------
         * Importing RSA Keys from file..
         * Succeeded!
         * =====================================================================
         * Importing RSA Keys from file..
         * Unable to find the specified file.
         * Failed!
         * -------------------------------------------------------------------------------
         * 
         * 
         * -------------------------------------------------------------------------------
         * Export Results
         * -------------------------------------------------------------------------------
         * Exporting RSA keys to file...
         * Succeeded!
         * =====================================================================
         * Exporting RSA keys to file...
         * The RSA key container was not found.
         * Failed!
         * -------------------------------------------------------------------------------
         */

        #endregion

        #region Fields

        /*
         * IMPORTANT!!!
         * 
         * Make sure to check this path when you first compile the code, yours may be in a different directory
         */
        private static string aspnet_regiis = Environment.Is64BitOperatingSystem
                ? @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe"
                : @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";

        #endregion

        #region Methods

        /// <summary>Creates the RSA PPK container.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The output from the aspnet_regiis executable or the exception if one occurred.</returns>
        public static string CreateRsaPpkContainer(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName));

            try
            {
                Process process = Process.Start(GetProcessStartInfo($"-pc {containerName} -exp"));
                process?.WaitForExit();

                var output = process?.StandardOutput.ReadToEnd();
                output = RemoveAspNetToolInfoFromResultString(output);

                return output;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>Deletes the RSA PPK container.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <returns>The output from the aspnet_regiis executable or the exception if one occurred.</returns>
        public static string DeleteRsaPpkContainer(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName));

            try
            {
                Process process = Process.Start(GetProcessStartInfo($"-pz {containerName}"));
                process?.WaitForExit();

                var output = process?.StandardOutput.ReadToEnd();
                output = RemoveAspNetToolInfoFromResultString(output);

                return output;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>Exports the RSA PPK container.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="outputFile">The output file.</param>
        /// <returns>The output from the aspnet_regiis executable or the exception if one occurred.</returns>
        public static string ExportRsaPpkContainer(string containerName, string outputFile)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrWhiteSpace(outputFile))
                throw new ArgumentNullException(nameof(outputFile));

            try
            {
                Process process = Process.Start(GetProcessStartInfo($"-px {containerName} \"{outputFile}\" -pri"));
                process?.WaitForExit();

                var output = process?.StandardOutput.ReadToEnd();
                output = RemoveAspNetToolInfoFromResultString(output);

                return output;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>Imports the RSA PPK container.</summary>
        /// <param name="containerName">Name of the container.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The output from the aspnet_regiis executable or the exception if one occurred.</returns>
        public static string ImportRsaPpkContainer(string containerName, string fileName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentNullException(nameof(containerName));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            try
            {
                Process process = Process.Start(GetProcessStartInfo($"-pi {containerName} \"{fileName}\" -exp"));
                process?.WaitForExit();

                var output = process?.StandardOutput.ReadToEnd();
                output = RemoveAspNetToolInfoFromResultString(output);

                return output;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        private static ProcessStartInfo GetProcessStartInfo(string arguments)
        {
            return new ProcessStartInfo
            {
                Arguments = arguments,
                CreateNoWindow = true,
                FileName = aspnet_regiis,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
        }

        private static string RemoveAspNetToolInfoFromResultString(string result)
        {
            int index = result.IndexOf("reserved.", StringComparison.OrdinalIgnoreCase);

            string returnString = result.Substring(index + 9, result.Length - index - 9).Replace("\n\r", "\r\n");

            string[] lines = returnString.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            returnString = string.Join(Environment.NewLine, lines);

            return returnString;
        }

        #endregion
    }
}
