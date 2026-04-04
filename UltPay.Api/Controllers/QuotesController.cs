using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Contracts.Requests;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;

namespace UltPay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly UltPayDbContext _context;

    public QuotesController(UltPayDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var quotes = await _context.Quotes
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        return Ok(quotes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var quote = await _context.Quotes
            .FirstOrDefaultAsync(x => x.Id == id);

        if (quote == null)
            return NotFound(new { message = "Quote not found." });

        return Ok(quote);
    }

   
    
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuoteRequest request)
        {
            if (request.SourceAmount <= 0)
                return BadRequest(new { message = "SourceAmount must be greater than zero." });

            decimal fxRate = 130.50m;
            decimal feeAmount = 2.50m;
            decimal destinationAmount = (request.SourceAmount - feeAmount) * fxRate;

            var quote = new Quote
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                SourceAmount = request.SourceAmount,
                DestinationAmount = destinationAmount,
                FeeAmount = feeAmount,
                FxRate = fxRate,
                SourceCurrency = request.SourceCurrency,
                DestinationCurrency = request.DestinationCurrency,
                Status = "ACTIVE",
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Quotes.Add(quote);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = quote.Id }, quote);
        }
    }

