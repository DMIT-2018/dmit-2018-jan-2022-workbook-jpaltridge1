#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional namespaces

using ChinookSystem.BLL;
using ChinookSystem.ViewModels;
using WebApp.Helpers;

#endregion


namespace WebApp.Pages.SamplePages
{
    public class AlbumsByGenreQueryModel : PageModel
    {
        #region Private variable and DI constructor

        private readonly ILogger<IndexModel> _logger;
        private readonly AlbumServices _albumServices;
        private readonly GenreServices _genreServices;


        public AlbumsByGenreQueryModel(ILogger<IndexModel> logger, AlbumServices albumservices, GenreServices genreservices)
        {
            _logger = logger;
            _albumServices = albumservices;
            _genreServices = genreservices;
        }

        #endregion

        #region FeedBack and ErrorHandling

        [TempData]
        public string FeedBack { get; set; }
        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedBack);
        
        [TempData]
        public string ErrorMsg { get; set; }
        public bool HasErrorMsg => !string.IsNullOrWhiteSpace(ErrorMsg);

        #endregion

        [BindProperty]
        public List<SelectionList> GenreList { get; set; }

        [BindProperty (SupportsGet = true)]
        public int? GenreId { get; set; }

        [BindProperty]
        public List<AlbumsListBy> AlbumsByGenre { get; set; }

        #region Paginator variables

        // Desired page size
        private const int PAGE_SIZE = 5;

        //instance for the Paginator
        public Paginator Pager { get; set; }

        //the current page value will appear on your URL as a Request parameter value
        //    url address...?currentPage=n


        #endregion


        public void OnGet(int? currentPage)
        {
            GenreList = _genreServices.GetAllGenres();
            // sort the List<T> using method .Sort

            GenreList.Sort((x,y) => x.DisplayText.CompareTo(y.DisplayText));

            if (GenreId.HasValue && GenreId.Value > 0)
            {
                //install of the paginator setup
                //detemine the page number to use with the paginator

                int pageNumber = currentPage.HasValue ? currentPage.Value : 1;

                //use the page state to setup data needed for paging
                
                PageState current = new PageState(pageNumber, PAGE_SIZE);

                //total rows in the complete query collection (data needed for paging)

                int totalrows = 0;

                //for efficiency of data being transfered, we will pass the current page number
                // and the desired page size to the backend query the returned collection will only
                // have the rows of the whole query collection that will actually be shown (PAGE_SIZE or less rows)
                // the total number of records for the whole query collection will be
                // returned as an out parameter. This value is needed by the paginator to setup its display logic

                AlbumsByGenre = _albumServices.AlbumsByGenre((int)GenreId, pageNumber, PAGE_SIZE, out totalrows);

                //once the query is complete, use the total rows in instantizating
                // an instance of the Paginator

                Pager = new Paginator(totalrows, current);

            }           

        }

        public IActionResult OnPost()
        {
            if(GenreId == 0)
            {
                FeedBack = "You did not select a genre";
            }

            else
            {
                FeedBack = $" You selected a genre id of {GenreId}";
            }

            return RedirectToPage(new {GenreId = GenreId}); //causes a Get request which forces OnGet request
        }

    }
}
