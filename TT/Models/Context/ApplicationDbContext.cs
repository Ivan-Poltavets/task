using System;
using Microsoft.EntityFrameworkCore;

namespace TT.Models.Context;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		:base(options)
	{
	}

	public DbSet<Folder> Folders { get; set; }
}

