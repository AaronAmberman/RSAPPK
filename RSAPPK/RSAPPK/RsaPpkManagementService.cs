using System;
using System.Diagnostics;

namespace RSAPPK
{
    /// <summary>Handles the creation, deletion, importing and exporting of RSA PPK's.</summary>
    public static class RsaPpkManagementService
    {
        #region Fields

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
            var index = result.IndexOf("reserved.", StringComparison.OrdinalIgnoreCase);

            var returnString = result.Substring(index + 9, result.Length - index - 9);

            // there is an improper carriage return line feed in result...correct it
            returnString = returnString.Replace("\n\r", "\r\n");

            // remove all the formatting (so we can format it the way we want it)
            var lines = returnString.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 2)
            {
                returnString = lines[0] + Environment.NewLine + Environment.NewLine + lines[1];
            }
            else if (lines.Length == 3)
            {
                returnString = lines[0] + Environment.NewLine + Environment.NewLine + lines[1] + Environment.NewLine + lines[2];
            }

            return returnString;
        }

        #endregion
    }
}
