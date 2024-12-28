using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Nest;
using SozcuWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SozcuWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IElasticClient _elasticClient;

        public IndexModel(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public List<Haber> Haberler { get; set; } = new List<Haber>();

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public long TotalCount { get; set; }


        public bool HasMore { get; set; }
        public int CurrentPageStart { get; set; }
        public int CurrentPageEnd { get; set; }


        public async Task OnGetAsync()
        {
            var mustClauses = new List<QueryContainer>();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var multiMatchQuery = new MultiMatchQuery
                {
                    Fields = Infer.Field<Haber>(p => p.title)
                           .And(Infer.Field<Haber>(p => p.content))
                           .And(Infer.Field<Haber>(p => p.describe)),
                    Query = SearchTerm,
                    Type = TextQueryType.PhrasePrefix
                };
                mustClauses.Add(multiMatchQuery);
            }

            if (!string.IsNullOrEmpty(SelectedCategory))
            {
                var matchQuery = new MatchQuery
                {
                    Field = Infer.Field<Haber>(p => p.kategori),
                    Query = SelectedCategory
                };
                mustClauses.Add(matchQuery);
            }

            var boolQuery = new BoolQuery
            {
                Must = mustClauses
            };


            var searchRequest = new SearchRequest<Haber>("haberler")
            {
                Query = boolQuery.Must.Any() ? (QueryContainer)boolQuery : new MatchAllQuery(),
                From = (PageNumber - 1) * PageSize,
                Size = PageSize
            };


            var searchResponse = await _elasticClient.SearchAsync<Haber>(searchRequest);

            if (searchResponse.IsValid)
            {
                if (PageNumber == 1) { Haberler = searchResponse.Documents.ToList(); }
                else { Haberler.AddRange(searchResponse.Documents.ToList()); }


                TotalCount = searchResponse.Total;
                TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
                HasMore = PageNumber < TotalPages;
                CurrentPageStart = (PageNumber - 1) * PageSize + 1;
                CurrentPageEnd = Math.Min(PageNumber * PageSize, (int)TotalCount);

            }
            else
            {
                Haberler = new List<Haber>();
                Console.WriteLine("Error: " + searchResponse.DebugInformation);
                HasMore = false;
            }

        }
        private List<int> GenerateDisplayPages()
        {
            int visiblePageCount = 5;
            int startPage = Math.Max(1, PageNumber - (visiblePageCount / 2));
            int endPage = Math.Min(TotalPages, startPage + visiblePageCount - 1);

            if (endPage - startPage < visiblePageCount - 1 && TotalPages > visiblePageCount)
            {
                if (startPage > 1) { startPage = Math.Max(1, endPage - visiblePageCount + 1); }
                else { endPage = Math.Min(TotalPages, startPage + visiblePageCount - 1); }
            }
            return Enumerable.Range(startPage, endPage - startPage + 1).ToList();
        }
    }
}