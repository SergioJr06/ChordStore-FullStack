namespace InstrumentsStore.Api.Models;

public enum SaleStatus
{
    Pendente = 0,
    Pago = 1,
    Cancelado = 2
}

public enum PaymentMethod
{
    Pix = 0,
    CartaoCredito = 1,
    CartaoDebito = 2,
    Boleto = 3,
    Dinheiro = 4
}
