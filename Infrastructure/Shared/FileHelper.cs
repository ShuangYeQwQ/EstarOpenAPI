using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Shared
{
    public static class FileHelper
    {
        private static readonly Dictionary<string, string> SupportedMimeTypes = new()
        {
            [".pdf"] = "application/pdf",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            [".txt"] = "text/plain",
            [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        };

        public static bool IsSupportedFileType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return SupportedMimeTypes.ContainsKey(extension);
        }

        public static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            return SupportedMimeTypes.GetValueOrDefault(extension, "application/octet-stream");
        }

        public static async Task<string> SaveTempFileAsync(IFormFile file, string tempDirectory = "temp")
        {
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            var tempFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var tempFilePath = Path.Combine(tempDirectory, tempFileName);

            using var stream = new FileStream(tempFilePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return tempFilePath;
        }

        public static void CleanupTempFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    // Log warning but don't throw
                    Console.WriteLine($"Warning: Could not delete temp file {filePath}: {ex.Message}");
                }
            }
        }

        public static string SanitizeFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }



        /// <summary>
        /// 文件提取文本信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static async Task<string> ExtractTextAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            using var stream = file.OpenReadStream();

            switch (extension)
            {
                case ".txt":
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        return await reader.ReadToEndAsync();
                    }

                case ".pdf":
                    return ExtractPdfText(stream);

                case ".docx":
                    return ExtractDocxText(stream);

                //case ".doc":
                //    return ExtractDocText(stream);

                default:
                    return $"不支持的文件类型: {extension}";
            }
        }

        private static string ExtractPdfText(Stream stream)
        {
            var sb = new StringBuilder();
            using var pdfReader = new PdfReader(stream);
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(pdfReader);

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var text = PdfTextExtractor.GetTextFromPage(page);
                sb.AppendLine(text);
            }

            return sb.ToString();
        }

        private static string ExtractDocxText(Stream stream)
        {
            var sb = new StringBuilder();
            using var doc = WordprocessingDocument.Open(stream, false);
            var body = doc.MainDocumentPart?.Document.Body;
            if (body != null)
            {
                sb.Append(body.InnerText);
            }
            return sb.ToString();
        }

        //private static string ExtractDocText(Stream stream)
        //{
        //    var sb = new StringBuilder();
        //    POIFSFileSystem fs = new POIFSFileSystem(stream);
        //    var doc = new HWPFDocument(fs);
        //    var range = doc.GetRange();
        //    for (int i = 0; i < range.NumParagraphs; i++)
        //    {
        //        sb.AppendLine(range.GetParagraph(i).Text);
        //    }
        //    return sb.ToString();
        //}
    }
}
