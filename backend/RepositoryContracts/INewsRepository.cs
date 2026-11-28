using Entities;

namespace RepositoryContracts;

public interface INewsRepository
{
    /// All published news.
    Task<List<NewsItem>> GetManyAsync();

    /// Publishes the news from one approved request.
    Task<NewsItem> CreateAsync(NewsSubmission news, int sourceRequestId); 
                                                            //records which request created the news
}