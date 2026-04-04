using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UltPay.Domain.Entities;
using UltPay.Infrastructure.Persistence;
using UltPay.Contracts.Requests;

namespace UltPay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficiariesController : ControllerBase
    {
        private readonly UltPayDbContext _context;

        public BeneficiariesController(UltPayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var data = await _context.Beneficiaries
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (beneficiary == null)
                return NotFound(new { message = "Beneficiary not found." });

            return Ok(beneficiary);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBeneficiaryRequest request)
        {
            if (request.PayoutMethod.ToLower() == "bank")
            {
                if (string.IsNullOrWhiteSpace(request.BankCode) || string.IsNullOrWhiteSpace(request.AccountNumber))
                    return BadRequest(new { message = "BankCode and AccountNumber are required for bank payout." });
            }

            if (request.PayoutMethod.ToLower() == "mobilemoney")
            {
                if (string.IsNullOrWhiteSpace(request.MobileMoneyProvider) || string.IsNullOrWhiteSpace(request.MobileMoneyNumber))
                    return BadRequest(new { message = "MobileMoneyProvider and MobileMoneyNumber are required for mobile money payout." });
            }

            var beneficiary = new Beneficiary
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                FullName = request.FullName,
                CountryCode = request.CountryCode,
                CurrencyCode = request.CurrencyCode,
                PayoutMethod = request.PayoutMethod,
                BankCode = request.BankCode,
                AccountNumber = request.AccountNumber,
                MobileMoneyProvider = request.MobileMoneyProvider,
                MobileMoneyNumber = request.MobileMoneyNumber,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.Beneficiaries.Add(beneficiary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = beneficiary.Id }, beneficiary);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, UpdateBeneficiaryRequest request)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (beneficiary == null)
                return NotFound(new { message = "Beneficiary not found." });

            if (request.PayoutMethod.ToLower() == "bank")
            {
                if (string.IsNullOrWhiteSpace(request.BankCode) || string.IsNullOrWhiteSpace(request.AccountNumber))
                    return BadRequest(new { message = "BankCode and AccountNumber are required for bank payout." });
            }

            if (request.PayoutMethod.ToLower() == "mobilemoney")
            {
                if (string.IsNullOrWhiteSpace(request.MobileMoneyProvider) || string.IsNullOrWhiteSpace(request.MobileMoneyNumber))
                    return BadRequest(new { message = "MobileMoneyProvider and MobileMoneyNumber are required for mobile money payout." });
            }

            beneficiary.UserId = request.UserId;
            beneficiary.FullName = request.FullName;
            beneficiary.CountryCode = request.CountryCode;
            beneficiary.CurrencyCode = request.CurrencyCode;
            beneficiary.PayoutMethod = request.PayoutMethod;
            beneficiary.BankCode = request.BankCode;
            beneficiary.AccountNumber = request.AccountNumber;
            beneficiary.MobileMoneyProvider = request.MobileMoneyProvider;
            beneficiary.MobileMoneyNumber = request.MobileMoneyNumber;

            await _context.SaveChangesAsync();

            return Ok(beneficiary);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var beneficiary = await _context.Beneficiaries
                .FirstOrDefaultAsync(x => x.Id == id);

            if (beneficiary == null)
                return NotFound(new { message = "Beneficiary not found." });

            _context.Beneficiaries.Remove(beneficiary);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}