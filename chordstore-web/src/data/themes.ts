/** Cada modal de produto tem uma paleta própria no Figma. Chave = slug do instrumento. */
export interface ProductTheme {
  bg: string;
  title: string;
  price: string;
  titleSize: number;
  priceSize: number;
  exit: string;
}

export const defaultTheme: ProductTheme = {
  bg: '#232222',
  title: '#f1f1f1',
  price: '#e5e5e5',
  titleSize: 39,
  priceSize: 40,
  exit: '/images/exit-dark.svg',
};

export const productThemes: Record<string, ProductTheme> = {
  guitarra: defaultTheme,
  saxofone: {
    bg: '#e7c648',
    title: '#3c3736',
    price: '#7d2c2c',
    titleSize: 45,
    priceSize: 48,
    exit: '/images/exit-sax.svg',
  },
  teclado: {
    bg: '#b31515',
    title: '#ffffff',
    price: '#420f0f',
    titleSize: 38,
    priceSize: 40,
    exit: '/images/exit-dark.svg',
  },
};
