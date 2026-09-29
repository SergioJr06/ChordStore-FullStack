namespace InstrumentsStore.Api.Models;
// codigo gerado para enumerar os status de venda e métodos de pagamento
public enum SaleStatus // 0 = Pendente, 1 = Pago, 2 = Cancelado
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
