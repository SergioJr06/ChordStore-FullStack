# ChordStore — Novidades (Área Administrativa + Checkout + Planos)

## O que foi adicionado

### Backend (`InstrumentsStore.Api`)
- **Categorias, Fornecedores, Clientes**: entidades novas com CRUD completo (`/api/admin/categories`, `/api/admin/suppliers`, `/api/admin/customers`).
- **Produtos**: `Instrument` agora tem `CategoryId`/`SupplierId` (relação real, antes era texto solto). CRUD completo em `/api/admin/products`.
- **Vendas**: `Sale`/`SaleItem` novos. `/api/admin/sales` (lançar venda manual tipo PDV, listar, mudar status) e `/api/checkout` (público, usado pelo carrinho do site — baixa estoque de verdade).
- **Login do admin**: `/api/auth/login` agora valida contra a tabela `AdminUsers` no banco (antes era fixo no código). Também tem `/api/auth/me`, `/api/auth/me` (PUT, trocar usuário/e-mail) e `/api/auth/change-password`.
- Todos os endpoints `/api/admin/*` exigem o token JWT (mesmo login usado no site).

**Login padrão do admin:** `contato@chordstore.com` / `admin123` — troque em "Minha Conta" assim que possível.

### Frontend (`chordstore-web`)
- **Roteamento** com `react-router-dom`: o site público continua em `/` (visualmente idêntico a antes) e ganhou a página `/planos`.
- **Checkout**: o carrinho agora tem um passo de finalização (dados do cliente + forma de pagamento/parcelas) que chama `/api/checkout` e limpa o carrinho ao concluir.
- **Área `/admin`**: login, dashboard com indicadores, e CRUD completo de Produtos, Categorias, Fornecedores, Clientes, além de Vendas (tela tipo PDV) e "Minha Conta" (trocar usuário/e-mail/senha).

## Como rodar

### 1. Backend
```bash
cd InstrumentsStore.Api
dotnet restore
```
Como o modelo do banco mudou bastante (novas tabelas, `Instrument` com `CategoryId`/`SupplierId`), gere uma nova migration:
```bash
dotnet ef migrations add AddAdminModules
dotnet ef database update
```
Ajuste a connection string em `appsettings.json` se necessário, depois:
```bash
dotnet run
```

### 2. Frontend
```bash
cd chordstore-web
npm install
npm run dev
```
Acesse o site em `/` e o painel em `/admin/login`.

## Observações
- O botão de "Entrar" do site (ícone de login no header) continua chamando o mesmo `/api/auth/login` — funciona igual a antes, só que agora autentica contra o banco.
- Vendas feitas pelo carrinho do site ficam com `origin = "loja"`; vendas lançadas pelo admin ficam com `origin = "admin"`.
- Ao cancelar uma venda pelo admin, o estoque dos produtos é devolvido automaticamente.
- Produtos com preço "sob consulta" (preço nulo) não podem ser vendidos pelo checkout/PDV — é preciso definir um preço antes.
