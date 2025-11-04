using crud_dotnet.Interfaces;
using crud_dotnet.Models;
using crud_dotnet.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IOrderRepository _repository;

    private readonly IOrderService _orderService;
    public PedidosController(IOrderRepository repository, IOrderService orderService)
    {
        _repository = repository;
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var order = await _orderService.CreateOrderAsync(dto);

            // 🔹 Retorna 201 Created + objeto criado
            return CreatedAtAction(
                nameof(GetOrderById), // método GET para recuperar o pedido
                new { id = order.Id }, // parâmetro da rota
                new
                {
                    Message = "Pedido criado com sucesso!",
                    Order = order
                });
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, new
            {
                Message = "Erro ao salvar no banco.",
                Detail = dbEx.InnerException?.Message ?? dbEx.Message
            });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        try
        {
            var order = await _repository.GetOrderByIdAsync(id);

            if (order == null)
                return NotFound(new { Message = $"Nenhum pedido encontrado com o Id {id}." });

            return Ok(order);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Erro inesperado: {ex.Message}" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _repository.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro inesperado: {ex.Message}");
        }
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
    {
        try
        {
            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var status))
            {
                return BadRequest($"Status inválido. Valores válidos: {string.Join(", ", Enum.GetNames(typeof(OrderStatus)))}");
            }

            var order = await _repository.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            if (order.Status == OrderStatus.Approved || order.Status == OrderStatus.Canceled)
            {
                return BadRequest($"Não é permitido alterar o status de um pedido que já está '{order.Status}'.");
            }
            order.Status = status;
            _repository.UpdateOrder(order);
            await _repository.SaveChangesAsync();

            return Ok(order);
        }
        catch (DbUpdateException dbEx)
        {
            return StatusCode(500, $"Erro ao atualizar o status do pedido: {dbEx.InnerException?.Message ?? dbEx.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro inesperado: {ex.Message}");
        }
    }
}