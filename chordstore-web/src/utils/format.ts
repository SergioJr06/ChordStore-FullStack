const brl = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL' });
const brlInt = new Intl.NumberFormat('pt-BR', { style: 'currency', currency: 'BRL', maximumFractionDigits: 0 });

/** R$ 3.900 (inteiro) ou R$ 157,50 (com centavos). */
export const money = (v: number) => (Number.isInteger(v) ? brlInt.format(v) : brl.format(v));

/** Sempre com centavos: R$ 325,00. */
export const moneyExact = (v: number) => brl.format(v);
