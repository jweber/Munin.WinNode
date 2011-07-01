using System.Runtime.InteropServices;

namespace Munin.WinNode.Plugins
{
    class MemoryPlugin : IPlugin
    {
        public string Name
        {
            get { return "memory"; }
        }

        public string GetConfiguration()
        {
            return new[]
                   {
                       "graph_args --base 1024 -l 0 --vertical-label Bytes --upper-limit 329342976",
                       "graph_title Memory usage",
                       "graph_category system",
                       "graph_info This graph shows what the machine uses its memory for.",
                       "graph_order apps free swap",
                       "apps.label apps",
                       "apps.draw AREA",
                       "apps.info Memory used by user-space applications.",
                       "swap.label swap",
                       "swap.draw STACK",
                       "swap.info Swap space used.",
                       "free.label unused",
                       "free.draw STACK",
                       "free.info Wasted memory. Memory that is not used for anything at all."
                   }.Combine();
        }

        public string GetValues()
        {
            ulong apps = 0, swap = 0, free = 0;

            var memoryStatus = new MemoryStatusEx();
            if (GlobalMemoryStatusEx(memoryStatus))
            {
                apps = memoryStatus.ullTotalPhys - memoryStatus.ullAvailPhys;
                swap = memoryStatus.ullTotalPageFile - memoryStatus.ullAvailPageFile;
                free = memoryStatus.ullAvailPhys;
            }

            return new[]
                   {
                       string.Format("apps.value {0}", apps),
                       string.Format("swap.value {0}", swap),
                       string.Format("free.value {0}", free)
                   }.Combine();
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MemoryStatusEx lpBuffer);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class MemoryStatusEx
        {
            public uint dwLength;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MemoryStatusEx()
            {
                this.dwLength = (uint) Marshal.SizeOf(typeof(MemoryStatusEx));
            }
        }
    }
}