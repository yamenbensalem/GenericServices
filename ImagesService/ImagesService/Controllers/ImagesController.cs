using ImagesService.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace ImagesService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly string _imageFolder;
    public ImagesController(IConfiguration configuration)
    {
        _imageFolder = configuration.GetSection("ImagesServiceSettings").GetValue<string>("ImageStoragePath") ?? "Images";
        if (!Directory.Exists(_imageFolder))
            Directory.CreateDirectory(_imageFolder);
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [RequestSizeLimit(10 * 1024 * 1024)] // Optional: Limit file size to 10MB  
    [RequestFormLimits(MultipartBodyLengthLimit = 10 * 1024 * 1024)] // Optional: Limit multipart body size  
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] FileUploadModel model,string folderImage)
    {
        var file = model.File;

        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (file.Length > 10 * 1024 * 1024) // Optional: Check file size limit
            return BadRequest("File size exceeds the limit of 10MB.");
        if (string.IsNullOrEmpty(folderImage))
            return BadRequest("Folder names cannot be empty.");
        

        // Create the folder structure if it doesn't exist
        var folderPath = Path.Combine(_imageFolder, folderImage);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath); 
      
        // Ensure the file name is unique

        var filePath = Path.Combine(folderPath, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        return Ok(new
        {
            file.Length
        });
    }

    [HttpGet("file")]
    public IActionResult Get(string folderImage, string fileName)
    {
        var filePath = Path.Combine(_imageFolder, folderImage, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        var image = System.IO.File.OpenRead(filePath);
        return File(image, "application/octet-stream", fileName);
    }

    [HttpGet("folder")]
    public ActionResult<List<string>> Get(string folderName)
    {
        var filePath = Path.Combine(_imageFolder, folderName);
        if (!Directory.Exists(filePath))
            return NotFound();

        var files = Directory.GetFiles(filePath).Select(Path.GetFileName).ToList();
        return Ok(files);
    }

    [HttpDelete("{fileName}")]
    public IActionResult Delete(string fileName)
    {
        var filePath = Path.Combine(_imageFolder, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound();
        System.IO.File.Delete(filePath);
        return NoContent();
    }
}
