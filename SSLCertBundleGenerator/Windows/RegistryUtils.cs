﻿namespace SSLCertBundleGenerator.Windows
{
  using Microsoft.Win32;

  public static class RegistryUtils
  {
    public static string? GetCurrentMajorVersionNumber()
    {
      using var registryKey = Registry.LocalMachine.OpenSubKey(
        "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion",
        false);

      return registryKey?.GetValue("CurrentMajorVersionNumber", string.Empty)?.ToString();
    }
  }
}