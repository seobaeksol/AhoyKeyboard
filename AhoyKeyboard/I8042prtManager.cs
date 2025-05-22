using System.Diagnostics;

namespace AhoyKeyboard
{
  public static class I8042prtManager
  {
    // Returns: "disabled", "demand start", "auto start", etc.
    public static string GetServiceStartType()
    {
      var psi = new ProcessStartInfo("sc.exe", "qc i8042prt")
      {
        RedirectStandardOutput = true,
        UseShellExecute = false,
        CreateNoWindow = true
      };
      using var proc = Process.Start(psi);

      if (proc == null) return "unknown";

      string output = proc.StandardOutput.ReadToEnd();
      proc.WaitForExit();
      foreach (var line in output.Split('\n'))
      {
        if (line.Trim().StartsWith("START_TYPE"))
        {
          if (line.Contains("DISABLED")) return "disabled";
          if (line.Contains("DEMAND_START")) return "demand start";
          if (line.Contains("AUTO_START")) return "auto start";
        }
      }
      return "unknown";
    }

    public static void SetServiceStartType(bool enabled)
    {
      var psi = new ProcessStartInfo("powershell", "-Command \"Start-Process sc.exe -ArgumentList 'config i8042prt start=" + (enabled ? "auto" : "disabled") + "' -Verb runAs -WindowStyle Hidden\"")
      {
        UseShellExecute = false,
        CreateNoWindow = true
      };
      Process.Start(psi)?.WaitForExit();
    }
  }
}