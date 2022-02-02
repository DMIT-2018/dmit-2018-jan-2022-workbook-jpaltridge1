using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.SamplePages
{
    public class BasicsModel : PageModel
    {
        //basically this is an object, treat it as such

        //it will have  data fields, properties, constructors, behaviours (aka methods)

        public string? MyName;


        public void OnGet()
        {
            //exectues in response to a Get Request from the Browser
            //when the page is "first" accessed, the browser issues a get request
            //when the page is refreshed without a post request, the browser issues a Get request
            //when the page is retrieved in response to a form's POST using RedirectToPage()
            //IF NO RedirectToPage is used on the POST, there is no Get request issued

            //Server-side processing
            //contains no html

            Random rnd = new Random();
            int oddeven = rnd.Next(0,25);
            if(oddeven %2 == 0)
            {
                MyName = $"Jeff is even {oddeven}";
            }
            else
            {
                MyName = null;
            }


        }
    }
}
