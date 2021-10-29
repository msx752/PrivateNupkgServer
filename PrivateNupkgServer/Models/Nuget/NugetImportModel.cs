using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace privatenupkgserver.Models.Nuget
{
    public class NugetImportModel
    {
        [Required]
        public IFormFile Package { get; set; }
    }
}