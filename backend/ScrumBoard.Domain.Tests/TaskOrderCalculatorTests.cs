using ScrumBoard.Domain.Services;

namespace ScrumBoard.Domain.Tests;

public class TaskOrderCalculatorTests
{
    [Fact]
    public void CalculateNewOrder_ColumnaVacia_DevuelveValorPorDefecto()
    {
        var result = TaskOrderCalculator.CalculateNewOrder(null, null);
        Assert.Equal(1024m, result);
    }

    [Fact]
    public void CalculateNewOrder_InsertarAlInicio_DevuelveMitadDelSiguiente()
    {
        var result = TaskOrderCalculator.CalculateNewOrder(null, 1024m);
        Assert.Equal(512m, result);
    }

    [Fact]
    public void CalculateNewOrder_InsertarAlFinal_SumaElGapPorDefecto()
    {
        var result = TaskOrderCalculator.CalculateNewOrder(1024m, null);
        Assert.Equal(2048m, result);
    }

    [Fact]
    public void CalculateNewOrder_InsertarEntreDos_DevuelvePuntoMedio()
    {
        var result = TaskOrderCalculator.CalculateNewOrder(1024m, 2048m);
        Assert.Equal(1536m, result);
    }

    [Fact]
    public void CalculateNewOrder_MovimientosSucesivosEntreLosMismosDosVecinos_ConvergeSinCruzarse()
    {
        // Simula arrastrar repetidamente una tarea al mismo punto: el valor
        // siempre queda estrictamente entre los vecinos, nunca los cruza.
        decimal previous = 0m, next = 1m;
        decimal? lastResult = null;

        for (var i = 0; i < 10; i++)
        {
            var result = TaskOrderCalculator.CalculateNewOrder(previous, next);
            Assert.True(result > previous && result < next);
            next = result; // la siguiente inserción ocurre cada vez más cerca de "previous"
            lastResult = result;
        }

        Assert.NotNull(lastResult);
    }
}