using System.Data.SqlClient;
using Vitaly_Manager.Entidades;
using Vitaly_Manager.Entidades.EntidadesAntiguas;

namespace Vitaly_Manager.Data
{
    public static class DatosVenta
    {

        //Temporal necesita mas revision
        public static List<Venta> ListaVentas()
        {
            List<Venta> listaVentas = new List<Venta>();
            using (SqlConnection coneccion = new SqlConnection(MainServidor.Servidor))
            {
                coneccion.Open();
                SqlCommand comando = new SqlCommand("SELECT * FROM venta", coneccion);
                SqlDataReader lector = comando.ExecuteReader();

                while (lector.Read())
                {
                    int folio = Convert.ToInt32(lector["folio"]);
                    int IDcliente = Convert.ToInt32(lector["IDcliente"]);
                    DateOnly fechaVenta = DateOnly.FromDateTime(Convert.ToDateTime(lector["fechaVenta"]));
                    decimal importeTotal = Convert.ToInt32(lector["importeTotal"]);


                    Venta nuevo = new Venta
                    {
                        Folio = folio,
                        ID_Cliente = IDcliente,
                        Fecha_Venta = fechaVenta,
                        Importe_Total = importeTotal
                    };

                    listaVentas.Add(nuevo);
                }
                lector.Close();
                return listaVentas;
            }
        }
    }
}
