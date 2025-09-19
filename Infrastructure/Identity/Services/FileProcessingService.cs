using Aspose.Slides;
using Infrastructure.Identity.Models;
using System.Collections.Generic;
using System.Text;
using Tesseract;
using Xceed.Words.NET;

namespace Infrastructure.Identity.Services
{
    // Services/FileProcessingService.cs
    //public class FileProcessingService
    //{
    //    private readonly string _uploadDirectory;

    //    public FileProcessingService(IWebHostEnvironment env)
    //    {
    //        _uploadDirectory = Path.Combine(env.ContentRootPath, "Uploads");
    //        Directory.CreateDirectory(_uploadDirectory);
    //    }

    //    public async Task<UploadedFile> ProcessUploadedFile(IFormFile file)
    //    {
    //        // 保存文件
    //        string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
    //        string filePath = Path.Combine(_uploadDirectory, fileName);

    //        using (var stream = new FileStream(filePath, FileMode.Create))
    //        {
    //            await file.CopyToAsync(stream);
    //        }

    //        // 提取内容
    //        string content = ExtractFileContent(filePath, file.ContentType);

    //        return new UploadedFile
    //        {
    //            Id = Guid.NewGuid(),
    //            FileName = file.FileName,
    //            FilePath = filePath,
    //            Content = content,
    //            UploadTime = DateTime.UtcNow
    //        };
    //    }

    //    private string ExtractFileContent(string filePath, string contentType)
    //    {
    //        string extension = Path.GetExtension(filePath).ToLower();

    //        // 支持的文件类型
    //        return extension switch
    //        {
    //            ".pdf" => ExtractPdfContent(filePath),
    //            ".docx" => ExtractDocxContent(filePath),
    //            //".doc" => ExtractDocContent(filePath),
    //            ".txt" => File.ReadAllText(filePath),
    //            ".jpg" or ".jpeg" or ".png" => ExtractImageText(filePath),
    //           // ".xlsx" => ExtractExcelContent(filePath),
    //            _ => throw new NotSupportedException("Unsupported file type")
    //        };
    //    }

    //    private string ExtractPdfContent(string filePath)
    //    {
    //        var pdfDocument = new IronPdf.PdfDocument(filePath);
    //        return pdfDocument.ExtractAllText();
    //    }

    //    private string ExtractDocxContent(string filePath)
    //    {
    //        using var doc = DocX.Load(filePath);
    //        return doc.Text;
    //    }

    //    private string ExtractImageText(string filePath)
    //    {
    //        using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    //        using var img = Pix.LoadFromFile(filePath);
    //        using var page = engine.Process(img);
    //        return page.GetText();
    //    }

    //}


}
