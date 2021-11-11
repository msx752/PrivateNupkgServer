namespace privatenupkgserver.Models.SearchQuery;

public class SearchOutputModel
{
    public SearchOutputModel()
    {
        Data = new List<SearchResultModel>();
    }

    public int TotalHits { get; set; }

    public List<SearchResultModel> Data { get; set; }
}
