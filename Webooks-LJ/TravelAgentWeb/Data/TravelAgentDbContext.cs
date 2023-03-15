using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using TravelAgentWeb.Models;

namespace TravelAgentWeb.Data;
public class TravelAgentDbContext :DbContext
{
	public TravelAgentDbContext(DbContextOptions<TravelAgentDbContext> opt) : base(opt) {}    

	public DbSet<WbhookSecret> SubscriptionSecrets { get; set; }

}

