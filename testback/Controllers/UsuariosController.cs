using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using testback.Data;
using testback.Models;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UsuariosController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Cedula) || string.IsNullOrEmpty(loginRequest.Contrasena))
                return BadRequest("Datos de inicio de sesión incompletos.");

            var usuario = await _context.Usuario.Include(u => u.Obra)
                                                .FirstOrDefaultAsync(u => u.Cedula == loginRequest.Cedula);

            if (usuario == null)
                return Unauthorized("Usuario no encontrado.");

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Contrasena, usuario.ContrasenaHash))
                return Unauthorized("Contraseña incorrecta.");

            var token = GenerateJwtToken(usuario);
            return Ok(new { token });
        }

        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Cedula),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreCompleto ?? ""),
                new Claim("role", usuario.Rol ?? ""),
                new Claim("id", usuario.Id.ToString()),
                new Claim("cedula", usuario.Cedula ?? ""),
                new Claim("nombreCompleto", usuario.NombreCompleto ?? ""),
                new Claim("cargo", usuario.Cargo ?? ""),
                new Claim("obraId", usuario.ObraId?.ToString() ?? "")
            };

            var secretKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuario
                .Include(u => u.Obra)
                .OrderByDescending(x => x.Id)
                .AsNoTracking()
                .ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int id)
        {
            var usuario = await _context.Usuario
                .Include(u => u.Obra)
                .FirstOrDefaultAsync(u => u.Id == id);
            return usuario == null ? NotFound() : Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (usuario.ObraId.HasValue)
            {
                var obraExiste = await _context.Obra.AnyAsync(o => o.Id == usuario.ObraId);
                if (!obraExiste)
                    return BadRequest("La obra asignada no existe.");
            }

            usuario.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.Id)
                return BadRequest("ID no coincide con el cuerpo de la solicitud.");

            if (usuario.ObraId.HasValue)
            {
                var obraExiste = await _context.Obra.AnyAsync(o => o.Id == usuario.ObraId);
                if (!obraExiste)
                    return BadRequest("La obra asignada no existe.");
            }

            try
            {
                var usuarioExistente = await _context.Usuario.FirstOrDefaultAsync(u => u.Id == id);

                if (usuarioExistente == null)
                    return NotFound("Usuario no encontrado.");

                if (!BCrypt.Net.BCrypt.Verify(usuario.ContrasenaHash, usuarioExistente.ContrasenaHash))
                {
                    usuarioExistente.ContrasenaHash = BCrypt.Net.BCrypt.HashPassword(usuario.ContrasenaHash);
                }

                usuarioExistente.Cedula = usuario.Cedula;
                usuarioExistente.NombreCompleto = usuario.NombreCompleto;
                usuarioExistente.Cargo = usuario.Cargo;
                usuarioExistente.ObraId = usuario.ObraId;
                usuarioExistente.Rol = usuario.Rol;
                usuarioExistente.Estado = usuario.Estado;

                if (usuario.ObraId.HasValue)
                {
                    var obra = await _context.Obra.FirstOrDefaultAsync(o => o.Id == usuario.ObraId.Value);
                    if (obra != null)
                    {
                        obra.ResponsableId = usuario.Id;
                    }
                }

                await _context.SaveChangesAsync();
                return Ok(usuarioExistente);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error actualizando usuario ID {id}: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Estado = "Inactivo";
            _context.Usuario.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(usuario);
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }

        public class LoginRequest
        {
            public string Cedula { get; set; }
            public string Contrasena { get; set; }
        }
    }
}
