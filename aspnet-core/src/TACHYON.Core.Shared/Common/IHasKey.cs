using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACHYON.Common
{
    public interface IHasKey
    {
        string Key { get; set; }
    }

    public interface IHasDisplayName
    {
        string DisplayName { get; set; }
    }
    public interface IHasName
    {
        string Name { get; set; }
    }
}