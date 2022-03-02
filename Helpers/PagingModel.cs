using System;

namespace Razor_11_Identity.Helpers
{
    public class PageModel
    {
        public int currentpage { set; get; }
        public int countpages { set; get; }
        public Func<int?, string> generateUrl { set; get; }
    }
}