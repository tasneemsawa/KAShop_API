using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public String User { get; set; }=null!;
        public string Name { get; set; }=null!;
        // public List<CategoryTranslationResponse> Translations { get; set; }

    }
}