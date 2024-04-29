

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Data.Servicios;
using ProyectoIntegradosPZ_KS_DM.Models;
using ProyectoIntegradosPZ_KS_DM.Models.ViewModels;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Claims;



namespace ProyectoIntegradosPZ_KS_DM.Controllers

{

    public class CuentaController : Controller

    {
        private readonly ContextoPZ_KS_DM _contexto;
        private readonly UsuarioServicio _usuarioServicio;

        public CuentaController(ContextoPZ_KS_DM con)
        {
            _contexto = con;
            _usuarioServicio = new UsuarioServicio(con);
        }

        public IActionResult Registrar()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Registrar(Usuario model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var connection = new SqlConnection(_contexto.Conexion))
                    {
                        connection.Open();
                        using (var command = new SqlCommand("RegistrarUsuario", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@Nombre", model.Nombre);
                            command.Parameters.AddWithValue("@Apellido", model.Apellido);
                            command.Parameters.AddWithValue("@Correo", model.Correo);

                            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Contrasenia);
                            command.Parameters.AddWithValue("@Contrasenia", hashedPassword);
                            command.Parameters.AddWithValue("@NombreUsuario", model.NombreUsuario);
                            DateTime fechaexpiracion = DateTime.UtcNow.AddMinutes(5);
                            command.Parameters.AddWithValue("@FechaExpiracion", fechaexpiracion);

                            var token = Guid.NewGuid();

                            command.Parameters.AddWithValue("@Token", token);

                            command.ExecuteNonQuery();

                            Email email = new();
                            if (model.Correo != null)
                                email.Enviar(model.Correo, token.ToString());


                        }
                    }
                    return RedirectToAction("Token");

                }
                catch (SqlException ex)
                {
                    if (ex.Number == 2627)
                    {
                        ViewBag.Error = "El correo y/o nombre ya se encuentra registrado";
                    }
                    else
                        ViewBag.Error = "No se pudo registrar al usuario" + ex.Message;
                    throw;

                }
            }
            return View(model);
        }
        public IActionResult Token()

        {

            string token = Request.Query["valor"];

            if (token != null)

            {
                try
                {
                    using (SqlConnection con = new SqlConnection(_contexto.Conexion))
                    {
                        using (SqlCommand cmd = new("ActivarCuenta", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Token", token);
                            DateTime fechaexpiracion = DateTime.UtcNow;
                            cmd.Parameters.AddWithValue("@Fecha", fechaexpiracion);
                            con.Open();
                            var resultado = cmd.ExecuteScalar();

                            int activada = Convert.ToInt32(resultado);
                            if (activada == 1)
                                ViewData["mensaje"] = "Su cuenta ha sido validada exitosamente.";

                            else
                                ViewData["mensaje"] = "Su enlace de activacion ha expirado.";
                            con.Close();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    ViewData["mensaje"] = e.Message;
                    return View();
                }
            }
            else
            {
                ViewData["mensaje"] = "Verifique su correo para activar su cuenta.";
                return View();
            }
            return View();
        }
        public IActionResult Login()
        {
            ClaimsPrincipal c = HttpContext.User;
            if (c.Identity != null)
            {
                if (c.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                using (SqlConnection con = new(_contexto.Conexion))
                {
                    using (SqlCommand cmd = new("ValidarUsuario", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Correo", model.Correo);
                        con.Open();
                        try
                        {
                            using (var dr = cmd.ExecuteReader())
                            {
                                if (dr.Read())
                                {
                                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(model.Contrasenia, dr["Contrasenia"].ToString());
                                    if (passwordMatch)
                                    {
                                        DateTime fechaExpiracion = DateTime.UtcNow;
                                        if (!(bool)dr["Estado"] && dr["FechaExpiracion"].ToString() != fechaExpiracion.ToString())
                                        {
                                            if (model.Correo != null)
                                                _usuarioServicio.ActualizarToken(model.Correo);
                                            ViewBag.Error = "Su cuenta no ha sido activada, se ha reenviado un correo de activación";
                                        }


                                    }
                                    else if (!(bool)dr["Estado"])
                                        ViewBag.Error = "Su cuenta no ha sido activada, verifique su bandeja";
                                    else
                                    {
                                        string? nombreusuario = dr["NombreUsuario"].ToString();
                                        int idUsuario = (int)dr["UsuarioId"];
                                        if (nombreusuario != null)
                                        {
                                            var claims = new List<Claim>();
                                            {
                                                new Claim(ClaimTypes.NameIdentifier, nombreusuario);
                                                new Claim("IdUsuario", idUsuario.ToString());
                                            };
                                            int rolId = (int)dr["RolId"];
                                            string rolNombre = rolId == 1 ? "Administrador" : "Usuario";
                                            claims.Add(new Claim(ClaimTypes.Role, rolNombre));
                                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                            var propiedades = new AuthenticationProperties
                                            {
                                                AllowRefresh = true,
                                                IsPersistent = model.MantenerActivo,
                                                ExpiresUtc = DateTimeOffset.UtcNow.Add(model.MantenerActivo ? TimeSpan.FromDays(1) : TimeSpan.FromMinutes(3))
                                            };
                                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), propiedades);
                                            return RedirectToAction("Index", "Home");
                                        }
                                    }
                                }
                                else
                                    ViewBag.Error = "Correo no registrado";
                                    dr.Close(); 
                            }


                        }

                        finally                        
                        {
                            if (cmd != null)
                            {
                                cmd.Dispose();
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                ViewBag.Error= ex.Message;
            }
            return View(model);
        }

        public async Task<IActionResult> CerrarSesion() 
        { 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
     }
 }




