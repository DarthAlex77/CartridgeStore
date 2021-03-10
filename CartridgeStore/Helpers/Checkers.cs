using System.Data.SqlClient;
using System.Diagnostics;
using DevExpress.Xpf.Core;

namespace CartridgeStore.Helpers
{
    public static class Checkers
    {
        public static bool IsLocalDBInstalled()
        {
            Process p = new Process
            {
                    StartInfo =
                    {
                            UseShellExecute        = false,
                            RedirectStandardOutput = true,
                            FileName               = "cmd.exe",
                            Arguments              = "/C sqllocaldb info",
                            CreateNoWindow         = true,
                            WindowStyle            = ProcessWindowStyle.Hidden
                    }
            };
            p.Start();
            string sOutput = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            if (sOutput.Trim().Length == 0 || sOutput.Contains("not recognized"))
            {
                return false;
            }

            return sOutput.ToLowerInvariant().Contains("mssqllocaldb");
        }

        public static bool IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException e)
                {
                    DXMessageBox.Show(e.Message);
                    return false;
                }
            }
        }
    }
}