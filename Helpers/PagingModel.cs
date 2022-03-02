using System;

namespace Razor_09_EntityFramework.Helpers
{
    public class PageModel
    {
        public int currentpage { set; get; }
        public int countpages { set; get; }
        public Func<int?, string> generateUrl { set; get; }
    }
}