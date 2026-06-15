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


            Console.WriteLine("===== ABB INICIAL =====");

buffer.Mostrar();


Console.WriteLine("Cantidad de mensajes: " + buffer.Count);



Console.WriteLine("===== BUSCAR A200 =====");

AbbNode encontrado = buffer.Search("A200");


if(encontrado != null)
{
    Console.WriteLine(
        "Encontrado: "
        + encontrado.HexCode
        + " Prioridad: "
        + encontrado.Prioridad
    );
}
else
{
    Console.WriteLine("No encontrado");
}



Console.WriteLine("===== DEQUEUE =====");


AbbNode eliminado = buffer.Dequeue();


if(eliminado != null)
{
    Console.WriteLine(
        "Salio: "
        + eliminado.HexCode
        + " Prioridad: "
        + eliminado.Prioridad
    );
}



Console.WriteLine("Cantidad restante: " + buffer.Count);

return Content("Prueba ABB terminada");
        }

    }
}