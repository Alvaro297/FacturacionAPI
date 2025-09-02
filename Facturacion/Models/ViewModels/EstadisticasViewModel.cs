namespace Facturacion.Models.ViewModels
{
    public class EstadisticasViewModel
    {
        public int Año { get; set; }
        public List<EstadisticasMes> FacturacionMensual { get; set; } = new List<EstadisticasMes>();
        public decimal TotalAño { get; set; }
        public int TotalFacturas { get; set; }
        public decimal PromedioMensual { get; set; }
        public string MesMayorFacturacion { get; set; } = "";
        public decimal MayorFacturacionMes { get; set; }
        public List<int> AñosDisponibles { get; set; } = new List<int>();
    }

    public class EstadisticasMes
    {
        public int Mes { get; set; }
        public string NombreMes { get; set; } = "";
        public decimal TotalFacturado { get; set; }
        public int CantidadFacturas { get; set; }
        public decimal PromedioFactura { get; set; }
    }
}