using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages.SamplePages
{
    public class BasicsModel : PageModel
    {
        //basically this is an object, treat it as such

        //it will have  data fields, properties, constructors, behaviours (aka methods)

        /*
            Properties

            the annotation [TempData] stores the data until it's read in another immediate request
            this annotation attribute has 2 methods called Keep(string) and Peek(string)
            (used on the content page)
            kept in a dictionary (name/value pair)
            usefull to redirect when data  is required for more than a single request
            Implemented by TempData providers using either cookies or session state
            TempData is NOT bound to any particular control like BindProperty

            the annotation BindProperty ties a property in the pagemodel class directly to a control on the content page
            data is transfered between the two automatically
            on the Content page, the control to use this property will have a helper-tag called asp-for


        */


        [TempData]
        public string? Feedback { get; set; }
        

        [BindProperty(SupportsGet = true)]
        public int? id { get; set; }

        //to retain a value in the control tied to this property AND retained via the @page use
        //the SupportsGet attribute = true


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

        //processing in response to a request from a form on a web page
        //this request is referred to as a Post (nethod="post")
        // General Post
        //a general post occurs when a asp-page-handler is NOT used
        //the return datatype can be void, however, you will normally encounter the datatype IActionResult
        //on the return statement of the method OnPost()
        //typical actions:
        // Page()
        //  :does NOT issue a OnGet request
        //  :remains on the current page
        //  :a good action for form processing involving validation an with the catch of a try/catch

        //RedirectToPage()
        //  :DOES issue a OnGet request
        //  :is used to retain input values via the @page and your BindProperty
        //      for controls on you form on theContent Page


        public IActionResult OnPost()
        {
            //this line of code is used to cause a delay in processing  so we can see on the Network activty
            //some type of simulated processing

            Thread.Sleep(2000);

            // retrieve data via the Request object
            // Request: webpage to server
            // Response: server to web page

            string buttonvalue = Request.Form["theButton"];
            Feedback = $"Button pressed is {buttonvalue} with numeric input of {id}";

            //return Page(); // this does not issue an OnGet()\

            return RedirectToPage(new {id = id});


        }



    }
}
