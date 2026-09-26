import type { Instrument } from '../types';

/**
 * Dados do Figma usados enquanto a API não responde (ou não tem os controllers ainda).
 * Quando GET /api/instruments funcionar, estes dados deixam de ser usados.
 */
export const fallbackInstruments: Instrument[] = [
  {
    id: 1,
    slug: 'guitarra',
    name: 'Guitarra Schecter Synyster Standard Black with Silver Pinstripes',
    section: 'novo',
    price: 3900,
    oldPrice: null,
    installments: 12,
    description: null,
    imageUrl: '/images/guitarra.png',
    galleryUrls: ['/images/guitarra.png', '/images/guitarra-2.png'],
  },
  {
    id: 2,
    slug: 'saxofone',
    name: 'Saxofone Alto Yamaha YAS62',
    section: 'novo',
    price: 1890,
    oldPrice: null,
    installments: 12,
    description: null,
    imageUrl: '/images/saxofone.png',
    galleryUrls: ['/images/saxofone.png', '/images/saxofone-2.png'],
  },
  {
    id: 3,
    slug: 'teclado',
    name: 'Teclado Sintetizador Roland XPS-30 Vermelho 61 Teclas',
    section: 'novo',
    price: 6690,
    oldPrice: null,
    installments: 12,
    description: null,
    imageUrl: '/images/teclado.png',
    galleryUrls: ['/images/teclado.png', '/images/teclado-2.png'],
  },
  {
    id: 4,
    slug: 'violino',
    name: 'Violino Profissional 4/4 Strad V. Fachinetti Ano 2026',
    section: 'exclusivo',
    price: null, // o Figma não define o preço do violino
    oldPrice: null,
    installments: 12,
    description: null,
    imageUrl: '/images/violino.png',
    galleryUrls: ['/images/violino.png'],
  },
  {
    id: 5,
    slug: 'bateria',
    name: 'Bateria Acústica Completa 8 Peças',
    section: 'promocao',
    price: 2900,
    oldPrice: 4990,
    installments: 12,
    description:
      'A Bateria Acústica Completa 8 Peças é perfeita para músicos iniciantes e intermediários que buscam um som encorpado, preciso e cheio de personalidade. Com casco em madeira de alta qualidade e acabamento impecável, entrega timbre quente e equilibrado, ideal para rock, pop, jazz, gospel e muito mais.',
    imageUrl: '/images/bateria.png',
    galleryUrls: ['/images/bateria.png'],
  },
];
