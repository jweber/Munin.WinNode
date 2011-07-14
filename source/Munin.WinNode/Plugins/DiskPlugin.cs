using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Munin.WinNode.Plugins
{
    class DiskPlugin : IPlugin
    {
        public string Name
        {
            get { return "df"; }
        }

        public bool IsApplicable
        {
            get { return true; }
        }


        private DriveInfo[] GetDrives()
        {
            return DriveInfo.GetDrives()
                .Where(d => d.DriveType == DriveType.Fixed)
                .ToArray();
        }

        private string CleanDriveName(string name)
        {
            return name.Replace(@"\", string.Empty);
        }

        public string GetConfiguration()
        {
            var output = new List<string>
                         {
                             "graph_title Filesystem usage (in %)",
                             "graph_category disk",
                             "graph_info This graph shows disk usage on the machine.",
                             "graph_args --upper-limit 100 -l 0",
                             "graph_vlabel %"
                         };

            DriveInfo[] drives = GetDrives();
            int count = 0;
            foreach (var drive in drives)
            {
                string prefix = string.Format("disk_{0}_", count++);
                if (! string.IsNullOrWhiteSpace(drive.VolumeLabel))
                {
                    output.Add(string.Format("{0}.label {1} ({2})", prefix, CleanDriveName(drive.Name), drive.VolumeLabel));
                }
                else
                {
                    output.Add(string.Format("{0}.label {1}", prefix, CleanDriveName(drive.Name)));
                }
                output.Add(string.Format("{0}.warning 92", prefix));
                output.Add(string.Format("{0}.critical 98", prefix));
            }

            return output.Combine();
        }

        public string GetValues()
        {
            var drives = GetDrives();

            var output = new List<string>(drives.Length);
            int count = 0;
            foreach (var drive in drives)
            {
                long total = drive.TotalSize;
                long free = drive.TotalFreeSpace;
                long used = total - free;
                decimal percentFree = (used / (decimal)total) * 100;

                string prefix = string.Format("disk_{0}_", count++);
                output.Add(string.Format("{0}.value {1:0.00}", prefix, percentFree));
            }

            return output.Combine();
        }
    }
}
