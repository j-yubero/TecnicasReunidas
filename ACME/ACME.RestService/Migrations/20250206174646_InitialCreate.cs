using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ACME.RestService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CanCUDVisitas = table.Column<bool>(type: "bit", nullable: false),
                    CanCUDVentas = table.Column<bool>(type: "bit", nullable: false),
                    CanCUDClientes = table.Column<bool>(type: "bit", nullable: false),
                    CanCUDUsuarios = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolId",
                        column: x => x.RolId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Historial",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Accion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Historial_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visitas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendedorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visitas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visitas_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrecioUnitario = table.Column<double>(type: "float", nullable: false),
                    Unidades = table.Column<int>(type: "int", nullable: false),
                    PrecioTotal = table.Column<double>(type: "float", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ventas_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ventas_Visitas_VisitaId",
                        column: x => x.VisitaId,
                        principalTable: "Visitas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Historial_UsuarioId",
                table: "Historial",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolId",
                table: "Usuarios",
                column: "RolId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_ProductoId",
                table: "Ventas",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_VisitaId",
                table: "Ventas",
                column: "VisitaId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_ClienteId",
                table: "Visitas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Visitas_VendedorId",
                table: "Visitas",
                column: "VendedorId");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: ["D71A27D6-8F5B-4E29-8835-494E8E298CC6", "admin", true, true, true, true, true]);
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: [Guid.NewGuid(), "Visitas", true, false, false, false, true]);
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: [Guid.NewGuid(), "Ventas", false, true, false, false, true]);
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: [Guid.NewGuid(), "Clientes", false, false, true, false, true]);
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: [Guid.NewGuid(), "Usuarios", false, false, false, false, true]);
            migrationBuilder.InsertData(
                table: "Roles",
                columns: ["Id", "Nombre", "CanCUDVisitas", "CanCUDVentas", "CanCUDClientes", "CanCUDUsuarios", "Activo"],
                values: [Guid.NewGuid(), "Lectura", false, false, false, false, true]);

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: ["Id", "UserName", "Password", "Nombre", "RolId", "Activo"],
                values: [Guid.NewGuid(), "admin", "YWRtaW4=", "Administrator", "D71A27D6-8F5B-4E29-8835-494E8E298CC6", true]);

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: ["Id", "Nombre", "Direccion", "Activo"],
                values: [Guid.NewGuid(), "Cliente Uno", "Calle Uno, 123", true]);
            migrationBuilder.InsertData(
                table: "Clientes",
                columns: ["Id", "Nombre", "Direccion", "Activo"],
                values: [Guid.NewGuid(), "Cliente Dos", "Calle Dos, 123", true]);
            migrationBuilder.InsertData(
                table: "Clientes",
                columns: ["Id", "Nombre", "Direccion", "Activo"],
                values: [Guid.NewGuid(), "Cliente Tres", "Calle Tres, 123", true]);

            migrationBuilder.InsertData(
                table: "Productos",
                columns: ["Id", "Nombre", "Descripcion", "Precio", "Stock", "Activo"],
                values: [Guid.NewGuid(), "Silla", "Silla de oficina", 100.00, 10, true]);
            migrationBuilder.InsertData(
                table: "Productos",
                columns: ["Id", "Nombre", "Descripcion", "Precio", "Stock", "Activo"],
                values: [Guid.NewGuid(), "Cajonera", "Cajonera baja con 2 cajones", 299.99, 3, true]);
            migrationBuilder.InsertData(
                table: "Productos",
                columns: ["Id", "Nombre", "Descripcion", "Precio", "Stock", "Activo"],
                values: [Guid.NewGuid(), "Archivador", "Armario alto dónde guardar los archivadores y las carpetas", 1000.50, 17, true]);
            migrationBuilder.InsertData(
                table: "Productos",
                columns: ["Id", "Nombre", "Descripcion", "Precio", "Stock", "Activo"],
                values: [Guid.NewGuid(), "Folios", "Paquete de 500 folios", 0.80, 10000, true]);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Historial");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Productos");

            migrationBuilder.DropTable(
                name: "Visitas");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
