using MyWordStystemWebapi.Models;

namespace MyWordStystemWebapi.Services.Interfaces
{
    public interface ICiKuService
    {

       

        Task<List<Myciku>> GetStartWordsBymyViewNameAsync(int userid);
        Task<List<CiKuWord>> GetWordsByViewNameAsync(string viewName);


        Task<List<CiKuWord>> GetUnlearnedWordsByViewNameAsync(int userid,string viewName);


        Task<List<Myciku>> GetUnlearnedStartWordsBymyViewNameAsync(int userid);

        Task<List<ZaixueMyciku>> GetzaixueWordsBymyViewName(int userid);

        //Task<List<Myciku>> GetAllWordsBymyViewNameAsync(int userid, string wordbook);
        //Task updateStartWord(int UserId, int wordId, string start);

        Task AddWordBook(int UserId, string WordBookName);

        Task updateWordBookname(int UserId, int WordBookId, string WordBookName);


        Task deleteWordBookname(int UserId, string WordBookname);
    }
}
