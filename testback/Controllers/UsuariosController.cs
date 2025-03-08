using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using testback.Data;
using testback.Models;
using System.Linq;
using System.Threading.Tasks;

namespace testback.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _secretKey = "YourSecretKeyHere"; // Cambia esto por una clave secreta más segura

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método de Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(u => u.Cedula == loginRequest.Cedula);

            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado");
            }

            // Aquí debes verificar la contraseña, por ejemplo, comparando el hash de la contraseña
            if (usuario.Contrasena != loginRequest.Contrasena)  // Asegúrate de usar un hash para la contraseña en producción
            {
                return Unauthorized("Contraseña incorrecta");
            }

            // Crear un JWT token que incluya toda la información del usuario
            var token = GenerateJwtToken(usuario);

            return Ok(new { token });
        }

        // Función para generar el JWT
        private string GenerateJwtToken(Usuario usuario)
        {
            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, usuario.Cedula),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, usuario.NombreCompleto),
        new Claim("role", "user"), // Puedes agregar más roles si los usas
        new Claim("id", usuario.Id.ToString()),
        new Claim("cedula", usuario.Cedula),
        new Claim("nombreCompleto", usuario.NombreCompleto),
        new Claim("cargo", usuario.Cargo),
        new Claim("obra", usuario.Obra)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "YourIssuer",
                audience: "YourAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            var usuarios = await _context.Usuario.OrderByDescending(x => x.Id).ToListAsync();
            return _context.Usuario != null ? Ok(usuarios) : Problem("Entity set 'ApplicationDbContext.Usuario' is null.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsuario(int? id)
        {
            if (id == null || _context.Usuario == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsuario([Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Contrasena")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, usuario);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUsuario(int id, [Bind("Id,Cedula,NombreCompleto,Cargo,Obra,Contrasena")] Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return NoContent();
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuario.FindAsync(id);

                if (usuario != null)
                {
                    _context.Usuario.Remove(usuario);
                }
                await _context.SaveChangesAsync();
                return Ok(usuario);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuario?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    // Clase para recibir las credenciales de login
    public class LoginRequest
    {
        public string Cedula { get; set; }
        public string Contrasena { get; set; }
    }
}
