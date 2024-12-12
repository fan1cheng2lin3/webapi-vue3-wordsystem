namespace MyWordStystemWebapi.DOT.Dtos
{
    public class ImportWordsDto
    {
        public string? WordBookName { get; set; }
        public IFormFile? File { get; set; } // 使用 IFormFile 接收文件

    }
}
