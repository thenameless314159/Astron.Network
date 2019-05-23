using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Astron.Network.Abstractions
{
    public interface IAsyncSetup
    {
        Task Setup();
    }
}
