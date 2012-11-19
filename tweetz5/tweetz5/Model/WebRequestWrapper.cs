using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tweetz5.Model
{
    public interface IWebRequest
    {
        
    }

    public class WebRequestWrapper : IWebRequest
    {
        public static Func<IWebRequest> OverrideImplementation;

        public static IWebRequest Create(Uri address)
        {
            return (OverrideImplementation != null) ? OverrideImplementation() : null;
        }
    }
}
