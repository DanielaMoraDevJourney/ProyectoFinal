using Microsoft.AspNetCore.Mvc;
using ProyectoIntegradosPZ_KS_DM.Data;
using ProyectoIntegradosPZ_KS_DM.Data.Enums;
using ProyectoIntegradosPZ_KS_DM.Models;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoIntegradosPZ_KS_DM.Data.Servicios
{
    public class PostServicio : Controller
    {

        private readonly ContextoPZ_KS_DM _contextoPZ_KS_DM;
        public PostServicio(ContextoPZ_KS_DM con)
        {
            _contextoPZ_KS_DM = con;
        }

        public Post ObtenerPostPorId(int id)
        {
            var post = new Post();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtenerPostPorId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"])



                            };
                        }
                        reader.Close();
                    }
                }
            }
            return post;
        }
        public List<Post> ObtenerPosts()
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {

                connection.Open();
                using (SqlCommand cmd = new("ObtenerTodosLosPosts", connection))
                {
                    //hi
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"])

                            };
                            posts.Add(post);

                        }
                    }

                }



            }




            return posts;
        }
        public List<Post> ObtenerPostsPorCategoria(CategoriaEnum categoria)
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {

                connection.Open();
                using (SqlCommand cmd = new("ObtenerPostsPorCategoria", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Categoria", categoria.ToString());
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var post = new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"])

                            };
                            posts.Add(post);

                        }
                    }

                }



            }




            return posts;
        }

        public List<Post> ObtenerPostsPorTitulo(string titulo)
        {
            var posts = new List<Post>();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {

                connection.Open();
                using (SqlCommand cmd = new("ObtenerPostsPorTitulo", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Titulo", titulo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(new Post
                            {
                                PostId = (int)reader["PostId"],
                                Titulo = (string)reader["Titulo"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                Categoria = (CategoriaEnum)Enum.Parse(typeof(CategoriaEnum), (string)reader["Categoria"])

                            });


                        }
                    }

                }



            }




            return posts;
        }


        public List<Comentario> ObtenerComentariosPorPostId(int id)
        {
            var comments = new List<Comentario>();
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                using (var command = new SqlCommand("ObtenerComentariosPorPostId", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PostId", id);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var comment = new Comentario
                            {
                                ComentarioId = (int)reader["ComentarioId"],
                                Contenido = (string)reader["Contenido"],
                                FechaCreacion = (DateTime)reader["FechaCreacion"],
                                UsuarioId = (int)reader["UsuarioId"],
                                PostId = (int)reader["PostId"],
                                NombreUsuario = (string)reader["NombreUsuario"]
                            };
                            comments.Add(comment);
                        }
                        reader.Close();
                    }
                }
            }
            return comments;
        }

        public List<Comentario> ObtenerComentariosHijos(List<Comentario> comments)
        {
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    using (var command = new SqlCommand("ObtenerComentariosHijosPorComentarioId", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ComentarioId", comment.ComentarioId);
                        using (var reader = command.ExecuteReader())
                        {
                            var comentarioshijos = new List<Comentario>();
                            while (reader.Read())
                            {
                                var comentariohijo = new Comentario
                                {
                                    ComentarioId = (int)reader["ComentarioId"],
                                    Contenido = (string)reader["Contenido"],
                                    FechaCreacion = (DateTime)reader["FechaCreacion"],
                                    UsuarioId = (int)reader["UsuarioId"],
                                    PostId = (int)reader["PostId"],
                                    NombreUsuario = (string)reader["NombreUsuario"],
                                    ComentarioPadreId = comment.ComentarioId
                                };
                                comentarioshijos.Add(comentariohijo);
                            }
                            reader.Close();
                            comment.ComentariosHijos = comentarioshijos;
                        }
                    }

                }


            }
            return comments;
        }

        public List<Comentario> ObtenerComentariosNietos(List<Comentario> comments)
        {
            using (var connection = new SqlConnection(_contextoPZ_KS_DM.Conexion))
            {
                connection.Open();
                foreach (var comment in comments)
                {
                    if (comment.ComentariosHijos is not null)
                    {
                        foreach (var comentariohijo in comment.ComentariosHijos)
                        {
                            using (var command = new SqlCommand("ObtenerComentariosHijosPorComentarioId", connection))
                            {
                                command.CommandType = CommandType.StoredProcedure;
                                command.Parameters.AddWithValue("@ComentarioId", comentariohijo.ComentarioId);
                                using (var reader = command.ExecuteReader())
                                {
                                    var comentariosnietos = new List<Comentario>();
                                    while (reader.Read())
                                    {
                                        var comentarionieto = new Comentario
                                        {
                                            ComentarioId = (int)reader["ComentarioId"],
                                            Contenido = (string)reader["Contenido"],
                                            FechaCreacion = (DateTime)reader["FechaCreacion"],
                                            UsuarioId = (int)reader["UsuarioId"],
                                            PostId = (int)reader["PostId"],
                                            NombreUsuario = (string)reader["NombreUsuario"],
                                            ComentarioPadreId = comentariohijo.ComentarioId,
                                            ComentarioAbueloId = comment.ComentarioId
                                        };
                                        comentariosnietos.Add(comentarionieto);
                                    }
                                    reader.Close();
                                    comentariohijo.ComentariosHijos = comentariosnietos;
                                }
                            }

                        }
                    }

                }

            }
            return comments;
        }

        //

    }
}
