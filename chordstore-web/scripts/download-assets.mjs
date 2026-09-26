// Baixa as imagens/ícones do Figma para public/images.
// ATENÇÃO: as URLs do Figma expiram em ~7 dias. Rode logo: npm run assets
import { mkdir, writeFile } from 'node:fs/promises';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const out = join(dirname(fileURLToPath(import.meta.url)), '..', 'public', 'images');
const A = 'https://www.figma.com/api/mcp/asset/';

const principal = A + 'fe3739ae-da59-46ae-821d-fab11c49f642/';
const carrinho = A + 'f22a40b9-d5d3-4fa9-842a-c7f68eff635d/';
const usuario = A + '487d7bcb-af06-4950-97c0-7c95f78c43e4/';
const local = A + 'dfacd674-cc11-4dc5-8d93-e28add91250a/';
const guitarra = A + '2aa0d06b-634c-4adc-aace-8bef10da3c1d/';
const sax = A + 'a03ff0e4-67c3-4afe-a0eb-ae5db1a4ce6b/';
const teclado = A + '0abbd211-3677-4b11-8946-21a4aad982a2/';
const promo = A + 'ba972b3f-42ee-45f8-8d93-f0efefce0aa9/';

const assets = [
  // Página principal
  [principal + 'c0681.png', 'hero-1.png'],
  [principal + '1a969.png', 'hero-2.png'],
  [principal + '86973.png', 'hero-3.png'],
  [principal + '9c929.png', 'fundo-musica.png'],
  [principal + '1856d.png', 'guitarra.png'],
  [principal + '074fe.png', 'saxofone.png'],
  [principal + 'e631c.png', 'teclado.png'],
  [principal + '21b3b.png', 'fundo-exclusivos.png'],
  [principal + '0804d.png', 'violino.png'],
  [principal + '9fd4a.png', 'bateria.png'],
  [principal + '1781e.png', 'logo.png'],
  [principal + 'eded3.svg', 'badge-30.svg'],
  [principal + '008ce.svg', 'header-line.svg'],
  [principal + 'e3824.svg', 'topbar-left.svg'],
  [principal + '7f5ad.svg', 'topbar-right.svg'],
  [principal + '166a9.svg', 'icon-cart.svg'],
  [principal + '8edcf.svg', 'icon-pin.svg'],
  [principal + 'f8538.svg', 'icon-pin-dot.svg'],
  [principal + '2ff59.svg', 'icon-user.svg'],
  [principal + '8518d.svg', 'search-field.svg'],
  [principal + '3adee.svg', 'icon-search.svg'],
  [principal + 'a3ccd.svg', 'whatsapp.svg'],
  // Fundo da seção "Sobre" (render do nó 14:41)
  [A + 'b32791f3-df51-4e7e-adfa-6ab9fa34e617.png', 'sobre-fundo.png'],
  // Modais
  [carrinho + 'e88ce.svg', 'exit-carrinho.svg'],
  [usuario + '65547.svg', 'exit-usuario.svg'],
  [local + '9b36d.svg', 'exit-localizacao.svg'],
  [guitarra + '72b41.svg', 'exit-dark.svg'],
  [guitarra + '7c148.png', 'guitarra-2.png'],
  [guitarra + '33568.svg', 'btn-cart-circle.svg'],
  [guitarra + 'e420f.svg', 'btn-cart-icon.svg'],
  [sax + 'c6e64.png', 'saxofone-2.png'],
  [sax + '59bde.svg', 'exit-sax.svg'],
  [teclado + '7e45a.png', 'teclado-2.png'],
  [promo + 'eee56.png', 'fundo-promo.png'],
  [promo + '39515.svg', 'exit-promo.svg'],
];

await mkdir(out, { recursive: true });
let failed = 0;
for (const [url, name] of assets) {
  try {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`HTTP ${res.status}`);
    await writeFile(join(out, name), Buffer.from(await res.arrayBuffer()));
    console.log('ok   ', name);
  } catch (err) {
    failed++;
    console.error('ERRO ', name, '-', err.message);
  }
}
console.log(failed ? `\n${failed} arquivo(s) falharam (URLs expiradas?).` : '\nTodas as imagens foram baixadas.');
process.exit(failed ? 1 : 0);
