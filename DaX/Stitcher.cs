using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaX
{
    public class Stitcher
    {
        internal void StitchAll()
        {
            var files = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXCaps"), "*" + "_DaX_" + "*" + "_XaD_" + "*");

            var ids = files.Select(x => x.Substring(0, x.IndexOf("_DaX_"))).Distinct();
            Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL"));
            foreach (var id in ids)
            {
                var idfiles = Directory.GetFiles(Path.Combine(Path.GetDirectoryName(id)), Path.GetFileName(id) + "_DaX_" + "*" + "_XaD_" + "*");
                var destname = idfiles[0];
                destname = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "DaXDL", destname.Substring(destname.IndexOf("_XaD_") + "_XaD_".Length));

                using (Stream destStream = File.OpenWrite(destname))
                {
                    foreach (string srcFileName in idfiles.OrderBy(x => x, new AlphanumComparatorFast()))
                    {
                        using (Stream srcStream = File.OpenRead(srcFileName))
                        {
                            srcStream.CopyTo(destStream);
                        }
                    }
                }
            }
        }
    }
}
