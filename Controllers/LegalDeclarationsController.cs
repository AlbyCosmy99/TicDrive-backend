using System.Data.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicDrive.Context;
using TicDrive.Models.Legal;

namespace TicDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LegalDeclarationsController : ControllerBase
    {
        private readonly TicDriveDbContext _dbContext;

        public LegalDeclarationsController(TicDriveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public class PostLegalDeclarationQuery
        {
            public string Name { get; set; } = string.Empty;

            public DateTime Issued { get; set; }

            public string? Version { get; set; }

            public string? Content { get; set; }

            public bool IsActive { get; set; } = true;

            public Enums.LegalDocumentContext Context { get; set; } = Enums.LegalDocumentContext.AllEcosystem;

            public Enums.LegalDocumentType Type { get; set; }
        }

        [HttpPost("")]
        public async Task<IActionResult> PostLegalDeclaration([FromQuery] PostLegalDeclarationQuery query)
        {
            var legalDocument = new LegalDeclaration
            {
                Name = query.Name,
                Issued = DateTime.SpecifyKind(query.Issued, DateTimeKind.Utc),
                Version = query.Version,
                Content = query.Content,
                IsActive = query.IsActive,
                Context = query.Context,
                Type = query.Type
            };

            _dbContext.LegalDeclarations.Add(legalDocument);
            await _dbContext.SaveChangesAsync();

            return Ok(legalDocument);
        }

        public class GetLegalDeclarationsQuery
        {
            public List<Enums.LegalDocumentContext> Contexts { get; set; } = new();
            public Enums.LegalDocumentType? Type { get; set; }
        }


        [HttpGet]
        public async Task<IActionResult> GetLegalDeclarations([FromQuery] GetLegalDeclarationsQuery query)
        {
            var queryable = _dbContext.LegalDeclarations.AsQueryable();

            if (query.Contexts.Any())
            {
                queryable = queryable.Where(ld => query.Contexts.Contains(ld.Context.Value));
            }

            if (query.Type.HasValue)
            {
                queryable = queryable.Where(ld => ld.Type == query.Type.Value);
            }

            var result = queryable.ToList();
            return Ok(result);
        }
    }
}
