using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.MigrationService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class MigrationController(MigrationService migrationService, ILogger<MigrationController> logger) : ControllerBase
    {
        private readonly MigrationService migrationService = migrationService;

        [HttpPost]
        public void CreateDb(int version)
        {
            migrationService.ApplyMigrations();

            var (applied, available) = migrationService.GetMigrationStatus();
            logger.LogInformation($"Migrations Applied: {applied.Length}, Available: {available.Length}");
            return;
        }
    }
}
