namespace MyWordStystemWebapi.Services.Interfaces
{
    public interface ICiKuService
    {
        Task<List<CiKuWord>> GetWordsByViewNameAsync(string viewName, int pageNumber, int pageSize);

        Task AddWordBook(int UserId, string WordBookName);

        Task updateWordBookname(int UserId, int WordBookId, string WordBookName);


        Task deleteWordBookname(int UserId, int WordBookId);

    }
}
