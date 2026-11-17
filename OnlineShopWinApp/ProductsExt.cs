using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopWinApp
{
    public partial class Products
    {
        public string FullImagePath
        {
            get { return "/Images/" + ImagePath; }
        }
    }
   
}