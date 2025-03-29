using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

[Table("active_users", Schema = "public")]
public class ActiveUser
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("session_id", TypeName = "varchar(255)")]
    [MaxLength(255)]
    public string SessionId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("ip_address", TypeName = "varchar(255)")]
    [MaxLength(255)]
    public string IpAddress { get; set; }

    [Column("last_active")]
    public DateTime LastActive { get; set; } = DateTime.UtcNow;

}
/*
 CREATE TABLE active_users (
id BIGSERIAL PRIMARY KEY,
session_id VARCHAR(255) UNIQUE NOT NULL,
user_id INT NULL,
ip_address VARCHAR(255),
last_active TIMESTAMP DEFAULT NOW()
);
 */