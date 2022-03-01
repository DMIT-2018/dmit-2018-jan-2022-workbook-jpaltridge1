#nullable disable

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional namespaces

using ChinookSystem.BLL;
using ChinookSystem.ViewModels;

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

        [BindProperty]
        public int GenreId { get; set; }

        [BindProperty]
        public List<AlbumsListBy> AlbumsByGenre { get; set; }


        public void OnGet()
        {
            GenreList = _genreServices.GetAllGenres();
            // sort the List<T> using method .Sort

            GenreList.Sort((x,y) => x.DisplayText.CompareTo(y.DisplayText));

            AlbumsByGenre = _albumServices.AlbumsByGenre((int)GenreId);

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

            return RedirectToPage(); //causes a Get request which forces OnGet request
        }

    }
}
