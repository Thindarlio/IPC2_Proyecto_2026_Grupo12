using Microsoft.AspNetCore.Mvc;
using OrbitNet.Models.Nodes;
using OrbitNet.Models.TDAs;


namespace OrbitNet.Controllers
{

    public class TestABBController : Controller
    {

        public IActionResult Index()
        {

            BufferMensajes buffer = new BufferMensajes();


            buffer.Enqueue(
                new AbbNode(
                    "A100",
                    "SAT-001",
                    "10.0.0.1",
                    3,
                    "Mensaje normal"
                )
            );


            buffer.Enqueue(
                new AbbNode(
                    "A200",
                    "SAT-002",
                    "10.0.0.2",
                    5,
                    "Alerta critica"
                )
            );


            buffer.Enqueue(
                new AbbNode(
                    "A300",
                    "SAT-003",
                    "10.0.0.3",
                    1,
                    "Mensaje bajo"
                )
            );


            Console.WriteLine("===== PRUEBA ABB =====");

            buffer.Mostrar();


            Console.WriteLine("======================");


            return Content("ABB probado correctamente");
        }

    }
}