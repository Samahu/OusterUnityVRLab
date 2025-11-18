using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OusterSdkCSharp {

internal static class NativeConstants
{
    public const int OU_CLIENT_TIMEOUT = 0;
    public const int OU_CLIENT_ERROR = 1;
    public const int OU_CLIENT_LIDAR_DATA = 2;
    public const int OU_CLIENT_IMU_DATA = 4;
    public const int OU_CLIENT_EXIT = 8;
}

internal static class NativeMethods
{
    // Base library name (no extension) for all platforms
    private const string LinuxLib = "ouster_c";   // resolves to libouster_c.so
    private const string MacLib = "ouster_c";     // resolves to libouster_c.dylib
    private const string WindowsLib = "ouster_c"; // resolves to ouster_c.dll

    // We still keep the symbolic name for DllImport, but we will manually
    // load the native library (SetDllDirectory + LoadLibrary) before any call.
    // This helps when Unity's default plugin resolution fails.
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || WINDOWS
    private const string Lib = WindowsLib;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || OSX
    private const string Lib = MacLib;
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX || LINUX
    private const string Lib = LinuxLib;
#else
    private const string Lib = LinuxLib;
#endif

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || WINDOWS
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern bool SetDllDirectory(string lpPathName);
    [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr LoadLibrary(string lpFileName);
    [DllImport("kernel32", SetLastError = true)]
    private static extern bool FreeLibrary(IntPtr hModule);
    [DllImport("kernel32", SetLastError = true)]
    private static extern uint GetLastError();
    private static IntPtr _moduleHandle = IntPtr.Zero;
    public static bool IsLibraryLoaded => _moduleHandle != IntPtr.Zero;

    static NativeMethods()
    {
        try
        {
            // Construct the expected plugin path (Editor / Standalone)
            string pluginDir = Path.Combine(Application.dataPath, "Plugins", "x86_64");
            string fullPath = Path.Combine(pluginDir, WindowsLib + ".dll");

            // Hint the loader to look into our plugin directory first.
            SetDllDirectory(pluginDir);

            _moduleHandle = LoadLibrary(fullPath);
            if (_moduleHandle == IntPtr.Zero)
            {
                uint err = GetLastError();
                Debug.LogError($"[NativeMethods] LoadLibrary failed: {fullPath} Win32Error={err}");
            }
            else
            {
                Debug.Log($"[NativeMethods] Successfully loaded native library at {fullPath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[NativeMethods] Exception during manual native load: " + ex);
        }
    }
#else
    // Non-Windows: rely on default dynamic loader behavior (DllImport symbolic name).
    static NativeMethods() { }
#endif

    // Client API
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr ouster_client_create(string hostname, int lidar_port, int imu_port);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ouster_client_destroy(IntPtr client);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_poll(IntPtr client, int timeout_sec);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_get_metadata(IntPtr client, IntPtr buffer, UIntPtr capacity);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_fetch_and_parse_metadata(IntPtr client, int timeout_sec);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_get_frame_dimensions(IntPtr client, out int width, out int height);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_get_packet_sizes(IntPtr client, out UIntPtr lidar_packet_size, out UIntPtr imu_packet_size);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_read_lidar_packet(IntPtr client, IntPtr buf, UIntPtr buf_size);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_read_imu_packet(IntPtr client, IntPtr buf, UIntPtr buf_size);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_get_lidar_port(IntPtr client);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_client_get_imu_port(IntPtr client);

    // Scan Source API
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_scan_source_create(string hostname, out IntPtr out_source);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ouster_scan_source_destroy(IntPtr source);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_scan_source_frame_dimensions(IntPtr source, out int width, out int height);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_scan_source_get_metadata(IntPtr source, IntPtr buffer, UIntPtr capacity);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr ouster_scan_source_next_scan(IntPtr source, int timeout_sec);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ouster_lidar_scan_destroy(IntPtr scan);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ouster_lidar_scan_get_dimensions(IntPtr scan, out int width, out int height);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_lidar_scan_get_field_u32(IntPtr scan, string field_name, int destagger, IntPtr out_buf, UIntPtr capacity, out UIntPtr out_count);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_lidar_scan_get_field_u16(IntPtr scan, string field_name, int destagger, IntPtr out_buf, UIntPtr capacity, out UIntPtr out_count);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_lidar_scan_get_field_u8(IntPtr scan, string field_name, int destagger, IntPtr out_buf, UIntPtr capacity, out UIntPtr out_count);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int ouster_lidar_scan_get_xyz(IntPtr scan, IntPtr lut, IntPtr xyz_out, UIntPtr capacity_points, out UIntPtr out_points, int filter_invalid);

    // XYZ LUT API
    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr ouster_scan_source_create_xyz_lut(IntPtr source, int use_extrinsics);

    [DllImport(Lib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void ouster_xyz_lut_destroy(IntPtr lut);
}

}