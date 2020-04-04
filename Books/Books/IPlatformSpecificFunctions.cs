using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books
{
    public interface IPlatformSpecificFunctions
    {
        Task<bool> FacebookLogin();
        Task<bool> SilentLogin();
        Task<bool> Logout();
    }
}
