using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IM10.Models
{
    public class UploadModel
    {
        public int Id { get; set; }
        public IFormFile FilePath { get; set; }

    }
}
